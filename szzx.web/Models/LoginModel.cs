using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="用户名必填")]        
        public string UserName { get; set; }

        [MaxLength(length:6,ErrorMessage ="最大6位")]
        [Required(ErrorMessage ="密码必填")]
        public string Password { get; set; }
    }
}
