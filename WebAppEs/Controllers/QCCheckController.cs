using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.Models;
using WebAppEs.Services;
using WebAppEs.ViewModel.QCCheck;

namespace WebAppEs.Controllers
{
    public class QCCheckController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<AdminController> _logger;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;

        public QCCheckController(UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IDataAccessService dataAccessService, IWebHostEnvironment _environment, IConfiguration _configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccessService = dataAccessService;
            Environment = _environment;
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");

            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            MobileRND_QcTaskHead_VM TaskHead = new MobileRND_QcTaskHead_VM();
            TaskHead.MobileRND_QcTaskHead_List = _dataAccessService.TaskHeadList();
            TaskHead.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            return View(TaskHead);
        }

        public IActionResult CreateTask(Guid? Id)
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");

            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            List<MobileRND_QcTaskHeadDetails_VM> empty = new List<MobileRND_QcTaskHeadDetails_VM>();
            if (Id == null || Id == Guid.Empty)
            {
                MobileRND_QcTaskHead_VM TaskHead = new MobileRND_QcTaskHead_VM();
                TaskHead.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                TaskHead.MobileRND_QcTaskHeadDetails_VM = empty;
                TaskHead.GetTopOrderValue = new MobileRND_QcTaskHeadDetails_VM();
                return View(TaskHead);
            }
            else
            {
                MobileRND_QcTaskHead_VM TaskHead = new MobileRND_QcTaskHead_VM();
                TaskHead = _dataAccessService.HeadWithDetails(Id);
                var topOrderValue = _dataAccessService.GetTopOrderValueForFalse(Id);
                if(topOrderValue!= null)
                {
                    TaskHead.GetTopOrderValue = topOrderValue;
                }
                else
                {
                    TaskHead.GetTopOrderValue = new MobileRND_QcTaskHeadDetails_VM();
                }
                
                TaskHead.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                return View(TaskHead);
            }
        }

        [HttpPost]
        public IActionResult CreateTask(MobileRND_QcTaskHead_VM vm)
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");

            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            MobileRND_QcTaskHead_VM newhead = new MobileRND_QcTaskHead_VM();
               ClaimsPrincipal currentUser = this.User;
            var ID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            Guid newGuid = Guid.Parse(ID);
            bool isSuperAdmin = currentUser.IsInRole("SuperAdmin");
            bool isAdmin = currentUser.IsInRole("Admin");

            var HedSubmit = false;
            var DetailsSubmit = false;
            //bool status = false;
            StatusModel status = new StatusModel();
            status.success = false;
            var LuserID = HttpContext.Session.GetString("Id");
            Guid newGuid2 = Guid.Parse(LuserID);
            
            var StoreHead = _dataAccessService.GetStoreHead(null, vm.ModelID, vm.LotNo);
            if(StoreHead == null)
            {
                ModelState.AddModelError("Error", "This model and lot wise record not found from store. Please add data from store then try again...");
                newhead = vm;
                newhead.MobileRND_QcTaskHeadDetails_VM = new List<MobileRND_QcTaskHeadDetails_VM>();
                newhead.GetTopOrderValue = new MobileRND_QcTaskHeadDetails_VM();
                newhead.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                return View(newhead);
            }
            var StoreDetails = _dataAccessService.StoreHeadDetailsList(StoreHead.Id);

            MobileRND_QcTaskHead_VM head = new MobileRND_QcTaskHead_VM();
            

            head.ModelID = (Guid)vm.ModelID;
            head.LotNo = vm.LotNo;
            head.EmployeeID = employeeID;
            head.TaskType = vm.TaskType;
            head.LUser = newGuid2;
            head.LineNo = vm.LineNo;
            var taskHead = _dataAccessService.AddTaskHead(head);

            if(vm.TaskType != "random")
            {
                foreach (var item in StoreDetails)
                {
                    MobileRND_QcTaskHeadDetails_VM headdetails = new MobileRND_QcTaskHeadDetails_VM();
                    headdetails.EmployeeID = employeeID;
                    headdetails.TaskHeadID = taskHead.Id;
                    headdetails.StoreDetailsID = (Guid)item.Id;
                    head.LUser = newGuid2;
                    _dataAccessService.AddTaskHeadDetails(headdetails);
                }
            }
           
            newhead = _dataAccessService.HeadWithDetails(taskHead.Id);
            newhead.GetTopOrderValue = new MobileRND_QcTaskHeadDetails_VM();
            newhead.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
           
            return View(newhead);
        }

        public JsonResult GetCurrentLocationForRandom(Guid TaskHeadID,  Guid ModelID, string lot, string Moduler, string feeder)
        {
            var item = _dataAccessService.GetTopOrderValueForRandom(TaskHeadID, ModelID, lot, Moduler, feeder);
            return Json(item);
        }

        public JsonResult GetCurrentLocation(Guid HeadID)
        {
            var item = _dataAccessService.GetTopOrderValueForFalse(HeadID);
            return Json(item);
        }

        public JsonResult UpdateQCScanData(Guid? Id, bool status)
        {
            var result = false;
            if (Id != Guid.Empty)
            {
                var r = _dataAccessService.UpdateSingelStatusFromQC((Guid)Id, status);
                result = r;
            }

            return Json(new { isSuccess = result });
        }

        public IActionResult AddQCRandomScanData(Guid? StoreDetailsId, Guid? TaskHeadID, bool status)
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");

            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            var LuserID = HttpContext.Session.GetString("Id");
            Guid newGuid2 = Guid.Parse(LuserID);
            
            var response = _dataAccessService.AddRandomDataStatusFromQC(StoreDetailsId, TaskHeadID, status, employeeID, newGuid2);

            var fullDetails = _dataAccessService.HeadWithDetails(response.TaskHeadID);
            var singeldata = fullDetails.MobileRND_QcTaskHeadDetails_VM.Where(x => x.Id == response.Id);
            fullDetails.MobileRND_QcTaskHeadDetails_VM = singeldata;
            return Json(fullDetails);
        }

    }
}
