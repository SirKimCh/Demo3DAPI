using System.ComponentModel.DataAnnotations;

namespace Demo3DAPI.DTOs
{
    public class CreatePlayerCharacterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        [Required]
        public int PlayerAccountID { get; set; }
    }

    public class UpdatePlayerCharacterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public int Level { get; set; }
    }
}

