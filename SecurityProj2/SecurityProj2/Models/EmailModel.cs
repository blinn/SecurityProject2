using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

//Model used by Email/RequestPage
namespace SecurityProj2.Models
{
    public class EmailModel
    {

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email format is required")]
        public virtual string Email { get; set; }
        [Required]
        public virtual string Message { get; set; }
        public virtual string Password { get; set; }
    }

    public class PasswordRecovery
    {
        public virtual string TempPassword { get; set; }
    }
}