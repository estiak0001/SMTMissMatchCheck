using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRND_StoreDetails")]
    public class MobileRND_StoreDetails
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("MobileRND_StoreHead")]
        public Guid HeadID { get; set; }
        public virtual MobileRND_StoreHead MobileRND_StoreHead { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [Required]
        //[StringLength(50)]
        public int SLNO { get; set; } = 0;

        [StringLength(50)]
        public string Moduler { get; set; }
        [StringLength(150)]
        public string Feeder { get; set; }
        [StringLength(150)]
        public string PartNumber { get; set; }
        [StringLength(150)]
        public string FeederName { get; set; }

        [StringLength(50)]
        public bool Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
