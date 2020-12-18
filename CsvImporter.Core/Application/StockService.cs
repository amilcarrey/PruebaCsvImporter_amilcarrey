using CsvHelper;
using CsvHelper.Configuration;
using CsvImporter.Core.Entities;
using CsvImporter.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CsvImporter.Core.Application
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICsvFromUrl _csvStream;        
        private readonly IBulk _bulk;
        public StockService(IStockRepository stockRepository, ICsvFromUrl csvStream, IBulk bulk)
        {
            _stockRepository = stockRepository;
            _csvStream = csvStream;
            _bulk = bulk;
        }
        public async void UpdateStockFromCsvAsync()
        {
            await ClearStock();
            
            Stream response = await GetStream("https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV");

            AddBySqlCopy(response);
        }

        /// <summary>
        /// Recorre un stream proveniente de un csv y lo parsea a la entidad StockModel, 
        /// realizando varios bulk para optimizar memoria. No utiliza EF Core. 
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public void AddBySqlCopy(Stream csvStream)
        {
            if (Stream.Null.Equals(csvStream)) throw new ArgumentNullException("csvStream");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 100000;

            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                //csv.ValidateHeader<StockModel>();
                
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        CreateBulk(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }

                    counter++;
                    listRecords.Add(record);
                    Console.WriteLine(counter);
                }

                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} ms");
                Console.ReadLine();
            }
        }


        /// <summary>
        /// Recorre un stream proveniente de un csv y lo parsea a la entidad StockModel, 
        /// realizando varios bulk para optimizar memoria. No utiliza EF Core. 
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public async Task AddBySqlCopyAsync(Stream csvStream)
        {
            if (Stream.Null.Equals(csvStream)) throw new ArgumentNullException("csvStream");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 500000;

            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                //csv.ValidateHeader<StockModel>();

                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        await CreateBulkAsync(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }

                    counter++;
                    listRecords.Add(record);
                    Console.WriteLine(counter);
                }

                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} ms");
                Console.ReadLine();
            }
        }


        /// <summary>
        /// Recorre un stream proveniente de un csv y lo parsea a la entidad StockModel, 
        /// realizando varios bulk para optimizar memoria. Utiliza EF Core.
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public void AddByEfCore(Stream csvStream)
        {
            if (csvStream == Stream.Null) throw new ArgumentNullException("csvStream");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 100000;

            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        CreateEFCoreBulk(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }

                    counter++;
                    listRecords.Add(record);
                    //Console.Clear();
                    Console.WriteLine(counter);
                }

                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} ms");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }


        /// <summary>
        /// Recorre un stream proveniente de un csv y lo parsea a la entidad StockModel, 
        /// realizando varios bulk para optimizar memoria. Utiliza EF Core de manera asincronica.
        /// </summary>
        /// <param name="csvStream">Stream de datos</param>
        public async Task AddByEfCoreAsync(Stream csvStream)
        {
            if (csvStream == Stream.Null) throw new ArgumentNullException("csvStream");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();
            int counter = 0;
            int limit = 50000;

            using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";
                foreach (var record in csv.GetRecords<StockModel>())
                {
                    if (counter == limit || (reader.EndOfStream && counter < limit))
                    {
                        await CreateEFCoreBulkAsync(listRecords);
                        counter = 0;
                        listRecords.Clear();
                    }

                    counter++;
                    listRecords.Add(record);
                    //Console.Clear();
                    Console.WriteLine(counter);
                }

                timeMeasure.Stop();
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} ms");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }


        /// <summary>
        /// Recorre un stream proveniente de un csv y lo parsea a la entidad StockModel, 
        /// utilizando paralelismo y EF Core. La memoria crece exponencialmente según el numero de registros.         
        /// </summary>
        /// <param name="csvStream"></param>
        /// <returns></returns>
        public async Task AddByEfCoreWithParallelismAsync(Stream csvStream)
        {
            if (csvStream == Stream.Null) throw new ArgumentNullException("csvStream");

            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();

            List<StockModel> listRecords = new List<StockModel>();

            using (var reader = new StreamReader(@"C:\bigtest.csv"))
            //using (var reader = new StreamReader(csvStream))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture, true))
            {
                csv.Configuration.Delimiter = ";";

                listRecords = csv.GetRecords<StockModel>()
                    .AsParallel()
                    .WithDegreeOfParallelism(5)
                    .ToList();

                await CreateEFCoreBulkAsync(listRecords);

                timeMeasure.Stop();

                Console.WriteLine(listRecords.Count);
                Console.WriteLine($"Tiempo: {timeMeasure.Elapsed.ToString("mm\\:ss")} m");
                Console.WriteLine("TERMINO!");
                Console.ReadLine();
            }
        }

        
        /// <summary>
        /// Guarda una lista de StockModels con SqlClient.
        /// </summary>
        /// <param name="listRecords"></param>
        public void CreateBulk(List<StockModel> listRecords)
        {
            _bulk.CreateBulk(listRecords);
        }


        /// <summary>
        /// Guarda una lista de StockModels con SqlClient de manera asincronica.
        /// </summary>
        /// <param name="listRecords"></param>
        public async Task CreateBulkAsync(List<StockModel> listRecords)
        {
            await _bulk.CreateBulkAsync(listRecords); 
        }


        /// <summary>
        /// Guarda una lista de StockModels a través de EFcore.
        /// </summary>
        /// <param name="listRecords"></param>
        public void CreateEFCoreBulk(List<StockModel> listRecords)
        {
            _stockRepository.CreateBulk(listRecords);
        }


        /// <summary>
        /// Guarda una lista de StockModels a través de EFcore de manera asincronica.
        /// </summary>
        /// <param name="listRecords"></param>
        public async Task CreateEFCoreBulkAsync(List<StockModel> listRecords)
        {
            await _stockRepository.CreateBulkAsync(listRecords);
        }


        /// <summary>
        /// Limpia la base de datos
        /// </summary>
        public Task<bool> ClearStock()
        {
            return _bulk.Clear();
        }


        /// <summary>
        /// Obtiene un stream a partir de una url. 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Stream> GetStream(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return await _csvStream.GetCSVStream(url);
            else
                return Stream.Null;
        }
    }
}
