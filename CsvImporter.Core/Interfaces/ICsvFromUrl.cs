using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CsvImporter.Core.Interfaces
{
    public interface ICsvFromUrl
    {
        public Stream GetCSVStream(string? url);        
    }
}
