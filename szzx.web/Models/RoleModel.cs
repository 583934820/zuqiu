using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime AddTime { get; set; }
    }

    public class AddRoleModel
    {
        [Required(ErrorMessage = "角色名必填")]
        public string RoleName { get; set; }        
        public string RoleDesc { get; set; }
        public string CreatedBy { get; set; }
    }

    public class EditRoleModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public string UpdatedBy { get; set; }
    }
}
