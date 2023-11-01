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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 2. Email(null) + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
        public async Task Test_Register_EmptyEmail()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 3. Email(thiếu @) + Pass + Phone  + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 4. Email(trùng email) + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 5. Email(có khoảng trắng) + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 6. Email + Pass(null) + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 7. Email + Pass(7 ký tự) + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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
        // 8. Email + Pass(có khoảng trắng) + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
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

        [Fact]
        // 9. Email + Pass(có @) + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "0987654321",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678@",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Mật khẩu không được chứa ký tự đặc biệt!", result.Value.ToString());
            }
        }

        [Fact]
        // 10. Email + Pass + Phone(null) + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 11. Email + Pass + Phone(9 số) + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765432",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại phải có 10 ký tự!", result.Value.ToString());
            }
        }

        [Fact]
        // 12. Email + Pass + Phone(k bđ = số 0) + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "9876543210",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại phải bắt đầu bằng số 0!", result.Value.ToString());
            }
        }

        [Fact]
        // 13. Email + Pass + Phone(có chữ) + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765432a",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không phải là số! Bạn cần nhập số điện thoại ở dạng số!", result.Value.ToString());
            }
        }

        [Fact]
        // 14. Email + Pass + Phone(có khoảng trắng) + ImageCertificate + Province + District + Commune + FirstName + LastName
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

                var registerDto = new RegisterPartnerDTO
                {
                    FirstName = "Pet",
                    LastName = "Service",
                    Phone = "098765 432",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Số điện thoại không được chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 15. Email + Pass + Phone + ImageCertificate(Null) + Province + District + Commune + FirstName + LastName
        public async Task Test_Register_Image_Null()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = ""
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Bạn cần cung cấp hình ảnh chứng chỉ!", result.Value.ToString());
            }
        }

        [Fact]
        // 16. Email + Pass + Phone + ImageCertificate(Null) + Province + District + Commune + FirstName + LastName
        public async Task Test_Register_Image_WhiteSpace()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/N sSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("URL ảnh không chứa khoảng trắng!", result.Value.ToString());
            }
        }

        [Fact]
        // 17. Email + Pass + Phone + ImageCertificate + Province(Null) + District + Commune + FirstName + LastName
        public async Task Test_Register_Province_WhiteSpace()
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
                    Province = "",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tỉnh không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 18. Email + Pass + Phone + ImageCertificate + Province(@123) + District + Commune + FirstName + LastName
        public async Task Test_Register_Province_NumberSpecialWord()
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
                    Province = "Hà Nội @123",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tỉnh chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }

        [Fact]
        // 19. Email + Pass + Phone + ImageCertificate + Province + District(Null) + Commune + FirstName + LastName
        public async Task Test_Register_District_WhiteSpace()
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
                    Province = "Hà Nội",
                    District = "",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Huyện không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 20. Email + Pass + Phone + ImageCertificate + Province + District(@123) + Commune + FirstName + LastName
        public async Task Test_Register_District_NumberSpecialWord()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất @123",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Huyện chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }

        [Fact]
        // 21. Email + Pass + Phone + ImageCertificate + Province + District + Commune(Null) + FirstName + LastName
        public async Task Test_Register_Commune_WhiteSpace()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Xã không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 22. Email + Pass + Phone + ImageCertificate + Province + District + Commune(@123) + FirstName + LastName
        public async Task Test_Register_Commune_NumberSpecialWord()
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
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc @123",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Xã chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }

        [Fact]
        // 23. Email + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName(Null) + LastName
        public async Task Test_Register_FirstName_WhiteSpace()
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
                    FirstName = "",
                    LastName = "Service",
                    Phone = "0987654321",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Họ không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 24. Email + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName(@123) + LastName
        public async Task Test_Register_FirstName_NumberSpecialWord()
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
                    FirstName = "Pet @123",
                    LastName = "Service",
                    Phone = "0987654321",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Họ chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }

        [Fact]
        // 25. Email + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName(Null)
        public async Task Test_Register_LastName_WhiteSpace()
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
                    LastName = "",
                    Phone = "0987654321",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tên không được để trống!", result.Value.ToString());
            }
        }

        [Fact]
        // 26. Email + Pass + Phone + ImageCertificate + Province + District + Commune + FirstName + LastName(@123)
        public async Task Test_Register_LastName_NumberSpecialWord()
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
                    LastName = "Service @123",
                    Phone = "0987654321",
                    Province = "Hà Nội",
                    District = "Thạch Thất",
                    Commune = "Hoà Lạc",
                    Email = "psmsg65@gmail.com",
                    Password = "12345678",
                    ImageCertificate = "https://s.net.vn/NsSG"
                };

                var result = await controller.RegisterPartner(registerDto) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Contains("Tên chỉ chấp nhận các ký tự văn bản và không được chứa ký tự đặc biệt hoặc số.", result.Value.ToString());
            }
        }
    }
}
