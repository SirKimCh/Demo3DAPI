﻿using System.ComponentModel.DataAnnotations;

namespace Demo3DAPI.DTOs
{
    public class CreatePlayerCharacterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1")]
        public int Level { get; set; } = 1;

        [Required]
        public int PlayerAccountID { get; set; }
    }

    public class UpdatePlayerCharacterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1")]
        public int Level { get; set; }
    }

    public class PlayerCharacterResponseDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int PlayerAccountID { get; set; }
        public string? PlayerAccountUserName { get; set; }
        public string? PlayerAccountFullName { get; set; }
    }

    public class CharacterBasicDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Level { get; set; }
    }
}

