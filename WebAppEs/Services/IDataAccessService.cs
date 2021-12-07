using WebAppEs.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppEs.ViewModel.Register;
using WebAppEs.ViewModel.Model;
using WebAppEs.ViewModel.Store;
using WebAppEs.Entity;
using WebAppEs.ViewModel.QCCheck;

namespace WebAppEs.Services
{
    public interface IDataAccessService
	{
		Task<bool> GetMenuItemsAsync(ClaimsPrincipal ctx, string ctrl, string act);
		Task<List<NavigationMenuViewModel>> GetMenuItemsAsync(ClaimsPrincipal principal);
		Task<List<NavigationMenuViewModel>> GetPermissionsByRoleIdAsync(string id);
		Task<bool> SetPermissionsByRoleIdAsync(string id, IEnumerable<Guid> permissionIds);
		List<EmployeeListVM> GetAllEmployeeList();

		//Model 

		Task<bool> AddPartsModel(PartsModelViewModel viewModel);
		List<PartsModelViewModel> GetAllPartsModelList();
		PartsModelViewModel GetPartsModelList(Guid Id);

		//Store
		MobileRND_StoreHead AddStoreHeadEntry(StoreViewModel viewModel);
		bool AddStoreDetails(StoreViewModel viewModel);
		MobileRND_StoreHead GetStoreHead(DateTime? date, Guid? ModelId, string Lot);
		StoreViewModel LoadHead(Guid Id);
		List<StoreHeadViewModel> StoreHeadList();
		List<StoreDetailsViewModel> StoreHeadDetailsList(Guid HeadID);


		bool UpdateSingelStatusFromStore(Guid Id, bool IsSuccess);

		bool AddSingelStatusFromStore(MobileRND_Store_ScanHistoryVM viewModel);

		//QC Check
		MobileRND_QcTaskHead AddTaskHead(MobileRND_QcTaskHead_VM viewModel);
		bool AddTaskHeadDetails(MobileRND_QcTaskHeadDetails_VM viewModel);

		MobileRND_QcTaskHeadDetails_VM GetTopOrderValueForFalse(Guid? TaskHeadID);

		MobileRND_QcTaskHeadDetails_VM GetTopOrderValueForRandom(Guid TaskHeadID, Guid ModelID, string lot, string Moduler, string feeder);

		MobileRND_QcTaskHead_VM HeadWithDetails(Guid? Id);
		List<MobileRND_QcTaskHead_VM> TaskHeadList();
		bool UpdateSingelStatusFromQC(Guid Id, bool IsSuccess);

		MobileRND_QcTaskHeadDetails AddRandomDataStatusFromQC(Guid? StoreDetailsId, Guid? TaskHeadID, bool status, string EmployeeID, Guid? Luser);
	}
}