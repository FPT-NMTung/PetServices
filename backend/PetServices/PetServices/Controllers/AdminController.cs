﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetServices.DTO;
using PetServices.Form;
using PetServices.Models;
using System.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Transactions;
using System.Net.Http.Json;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace PetServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private PetServicesContext _context;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminController(PetServicesContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetAllAccount")]
        public async Task<ActionResult> GetAllAccountTest()
        {
            var acc = await _context.Accounts.ToListAsync();

            return Ok(acc);
        }

        [HttpGet("GetAllUser")]
        public async Task<ActionResult> GetAllUser()
        {
            var user = await _context.UserInfos.ToListAsync();

            return Ok(user);
        }

        [HttpGet("GetAllPartner")]
        public async Task<ActionResult> GetAllPartner()
        {
            var partner = await _context.PartnerInfos.ToListAsync();

            return Ok(partner);
        }

        [HttpGet("GetAllAccountByAdmin")]
        public async Task<ActionResult> GetAllAccount()
        {
            var accounts = await _context.Accounts
                            .Include(a => a.UserInfo)
                            .Include(a => a.PartnerInfo)
                            .Include(a => a.Role)
                            .ToListAsync();

            var accountsViewModel = _mapper.Map<List<AccountByAdminDTO>>(accounts);

            return Ok(accountsViewModel);
        }

        [HttpGet("GetRole")]
        public async Task<ActionResult> GetRole()
        {
            var roles = await _context.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpPut("UpdateAccount")]
        public async Task<ActionResult> UpdateAccount(string email, int roleId, bool status)
        {
            try
            {
                var account = await _context.Accounts
                            .Include(a => a.UserInfo)
                            .Include(a => a.PartnerInfo)
                            .Where(a => a.Email == email).FirstOrDefaultAsync();
                if (account == null)
                {
                    return BadRequest("Không tìm thấy tài khoản");
                }

                else if (account.RoleId == roleId && account.Status == status)
                {
                    return BadRequest("Bạn không có gì thay đổi so với ban đầu.");
                }

                else
                {
                    if (account.PartnerInfoId == null && roleId == 4)
                    {
                        account.PartnerInfo = new PartnerInfo
                        {
                            FirstName = account.UserInfo?.FirstName ?? null,
                            LastName = account.UserInfo?.LastName ?? null,
                            Phone = account.UserInfo?.Phone ?? null,
                            Province = account.UserInfo?.Province ?? null,
                            District = account.UserInfo?.District ?? null,
                            Commune = account.UserInfo?.Commune ?? null,
                            Address = account.UserInfo?.Address ?? null,
                            Descriptions = account.UserInfo?.Descriptions ?? null,
                            ImagePartner = account.UserInfo?.ImageUser ?? null,
                        };
                    }

                    else if (account.UserInfoId == null && roleId != 4)
                    {
                        account.UserInfo = new UserInfo
                        {
                            FirstName = account.PartnerInfo?.FirstName ?? null,
                            LastName = account.PartnerInfo?.LastName ?? null,
                            Phone = account.PartnerInfo?.Phone ?? null,
                            Province = account.PartnerInfo?.Province ?? null,
                            District = account.PartnerInfo?.District ?? null,
                            Commune = account.PartnerInfo?.Commune ?? null,
                            Address = account.PartnerInfo?.Address ?? null,
                            Descriptions = account.PartnerInfo?.Descriptions ?? null,
                            ImageUser = account.PartnerInfo?.ImagePartner ?? null,
                        };
                    }

                    account.RoleId = roleId;
                    account.Status = status;
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    

                    return Ok(account);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPost("AddAccount")]
        public async Task<ActionResult> AddAccount(string email, string password, int roleId )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Accounts.Where(a => a.Email == email).FirstOrDefault() != null)
            {
                return BadRequest("Email đã tồn tại. Vui lòng nhập email khác!");
            }

            if (!IsValidEmail(email))
            {
                ModelState.AddModelError("Email không hợp lệ", "Email cần có @");
                return BadRequest(ModelState);
            }

            if (!IsValidPassword(password))
            {
                ModelState.AddModelError("Mật khẩu không hợp lệ", "Mật khẩu cần tối thiểu 8 ký tự và không chứa ký tự đặc biệt!");
                return BadRequest(ModelState);
            }

            var newAcc = new Account
            {
                Email = email,
                Password = password,
                Status = true,
                RoleId = roleId
            };

            await _context.Accounts.AddAsync(newAcc);
            await _context.SaveChangesAsync();

            if (roleId == 4)
            {
                newAcc.PartnerInfo = new PartnerInfo();
            }
            else
            {
                newAcc.UserInfo = new UserInfo();
            }

            _context.Update(newAcc);
            await _context.SaveChangesAsync();

            return Ok(newAcc);
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8 && !password.Contains(" ");
        }
    }
}
