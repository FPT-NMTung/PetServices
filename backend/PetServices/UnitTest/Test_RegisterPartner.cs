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
    public class Test_RegisterPartner
    {
        [Fact]
        // 1. Đăng ký thành công
        public async Task Test_Register_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Đăng ký thành công! Vui lòng chờ đợi quản lý xác nhận tài khoản của bạn trước khi đăng nhập", result.Value);
            }
        }

        [Fact]
        // 2. Email(null) + Pass + Phone + ImageCertificate
        public async Task Test_Register_EmptyEmail_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Email không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 3. Email(thiếu @) + Pass + Phone  + ImageCertificate
        public async Task Test_Register_Email_InvalidEmailFormat()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.True(controller.ModelState.ContainsKey("Email không hợp lệ"));

                var errorMessages = controller.ModelState["Email không hợp lệ"].Errors;
                var errorMessage = errorMessages[0].ErrorMessage;
                Assert.Contains("Email cần có @", errorMessage);
            }
        }

        [Fact]
        // 4. Email(trùng email) + Pass + Phone + ImageCertificate
        public async Task Test_Register_EmailAlreadyExists()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = "12345678",
                    RoleId = 4
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(409, result.StatusCode);
                Assert.Equal("Email đã được đăng ký", result.Value);
            }
        }

        [Fact]
        // 5. Email(có khoảng trắng) + Pass + Phone + ImageCertificate
        public async Task Test_Register_Email_WhiteSpaces()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {               
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg 65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Email không chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 6. Email + Pass(null) + Phone + ImageCertificate
        public async Task Test_Register_Pass_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 7. Email + Pass(7 ký tự) + Phone + ImageCertificate
        public async Task Test_Register_Pass_ToShort()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "1234567",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu phải có ít nhất 8 ký tự!", result.Value.ToString());
            }
        }

        [Fact]
        // 8. Email + Pass(có khoảng trắng) + Phone + ImageCertificate
        public async Task Test_Register_Pass_WhiteSpace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345 678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được chứa khoảng trắng!", result.Value.ToString());
            }
        }
    }
}
