using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SecurityProj2.Models
{
    public class PasswordKey
    {
        [Key]
        public Guid PasswordId { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Accounts")]
        public string HandleName { get; set; }
  
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public bool includeUpper { get; set; }
        public bool includeNumbers { get; set; }
        public bool includeSpecial { get; set; }
        public int passwordLength { get; set; }
        
    }

    public class PasswordKeyDBContext : DbContext
    {
        public DbSet<PasswordKey> Keys { get; set; }
    }
}