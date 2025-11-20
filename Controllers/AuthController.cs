using Microsoft.AspNetCore.Mvc;
using Demo3DAPI.DTOs;
using Demo3DAPI.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Demo3DAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPlayerAccountService _accountService;
        private readonly IJwtService _jwtService;

        public AuthController(IPlayerAccountService accountService, IJwtService jwtService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
        }

        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Đăng nhập", Description = "Đăng nhập với username và password, trả về JWT token")]
        [SwaggerResponse(200, "Đăng nhập thành công", typeof(LoginResponseDto))]
        [SwaggerResponse(401, "Username hoặc password không đúng")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.Login(loginDto, _jwtService);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(result);
        }

        [HttpPost("Register")]
        [SwaggerOperation(Summary = "Đăng ký", Description = "Đăng ký tài khoản mới (tự động Role = User)")]
        [SwaggerResponse(201, "Đăng ký thành công", typeof(PlayerAccountResponseDto))]
        [SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc username đã tồn tại")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newAccount = await _accountService.Register(registerDto);
                return CreatedAtAction(nameof(Register), new { id = newAccount.ID }, newAccount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

