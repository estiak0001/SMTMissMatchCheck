using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.Entity
{
    [Table(name: "MobileRND_Store_ScanHistory")]
    public class MobileRND_Store_ScanHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public DateTime? DateAndTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(150)]
        public Guid DetailsID { get; set; }

        [Required]
        [StringLength(50)]
        public bool Status { get; set; }
    }
}
