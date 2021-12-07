using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Model;

namespace WebAppEs.ViewModel.QCCheck
{
    public class MobileRND_QcTaskHead_VM
    {
        public Guid Id { get; set; }
        public string EmployeeID { get; set; }
        public DateTime? Date { get; set; }

        public TimeSpan OrderByDate { get; set; }
        public string DateString { get; set; }
        public Guid ModelID { get; set; }
        public string ModelName { get; set; }
        public string LotNo { get; set; }
        public DateTime? StartTime { get; set; }
        public string startString { get; set; }
        public DateTime? EndTime { get; set; }
        public string EndString { get; set; }
        public string TaskType { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public Guid LUser { get; set; }
        public string IsUpdate { get; set; }
        public string IsComplete { get; set; }
        public string Duration { get; set; }
        public string isDisable { get; set; }
        public string LineNo { get; set; }
        public bool StatusIsToday { get; set; }
        public MobileRND_QcTaskHeadDetails_VM GetTopOrderValue { get; set; }
        public IEnumerable<MobileRND_QcTaskHead_VM> MobileRND_QcTaskHead_List { get; set; }
        public IEnumerable<MobileRND_QcTaskHeadDetails_VM> MobileRND_QcTaskHeadDetails_VM { get; set; }
        public IEnumerable<PartsModelViewModel> PartsModelViewModel { get; set; }
    }
}
