using CsvImporter.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CsvImporter.Infraestructure.Data.Services
{
    public class CsvFromUrl : ICsvFromUrl
    {
        private ILogger<CsvFromUrl> _logger;
        public CsvFromUrl(ILogger<CsvFromUrl> logger)
        {
            _logger = logger;
        }
        public Stream GetCSVStream(string url)
        {
            _logger.LogInformation("Waiting response from url...");
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            _logger.LogInformation("Building response...");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            return response.GetResponseStream();
        }
    }
}
