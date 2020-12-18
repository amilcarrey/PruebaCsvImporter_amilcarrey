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
        public void IfStreamIsNull_AddBySqlCopy_ShouldThrowsAnException()
        {
            var exception = Record.Exception(() => _service.AddBySqlCopy(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task IfStreamIsNull_AddBySqlCopyAsync_ShouldThrowsAnException()
        {
            var exception = await Record.ExceptionAsync(() => _service.AddBySqlCopyAsync(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void IfStreamIsNull_AddByEfCore_ShouldThrowsAnException()
        {
            var exception = Record.Exception(() => _service.AddByEfCore(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task IfStreamIsNull_AddByEfCoreWithParallelismAsync_ShouldThrowsAnException()
        {
            var exception = await Record.ExceptionAsync(() => _service.AddByEfCoreWithParallelismAsync(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task IfStreamIsNull_AddByEfCoreAsync_ShouldThrowsAnException()
        {
            var exception = await Record.ExceptionAsync(() => _service.AddByEfCoreAsync(Stream.Null));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        
    }
}
