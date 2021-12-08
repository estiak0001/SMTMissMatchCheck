using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebAppEs.Data;
using WebAppEs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Register;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WebAppEs.ViewModel.Model;
using WebAppEs.Entity;
using WebAppEs.ViewModel.Store;
using EFCore.BulkExtensions;
using WebAppEs.ViewModel.QCCheck;
using System.Globalization;
using WebAppEs.ViewModel.Home;

namespace WebAppEs.Services
{
    public class DataAccessService : IDataAccessService
	{
		private readonly IMemoryCache _cache;
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		public DataAccessService(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IMemoryCache cache)
		{
			_cache = cache;
			_context = context;
			_hostEnvironment = hostEnvironment;
		}

		public async Task<List<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal principal)
		{
			var isAuthenticated = principal.Identity.IsAuthenticated;
			if (!isAuthenticated)
			{
				return new List<NavigationMenuViewModel>();
			}

			var roleIds = await GetUserRoleIds(principal);

			var permissions = await _cache.GetOrCreateAsync("Permissions",
				async x => await (from menu in _context.NavigationMenu select menu).ToListAsync());

			var rolePermissions = await _cache.GetOrCreateAsync("RolePermissions",
				async x => await (from menu in _context.RoleMenuPermission select menu).Include(x => x.NavigationMenu).ToListAsync());

			var data = (from menu in rolePermissions
						join p in permissions on menu.NavigationMenuId equals p.Id
						where roleIds.Contains(menu.RoleId)
						select p)
							  .Select(m => new NavigationMenuViewModel()
							  {
								  Id = m.Id,
								  Name = m.Name,
								  Area = m.Area,
								  Visible = m.Visible,
								  IsExternal = m.IsExternal,
								  ActionName = m.ActionName,
								  ExternalUrl = m.ExternalUrl,
								  DisplayOrder = m.DisplayOrder,
								  ParentMenuId = m.ParentMenuId,
								  ControllerName = m.ControllerName,
								  Icon = m.Icon,
							  }).Distinct().OrderBy(x=> x.DisplayOrder).ToList();
			return data;
		}

		public async Task<bool> GetMenuItemsAsync(ClaimsPrincipal ctx, string ctrl, string act)
		{
			var result = false;
			var roleIds = await GetUserRoleIds(ctx);
			var data = await (from menu in _context.RoleMenuPermission
							  where roleIds.Contains(menu.RoleId)
							  select menu)
							  .Select(m => m.NavigationMenu)
							  .Distinct()
							  .ToListAsync();

			foreach (var item in data)
			{
				result = (item.ControllerName == ctrl && item.ActionName == act);
				if (result)
				{
					break;
				}
			}

			return result;
		}

		public async Task<List<NavigationMenuViewModel>> GetPermissionsByRoleIdAsync(string id)
		{
			var items = await (from m in _context.NavigationMenu
							   join rm in _context.RoleMenuPermission
								on new { X1 = m.Id, X2 = id } equals new { X1 = rm.NavigationMenuId, X2 = rm.RoleId }
								into rmp
							   from rm in rmp.DefaultIfEmpty()
							   select new NavigationMenuViewModel()
							   {
								   Id = m.Id,
								   Name = m.Name,
								   Area = m.Area,
								   ActionName = m.ActionName,
								   ControllerName = m.ControllerName,
								   IsExternal = m.IsExternal,
								   ExternalUrl = m.ExternalUrl,
								   DisplayOrder = m.DisplayOrder,
								   ParentMenuId = m.ParentMenuId,
								   Visible = m.Visible,
								   Permitted = rm.RoleId == id,
								   Icon = m.Icon
							   })
							   .AsNoTracking()
							   .OrderBy(x => x.DisplayOrder)
							   .ToListAsync();
			return items;
		}

		public async Task<bool> SetPermissionsByRoleIdAsync(string id, IEnumerable<Guid> permissionIds)
		{
			var existing = await _context.RoleMenuPermission.Where(x => x.RoleId == id).ToListAsync();
			_context.RemoveRange(existing);

			foreach (var item in permissionIds)
			{
				await _context.RoleMenuPermission.AddAsync(new RoleMenuPermission()
				{
					RoleId = id,
					NavigationMenuId = item,
				});
			}

			var result = await _context.SaveChangesAsync();

			// Remove existing permissions to roles so it can re evaluate and take effect
			_cache.Remove("RolePermissions");

			return result > 0;
		}

		private async Task<List<string>> GetUserRoleIds(ClaimsPrincipal ctx)
		{
			var userId = GetUserId(ctx);
			var data = await (from role in _context.UserRoles
							  where role.UserId == userId
							  select role.RoleId).ToListAsync();
			return data;
		}

		private string GetUserId(ClaimsPrincipal user)
		{
			return ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		public List<EmployeeListVM> GetAllEmployeeList()
		{
			var items = (from user in _context.Users

						 select new EmployeeListVM()
						 {
							 EmployeeID = user.EmployeeID,
							 EmployeeName = user.Name + " (" + user.EmployeeID + ")"
						 }).ToList();
			return items;
		}

		public async Task<bool> AddPartsModel(PartsModelViewModel viewModel)
		{
			var result = 0;
			var UpdateDataSet = await _context.MobileRND_Models.Where(x => x.Id == viewModel.ID).FirstOrDefaultAsync();
			var existing = await _context.MobileRND_Models.Where(x => x.ModelName == viewModel.Name).FirstOrDefaultAsync();

			if (existing != null)
			{
				return false;
			}
			else
			{
				if (UpdateDataSet != null)
				{
					UpdateDataSet.ModelName = viewModel.Name;

					_context.MobileRND_Models.Update(UpdateDataSet);
					result = await _context.SaveChangesAsync();
				}
				else
				{
					_context.MobileRND_Models.Add(new MobileRND_Models()
					{
						ModelName = viewModel.Name
					});
					result = await _context.SaveChangesAsync();
				}
			}
			return result > 0;
		}

		public List<PartsModelViewModel> GetAllPartsModelList()
		{
			var items = (from partModel in _context.MobileRND_Models

						 select new PartsModelViewModel()
						 {
							 ID = partModel.Id,
							 Name = partModel.ModelName
						 }).ToList();
			return items;
		}

		public PartsModelViewModel GetPartsModelList(Guid Id)
		{
			var items = (from partModel in _context.MobileRND_Models.Where(x => x.Id == Id)

						 select new PartsModelViewModel()
						 {
							 ID = partModel.Id,
							 Name = partModel.ModelName,
							 IsUpdate = "Update"
						 }).FirstOrDefault();

			return items;
		}

        public MobileRND_StoreHead AddStoreHeadEntry(StoreViewModel viewModel)
        {
			MobileRND_StoreHead dd = new MobileRND_StoreHead();
			if (viewModel == null)
			{
				return null;
			}
			else
			{
				dd.Date = viewModel.Date;
				dd.EmployeeID = viewModel.EmployeeID;
				dd.ModelID = (Guid)viewModel.PartsModelID;
				dd.LotNo = viewModel.LotNo;
				dd.LUser = (Guid)viewModel.LUser;

				_context.MobileRND_StoreHead.Add(dd);
			}
		    _context.SaveChanges();

			return dd;
		}

        public bool AddStoreDetails(StoreViewModel viewModel)
        {
			List<MobileRND_StoreDetails> StoreDetails = new List<MobileRND_StoreDetails>(); // C# 9 Syntax.  

            foreach (var item in viewModel.StoreViewModels)
            {
				StoreDetails.Add(new MobileRND_StoreDetails()
				{
					HeadID = (Guid)viewModel.HeadID,
					EmployeeID = viewModel.EmployeeID,
					SLNO = item.SLNO,
					Moduler = item.Moduler,
					Feeder = item.Feeder,
					PartNumber = item.PartNumber,
					FeederName = item.FeederName,
					LUser = (Guid)viewModel.LUser
				});
			}

            _context.MobileRND_StoreDetails.AddRange(StoreDetails);

			var result = _context.SaveChanges();
			return result > 0;
		}

        public MobileRND_StoreHead GetStoreHead(DateTime? date, Guid? ModelId, string Lot)
        {
			return _context.MobileRND_StoreHead.Where(x => (x.Date == date || date == null) && x.ModelID == ModelId && x.LotNo == Lot).FirstOrDefault();
        }

        public List<StoreViewModel> GetStoreDetails(Guid Id)
        {
			var data = (from head in _context.MobileRND_StoreHead.Where(x=> x.Id == Id)
						select new StoreViewModel()
						{
							HeadID = head.Id,

						}).ToList();
			return data;
		}

        public List<StoreHeadViewModel> StoreHeadList()
        {
			var items = (from m in _context.MobileRND_StoreHead
							  join rm in _context.MobileRND_Models
							   on new { X1 = m.ModelID } equals new { X1 = rm.Id }
							   into rmp
							  from rm in rmp.DefaultIfEmpty()
							  select new StoreHeadViewModel()
							  {
								  Id = m.Id,
								  EmployeeID = m.EmployeeID,
								  Date = m.Date,
								  DateString = String.Format("{0:MM/dd/yyyy}", m.Date),
								  ModelName = rm.ModelName,
								  LotNo = m.LotNo,
								  IsComplete = (_context.MobileRND_StoreDetails.Where(x => x.HeadID == m.Id).Any(p => p.Status == false)) == true ? "In Complete" : "Complete"
							  }).AsNoTracking().OrderByDescending(x => x.Date).ToList();
			return items;
		}

        public List<StoreDetailsViewModel> StoreHeadDetailsList(Guid HeadID)
        {
			var items = (from m in _context.MobileRND_StoreHead.Where(x=> x.Id == HeadID)
						 join rm in _context.MobileRND_StoreDetails
						  on new { X1 = m.Id } equals new { X1 = rm.HeadID }
						  into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new StoreDetailsViewModel()
						 {
							 Id = rm.Id,
							 HeadID = rm.HeadID,
							 EmployeeID = rm.EmployeeID,
							 Date = m.Date,
							 SLNO = rm.SLNO,
							 Moduler = rm.Moduler,
							 Feeder = rm.Feeder,
							 PartNumber = rm.PartNumber,
							 FeederName = rm.FeederName,
							 Location = rm.Moduler+ "-" + rm.Feeder,
							 Status = rm.Status,
							 StatusMessage = rm.Status == true ? "Matched" : "Not Matched",
							 
						 }).AsNoTracking().OrderBy(x => x.SLNO).ToList();
			return items;
		}

        public StoreViewModel LoadHead(Guid Id)
        {
			var items = (from m in _context.MobileRND_StoreHead
						 join rm in _context.MobileRND_Models
						  on new { X1 = m.ModelID } equals new { X1 = rm.Id }
						  into rmp
						 from rm in rmp.DefaultIfEmpty()
						 select new StoreViewModel()
						 {
							 HeadID = m.Id,
							 EmployeeID = m.EmployeeID,
							 Date = m.Date,
							 PartsModelID = m.ModelID,
							 LotNo = m.LotNo,

						 }).FirstOrDefault();
			return items;
		}

        public bool UpdateSingelStatusFromStore(Guid Id, bool IsSuccess)
        {
			var Details = _context.MobileRND_StoreDetails.Where(x => x.Id == Id).FirstOrDefault();
			Details.Status = IsSuccess;
			_context.MobileRND_StoreDetails.Update(Details);
			var result = _context.SaveChanges();
			return result > 0;
		}

        public bool AddSingelStatusFromStore(MobileRND_Store_ScanHistoryVM viewModel)
        {
            throw new NotImplementedException();
        }

        public MobileRND_QcTaskHead AddTaskHead(MobileRND_QcTaskHead_VM viewModel)
        {
			MobileRND_QcTaskHead dd = new MobileRND_QcTaskHead();
			if (viewModel == null)
			{
				return null;
			}
			else
			{
				dd.Date = DateTime.Now;
				dd.EmployeeID = viewModel.EmployeeID;
				dd.ModelID = (Guid)viewModel.ModelID;
				dd.LotNo = viewModel.LotNo;
				dd.LUser = (Guid)viewModel.LUser;
				dd.StartTime = DateTime.Now;
				dd.EndTime = DateTime.Now;
				dd.TaskType = viewModel.TaskType;
				dd.LineNo = viewModel.LineNo;

				_context.MobileRND_QcTaskHead.Add(dd);
				_context.SaveChanges();
			}
			return dd;
		}

        public bool AddTaskHeadDetails(MobileRND_QcTaskHeadDetails_VM viewModel)
        {
			MobileRND_QcTaskHeadDetails dd = new MobileRND_QcTaskHeadDetails();
			if (viewModel == null)
			{
				return false;
			}
			else
			{
				dd.EmployeeID = viewModel.EmployeeID;
				dd.TaskHeadID = viewModel.TaskHeadID;
				dd.DateAndTime = DateTime.Now;
				dd.StoreDetailsID = viewModel.StoreDetailsID;
				dd.LUser = (Guid)viewModel.LUser;
				dd.LUser = viewModel.LUser;
				
				_context.MobileRND_QcTaskHeadDetails.Add(dd);
				_context.SaveChanges();
			}
			return true;
		}

        public MobileRND_QcTaskHeadDetails_VM GetTopOrderValueForFalse(Guid? TaskHeadID)
        {
			var items = (from details in _context.MobileRND_QcTaskHeadDetails.Where(x=> x.TaskHeadID == TaskHeadID && x.Status == false)
						 
						 join sdd in _context.MobileRND_StoreDetails
						  on new { X1 = details.StoreDetailsID } equals new { X1 = sdd.Id }
						  into sddp
						 from sdetails in sddp.DefaultIfEmpty()
						 select new MobileRND_QcTaskHeadDetails_VM()
						 {
							 Id = details.Id,
							 EmployeeID = details.EmployeeID,
							 TaskHeadID = details.TaskHeadID,
							 DateAndTime = details.DateAndTime,
							 StoreDetailsID = details.StoreDetailsID,
							 Status = details.Status,
							 SLNO = sdetails.SLNO,
							 Location = sdetails.Moduler + "-" + sdetails.Feeder,
							 PartNumber = sdetails.PartNumber,
							 Moduler = sdetails.Moduler,
							 Feeder = sdetails.Feeder,
						 }).OrderBy(x => x.SLNO).FirstOrDefault();
			return items;
		}

        public MobileRND_QcTaskHead_VM HeadWithDetails(Guid? Id)
        {
			MobileRND_QcTaskHead_VM headwithDetails = new MobileRND_QcTaskHead_VM();
			List<MobileRND_QcTaskHeadDetails_VM> ddd = new List<MobileRND_QcTaskHeadDetails_VM>();
			var taskhead = (from head in _context.MobileRND_QcTaskHead.Where(x => x.Id == Id)

						 select new MobileRND_QcTaskHead_VM()
						 {
							 Id = head.Id,
							 EmployeeID = head.EmployeeID,
							 Date = head.Date,
							 ModelID = head.ModelID,
							 LotNo = head.LotNo,
							 StartTime = head.StartTime,
							 EndTime = head.EndTime,
							 TaskType = head.TaskType,
							 IsUpdate = "Update",
							 isDisable = head.TaskType == "random" ? "" : "display:none",
							 LineNo = head.LineNo
						 }).FirstOrDefault();

			var taskdetails = (from details  in _context.MobileRND_QcTaskHeadDetails.Where(x => x.TaskHeadID == Id)

						join dd in _context.MobileRND_QcTaskHead
						 on new { X1 = details.TaskHeadID } equals new { X1 = dd.Id }
						 into ddp
						from head in ddp.DefaultIfEmpty()


						join sdd in _context.MobileRND_StoreDetails
						 on new { X1 = details.StoreDetailsID } equals new { X1 = sdd.Id }
						 into sddp
						from sdetails in sddp.DefaultIfEmpty()

						select new MobileRND_QcTaskHeadDetails_VM()
						{
							Id = details.Id,
							TaskHeadID = details.TaskHeadID,
							EmployeeID = details.EmployeeID,
							DateAndTime = details.DateAndTime,
							SLNO = sdetails.SLNO,
							Moduler = sdetails.Moduler,
							Feeder = sdetails.Feeder,
							PartNumber = sdetails.PartNumber,
							FeederName = sdetails.FeederName,
							Location = sdetails.Moduler + "-" + sdetails.Feeder,
							Status = details.Status,
							StatusMessage = sdetails.Status == true ? "Matched" : "Not Matched",

						}).OrderBy(x => x.SLNO).ToList();
			if(taskhead != null)
            {
				headwithDetails = taskhead;

				if(taskdetails.Count != 0)
                {
					headwithDetails.MobileRND_QcTaskHeadDetails_VM = taskdetails;
				}
                else
                {
					headwithDetails.MobileRND_QcTaskHeadDetails_VM = ddd;
                }
			}

			return headwithDetails;
        }

        public List<MobileRND_QcTaskHead_VM> TaskHeadList(DateTime? datetimeforsort, string type)
        {
			//DateTime? ts = DateTime.Now;
			//var v = String.Format("{0:dd/MM/yyyy h:mm:ss tt}",ts);
			//var t = DateTime.ParseExact(v, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

			//var query = _context.MobileRND_QcTaskHead.Select(tb => String.Format("{0:dd/MM/yyyy h:mm:ss tt}", tb.Date))
			//	  .AsEnumerable()
			//	  .Select(x => DateTime.ParseExact(x, "dd/MM/yyyy h:mm:ss tt",
			//									CultureInfo.InvariantCulture));


			var taskhead = (from head in _context.MobileRND_QcTaskHead.AsEnumerable()

							join rm in _context.MobileRND_Models
						    on new { X1 = head.ModelID } equals new { X1 = rm.Id }
						    into rmp
							from rm in rmp.DefaultIfEmpty()

							select new MobileRND_QcTaskHead_VM()
							{
								Id = head.Id,
								EmployeeID = head.EmployeeID,
								Date = DateTime.ParseExact(String.Format("{0:dd/MM/yyyy}", head.Date), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
								ModelID = head.ModelID,
								ModelName = rm.ModelName,
								LotNo = head.LotNo,
								StartTime = head.StartTime,
								EndTime = head.EndTime,
								TaskType = head.TaskType,
								LineNo = "Line "+ head.LineNo,
								StatusIsToday = String.Format("{0:dd/MM/yyyy}", head.Date) == String.Format("{0:dd/MM/yyyy}", DateTime.Today) ? true : false,
								//OrderByDate = _context.MobileRND_QcTaskHead.Where(x => x.Id == head.Id).Select(tb => String.Format("{0:dd/MM/yyyy h:mm:ss tt}", tb.Date)).AsEnumerable().Select(x => DateTime.ParseExact(x, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)).FirstOrDefault(),
								OrderByDate = DateTime.ParseExact(String.Format("{0:dd/MM/yyyy h:mm:ss tt}", head.Date), "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture).TimeOfDay,
								DateString = String.Format("{0:dd/MM/yyyy h:mm:ss tt}", head.Date),
								startString = String.Format("{0:h:mm:ss tt}", head.StartTime),
								EndString = String.Format("{0:h:mm:ss tt}", head.EndTime),
								IsComplete = (_context.MobileRND_QcTaskHeadDetails.Where(x => x.TaskHeadID == head.Id).Any(p => p.Status == false)) == true ? "In Complete" : "Complete",
								Duration = (_context.MobileRND_QcTaskHeadDetails.Where(x => x.TaskHeadID == head.Id).Any(p => p.Status == false)) == true ? "("+String.Format("{0:h:mm:ss tt}", head.StartTime) +" - Continue..)" : "("+String.Format("{0:h:mm:ss tt}", head.StartTime) + " - " + String.Format("{0:h:mm:ss tt}", head.EndTime) +")"
							}).Where(w=> (w.Date == datetimeforsort || datetimeforsort == null) && (type == "" || w.TaskType == type)).OrderByDescending(x=> x.Date).ThenBy(x=> x.OrderByDate).ToList();

			return taskhead;
		}

		public DateTime returnDateTime(string t)
        {
			var tt = DateTime.ParseExact(t, "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
			return tt;
		}

        public bool UpdateSingelStatusFromQC(Guid Id, bool IsSuccess)
        {
			var Details = _context.MobileRND_QcTaskHeadDetails.Where(x => x.Id == Id).FirstOrDefault();
			Details.Status = IsSuccess;
			_context.MobileRND_QcTaskHeadDetails.Update(Details);
			var result = _context.SaveChanges();
			return result > 0;
		}

        public MobileRND_QcTaskHeadDetails_VM GetTopOrderValueForRandom(Guid TaskHeadID, Guid ModelID, string lot, string Moduler, string feeder)
        {
			var items = (from details in _context.MobileRND_StoreHead.Where(x => x.ModelID == ModelID && x.LotNo == lot)

						 join sdd in _context.MobileRND_StoreDetails
						  on new { X1 = details.Id } equals new { X1 = sdd.HeadID }
						  into sddp
						 from sdetails in sddp.DefaultIfEmpty()
						 select new MobileRND_QcTaskHeadDetails_VM()
						 {
							 //Id = details.Id,
							 EmployeeID = details.EmployeeID,
							 //TaskHeadID = details.TaskHeadID,
							 //DateAndTime = details.DateAndTime,
							 StoreDetailsID = sdetails.Id,
							 Status = sdetails.Status,
							 SLNO = sdetails.SLNO,
							 Location = sdetails.Moduler + "-" + sdetails.Feeder,
							 PartNumber = sdetails.PartNumber,
							 Moduler = sdetails.Moduler,
							 Feeder = sdetails.Feeder,
						 }).Where(m=> m.Moduler == Moduler && m.Feeder == feeder).OrderBy(x => x.SLNO).FirstOrDefault();

			return items;
        }

        public MobileRND_QcTaskHeadDetails AddRandomDataStatusFromQC(Guid? StoreDetailsId, Guid? TaskHeadID, bool status, string EmployeeID, Guid? Luser)
        {
			var isExist = _context.MobileRND_QcTaskHeadDetails.Where(x => (x.TaskHeadID == TaskHeadID) && (x.StoreDetailsID == StoreDetailsId)).FirstOrDefault();
			
			if(isExist != null)
            {
				var Details = _context.MobileRND_QcTaskHeadDetails.Where(x => x.Id == isExist.Id).FirstOrDefault();
				Details.Status = status;
				_context.MobileRND_QcTaskHeadDetails.Update(Details);
				var result = _context.SaveChanges();
				return isExist;
			}
            else
            {
				var StoreDetails = _context.MobileRND_StoreDetails.Where(x => x.Id == StoreDetailsId).FirstOrDefault();
				MobileRND_QcTaskHeadDetails headDetails = new MobileRND_QcTaskHeadDetails();

				headDetails.EmployeeID = EmployeeID;
				headDetails.TaskHeadID = (Guid)TaskHeadID;
				headDetails.DateAndTime = DateTime.Now;
				headDetails.StoreDetailsID = (Guid)StoreDetailsId;
				headDetails.LUser = (Guid)Luser;
				headDetails.Status = status;

				_context.MobileRND_QcTaskHeadDetails.Add(headDetails);
				_context.SaveChanges();

				return headDetails;
			}
		}

        public DashBoardTopHeaderValueUpdate_VM DashBoardHeaderData()
        {
			DashBoardTopHeaderValueUpdate_VM data = new DashBoardTopHeaderValueUpdate_VM();

			data.TodayTotal = _context.MobileRND_QcTaskHead.AsEnumerable().Where(x => (DateTime.ParseExact(String.Format("{0:dd/MM/yyyy}", x.Date), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date) == DateTime.Today).ToList().Count();
			data.LastDayTotal = _context.MobileRND_QcTaskHead.AsEnumerable().Where(x => (DateTime.ParseExact(String.Format("{0:dd/MM/yyyy}", x.Date), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date) == DateTime.Now.Date.AddDays(-1)).ToList().Count();
			data.TodayInComplete = _context.MobileRND_QcTaskHead.AsEnumerable().Where(x => ((DateTime.ParseExact(String.Format("{0:dd/MM/yyyy}", x.Date), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date) == DateTime.Today) && ((_context.MobileRND_QcTaskHeadDetails.Where(m => m.TaskHeadID == x.Id).Any(p => p.Status == false)==true))).ToList().Count();
			data.TodayComplete = _context.MobileRND_QcTaskHead.AsEnumerable().Where(x => ((DateTime.ParseExact(String.Format("{0:dd/MM/yyyy}", x.Date), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date) == DateTime.Today) && ((_context.MobileRND_QcTaskHeadDetails.Where(m => m.TaskHeadID == x.Id).Any(p => p.Status == false) == false))).ToList().Count();
			return data;
        }
    }
}