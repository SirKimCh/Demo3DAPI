using Microsoft.AspNetCore.Mvc;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Demo3DAPI.Models;
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

        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Xem tất cả nhân vật", Description = "Lấy danh sách tất cả các nhân vật")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacter>))]
        public async Task<IActionResult> GetAll()
        {
            var characters = await _characterService.GetAllCharacters();
            return Ok(characters);
        }

        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Xem một nhân vật", Description = "Lấy thông tin chi tiết nhân vật theo ID")]
        [SwaggerResponse(200, "Thành công", typeof(PlayerCharacter))]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> GetById(int id)
        {
            var character = await _characterService.GetCharacterById(id);
            if (character == null) return NotFound();
            return Ok(character);
        }

        [HttpGet("GetByAccountId/{accountId}")]
        [SwaggerOperation(Summary = "Xem nhân vật theo tài khoản", Description = "Lấy danh sách tất cả nhân vật của một tài khoản")]
        [SwaggerResponse(200, "Thành công", typeof(IEnumerable<PlayerCharacter>))]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            var characters = await _characterService.GetCharactersByAccountId(accountId);
            return Ok(characters);
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Thêm nhân vật mới", Description = "Tạo nhân vật mới (bắt buộc phải có PlayerAccountID)")]
        [SwaggerResponse(201, "Tạo thành công", typeof(PlayerCharacter))]
        [SwaggerResponse(400, "PlayerAccountID không tồn tại")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerCharacterDto createDto)
        {
            var newCharacter = await _characterService.CreateCharacter(createDto);

            if (newCharacter == null)
            {
                return BadRequest(new { message = $"PlayerAccountID {createDto.PlayerAccountID} does not exist." });
            }

            return CreatedAtAction(nameof(GetById), new { id = newCharacter.ID }, newCharacter);
        }

        [HttpPost("Update/{id}")]
        [SwaggerOperation(Summary = "Sửa nhân vật", Description = "Cập nhật thông tin nhân vật theo ID")]
        [SwaggerResponse(200, "Cập nhật thành công")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerCharacterDto updateDto)
        {
            var result = await _characterService.UpdateCharacter(id, updateDto);
            if (!result) return NotFound(new { message = $"Character with ID {id} not found." });
            return Ok(new { message = "Character updated successfully." });
        }

        [HttpPost("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa nhân vật", Description = "Xóa nhân vật theo ID")]
        [SwaggerResponse(200, "Xóa thành công")]
        [SwaggerResponse(404, "Không tìm thấy nhân vật")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _characterService.DeleteCharacter(id);
            if (!result) return NotFound(new { message = $"Character with ID {id} not found." });
            return Ok(new { message = "Character deleted successfully." });
        }
    }
}

