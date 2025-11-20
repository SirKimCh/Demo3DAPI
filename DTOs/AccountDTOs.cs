﻿using System.ComponentModel.DataAnnotations;

namespace Demo3DAPI.DTOs
{
    public class CreatePlayerAccountDto
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }

    public class UpdatePlayerAccountDto
    {
        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }

    public class PlayerAccountResponseDto
    {
        public int ID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public List<CharacterBasicDto>? Characters { get; set; }
    }

    public class PlayerAccountBasicDto
    {
        public int ID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
    }
}

