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
            var options = new DbContextOptionsBuilder<PetServicesContext> ()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
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
                    Slot = 3
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Thêm phòng thành công!", result.Value);
            }
        }

        [Fact]
        // 2. NameRoom(null)
        public async Task Test_AddRoom_RoomName_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
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
                    Slot = 3
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. NameRoom(lengh > 500)
        public async Task Test_AddRoom_RoomName_Length()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
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
                    Slot = 3
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Desciptions(null)
        public async Task Test_AddRoom_Desciptions_Null()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
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
                    Slot = 3
                };

                var result = await controller.AddRoom(testAddRoom) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }
    }
}
