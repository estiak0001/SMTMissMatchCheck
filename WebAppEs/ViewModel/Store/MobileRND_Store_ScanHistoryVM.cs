using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Store
{
    public class MobileRND_Store_ScanHistoryVM
    {
        public Guid Id { get; set; }
        public DateTime? DateAndTime { get; set; }
        public Guid DetailsID { get; set; }
        public bool Status { get; set; }
    }
}
