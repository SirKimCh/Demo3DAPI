using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo3DAPI.Models
{
    public class PlayerCharacter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Level { get; set; }

        public int PlayerAccountID { get; set; }

        [ForeignKey("PlayerAccountID")]
        public virtual PlayerAccount? PlayerAccount { get; set; }
    }
}

