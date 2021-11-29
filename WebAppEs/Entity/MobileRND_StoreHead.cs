using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRND_StoreHead")]
    public class MobileRND_StoreHead
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

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
