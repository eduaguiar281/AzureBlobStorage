using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageConsole.AzureTableStorage
{
    public static class TableQueryOperation
    {
        public const string Equal = "eq";
        public const string NotEqual = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqual = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqual = "le";
    }

    public static class TableQueryLogicalOperators
    {
        public const string And = "and";
        public const string Not = "not";
        public const string Or = "or";
    }
}
