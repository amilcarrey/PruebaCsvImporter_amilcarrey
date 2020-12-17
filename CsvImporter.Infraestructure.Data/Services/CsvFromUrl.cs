using CsvImporter.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Infraestructure.Data.Services
{
    public class CsvFromUrl : ICsvFromUrl
    {
        private ILogger<CsvFromUrl> _logger;
        public CsvFromUrl(ILogger<CsvFromUrl> logger)
        {
            _logger = logger;
        }
        public async Task<Stream> GetCSVStream(string url)
        {
            try
            {
                _logger.LogInformation("Waiting response from url...");
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                _logger.LogInformation("Building response...");
                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                return response.GetResponseStream();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Stream.Null;
            }
        }
    }
}
