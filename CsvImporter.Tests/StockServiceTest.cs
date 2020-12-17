using CsvHelper;
using CsvImporter.Core.Application;
using CsvImporter.Core.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CsvImporter.Tests
{
    public class StockServiceTest
    {
        private readonly Mock<IStockRepository> _stockRepository;
        private readonly Mock<ICsvFromUrl> _csvStream;
        private readonly Mock<IBulk> _bulkCreate;
        private readonly StockService _service;
        public StockServiceTest()
        {
            this._stockRepository = new Mock<IStockRepository>();
            this._csvStream = new Mock<ICsvFromUrl>();
            this._bulkCreate = new Mock<IBulk>();
            this._service = new StockService(_stockRepository.Object, _csvStream.Object, _bulkCreate.Object);
        }

        [Fact]
        public async Task IfEverythingIsOk_Clear_ShouldReturnTrue()
        {
            _bulkCreate.Setup(x => x.Clear()).ReturnsAsync(true);
            var result = await _service.ClearStock();

            Assert.True(result);
        }

        [Theory]
        [InlineData("google.com")]
        [InlineData("/y9ne9ilzmfld/Stock.CSV")]
        [InlineData("blob/asdasdas")]
        public async Task IfUrlIsNotValid_GetStream_ShouldReturnNullStream(string url)
        {
            var result = await _service.GetStream(url);

            Assert.Equal(Stream.Null, result);
        }

        [Fact]
        public async Task IfStreamIsNull_AddFromCsvStreamSqlCopyWay_ShouldThrowsAnException()
        {
            var exception = await Record.ExceptionAsync(() => _service.AddFromCsvStreamSqlCopyWay(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
        
        //[Fact]
        //public async Task IfStreamHasWrongStructure_AddFromCsvStreamSqlCopyWay_ShouldThrowsAnException()
        //{
        //    //Moq csv
        //    //var exception = await Record.ExceptionAsync(() => _service.AddFromCsvStreamSqlCopyWay(Stream.Null));

        //    //Assert.NotNull(exception);
        //    //Assert.IsAssignableFrom<ValidationException>(exception);            
        //}
    }
}
