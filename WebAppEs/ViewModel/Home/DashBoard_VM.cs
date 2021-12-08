using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.QCCheck;

namespace WebAppEs.ViewModel.Home
{
    public class DashBoard_VM
    {
        public int TodayTotal { get; set; }
        public int LastDayTotal { get; set; }
        public int TodayInComplete { get; set; }
        public int TodayComplete { get; set; }
        public DateTime SortingDate { get; set; } = DateTime.Today;
        public IEnumerable<MobileRND_QcTaskHead_VM> MobileRND_QcTaskHead_List { get; set; }
        public IEnumerable<MobileRND_QcTaskHead_VM> sequential { get; set; }
        public IEnumerable<MobileRND_QcTaskHead_VM> random { get; set; }
        public IEnumerable<MobileRND_QcTaskHeadDetails_VM> MobileRND_QcTaskHeadDetails_VM { get; set; }
    }
}
