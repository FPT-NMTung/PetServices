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
    public class Test_UpdateProduct
    {
        [Fact]
        // 1. Update sản phẩm thành công
        public async Task Test_UpdateProduct_Success()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhật sản phẩm thành công!", result.Value);
            }
        }

        [Fact]
        // 2. ProductName(Null)
        public async Task Test_UpdateProduct_ProductName_Null()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên sản phẩm không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. ProductName(>500)
        public async Task Test_UpdateProduct_ProductName_ToLong()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longProductName = new string('A', 501);
                var testUpdateProduct = new ProductDTO
                {
                    ProductName = longProductName,
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên sản phẩm vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciption(Null)
        public async Task Test_UpdateProduct_Desciption_Null()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Picture(Null)
        public async Task Test_UpdateProduct_Picture_Null()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh sản phẩm không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Picture(Null)
        public async Task Test_UpdateProduct_Picture_WhiteSpace()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.v n/NsSG",
                    Price = 12000,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. ProCategoriesId(Not exist)
        public async Task Test_UpdateProduct_ProCategoriesId_NotExist()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    ProCategoriesId = 2,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại sản phẩm không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Change product status for a non-existent product
        public async Task Test_ChangeStatusProduct_ProductNotFound()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusProduct(2, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Không tìm thấy sản phẩm cần thay đổi.", result.Value);
            }
        }

        [Fact]
        // 9. Change product status successfully
        public async Task Test_ChangeStatusProduct_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusProduct(1, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhật sản phẩm thành công!", result.Value);
            }
        }

        [Fact]
        // 10. Price = 0
        public async Task Test_UpdateProduct_Pricezero()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 0,
                    ProCategoriesId = 1,
                    Quantity = 4
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }

        [Fact]
        // 11. Quantity = 0
        public async Task Test_UpdateProduct_Quantityzero()
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

                var Product = new Product
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    ProCategoriesId = 1,
                    Quantity = 3
                };

                context.Products.Add(Product);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ProductController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateProduct = new ProductDTO
                {
                    ProductName = "Sản phẩm cho Chó",
                    Desciption = "Sản phẩm dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 1200,
                    ProCategoriesId = 1,
                    Quantity = 0
                };

                var result = await controller.Update(testUpdateProduct, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Số lượng phải lớn hơn 0!", result.Value);
            }
        }
    }
}
