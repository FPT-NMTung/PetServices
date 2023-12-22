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
    public class Test_UpdateService
    {
        [Fact]
        // 1. Update sản phẩm thành công
        public async Task Test_UpdateService_Success()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhât dịch vụ thành công!", result.Value);
            }
        }

        [Fact]
        // 2. ServiceName(Null)
        public async Task Test_UpdateProduct_ServiceName_Null()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên dịch vụ không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. ServiceName(>500)
        public async Task Test_UpdateProduct_ServiceName_ToLong()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = longServiceName,
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên dịch vụ vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciption(Null)
        public async Task Test_UpdateService_Desciption_Null()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Picture(Null)
        public async Task Test_UpdateService_Picture_Null()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "",
                    Time = 60,
                    Price = 11000,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh dịch vụ không được để trống!", result.Value);
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
                var serCategory = new ServiceCategory
                {
                    SerCategoriesId = 1
                };

                context.ServiceCategories.Add(serCategory);
                context.SaveChanges();

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/ NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. ProCategoriesId(Not exist)
        public async Task Test_UpdateService_SerCategoriesId_NotExist()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 11000,
                    Time = 60,
                    SerCategoriesId = 2
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại dịch vụ không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Change product status for a non-existent product
        public async Task Test_ChangeStatusService_ServiceNotFound()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusService(2, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Không tìm thấy dịch vụ cần thay đổi.", result.Value);
            }
        }

        [Fact]
        // 9. Change product status successfully
        public async Task Test_ChangeStatusService_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 12000,
                    SerCategoriesId = 1
                };

                context.Services.Add(service);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusService(1, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhật dịch vụ thành công!", result.Value);
            }
        }

        [Fact]
        // 10. Pric=0
        public async Task Test_UpdateService_Pricezero()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 0,
                    Time = 60,
                    SerCategoriesId = 2
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }       

        [Fact]
        // 11. Time=0
        public async Task Test_UpdateService_Timezero()
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

                var Service = new Service
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 60,
                    SerCategoriesId = 1
                };

                context.Services.Add(Service);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new ServiceController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longServiceName = new string('A', 501);
                var testUpdateService = new ServiceDTO
                {
                    ServiceName = "Dịch vụ Spa",
                    Desciptions = "Dịch vụ chăm sóc sắc đẹp cho thú cưng",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    Time = 0,
                    SerCategoriesId = 2
                };

                var result = await controller.UpdateServce(testUpdateService, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Thời gian phải lớn hơn 0!", result.Value);
            }
        }
    }
}
