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
    public class Test_AddService
    {       
        [Fact]
        // 1. Add dịch vụ thành công
        public async Task Test_AddService_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Thêm dịch vụ thành công!", result.Value);
            }
        }

        [Fact]
        // 2. ServiceName(Null)
        public async Task Test_AddService_ServiceName_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên dịch vụ không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. ServiceName(>500)
        public async Task Test_AddService_ServiceName_ToLong()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testAddService = new ServiceDTO
                {
                    ServiceName = longServiceName,
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên dịch vụ vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciptions(Null)
        public async Task Test_AddService_Desciptions_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Image(Null)
        public async Task Test_AddService_Image_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "",
                    Time = 60,
                    Price = 10000,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Image(WhiteSpace)
        public async Task Test_AddServicet_Image_WhiteSpace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/N sSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. SerCategoriesId(không tồn tại)
        public async Task Test_AddService_SerCategoriesId_NotExist()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 2
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại dịch vụ không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Price = 0
        public async Task Test_AddService_Pricezero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 0,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }

        [Fact]
        // 9. Time = 0
        public async Task Test_AddService_Timezero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 0,
                    SerCategoriesId = 1
                };

                var result = await controller.CreateService(testAddService) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Thời gian phải lớn hơn 0!", result.Value);
            }
        }       
    }
}
