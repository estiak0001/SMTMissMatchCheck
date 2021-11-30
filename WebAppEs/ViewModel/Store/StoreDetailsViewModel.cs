using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Store
{
    public class StoreDetailsViewModel
    {
        public Guid? Id { get; set; }
        public Guid? HeadID { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? Date { get; set; }
        public int SLNO { get; set; }
        public string Location { get; set; }
        public string Moduler { get; set; }
        public string Feeder { get; set; }
        public string PartNumber { get; set; }
        public string FeederName { get; set; }
        public Guid? PartsModelID { get; set; }
        public Guid? LUser { get; set; }
        public bool Status { get; set; }
        public string StatusMessage { get; set; }
    }
}
