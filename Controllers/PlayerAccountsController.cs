using Microsoft.AspNetCore.Mvc;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Demo3DAPI.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Demo3DAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerAccountsController : ControllerBase
    {
        private readonly IPlayerAccountService _accountService;

        public PlayerAccountsController(IPlayerAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Xem tất cả tài khoản", Description = "Lấy danh sách tất cả các tài khoản người chơi")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerAccount>))]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Xem một tài khoản", Description = "Lấy thông tin chi tiết tài khoản theo ID")]
        [SwaggerResponse(200, "Thành công", typeof(PlayerAccount))]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountService.GetAccountById(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Thêm tài khoản mới", Description = "Tạo tài khoản người chơi mới")]
        [SwaggerResponse(201, "Tạo thành công", typeof(PlayerAccount))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerAccountDto createDto)
        {
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

        [HttpPost("Update/{id}")]
        [SwaggerOperation(Summary = "Sửa tài khoản", Description = "Cập nhật thông tin tài khoản theo ID")]
        [SwaggerResponse(200, "Cập nhật thành công")]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerAccountDto updateDto)
        {
            var result = await _accountService.UpdateAccount(id, updateDto);
            if (!result) return NotFound(new { message = $"Account with ID {id} not found." });
            return Ok(new { message = "Account updated successfully." });
        }

        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa tài khoản", Description = "Xóa tài khoản theo ID (tự động xóa tất cả Character liên quan)")]
        [SwaggerResponse(200, "Xóa thành công")]
        [SwaggerResponse(404, "Không tìm thấy tài khoản")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountService.DeleteAccount(id);
            if (!result) return NotFound(new { message = $"Account with ID {id} not found." });
            return Ok(new { message = "Account deleted successfully." });
        }
    }
}

