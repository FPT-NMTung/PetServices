using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PetServices.Controllers;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using Xunit;

namespace UnitTest
{
    public class Test_AddProduct
    {
        [Fact]
        // 1. Thêm sản phẩm thành công
        public async Task Test_AddProduct_ProductName_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testCreateProduct = new ProductDTO
                {
                    ProductName = "aaaa",
                    Desciption = "hạt óc chó giành cho cún cưng",
                    Picture = "https://s.net.vn/S7CD",
                    Price = 10000,
                    ProCategoriesId = 9,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testCreateProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Thêm sản phẩm thành công!", result.Value);
            }
        }

        [Fact]
        // 2. Tên sản phẩm null
        public async Task Test_AddProduct_ProductName_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testCreateProduct = new ProductDTO
                {
                    ProductName = "",
                    Desciption = "hạt óc chó giành cho cún cưng",
                    Picture = "https://s.net.vn/S7CD",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testCreateProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên sản phẩm không được để trống!", result.Value);
            }
        }
    }
}
