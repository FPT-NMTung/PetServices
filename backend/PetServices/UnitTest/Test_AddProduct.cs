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
    //fix connnect
    public class Test_AddProduct
    {       
        [Fact]
        // 1. Add sản phẩm thành công
        public async Task Test_AddProduct_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Thêm sản phẩm thành công!", result.Value);
            }
        }

        [Fact]
        // 2. ProductName(Null)
        public async Task Test_AddProduct_ProductName_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên sản phẩm không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. ProductName(>500)
        public async Task Test_AddProduct_ProductName_ToLong()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longProductName = new string('A', 501);
                var testAddProduct = new ProductDTO
                {
                    ProductName = longProductName,
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên sản phẩm vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciptions(Null)
        public async Task Test_AddProduct_Desciptions_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Image(Null)
        public async Task Test_AddProduct_Image_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh sản phẩm không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Image(WhiteSpace)
        public async Task Test_AddProduct_Image_WhiteSpace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/N sSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. ProCategoriesId(không tồn tại)
        public async Task Test_AddProduct_ProCategoriesId_NotExist()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 2,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại sản phẩm không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Price = 0
        public async Task Test_AddProduct_Pricezero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 0,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }

        [Fact]
        // 9. Price = 0
        public async Task Test_AddProduct_Quantityzero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var proCategory = new ProductCategory
                {
                    ProCategoriesId = 1
                };

                context.ProductCategories.Add(proCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 1000,
                    ProCategoriesId = 1,
                    Quantity = 0
                };

                var result = await controller.CreateProduct(testAddProduct) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Số lượng phải lớn hơn 0!", result.Value);
            }
        }
    }
}
