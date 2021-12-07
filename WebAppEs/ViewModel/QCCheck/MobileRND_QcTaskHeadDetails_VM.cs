using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.QCCheck
{
    public class MobileRND_QcTaskHeadDetails_VM
    {
        public Guid Id { get; set; }
        public string EmployeeID { get; set; }
        public Guid TaskHeadID { get; set; }
        public DateTime? DateAndTime { get; set; }
        public Guid StoreDetailsID { get; set; }
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        //vm
        public int SLNO { get; set; }
        public string Location { get; set; }
        public string Moduler { get; set; }
        public string Feeder { get; set; }
        public string PartNumber { get; set; }
        public string FeederName { get; set; }
        //end

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
    }
}
