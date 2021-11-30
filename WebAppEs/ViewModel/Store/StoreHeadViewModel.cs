using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppEs.ViewModel.Store
{
    public class StoreHeadViewModel
    {
        public Guid Id { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? Date { get; set; }
        public string DateString { get; set; }
        public Guid ModelID { get; set; }
        public string ModelName { get; set; }
        public string LotNo { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }

        public string IsComplete { get; set; }

    }
}
