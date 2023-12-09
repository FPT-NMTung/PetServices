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
    public class Test_AddRoom
    {       
        [Fact]
        // 1. Add phòng thành công
        public async Task Test_AddRoom_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Thêm phòng thành công!", result.Value);
            }
        }

        [Fact]
        // 2. RoomName(Null)
        public async Task Test_AddRoom_RoomName_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. RoomName(>500)
        public async Task Test_AddRoom_RoomName_ToLong()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longRoomName = new string('A', 501);
                var testAddRoom = new RoomDTO
                {
                    RoomName = longRoomName,
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciptions(Null)
        public async Task Test_AddRoom_Desciptions_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Image(Null)
        public async Task Test_AddRoom_Image_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Image(WhiteSpace)
        public async Task Test_AddRoom_Image_WhiteSpace()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/N sSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. RoomCategories(không tồn tại)
        public async Task Test_AddRoom_RoomCategories_NotExist()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 2,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại phòng không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Slot = 0
        public async Task Test_AddRoom_Slotzero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 0,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Số lượng Slot phải lớn hơn 0!", result.Value);
            }
        }

        [Fact]
        // 9. Price = 0
        public async Task Test_AddRoom_Pricezero()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var roomCategory = new RoomCategory
                {
                    RoomCategoriesId = 1
                };

                context.RoomCategories.Add(roomCategory);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testAddRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 0,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int>()
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }
    }
}
