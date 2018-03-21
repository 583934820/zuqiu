using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace szzx.web.Entity
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            CreatedTime = DateTime.Now;
            UpdatedTime = DateTime.Now;
        }
    }
}
