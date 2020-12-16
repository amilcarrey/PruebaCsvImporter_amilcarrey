using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvImporter.Core.Interfaces
{
    public interface ICsvFromAzureBlobStorage
    {
        Stream GetCSVStream(string key, string container, string fileName);
    }
}
