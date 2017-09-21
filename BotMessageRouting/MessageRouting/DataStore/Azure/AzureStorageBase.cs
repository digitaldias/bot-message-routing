using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underscore.Bot.MessageRouting.Models;
using Underscore.Bot.MessageRouting.Utils;

namespace Underscore.Bot.MessageRouting.MessageRouting.DataStore.Azure
{
    public class AzureStorageBase
    {
        private const string CLASS = "AzureStorageBase";

        private readonly ExceptionHandler _exceptionHandler;
        private CloudTableClient          _tableClient;
        private readonly ILogger          _logger;

        public AzureStorageBase(ILogger logger = null)
        {
            _logger = logger ?? new ConsoleLogger();
            _exceptionHandler  = new ExceptionHandler(_logger);
        }


        public void InitializeWithConnectionString(string connectionString)
        {
            _logger.Enter(CLASS);

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _tableClient = cloudStorageAccount.CreateCloudTableClient();
        }


        public CloudTable GetOrCreateCloudTable(string tableName)
        {
            _logger.Enter(CLASS);

            if (string.IsNullOrEmpty(tableName))
                return null;

            var tableReference         = _tableClient.GetTableReference(tableName);
            var tableReferenceObtained = _exceptionHandler.Get(() => tableReference.CreateIfNotExists());

            if (tableReferenceObtained)
                return tableReference;

            return null;
        }


        public IEnumerable<TResult> GetAll<TResult>(CloudTable table) where TResult : ITableEntity, new()
        {
            _logger.Enter(CLASS);

            var tableQuery = new TableQuery<TResult>();

            return _exceptionHandler.Get(() => table.ExecuteQuery<TResult>(tableQuery));
        }


        public bool InsertEntity(CloudTable table, ITableEntity entityToInsert)
        {
            _logger.Enter(CLASS);

            var insertOperation = TableOperation.InsertOrReplace(entityToInsert);
            var insertResult = _exceptionHandler.Get(() => table.Execute(insertOperation));

            if (insertResult == null)
                return false;

            return insertResult.HttpStatusCode == 200 || insertResult.HttpStatusCode == 201;
        }
    }
}
