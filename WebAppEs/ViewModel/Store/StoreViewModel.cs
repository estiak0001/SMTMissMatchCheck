using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Model;

namespace WebAppEs.ViewModel.Store
{
    public class StoreViewModel
    {
        public Guid? HeadID { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? Date { get; set; }
        public Guid? PartsModelID { get; set; }
        public Guid? LUser { get; set; }
        public string IsUpdate { get; set; }
        public string LotNo { get; set; }
        public List<StoreDetailsViewModel> StoreViewModels { get; set; }
        public IEnumerable<PartsModelViewModel> PartsModelViewModel { get; set; }
    }
}
