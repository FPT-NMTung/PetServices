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
    public class Test_Register
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

                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Đăng ký thành công! Đăng nhập để trải nghiệm hệ thống", result.Value);
            }
        }

        [Fact]
        // 2.Đăng ký với email đã có
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
                    RoleId = 2
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterDTO
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(409, result.StatusCode);
                Assert.Equal("Email đã được đăng ký", result.Value);
            }
        }

        [Fact]
        // 3. Đăng ký với email không có '@'
        public async Task Test_Register_InvalidEmail()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.True(controller.ModelState.ContainsKey("Email không hợp lệ"));

                var errorMessages = controller.ModelState["Email không hợp lệ"].Errors;
                var errorMessage = errorMessages[0].ErrorMessage;
                Assert.Contains("Email cần có @", errorMessage);
            }
        }

        [Fact]
        // 4. Đăng ký với pass không đủ 8 ký tự
        public async Task Test_Register_PasswordTooShort()
        {
            // Tạo một cơ sở dữ liệu ảo trên bộ nhớ để test
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // context với cơ sở dữ liệu ảo
            using (var context = new PetServicesContext(options))
            { 
                // Tạo đối tượng giả (mock) cho IMapper và IConfiguration
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                // Khởi tạo một đối tượng controller AccountController với cơ sở dữ liệu ảo và các đối tượng mock
                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                // Tạo một đối tượng RegisterDTO với Email và Password 
                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65@gmail.com",
                    Password = "123456" // Mật khẩu có ít hơn 8 ký tự
                };

                // Gọi action Register trên controller và nhận kết quả trả về
                var result = await controller.Register(registerDto) as BadRequestObjectResult;  

                Assert.NotNull(result); 
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mật khẩu phải có ít nhất 8 ký tự!", result.Value); 
            }
        }

        [Fact]
        // 5. Đăng ký với pass rỗng
        public async Task Test_Register_PasswordIsEmpty()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65@gmail.com",
                    Password = "" // Mật khẩu có ít hơn 8 ký tự
                };

                var result = await controller.Register(registerDto) as BadRequestObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mật khẩu không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Đăng ký với pass không chứa khoảng trắng
        public async Task Test_Register_PasswordNoWhiteSpace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65@gmail.com",
                    Password = "1 234567" // Mật khẩu chứa khoảng trắng
                };

                var result = await controller.Register(registerDto) as BadRequestObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mật khẩu không được chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. Đăng ký với pass có ký tự đặc biệt
        public async Task Test_Register_PasswordSpecialCharacters()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var registerDto = new RegisterDTO
                {
                    Email = "psmsg65@gmail.com",
                    Password = "123456@7" // Mật khẩu chứa ký tự đặc biệt
                };

                var result = await controller.Register(registerDto) as BadRequestObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mật khẩu không được chứa ký tự đặc biệt!", result.Value);
            }
        }
    }
}
