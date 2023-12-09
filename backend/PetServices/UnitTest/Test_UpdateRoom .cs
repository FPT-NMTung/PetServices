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
    public class Test_UpdateRoom
    {
        [Fact]
        // 1. Update phòng thành công
        public async Task Test_UpdateRoom_Success()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng mới cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo dễ thương",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhật phòng thành công!", result.Value);
            }
        }

        [Fact]
        // 2. RoomName(Null)
        public async Task Test_UpdateProduct_RoomName_Null()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "",
                    Desciptions = "Phòng dành cho những chú mèo dễ thương",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 3. RoomName(To long)
        public async Task Test_UpdateProduct_RoomName_ToLong()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var longRoomName = new string('A', 501);
                var testUpdateRoom = new RoomDTO
                {
                    RoomName = longRoomName,
                    Desciptions = "Phòng dành cho những chú mèo dễ thương",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!", result.Value);
            }
        }

        [Fact]
        // 4. Description(Null)
        public async Task Test_UpdateProduct_Description_Null()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Mô tả không được để trống!", result.Value);
            }
        }

        [Fact]
        // 5. Image(Null)
        public async Task Test_UpdateProduct_Image_Null()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Ảnh phòng không được để trống!", result.Value);
            }
        }

        [Fact]
        // 6. Image(có khoảng trắng)
        public async Task Test_UpdateProduct_Image_WhiteSpace()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/ M9F4",
                    Price = 15000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("URL ảnh không chứa khoảng trắng!", result.Value);
            }
        }

        [Fact]
        // 7. RoomCategories(không tồn tại)
        public async Task Test_UpdateProduct_RoomCategories_NotExist()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 15000,
                    RoomCategoriesId = 2,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Loại phòng không tồn tại!", result.Value);
            }
        }

        [Fact]
        // 8. Change room status successfully
        public async Task Test_ChangeStatusRoom_Success()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new PetServicesContext(options))
            {
                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Chó",
                    Desciptions = "Phòng dành cho những chú chó đáng yêu",
                    Picture = "https://s.net.vn/NsSG",
                    Price = 10000,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    Status = true
                };

                context.Rooms.Add(room);
                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusRoom(1, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
                Assert.Equal("Cập nhật phòng thành công!", result.Value);               
            }
        }

        [Fact]
        // 9. Change room status for a non-existent room
        public async Task Test_ChangeStatusRoom_RoomNotFound()
        {
            var options = new DbContextOptionsBuilder<PetServicesContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

            using (var context = new PetServicesContext(options))
            {
                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var newStatus = false;

                var result = await controller.ChangeStatusRoom(1, newStatus) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Không tìm thấy phòng cần thay đổi.", result.Value);
            }
        }

        [Fact]
        // 10. Update Price = 0
        public async Task Test_UpdateRoom_PriceZero()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng mới cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo dễ thương",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 0,
                    RoomCategoriesId = 1,
                    Slot = 3,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Giá phải lớn hơn 0!", result.Value);
            }
        }

        [Fact]
        // 11. Update Price = 0
        public async Task Test_UpdateRoom_SlotZero()
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

                var service = new Service
                {
                    ServiceId = 1
                };

                context.Services.Add(service);

                var room = new Room
                {
                    RoomId = 1,
                    RoomName = "Phòng cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo đáng yêu",
                    Picture = "https://s.net.vn/M9F4",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 2
                };

                context.Rooms.Add(room);

                context.SaveChanges();

                var mockMapper = new Mock<IMapper>();
                var mockConfiguration = new Mock<IConfiguration>();

                var controller = new RoomController(new PetServicesContext(options), mockMapper.Object, mockConfiguration.Object);

                var testUpdateRoom = new RoomDTO
                {
                    RoomName = "Phòng mới cho Mèo",
                    Desciptions = "Phòng dành cho những chú mèo dễ thương",
                    Picture = "https://s.net.vn/M9F5",
                    Price = 12000,
                    RoomCategoriesId = 1,
                    Slot = 0,
                    ServiceIds = new List<int> { 1 }
                };

                var result = await controller.UpdateRoom(testUpdateRoom, 1) as ObjectResult;

                Assert.NotNull(result);
                Assert.Equal(400, result.StatusCode);
                Assert.Equal("Số lượng Slot phải lớn hơn 0!", result.Value);
            }
        }
    }
}
