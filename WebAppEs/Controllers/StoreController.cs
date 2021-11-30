using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.Models;
using WebAppEs.Services;
using WebAppEs.ViewModel.Store;

namespace WebAppEs.Controllers
{
    public class StoreController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<AdminController> _logger;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        public StoreController(UserManager<ApplicationUser> userManager,
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
            List<StoreHeadViewModel> head = new List<StoreHeadViewModel>();
            head = _dataAccessService.StoreHeadList();
            return View(head);
        }


        public IActionResult CreateStore(Guid Id)
        {
            if(Id != Guid.Empty)
            {
                var head = _dataAccessService.LoadHead(Id);
                var details = _dataAccessService.StoreHeadDetailsList(Id);
                StoreViewModel ImportedData = new StoreViewModel();
                List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                ImportedData.StoreViewModels = empty;
                if (head!=null)
                {
                    ImportedData = head;
                }
                if(details.Count != 0)
                {
                    ImportedData.StoreViewModels = details;
                }
                ImportedData.IsUpdate = "Update";
                ImportedData.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                return View(ImportedData);
            }
            else
            {
                StoreViewModel ImportedData = new StoreViewModel();
                List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                ImportedData.StoreViewModels = empty;
                ImportedData.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
                return View(ImportedData);
            }
        }

        [HttpPost]
        public IActionResult CreateStore(StoreViewModel ImportedData, IFormFile postedFile)
        {
            var employeeID = HttpContext.Session.GetString("EmployeeID");
            if (employeeID == null)
            {
                return RedirectToAction("Logout", "Account");
            }
            var ID = HttpContext.Session.GetString("Id");
            Guid newGuid = Guid.Parse(ID);

            ClaimsPrincipal currentUser = this.User;
            bool isStore = currentUser.IsInRole("Store");
            ImportedData.LUser = newGuid;
            ImportedData.EmployeeID = employeeID;
            ImportedData.PartsModelViewModel = _dataAccessService.GetAllPartsModelList();
            string mess = "";
            if (isStore)
            {
                if (postedFile != null)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    string conString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                       @"Data Source=" + filePath + ";" +
                       @"Extended Properties=" + Convert.ToChar(34).ToString() +
                       @"Excel 8.0" + Convert.ToChar(34).ToString() + ";";

                    DataTable dt = new DataTable();
                    //conString = string.Format(conString, filePath);
                    try
                    {
                        using (OleDbConnection connExcel = new OleDbConnection(conString))
                        {
                            using (OleDbCommand cmdExcel = new OleDbCommand())
                            {
                                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                {
                                    cmdExcel.Connection = connExcel;
                                    connExcel.Open();
                                    DataTable dtExcelSchema;
                                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    connExcel.Close();
                                    connExcel.Open();
                                    //cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                    cmdExcel.CommandText = "SELECT * From [Sheet1$A1:C2000]";

                                    odaExcel.SelectCommand = cmdExcel;
                                    odaExcel.Fill(dt);
                                    connExcel.Close();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        mess = "You may be entered wrong formated file. Please select correct formated file!";
                        ViewData["Messege"] = mess;
                        ViewData["Icon"] = "fa fa-exclamation-triangle";
                        List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                        ImportedData.StoreViewModels = empty;
                        return View(ImportedData);
                    }

                    var IsExist = _dataAccessService.GetStoreHead(ImportedData.Date, ImportedData.PartsModelID, ImportedData.LotNo);

                    if(IsExist == null)
                    {
                        ImportedData.StoreViewModels = GetImportedList(dt);
                        if (ImportedData.StoreViewModels.Count == 0)
                        {
                            mess = "You may be entered wrong formated file. Please select correct formated file!";
                            ViewData["Messege"] = mess;
                            ViewData["Icon"] = "fa fa-exclamation-triangle";
                            List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                            ImportedData.StoreViewModels = empty;
                            return View(ImportedData);
                        }
                        else
                        {
                            var head = _dataAccessService.AddStoreHeadEntry(ImportedData);
                            if (head != null)
                            {
                                ImportedData.HeadID = head.Id;
                                var details = _dataAccessService.AddStoreDetails(ImportedData);
                                if(details)
                                {
                                    List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                                    ImportedData.StoreViewModels = empty;
                                    ImportedData.StoreViewModels = _dataAccessService.StoreHeadDetailsList(head.Id);
                                }
                            }
                            
                            ViewBag.Status = "success";
                            ViewBag.Message = string.Format("Success! Data Stored successfully!");
                            return View(ImportedData);
                        }
                    }
                    else
                    {
                        List<StoreDetailsViewModel> empty = new List<StoreDetailsViewModel>();
                        ImportedData.StoreViewModels = empty;
                        ViewBag.Status = "error";
                        ViewBag.Message = string.Format("Error! This record already stored! Please select from list to update.");
                        return View(ImportedData);
                    }
                }
                mess = "Please select correct formated file to store!";
                ViewData["Messege"] = mess;
                ViewData["Icon"] = "fa fa-exclamation-triangle";
                return View(ImportedData);
            }
            ModelState.AddModelError("Error", "Unauthorized! You do not have any permission to modify store data!");
            List<StoreDetailsViewModel> empty1 = new List<StoreDetailsViewModel>();
            ImportedData.StoreViewModels = empty1;
            return View(ImportedData);
        }
        public List<StoreDetailsViewModel> GetImportedList(DataTable SetData)
        {
            List<StoreDetailsViewModel> DataList = new List<StoreDetailsViewModel>();
            try
            {
                int count = 0;
                for (int i = 0; i < SetData.Rows.Count; i++)
                {
                    count++;
                    string[] authorsList = SetData.Rows[i]["Location"].ToString().Split("-");
                    var Moduler = authorsList[0].Trim();
                    var Feeder = authorsList[1].Trim();
                    StoreDetailsViewModel data = new StoreDetailsViewModel();
                    data.Location = Moduler + "-" + Feeder;
                    data.Moduler = Moduler;
                    data.Feeder = Feeder;
                    data.PartNumber = SetData.Rows[i]["PartNumber"].ToString();
                    data.FeederName = SetData.Rows[i]["FeederName"].ToString();
                    data.SLNO = count;
                    DataList.Add(data);
                }

                return DataList;
            }
            catch (Exception)
            {
                List<StoreDetailsViewModel> DataList2 = new List<StoreDetailsViewModel>();
                return DataList2;
            }
            
           // return DataList;
        }

        public JsonResult UpdateStoreScanData(Guid? Id, bool status)
        {
            var result = false;
            if(Id != Guid.Empty)
            {
                var r = _dataAccessService.UpdateSingelStatusFromStore((Guid)Id, status);
                result = r;
            }
            
            return Json(new { isSuccess = result });
        }
    }
}
