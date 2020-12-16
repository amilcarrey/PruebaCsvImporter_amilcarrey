using Azure.Storage.Blobs;
using CsvImporter.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvImporter.Infraestructure.Data.Services
{
    class CsvFromAzureBlobStorage
    {
        private readonly IConfiguration _configuration;
        private ILogger<BulkUploadToSql> _logger;

        public CsvFromAzureBlobStorage(IConfiguration configuration, ILogger<BulkUploadToSql> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public Stream GetCSVStream(string key, string container, string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(key);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (blobClient.Exists())
            {
                _logger.LogInformation("Cliente creado, obteniendo stream...");
                return blobClient.OpenRead();
            }
            else
            {
                _logger.LogError("Cliente de Blob storage no encontrado.");
                throw new InvalidOperationException("El cliente no existe");
            }
        }
    }
}
