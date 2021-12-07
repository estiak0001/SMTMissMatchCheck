using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRND_QcTaskHead")]
    public class MobileRND_QcTaskHead
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? Date { get; set; }

        [Required]
        [StringLength(50)]
        public Guid ModelID { get; set; }

        [Required]
        [StringLength(50)]
        public string LotNo { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? StartTime { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string TaskType { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LineNo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
