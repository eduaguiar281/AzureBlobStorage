using AzureStorageConsole.Blobs.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageConsole.Services.Storages
{
    public interface IBlobStorageService
    {

        /// <summary>
        /// Configura conforme a connection String
        /// </summary>
        /// <param name="settings">Configuração de Storage</param>
        void Configure(IStorageSettings settings);

        /// <summary>
        /// Faz o Upload de múltiplos arquivos para o Container de BlobStorage
        /// </summary>
        /// <param name="files">Lista com o caminho completo para o Upload de Arquivo</param>
        /// <param name="destinationFolder">Pasta onde será gravado o arquivo</param>
        /// <returns>Lista com os arquivos gravados no storage</returns>
        Task<IEnumerable<StorageFile>> UploadFiles(IEnumerable<string> files, string destinationFolder);

        /// <summary>
        /// Faz o Upload de um único arquivo para o Container de BlobStorage
        /// </summary>
        /// <param name="file">Caminho e nome completo do arquivo</param>
        /// <param name="destinationFolder">Pasta onde será gravado o arquivo no Storage</param>
        /// <returns>Arquivo gravado no storage</returns>
        Task<StorageFile> UploadFile(string file, string destinationFolder);

        /// <summary>
        /// Faz o Upload de um único arquivo para o Container de BlobStorage
        /// </summary>
        /// <param name="file">Caminho e nome completo do arquivo</param>
        /// <param name="destinationFolder">Pasta onde será gravado o arquivo no Storage</param>
        /// <param name="newName">Novo nome do arquivo no Storage</param>
        /// <returns>Arquivo gravado no storage</returns>
        Task<StorageFile> UploadFile(string file, string destinationFolder, string newName);

        /// <summary>
        /// Faz o Upload de um único arquivo para o Container de BlobStorage passando um array de bytes
        /// </summary>
        /// <param name="byteArray">Byte array do arquivo</param>
        /// <param name="fileName">Nome do arquivo para salvar</param>
        /// <returns>Retorna a Url do Arquivo</returns>
        Task<string> UploadByArray(byte[] byteArray, string fileName);

        /// <summary>
        /// Apaga o arquivo do container
        /// </summary>
        /// <param name="filePathStorage">nome e caminho do arquivo</param>
        /// <returns>True caso consiga apagar ou false caso não consiga apagar</returns>
        Task<bool> DeleteFile(string filePathStorage);

        /// <summary>
        /// Faz a busca de um arquivo no container
        /// </summary>
        /// <param name="searchedFile">Nome do arquivo a ser procurado</param>
        /// <returns>Retorna uma lista de arquivos encontrados na pesquisa</returns>
        Task<IEnumerable<StorageFile>> SearchFilesInContainer(string searchedFile);

        /// <summary>
        /// Faz o Download do Arquivo
        /// </summary>
        /// <param name="filetoDownload">Nome do arquivo procurado</param>
        void DowloadFile(string filetoDownload);

        /// <summary>
        /// Faz o Download do Arquivo
        /// </summary>
        /// <param name="blobUri">Nome do arquivo procurado</param>
        /// <returns>Retorna um stream do arquivo</returns>
        Stream DowloadFileToStream(string blobUri);

    }
}
