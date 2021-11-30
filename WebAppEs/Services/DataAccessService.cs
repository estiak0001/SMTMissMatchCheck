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
			return _context.MobileRND_StoreHead.Where(x => x.Date == date && x.ModelID == ModelId && x.LotNo == Lot).FirstOrDefault();
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
    }
}