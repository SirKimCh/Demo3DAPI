﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Demo3DAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlayerAccountsController : ControllerBase
    {
        private readonly IPlayerAccountService _accountService;

        public PlayerAccountsController(IPlayerAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xem tất cả tài khoản (Admin only)", Description = "Lấy danh sách tất cả các tài khoản người chơi")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerAccountBasicDto>))]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem một tài khoản", Description = "Admin: xem bất kỳ, User: chỉ xem account của mình")]
        [SwaggerResponse(200, "Thành công", typeof(PlayerAccountResponseDto))]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only view their own account, Admin can view any
            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var account = await _accountService.GetAccountById(id);
            if (account == null) 
                return NotFound(new { message = $"Account with ID {id} not found." });
            return Ok(account);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Thêm tài khoản mới (Admin only)", Description = "Tạo tài khoản người chơi mới (password sẽ được hash tự động)")]
        [SwaggerResponse(201, "Tạo thành công", typeof(PlayerAccountResponseDto))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerAccountDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newAccount = await _accountService.CreateAccount(createDto);
                return CreatedAtAction(nameof(GetById), new { id = newAccount.ID }, newAccount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}")]
        [SwaggerOperation(Summary = "Sửa tài khoản", Description = "Admin: sửa bất kỳ, User: chỉ sửa account của mình")]
        [SwaggerResponse(200, "Cập nhật thành công")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerAccountDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only update their own account, Admin can update any
            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var result = await _accountService.UpdateAccount(id, updateDto);
            if (!result) 
                return NotFound(new { message = $"Account with ID {id} not found." });
            return Ok(new { message = "Account updated successfully." });
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xóa tài khoản (Admin only)", Description = "Xóa tài khoản theo ID (tự động xóa tất cả Character liên quan, KHÔNG THỂ XÓA ADMIN)")]
        [SwaggerResponse(200, "Xóa thành công")]
        [SwaggerResponse(400, "Không thể xóa Admin")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _accountService.DeleteAccount(id);
                if (!result) 
                    return NotFound(new { message = $"Account with ID {id} not found." });
                return Ok(new { message = "Account deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

