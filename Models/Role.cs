using System.Collections.Generic;

namespace Demo3DAPI.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<PlayerAccount> PlayerAccounts { get; set; } = new List<PlayerAccount>();
    }
}

