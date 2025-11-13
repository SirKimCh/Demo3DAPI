using System.ComponentModel.DataAnnotations;

namespace Demo3DAPI.DTOs
{
    public class CreatePlayerAccountDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
    }

    public class UpdatePlayerAccountDto
    {
        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}

