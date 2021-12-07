using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppEs.ViewModel.QCCheck;

namespace WebAppEs.ViewModel.Home
{
    public class DashBoard_VM
    {
        public IEnumerable<MobileRND_QcTaskHead_VM> MobileRND_QcTaskHead_List { get; set; }
        public IEnumerable<MobileRND_QcTaskHeadDetails_VM> MobileRND_QcTaskHeadDetails_VM { get; set; }
    }
}
