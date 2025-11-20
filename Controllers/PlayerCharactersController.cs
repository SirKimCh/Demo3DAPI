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
    public class PlayerCharactersController : ControllerBase
    {
        private readonly IPlayerCharacterService _characterService;

        public PlayerCharactersController(IPlayerCharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xem tất cả nhân vật (Admin only)", Description = "Lấy danh sách tất cả các nhân vật")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacterResponseDto>))]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _characterService.GetAllCharacters();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem một nhân vật", Description = "Admin: xem bất kỳ, User: chỉ xem character của mình")]
        [SwaggerResponse(200, "Thành công", typeof(PlayerCharacterResponseDto))]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null) 
                return NotFound(new { message = $"Character with ID {id} not found." });

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only view their own characters, Admin can view any
            if (currentUserRole != "Admin" && character.PlayerAccountID != currentUserId)
            {
                return Forbid();
            }

            return Ok(character);
        }

        [HttpGet("Account/{accountId}")]
        [SwaggerOperation(Summary = "Xem nhân vật theo tài khoản", Description = "Admin: xem bất kỳ account, User: chỉ xem account của mình")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacterResponseDto>))]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only view their own account's characters, Admin can view any
            if (currentUserRole != "Admin" && currentUserId != accountId)
            {
                return Forbid();
            }

            var characters = await _characterService.GetCharactersByAccountId(accountId);
            return Ok(characters);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Thêm nhân vật mới", Description = "Admin: tạo cho bất kỳ account, User: chỉ tạo cho account của mình")]
        [SwaggerResponse(201, "Tạo thành công", typeof(PlayerCharacterResponseDto))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc PlayerAccountID không tồn tại")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerCharacterDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only create characters for their own account, Admin can create for any
            if (currentUserRole != "Admin" && createDto.PlayerAccountID != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var newCharacter = await _characterService.CreateCharacter(createDto);

                if (newCharacter == null)
                {
                    return BadRequest(new { message = $"PlayerAccountID {createDto.PlayerAccountID} does not exist." });
                }

                return CreatedAtAction(nameof(GetById), new { id = newCharacter.ID }, newCharacter);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}")]
        [SwaggerOperation(Summary = "Sửa nhân vật", Description = "Admin: sửa bất kỳ, User: chỉ sửa character của mình")]
        [SwaggerResponse(200, "Cập nhật thành công")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerCharacterDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var character = await _characterService.GetCharacterById(id);
            if (character == null)
                return NotFound(new { message = $"Character with ID {id} not found." });

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only update their own characters, Admin can update any
            if (currentUserRole != "Admin" && character.PlayerAccountID != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var result = await _characterService.UpdateCharacter(id, updateDto);
                if (!result) 
                    return NotFound(new { message = $"Character with ID {id} not found." });
                return Ok(new { message = "Character updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa nhân vật", Description = "Admin: xóa bất kỳ, User: chỉ xóa character của mình")]
        [SwaggerResponse(200, "Xóa thành công")]
        [SwaggerResponse(403, "Không có quyền truy cập")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Delete(int id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null)
                return NotFound(new { message = $"Character with ID {id} not found." });

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // User can only delete their own characters, Admin can delete any
            if (currentUserRole != "Admin" && character.PlayerAccountID != currentUserId)
            {
                return Forbid();
            }

            var result = await _characterService.DeleteCharacter(id);
            if (!result) 
                return NotFound(new { message = $"Character with ID {id} not found." });
            return Ok(new { message = "Character deleted successfully." });
        }
    }
}

