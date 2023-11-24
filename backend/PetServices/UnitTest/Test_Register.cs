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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
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
        // 2. Email(null) + Pass + Phone
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Email không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 3. Email(thiếu @) + Pass + Phone 
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65gmail.com",
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
        // 4. Email(trùng email) + Pass + Phone 
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "hungnvhe153434@fpt.edu.vn", // Email đã tồn tại
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(409, result.StatusCode);
                Assert.Equal("Email đã được đăng ký", result.Value);
            }
        }

        [Fact]
        // 5. Email(có khoảng trắng) + Pass + Phone 
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg 65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Email không chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 6. Email + Pass(null) + Phone 
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = ""
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 7. Email + Pass(7 ký tự) + Phone 
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "1234567"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu phải có ít nhất 8 ký tự!", result.Value.ToString());
            }
        }

        [Fact]
        // 8. Email + Pass(có khoảng trắng) + Phone 
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

                var registerDto = new RegisterDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345 678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 9. Email + Pass(có @) + Phone
        public async Task Test_Register_Pass_Special()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678@"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được chứa ký tự đặc biệt!", result.Value.ToString());
            }
        }

        [Fact]
        // 10. Email + Pass + Phone(null)
        public async Task Test_Register_Phone_Null()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 11. Email + Pass + Phone(9 số)
        public async Task Test_Register_Phone_ToShort()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765432",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại phải có 10 ký tự!", result.Value.ToString());
            }
        }

        [Fact]
        // 12. Email + Pass + Phone(k bđ = số 0)
        public async Task Test_Register_Phone_NoZeroStart()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "9876543210",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại phải bắt đầu bằng số 0!", result.Value.ToString());
            }
        }

        [Fact]
        // 13. Email + Pass + Phone(có chữ)
        public async Task Test_Register_Phone_HaveWord()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765432a",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không phải là số! Bạn cần nhập số điện thoại ở dạng số!", result.Value.ToString());
            }
        }

        [Fact]
        // 14. Email + Pass + Phone(có khoảng trắng)
        public async Task Test_Register_Phone_WhiteSpace()
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
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765 432",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không được chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 15. FirstName null
        public async Task Test_Register_FirstName_Null()
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
                    FirstName = "",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Họ không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 16. FristName @
        public async Task Test_Register_FirstName_special()
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
                    FirstName = "Pet@@@123",
                    LastName = "Service",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Họ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }

        [Fact]
        // 17. LastName null
        public async Task Test_Register_LastName_Null()
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
                    FirstName = "Pet",
                    LastName = "",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tên không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 18. LastName @@@
        public async Task Test_Register_LastName_Special()
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
                    FirstName = "Pet",
                    LastName = "Service@123",
                    Phone = "0987654321",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678"
                };

                var result = await controller.Register(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tên chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }
    }
}
