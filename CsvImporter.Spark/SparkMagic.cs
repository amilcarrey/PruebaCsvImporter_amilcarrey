using System;
using Microsoft.Spark.Sql;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace CsvImporter.Spark
{
    public class SparkMagic
    {
        public async void Magic()
        {
            SparkSession _spark = SparkSession
            .Builder()
            .AppName("Connect to SQL Server")
            .GetOrCreate();
        
            string connection_url = "jdbc: sqlserver://localhost:1433;databaseName=AcmeCorporation;integratedSecurity=true;Trusted_Connection=True";
            string dbtable = "Stock";

            Stream stream = await GetCSVStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");

            DataFrame stockTable = _spark.Read()
            .Format("jdbc")
            .Option("driver", "com.microsoft.sqlserver.jdbc.SQLServerDriver")
            .Option("url", connection_url)
            .Option("dbtable", dbtable)
            .Load();

            stockTable.Show();
        }

        private async Task<Stream> GetCSVStream(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;


                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                return response.GetResponseStream();

            }
            catch (Exception ex)
            {

                return Stream.Null;
            }
        }
    }
}
