using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underscore.Bot.MessageRouting.Utils;

namespace Underscore.Bot.MessageRouting.MessageRouting.DataStore.Azure
{
    internal class AzureStorageBase
    {
        private readonly string _connectionString;
        private readonly ExceptionHandler _exceptionHandler;
        private CloudTableClient _tableClient;

        public AzureStorageBase(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("message", nameof(connectionString));
            }
            _connectionString = connectionString;
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = cloudStorageAccount.CreateCloudTableClient();

            _exceptionHandler = new ExceptionHandler();
        }


        public async Task<CloudTable> GetOrCreateCloudTable(string tableName)
        {
            var tableReference = _tableClient.GetTableReference(tableName);

            var tableReferenceOk = await _exceptionHandler.GetAsync(async () => await tableReference.CreateIfNotExistsAsync());

            if (tableReferenceOk)
                return tableReference;

            return null;
        }


        public bool InsertEntity(CloudTable table, ITableEntity entityToInsert)
        {
            var insertOperation = TableOperation.InsertOrReplace(entityToInsert);
            var insertResult = _exceptionHandler.Get(() => table.Execute(insertOperation));

            if (insertResult == null)
                return false;

            return insertResult.HttpStatusCode == 200 || insertResult.HttpStatusCode == 201;
        }
    }
}
