﻿using System;
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
    public class Test_OrderProduct
    {
        [Fact]
        // 1. Order Product thành công.
        public async Task Test_CreateOrder_Success()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var Product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(Product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1, 

                    OrderProductDetails = new List<OrderProductDetailDTO>
                    {
                        new OrderProductDetailDTO
                        {
                            Quantity = 2,
                            ProductId = 1,
                        }
                    }
                };
                var result = await controller.CreateOrder(testOrder) as OkObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                var successMessage = result.Value as string;
                Assert.Equal("Order thành công!", successMessage);
                var createdOrder = await context.Orders.FirstOrDefaultAsync();
                Assert.NotNull(createdOrder);
                Assert.Equal(testOrder.OrderDate, createdOrder.OrderDate);
            }
        }

        [Fact]
        // 2. Order Product - Province(null)
        public async Task Test_CreateOrder_Province_Null()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
                    {
                        new OrderProductDetailDTO
                        {
                            Quantity = 2,
                            ProductId = 1,
                        }
                    }
                };
                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Equal("Tỉnh không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 3. Order Product - District(null)
        public async Task Test_CreateOrder_District_Null()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
                    {
                        new OrderProductDetailDTO
                        {
                            Quantity = 2,
                            ProductId = 1,
                        }
                    }
                };
                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Equal("Huyện/Thành Phố không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 4. Order Product - Commune(null)
        public async Task Test_CreateOrder_Commune_Null()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
                    {
                        new OrderProductDetailDTO
                        {
                            Quantity = 2,
                            ProductId = 1,
                        }
                    }
                };
                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Equal("Phường/Xã không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 5. Order Product - Address(null)
        public async Task Test_CreateOrder_Address_Null()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
                    {
                        new OrderProductDetailDTO
                        {
                            Quantity = 2,
                            ProductId = 1,
                        }
                    }
                };
                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Equal("Địa chỉ không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 6. Order Product - ProductId Not Found
        public async Task Test_CreateOrder_ProductId_NotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
            {
                new OrderProductDetailDTO
                {
                    Quantity = 2,
                    ProductId = 2, 
                }
            }
                };

                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Contains("Sản phẩm không hợp lệ", errorMessage);
                Assert.Contains("Sản phẩm với ID 2 không tồn tại", errorMessage);
            }
        }

        [Fact]
        // 7. Order Product - Quantity Null 
        public async Task Test_CreateOrder_QuantityNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
            {
                new OrderProductDetailDTO
                {
                    Quantity = 0,
                    ProductId = 1,
                }
            }
                };

                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Contains("Sản phẩm không hợp lệ", errorMessage);
                Assert.Contains("Số lượng không hợp lệ cho sản phẩm với ID 1", errorMessage);
            }
        }

        [Fact]
        // 8. Order Product - Quantity Exceeds Available Quantity
        public async Task Test_CreateOrder_Quantity_Exceeds_Available_Quantity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var product = new Product
                {
                    ProductId = 1,
                    Price = 100,
                    Quantity = 100
                };
                context.Products.Add(product);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new OrderController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,

                    OrderProductDetails = new List<OrderProductDetailDTO>
            {
                new OrderProductDetailDTO
                {
                    Quantity = 1000,
                    ProductId = 1,
                }
            }
                };

                var result = await controller.CreateOrder(testOrder) as BadRequestObjectResult;
                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                var errorMessage = result.Value as string;
                Assert.Equal("Số lượng sản phẩm không đủ", errorMessage);
            }
        }
    }
}
