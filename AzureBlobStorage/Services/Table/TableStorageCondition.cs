using AzureStorageConsole.AzureTableStorage;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureStorageConsole.Services.Table
{
    public class TableStorageCondition
    {
        public TableStorageCondition(string property, string operation, string value)
        {
            Property = property;
            Operation = operation;
            Value = value;
        }

        public string Property { get; }
        public string Operation { get; }
        public string Value { get; }
    }

    public class TableStorageConditionCollection : IEnumerable<TableStorageCondition>
    {
        private IList<Tuple<string,TableStorageCondition>> _itens;

        public TableStorageConditionCollection()
        {
            _itens = new List<Tuple<string, TableStorageCondition>>();
        }

        public int Count => _itens.Count;

        public bool IsReadOnly => false;

        public void Add(TableStorageCondition item)
        {
            _itens.Add(new Tuple<string, TableStorageCondition>(TableQueryLogicalOperators.And, item));
        }
        public void And(TableStorageCondition item)
        {
            _itens.Add(new Tuple<string, TableStorageCondition>(TableQueryLogicalOperators.And, item));
        }
        public void AndNot(TableStorageCondition item)
        {
            _itens.Add(new Tuple<string, TableStorageCondition>($"{TableQueryLogicalOperators.And} {TableQueryLogicalOperators.Not}", item));
        }
        public void Or(TableStorageCondition item)
        {
            _itens.Add(new Tuple<string, TableStorageCondition>(TableQueryLogicalOperators.Or, item));
        }

        public void OrNot(TableStorageCondition item, string logicOper)
        {
            _itens.Add(new Tuple<string, TableStorageCondition>($"{TableQueryLogicalOperators.Or} {TableQueryLogicalOperators.Not}", item));
        }

        public void Clear()
        {
            _itens.Clear();
        }

        public bool Remove(TableStorageCondition item)
        {
            var itemToDelete = _itens.FirstOrDefault(x => x.Item2.Property == item.Property &&
                     x.Item2.Operation == item.Operation &&
                     x.Item2.Value == item.Value);
            return _itens.Remove(itemToDelete);
        }

        public IEnumerator<TableStorageCondition> GetEnumerator()
        {
            return _itens.Select(i => i.Item2).ToList().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _itens.Select(i => i.Item2).ToList().GetEnumerator();
        }

        public string CombineConditions()
        {
            string expression = string.Empty;
            foreach(var item in _itens)
            {
                string statment = TableQuery.GenerateFilterCondition(item.Item2.Property, item.Item2.Operation, item.Item2.Value);
                if (string.IsNullOrEmpty(expression))
                    expression = statment;
                else
                    expression = TableQuery.CombineFilters(expression, item.Item1, statment);
            }
            return expression;
        }

    }

}
