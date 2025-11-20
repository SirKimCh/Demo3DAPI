﻿using Microsoft.AspNetCore.Mvc;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Demo3DAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerCharactersController : ControllerBase
    {
        private readonly IPlayerCharacterService _characterService;

        public PlayerCharactersController(IPlayerCharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Xem tất cả nhân vật", Description = "Lấy danh sách tất cả các nhân vật")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacterResponseDto>))]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _characterService.GetAllCharacters();
            return Ok(characters);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Xem một nhân vật", Description = "Lấy thông tin chi tiết nhân vật theo ID")]
        [SwaggerResponse(200, "Thành công", typeof(PlayerCharacterResponseDto))]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null) 
                return NotFound(new { message = $"Character with ID {id} not found." });
            return Ok(character);
        }

        [HttpGet("Account/{accountId}")]
        [SwaggerOperation(Summary = "Xem nhân vật theo tài khoản", Description = "Lấy danh sách tất cả nhân vật của một tài khoản")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacterResponseDto>))]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            var characters = await _characterService.GetCharactersByAccountId(accountId);
            return Ok(characters);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Thêm nhân vật mới", Description = "Tạo nhân vật mới (bắt buộc phải có PlayerAccountID hợp lệ, Level >= 1)")]
        [SwaggerResponse(201, "Tạo thành công", typeof(PlayerCharacterResponseDto))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc PlayerAccountID không tồn tại")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerCharacterDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        [SwaggerOperation(Summary = "Sửa nhân vật", Description = "Cập nhật thông tin nhân vật theo ID (Name và Level, Level >= 1)")]
        [SwaggerResponse(200, "Cập nhật thành công")]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerCharacterDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        [SwaggerOperation(Summary = "Xóa nhân vật", Description = "Xóa nhân vật theo ID")]
        [SwaggerResponse(200, "Xóa thành công")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _characterService.DeleteCharacter(id);
            if (!result) 
                return NotFound(new { message = $"Character with ID {id} not found." });
            return Ok(new { message = "Character deleted successfully." });
        }
    }
}

