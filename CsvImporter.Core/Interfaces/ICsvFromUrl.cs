using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Interfaces
{
    public interface ICsvFromUrl
    {
        public Task<Stream> GetCSVStream(string url);
    }
}
