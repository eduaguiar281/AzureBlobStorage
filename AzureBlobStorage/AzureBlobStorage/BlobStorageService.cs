using AzureStorageConsole.Blobs.Files;
using AzureStorageConsole.Services.Storages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageConsole.AzureBlobStorage
{
    public class BlobStorageService :IBlobStorageService
    {
        protected CloudStorageAccount _storageAccount = null;
        protected CloudBlobContainer _storageContainer = null;
        protected CloudBlobClient _blobClient = null;
        protected IStorageSettings _settings;
        private static string storageName = null;

        public BlobStorageService(IStorageSettings settings)
        {
            Configure(settings);
        }

        public async Task<bool> DeleteFile(string filePathStorage)
        {
            VerifyContainerExistence(_settings.ContainerName);

            if (string.IsNullOrWhiteSpace(filePathStorage))
                throw new ArgumentNullException("Arquivo não foi informado!");

            try
            {
                CloudBlockBlob cloudBlockBlob = _storageContainer.GetBlockBlobReference(filePathStorage);
                await cloudBlockBlob.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao realizar a Exclusão do Arquivo do Azure Storage. Exception: {ex}");
            }
        }

        public async Task<IEnumerable<StorageFile>> SearchFilesInContainer(string searchedFile)
        {
            var listBlobItem = await GetAllBlobFilesInContainer(_settings.ContainerName);

            return listBlobItem.Where(x => x.FileName.Contains(searchedFile));
        }

        /// <summary>
        /// Faz o Upload para o Container de BlobStorage
        /// </summary>
        /// <param name="files">Key: FileName, Value:File Path </param>
        /// <param name="">Pasta onde será gravado o arquivo</param>
        /// <returns>Lista com os arquivos</returns>
        public async Task<IEnumerable<StorageFile>> UploadFiles(IEnumerable<string> files, string destinationFolder)
        {
            List<StorageFile> result = new List<StorageFile>();
            try
            {
                foreach (var file in files)
                {
                    result.Add(await (UploadFile(file, destinationFolder)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao realizar Upload do Arquivo para o Azure Storage. Exception: {ex}");
            }
            return result;
        }

        public async Task<StorageFile> UploadFile(string file, string destinationFolder)
        {
            return await UploadFile(file, destinationFolder, null);
        }

        public async Task<StorageFile> UploadFile(string file, string destinationFolder, string newName)
        {
            LocalFile localFile = CheckFileExists(file);
            if (!localFile.Exists)
                throw new ArgumentException($"Arquivo informado {file} não foi localizado ou não é um arquivo válido!");

            string fileToStorage = localFile.FullName;
            if (string.IsNullOrEmpty(newName))
                newName = localFile.Name;

            VerifyContainerExistence(_settings.ContainerName);

            storageName = $"{destinationFolder}/{newName}{localFile.Extension}";
            CloudBlockBlob cloudBlockBlob = _storageContainer.GetBlockBlobReference(storageName);
            await cloudBlockBlob.UploadFromFileAsync(fileToStorage);
            return new StorageFile(cloudBlockBlob.Name, cloudBlockBlob.Properties.Length, cloudBlockBlob.Uri.ToString());
        }

        private LocalFile CheckFileExists(string file)
        {
            return new LocalFile(file);
        }


        public async Task<string> UploadByArray(byte[] imageByteArr, string fileName)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_settings.ContainerName);

            await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            //string storageName = 

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromByteArrayAsync(imageByteArr, 0, imageByteArr.Length);

            //blockBlob.Properties.ContentType = "xls/xlsx";
            await blockBlob.SetPropertiesAsync();

            return blockBlob.Uri.ToString();
        }

        public void DowloadFile(string filetoDownload)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_settings.ContainerName);

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(storageName);

            Stream file = File.OpenWrite(Path.Combine(Path.GetTempPath(), filetoDownload));

            cloudBlockBlob.DownloadToStream(file);

            file.Close();
        }


        public Stream DowloadFileToStream(string blobUri)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_settings.ConnectionString);
            CloudBlockBlob cloudBlockBlob = GetBlockBlobReference(storageAccount, blobUri);
            Stream mem = new MemoryStream();
            if (cloudBlockBlob != null)
            {
                cloudBlockBlob.DownloadToStream(mem);
            }
            return mem;
        }


        private CloudBlockBlob GetBlockBlobReference(CloudStorageAccount account, string blobUri)
        {
            string blobName = blobUri.Substring(blobUri.IndexOf("/" + _settings.ContainerName + "/")).Replace("/" + _settings.ContainerName + "/", "");
            CloudBlobClient blobclient = account.CreateCloudBlobClient();
            CloudBlobContainer container = _blobClient.GetContainerReference(_settings.ContainerName);
            container.CreateIfNotExists();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
            return blob;
        }

        private async Task<IEnumerable<StorageFile>> GetAllBlobFilesInContainer(string _containerName)
        {
            VerifyContainerExistence(_containerName);

            List<IListBlobItem> results = new List<IListBlobItem>();
            try
            {
                BlobContinuationToken continuationToken = null;
                do
                {
                    bool useFlatBlobListing = true;
                    BlobListingDetails blobListingDetails = BlobListingDetails.None;
                    int maxBlobsPerRequest = 500;
                    var response = await _storageContainer.ListBlobsSegmentedAsync(null, useFlatBlobListing, blobListingDetails, maxBlobsPerRequest, continuationToken, null, null);
                    continuationToken = response.ContinuationToken;
                    results.AddRange(response.Results);
                }
                while (continuationToken != null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return results.Select(x => {
                var s = (CloudBlockBlob)x;
                return new StorageFile(s.Name, s.Properties.Length, s.Uri.ToString());
            });
        }


        private CloudStorageAccount GetCloudStorageConnection(string stringConn)
        {
            if (CloudStorageAccount.TryParse(stringConn, out CloudStorageAccount storageAccount))
                return storageAccount;

            throw new Exception("Não foi possível estabelecer a conexão com o Azure Storage!");
        }

        private bool VerifyContainerExistence(string _containerName)
        {
            var blobContainerReference = _blobClient.GetContainerReference(_containerName);

            if (!blobContainerReference.Exists())
                throw new Exception("Container informado não existe no Azure Storage");

            _storageContainer = blobContainerReference;

            return true;
        }

        public void Configure(IStorageSettings settings)
        {
            _settings = settings;
            _storageAccount = GetCloudStorageConnection(_settings.ConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
        }
    }
}
