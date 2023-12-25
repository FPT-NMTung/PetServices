using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Models;
using System;
using System.Collections.Generic;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public RoomController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllRoom")]
        public async Task<ActionResult> GetAllRoom()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomCategories)
                .OrderByDescending(o => o.RoomId)
                .ToListAsync();

            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("GetAllRoomCustomer")]
        public async Task<ActionResult> GetAllRoomCustomer()
        {
            var rooms = await _context.Rooms.Include(r => r.RoomCategories).Where(r => r.Status == true).ToListAsync();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("GetServiceInRoom")]
        public async Task<ActionResult> GetServiceInRoom(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            var services = _mapper.Map<List<ServiceDTO>>(room.Services.Where(s => s.Status == true).ToList());

            return Ok(services);
        }

        [HttpGet("GetServiceOutRoom")]
        public async Task<ActionResult> GetServiceOutRoom(int roomId)
        {
            var allServices = await _context.Services.ToListAsync();

            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            var servicesInRoom = room?.Services.Select(s => s.ServiceId).ToList();

            var remainingServices = allServices.Where(s => !servicesInRoom.Contains(s.ServiceId))
                                               .Select(service => _mapper.Map<ServiceDTO>(service))
                                               .ToList();

            var services = _mapper.Map<List<ServiceDTO>>(remainingServices.Where(s => s.Status == true).ToList());

            return Ok(services);
        }

        [HttpGet("GetRoom/{roomId}")]
        public async Task<ActionResult> GetRoom(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .Include(r => r.RoomCategories)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var roomDto = _mapper.Map<RoomDTO>(room);

            roomDto.ServiceIds = room.Services.Select(s => s.ServiceId).ToList();

            return Ok(roomDto);
        }

        [HttpGet("GetAllRoomDetail")]
        public async Task<ActionResult> GetAllRoomDetail()
        {
            var roomList = await _context.Rooms
                .Include(r => r.Services)
                .Include(r => r.RoomCategories)
                .ToListAsync();

            var roomlist = _mapper.Map<List<RoomDTO>>(roomList);

            foreach (var room in roomlist)
            {
                var roomservice = roomList.FirstOrDefault(r => r.RoomId == room.RoomId);

                room.ServiceIds = roomservice.Services.Select(s => s.ServiceId).ToList();
            }

            return Ok(roomlist);
        }

        [HttpGet("GetRoomCategory")]
        public async Task<ActionResult> GetRoomCategory()
        {
            var roomCategory = await _context.RoomCategories.Where(r => r.Status == true)
                .ToListAsync();

            return Ok(_mapper.Map<List<RoomCategoryDTO>>(roomCategory));
        }

        [HttpGet("GetAllRoomWhenCategoryTrue")]
        public IActionResult GetRoomWhenCategoryTrue()
        {
            // Filter services based on the status of their associated service categories
            List<Room> room = _context.Rooms
                .Include(s => s.RoomCategories)
                .Where(s => s.RoomCategories.Status == true) // Filter based on service category status
                .ToList();

            return Ok(_mapper.Map<List<RoomDTO>>(room));
        }

        [HttpGet("GetAllService")]
        public async Task<ActionResult> GetAllService()
        {
            var listService = await _context.Services.Where(r => r.Status == true).ToListAsync();

            return Ok(listService);
        }

        [HttpPost("AddRoom")]
        public async Task<ActionResult> AddRoom(RoomDTO roomDTO)
        {
            // check tên phòng
            if (string.IsNullOrWhiteSpace(roomDTO.RoomName))
            {
                string errorMessage = "Tên phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            if (roomDTO.RoomName.Length > 500)
            {
                string errorMessage = "Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(roomDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(roomDTO.Picture))
            {
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (roomDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check số lượng Slot
            if (roomDTO.Slot <= 0)
            {
                string errorMessage = "Số lượng Slot phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (roomDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check loại phòng           
            var roomcategory = _context.RoomCategories.FirstOrDefault(r => r.RoomCategoriesId == roomDTO.RoomCategoriesId);

            if (roomcategory == null)
            {
                string errorMessage = "Loại phòng không tồn tại!";
                return BadRequest(errorMessage);
            }


            try
            {
                var newRoom = new Room
                {
                    RoomId = roomDTO.RoomId,
                    RoomName = roomDTO.RoomName,
                    Desciptions = roomDTO.Desciptions,
                    Picture = roomDTO.Picture,
                    Price = roomDTO.Price,
                    Slot = roomDTO.Slot,
                    Status = true,
                    RoomCategoriesId = roomDTO.RoomCategoriesId,
                };

                var services = _context.Services.Where(s => roomDTO.ServiceIds.Contains(s.ServiceId)).ToList();

                foreach (var service in services)
                {
                    newRoom.Services.Add(service);
                }

                await _context.Rooms.AddAsync(newRoom);
                await _context.SaveChangesAsync();

                /*return Ok(_mapper.Map<RoomDTO>(newRoom));*/
                return Ok("Thêm phòng thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("UpdateRoom")]
        public async Task<ActionResult> UpdateRoom(RoomDTO roomDTO, int roomId)
        {
            // check tên phòng
            if (string.IsNullOrWhiteSpace(roomDTO.RoomName))
            {
                string errorMessage = "Tên phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            if (roomDTO.RoomName.Length > 500)
            {
                string errorMessage = "Tên phòng vượt quá số ký tự. Tối đa 500 ký tự!";
                return BadRequest(errorMessage);
            }
            // check mô tả
            if (string.IsNullOrWhiteSpace(roomDTO.Desciptions))
            {
                string errorMessage = "Mô tả không được để trống!";
                return BadRequest(errorMessage);
            }
            // check ảnh
            if (string.IsNullOrWhiteSpace(roomDTO.Picture))
            {
                string errorMessage = "Ảnh phòng không được để trống!";
                return BadRequest(errorMessage);
            }
            else if (roomDTO.Picture.Contains(" "))
            {
                string errorMessage = "URL ảnh không chứa khoảng trắng!";
                return BadRequest(errorMessage);
            }
            // check số lượng Slot
            if (roomDTO.Slot <= 0)
            {
                string errorMessage = "Số lượng Slot phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check giá
            if (roomDTO.Price <= 0)
            {
                string errorMessage = "Giá phải lớn hơn 0!";
                return BadRequest(errorMessage);
            }
            // check loại phòng           
            var roomcategory = _context.RoomCategories.FirstOrDefault(r => r.RoomCategoriesId == roomDTO.RoomCategoriesId);

            if (roomcategory == null)
            {
                string errorMessage = "Loại phòng không tồn tại!";
                return BadRequest(errorMessage);
            }
            try
            {
                var room = await _context.Rooms.Include(r => r.RoomCategories).Include(r => r.Services).FirstOrDefaultAsync(p => p.RoomId == roomId);
                if (room == null)
                {
                    return BadRequest("Không tìm thấy phòng bạn chọn.");
                }

                var servicesToRemove = room.Services.ToList();
                room.Services.Clear();
                _context.SaveChanges();

                room.RoomName = roomDTO.RoomName;
                room.Desciptions = roomDTO.Desciptions;
                room.Picture = roomDTO.Picture;
                room.Price = roomDTO.Price;
                room.Status = roomDTO.Status;
                room.Slot = roomDTO.Slot;
                room.RoomCategoriesId = roomDTO.RoomCategoriesId;

                var services = _context.Services.Where(s => roomDTO.ServiceIds.Contains(s.ServiceId)).ToList();

                foreach (var service in services)
                {
                    room.Services.Add(service);
                }

                _context.Entry(room).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Cập nhật phòng thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPut("ChangeStatusRoom")]
        public async Task<ActionResult> ChangeStatusRoom(int RoomId, bool status)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(p => p.RoomId == RoomId);
                if (room == null)
                {
                    return BadRequest("Không tìm thấy phòng cần thay đổi.");
                }

                room.Status = status;

                _context.Entry(room).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Cập nhật phòng thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpGet("SearchRoomByDate")]
        public async Task<ActionResult> SearchRoomByDate(DateTime startDate, DateTime endDate)
        {
            var listroom = await _context.Rooms.ToListAsync();

            List<Room> room = new List<Room>();

            foreach (var r in listroom)
            {
                //tìm các hóa đơn của phòng được đặt trong khoảng thời gian 
                var orders = await _context.BookingRoomDetails.Where(o => o.RoomId == r.RoomId
                                            && ((startDate >= o.StartDate && startDate < o.EndDate) // time bắt đầu khách đặt nằm trong khoảng thời gian trong hóa đơn
                                            || (endDate > o.StartDate && endDate <= o.EndDate) // time kết thúc khách đặt nằm trong khoảng thời gian trong hóa đơn
                                            || (startDate <= o.StartDate && endDate >= o.EndDate))) // thời gian trong hóa đơn nằm trong khoảng thời gian khách đặt
                                                                .ToListAsync();

                int a = r.Slot ?? 0;

                if (a < 0) //Nếu số phòng = 0 đồng nghĩa với phòng đó chưa đi vào hoạt động
                {
                    continue;
                }
                else if (orders == null) // Nếu ko thấy hóa đơn nào thì đồng nghĩa với trong khoảng thời gian đó chưa có phòng nào được đặt
                {
                    room.Add(r);
                    continue;
                }
                else if (orders.Count() < a)
                { // small check nếu số hóa đơn nhỏ hơn số phòng thì giả sử mỗi hóa đơn 1 phòng thì vẫn còn phòng trống 
                    room.Add(r);
                    continue;
                }
                else
                {
                    // tạo room ảo lưu trữ và gán hóa đơn cho các phòng ( áp dụng thuật toán greedy algorithm )
                    List<List<BookingRoomDetail>> rooms = new List<List<BookingRoomDetail>>();

                    foreach (var order in orders)
                    {
                        bool added = false;

                        // Thử xếp vào các lớp đã có
                        foreach (var timeClass in rooms)
                        {
                            if (timeClass.All(o => order.EndDate <= o.StartDate || order.StartDate >= o.EndDate)) //kiểm tra xem hóa đơn order có thể được thêm vào lớp timeClass hay không.
                            {
                                timeClass.Add(order);
                                added = true; // nếu thời gian hoàn toàn phù hợp thì được add vào phòng
                                break;
                            }
                        }

                        // Nếu không thể xếp vào phòng nào đã có, tạo phòng mới
                        if (!added)
                        {
                            rooms.Add(new List<BookingRoomDetail> { order });
                        }

                        if (rooms.Count >= a) // trong trường hợp manager đổi lại số slot của phòng thì có thể phòng cũ sẽ bị dư ra.
                        {
                            continue;
                        }
                    }

                    bool addtoroom = false;

                    foreach (var timeClass in rooms)
                    {
                        if (timeClass.All(o => endDate <= o.StartDate || startDate >= o.EndDate)) // Kiểm tra xem khung giờ có thể được thêm vào lớp không
                        {
                            addtoroom = true;
                            break;
                        }
                    }

                    if (addtoroom)
                    {
                        room.Add(r);
                        continue;

                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return Ok(_mapper.Map<List<RoomDTO>>(room)); ;
        }

        [HttpGet("CheckSlotInRoom")]
        public async Task<ActionResult> CheckSlotInRoom(int RoomId, DateTime startDate, DateTime endDate)
        {
            try
            {
                //tìm phòng theo id
                var room = await _context.Rooms.FirstOrDefaultAsync(p => p.RoomId == RoomId);

                //tìm các hóa đơn của phòng được đặt trong khoảng thời gian 
                var orders = await _context.BookingRoomDetails.Where(o => o.RoomId == RoomId 
                                            && ((startDate >= o.StartDate && startDate < o.EndDate) // time bắt đầu khách đặt nằm trong khoảng thời gian trong hóa đơn
                                            || (endDate > o.StartDate && endDate <= o.EndDate) // time kết thúc khách đặt nằm trong khoảng thời gian trong hóa đơn
                                            || (startDate <= o.StartDate && endDate >= o.EndDate))) // thời gian trong hóa đơn nằm trong khoảng thời gian khách đặt
                                                                .ToListAsync();

                if (room == null)
                {
                    return NotFound("Không tìm thấy phòng.");
                }
                else
                {
                    int a = room.Slot ?? 0;

                    if (a < 0) //Nếu số phòng = 0 đồng nghĩa với phòng đó chưa đi vào hoạt động
                    {
                        return NotFound("Không tìm thấy phòng hợp lệ!");
                    }
                    else if ( orders == null) // Nếu ko thấy hóa đơn nào thì đồng nghĩa với trong khoảng thời gian đó chưa có phòng nào được đặt
                    {
                        return Ok("Còn phòng trống.");
                    }
                    else if (orders.Count() < a){ // small check nếu số hóa đơn nhỏ hơn số phòng thì giả sử mỗi hóa đơn 1 phòng thì vẫn còn phòng trống 
                        return Ok("Còn phòng trống.");
                    }
                    else
                    {
                        // tạo room ảo lưu trữ và gán hóa đơn cho các phòng ( áp dụng thuật toán greedy algorithm )
                        List<List<BookingRoomDetail>> rooms  = new List<List<BookingRoomDetail>>();

                        foreach (var order in orders)
                        {
                            bool added = false;

                            // Thử xếp vào các lớp đã có
                            foreach (var timeClass in rooms)
                            {
                                if (timeClass.All(o => order.EndDate <= o.StartDate || order.StartDate >= o.EndDate)) //kiểm tra xem hóa đơn order có thể được thêm vào lớp timeClass hay không.
                                {
                                    timeClass.Add(order);

                                    added = true; // nếu thời gian hoàn toàn phù hợp thì được add vào phòng
                                    break;
                                }
                            }

                            // Nếu không thể xếp vào phòng nào đã có, tạo phòng mới
                            if (!added)
                            {
                                rooms.Add(new List<BookingRoomDetail> { order });
                            }

                            if (rooms.Count >= a) // trong trường hợp manager đổi lại số slot của phòng thì có thể phòng cũ sẽ bị dư ra.
                            {
                                return NotFound("Không tìm thấy phòng hợp lệ!");
                            }
                        }

                        bool addtoroom = false;

                        foreach (var time in rooms)
                        {
                            if (time.All(o => endDate <= o.StartDate || startDate >= o.EndDate)) // Kiểm tra xem khung giờ có thể được thêm vào lớp không
                            {
                                addtoroom = true;
                                break;
                            }
                        }
                        if (addtoroom)
                        {
                            return Ok("Còn phòng trống.");

                        }
                        else
                        {
                            var optimaltime = (startDate, endDate);
                            var timeduration = TimeSpan.MinValue;

                            foreach (var time in rooms)
                            {
                                var sortedtime = time.OrderBy(o => o.StartDate).ToList();

                                sortedtime.Insert(0, new BookingRoomDetail { EndDate = startDate }); // tạo booking ảo (0, startDate) để tính trường hợp ngày bắt đầu khách muốn thuê tới ngày đầu tiên phòng bận.
                                sortedtime.Add(new BookingRoomDetail { StartDate = endDate }); // tạo booking ảo (EndDate, ...) để tính trường hợp ngày cuối cùng phòng bận tới ngày kết thúc khách muốn thuê.

                                for (int i = 0; i < sortedtime.Count - 1; i++)
                                {
                                    var gap = sortedtime[i + 1].StartDate - sortedtime[i].EndDate; // khoảng thời gian trống giữa 2 đơn đặt phòng liên tiếp

                                    if (gap > timeduration) // Nếu khoảng thời gian này lớn hơn khoảng thời gian lớn nhất đã tìm được
                                    {
                                        timeduration = (TimeSpan)gap;

                                        optimaltime = ((DateTime)sortedtime[i].EndDate, (DateTime)sortedtime[i + 1].StartDate);
                                    }
                                }
                            }

                            if (timeduration != TimeSpan.MinValue)
                            {
                                return BadRequest("Bạn có thể đặt phòng trong khoảng thời gian: " + optimaltime.startDate + " - " + optimaltime.endDate);
                            }
                            else
                            {
                                return NotFound("Không tìm thấy phòng hợp lệ!");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound($"Đã xảy ra lỗi: {ex.Message}");
            }
        }


        [HttpGet("CheckService")]
        public async Task<ActionResult> CheckService(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            var services = _mapper.Map<List<ServiceDTO>>(room.Services.Where(s => s.Status == true).ToList());

            var timeDuration = services.Sum(s => s.Time);

            return Ok(timeDuration);
        }
    }
}
