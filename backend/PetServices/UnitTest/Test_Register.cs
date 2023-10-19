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
                    Email = "hungnvhe153434@gpt.edu.vn",
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
                    Email = "hungnvhe153434@gpt.edu.vn",
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
                    Password = "123456"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.True(controller.ModelState.ContainsKey("Mật khẩu không hợp lệ"));
                var errorMessages = controller.ModelState["Mật khẩu không hợp lệ"].Errors;

                var errorMessage = errorMessages[0].ErrorMessage;
                Assert.Contains("Mật khẩu cần tối thiểu 8 ký tự và không chứa ký tự đặc biệt", errorMessage);
            }
        }
    }
}
