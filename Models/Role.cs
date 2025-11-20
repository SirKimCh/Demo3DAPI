﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Demo3DAPI.Models
{
    public class Role
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public virtual ICollection<PlayerAccount> PlayerAccounts { get; set; } = new List<PlayerAccount>();
    }
}

