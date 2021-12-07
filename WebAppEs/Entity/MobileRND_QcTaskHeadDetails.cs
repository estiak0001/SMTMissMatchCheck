using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRND_QcTaskHeadDetails")]
    public class MobileRND_QcTaskHeadDetails
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [ForeignKey("MobileRND_QcTaskHead")]
        public Guid TaskHeadID { get; set; }
        public virtual MobileRND_QcTaskHead MobileRND_QcTaskHead { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? DateAndTime { get; set; }

        [Required]
        [StringLength(50)]
        public Guid StoreDetailsID { get; set; }

        [StringLength(50)]
        public bool Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
