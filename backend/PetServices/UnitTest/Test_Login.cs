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
    public class Test_Login
    {
        [Fact]
        // 1. Đăng nhập thành công
        public async Task Test_Login_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");

                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" },
                    UserInfo = new UserInfo { FirstName = "Thị", LastName = "Nở" },
                    Status = true // Assuming the account is activated
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "12345678"
            };

            // Act
            var result = await controller.Login(loginForm) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var loginResponse = result.Value as LoginResponse;
            Assert.NotNull(loginResponse);
            Assert.NotEmpty(loginResponse.Token);
            Assert.Equal(true, loginResponse.Successful);
            Assert.Equal("CUSTOMER", loginResponse.RoleName);
            Assert.Equal("Thị Nở", loginResponse.UserName);
        }

        [Fact]
        // 2. Email + Pass(sai)
        public async Task Test_Login_Fail_wrongPassword()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "1234abvcaas"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Đăng nhập không hợp lệ.", result.Value);
        }

        [Fact]
        // 3. Email + Pass(null)
        public async Task Test_Login_Fail_PasswordIsEmpty()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "" 
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Mật khẩu không được để trống!", result.Value);
        }

        [Fact]
        // 4. Email + Pass(7 ký tự)
        public async Task Test_Login_Fail_PasswordTooShort()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "12345"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Mật khẩu phải có ít nhất 8 ký tự.", result.Value);
        }

        [Fact]
        // 5. Email + Pass(có khoảng trắng)
        public async Task Test_Login_Fail_PasswordContainsWhitespace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "1234 5678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Mật khẩu không được chứa khoảng trắng.", result.Value);
        }

        [Fact]
        // 6. Email + Pass(có ký tự đặc biệt)
        public async Task Test_Login_Fail_PasswordContainsSpecialCharacters()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "12345678@"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Mật khẩu không được chứa ký tự đặc biệt.", result.Value);
        }

        [Fact]
        // 7. Email(null) + Pass
        public async Task Test_Login_Fail_EmailIsEmpty()
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
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "",  
                Password = "12345678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Email không được để trống!", result.Value); 
        }

        [Fact]
        // 8. Email(chứa khoảng trắng) + Pass
        public async Task Test_Login_Fail_EmailContainsWhitespace()
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
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn     ",  
                Password = "12345678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Email không chứa khoảng trắng!", result.Value); 
        }

        [Fact]
        // 9. Email(thiếu @) + Pass
        public async Task Test_Login_Fail_InvalidEmailFormat()
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
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434.fpt.edu",
                Password = "12345678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.True(controller.ModelState.ContainsKey("Email không hợp lệ"));

            var errorMessages = controller.ModelState["Email không hợp lệ"].Errors;
            var errorMessage = errorMessages[0].ErrorMessage;
            Assert.Contains("Email cần có @", errorMessage);
        }

        [Fact]
        // 10 Email(sai định dạng) + Pass
        public async Task Test_Login_Fail_InvalidEmail_Format()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn", // Email không có dấu "@"
                    Password = "12345678",
                    Role = new Role { RoleName = "CUSTOMER" }
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            // Thiết lập cấu hình của JWT

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434",
                Password = "12345678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.True(controller.ModelState.ContainsKey("Email không hợp lệ"));

            var errorMessages = controller.ModelState["Email không hợp lệ"].Errors;
            var errorMessage = errorMessages[0].ErrorMessage;
            Assert.Contains("Email cần có @", errorMessage);
        }

        [Fact]
        // 11. Email not active
        public async Task Test_Login_Fail_Email_NotActive()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword("12345678");
                var testUser = new Account
                {
                    Email = "hungnvhe153434@fpt.edu.vn",
                    Password = hashedPassword,
                    Role = new Role { RoleName = "CUSTOMER" },
                    Status = false,
                };

                context.Accounts.Add(testUser);
                context.SaveChanges();
            }

            var mockMapper = new Mock<IMapper>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("Imsdg2wmP9DigIlxBV8czvXOa7XB442TBtioyAsVo5JEVCuOteFIGGJeo4nz4wa");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("http://project");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("http://localhost5xxx");
            mockConfiguration.Setup(x => x["Jwt:ExpiryInDays"]).Returns("1");

            var controller = new AccountController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

            var loginForm = new LoginForm
            {
                Email = "hungnvhe153434@fpt.edu.vn",
                Password = "12345678"
            };

            var result = await controller.Login(loginForm) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Tài khoản chưa được kích hoạt.", result.Value);
        }
    }
}
