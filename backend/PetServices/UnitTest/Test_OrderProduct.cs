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
    public class Test_OrderProduct
    {
        [Fact]
        // 1. Order Product thành công. Success
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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    FullName = "Nguyễn Văn Hùng",
                    Phone = "0987654321",
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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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

        [Fact]
        // 9. Order Product - Address(>500 word)
        public async Task Test_CreateOrder_Address_ToLong()
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

                var address = new string('A', 501);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = address,
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Địa chỉ vượt quá số ký tự. Tối đa 500 ký tự!", errorMessage);
            }
        }

        [Fact]
        // 10. Order Product - Province(>number @)
        public async Task Test_CreateOrder_Province_SpecialNumber()
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
                    Province = "Hà Tĩnh 123@",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Tỉnh phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!", errorMessage);
            }
        }

        [Fact]
        // 11. Order Product - District(123@)
        public async Task Test_CreateOrder_District_NumberSpecial()
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
                    District = "Hà Tĩnh 123@",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Huyện/Thành phố phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!", errorMessage);
            }
        }

        [Fact]
        // 12. Order Product - Commune(123@)
        public async Task Test_CreateOrder_Commune_NumberSpecial()
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
                    Commune = "Thạch Linh 123@",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Phường/Xã phải là ký tự chữ, không chấp nhận số hay ký tự đặc biệt!", errorMessage);
            }
        }

        [Fact]
        // 13. Order Product - Province(>50 word)
        public async Task Test_CreateOrder_Province_ToLong()
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

                var province = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = province,
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Tỉnh vượt quá số ký tự. Tối đa 50 ký tự!", errorMessage);
            }
        }

        [Fact]
        // 14. Order Product - District(>50 word)
        public async Task Test_CreateOrder_District_ToLong()
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

                var disrict = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = disrict,
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Huyện/Thành Phố vượt quá số ký tự. Tối đa 50 ký tự!", errorMessage);
            }
        }

        [Fact]
        // 15. Order Product - Commune(>50 word)
        public async Task Test_CreateOrder_Commune_ToLong()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = commune,
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Phường/Xã vượt quá số ký tự. Tối đa 50 ký tự!", errorMessage);
            }
        }

        [Fact]
        // 16. Order Product - FullName(Null)
        public async Task Test_CreateOrder_FullName_Null()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "",

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
                Assert.Equal("Tên liên hệ không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 17. Order Product - FullName(123@)
        public async Task Test_CreateOrder_FullName_SpecialCharacter()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "0964418085",
                    FullName = "Nguyễn Văn Hùng 123@",

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
                Assert.Equal("Tên liên hệ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", errorMessage);
            }
        }

        [Fact]
        // 18. Order Product - Phone(Null)
        public async Task Test_CreateOrder_Phone_Null()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Số điện thoại không được để trống!", errorMessage);
            }
        }

        [Fact]
        // 19. Order Product - Phone(8 số)
        public async Task Test_CreateOrder_Phone_8character()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "09876543",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Số điện thoại phải có 10 ký tự!", errorMessage);
            }
        }

        [Fact]
        // 20. Order Product - Phone(khoảng trắng)
        public async Task Test_CreateOrder_Phone_whitespace()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "096441 808",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Số điện thoại không được chứa khoảng trắng!", errorMessage);
            }
        }

        [Fact]
        // 21. Order Product - Phone(start 8)
        public async Task Test_CreateOrder_Phone_nonestartzero()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "9644180850",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Số điện thoại phải bắt đầu bằng số 0!", errorMessage);
            }
        }

        [Fact]
        // 22. Order Product - Phone(text)
        public async Task Test_CreateOrder_Phone_text()
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

                var commune = new string('A', 51);
                var testOrder = new OrdersDTO
                {
                    OrderDate = DateTime.Now,
                    OrderStatus = "Waiting",
                    Province = "Hà Tĩnh",
                    District = "Hà Tĩnh",
                    Commune = "Thạch Linh",
                    Address = "Thạch Linh-Hà Tĩnh",
                    UserInfoId = 1,
                    Phone = "098765432a",
                    FullName = "Nguyễn Văn Hùng",

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
                Assert.Equal("Số điện thoại không phải là số! Bạn cần nhập số điện thoại ở dạng số!", errorMessage);
            }
        }
    }
}
