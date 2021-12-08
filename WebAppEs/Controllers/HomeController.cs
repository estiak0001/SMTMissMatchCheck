using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppEs.Models;
using System.Diagnostics;
using WebAppEs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using WebAppEs.ViewModel.Home;

namespace WebAppEs.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IDataAccessService _dataAccessService;

		public HomeController(UserManager<ApplicationUser> userManager,
				RoleManager<IdentityRole> roleManager,
				IDataAccessService dataAccessService, ILogger<HomeController> logger)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_dataAccessService = dataAccessService;
			_logger = logger;
		}

		//[Authorize("Authorization")]
		public IActionResult Index()
		{
			var rolename = HttpContext.Session.GetString("Role");
			if (rolename == "Store")
			{
				return RedirectToAction("Index", "Store");
			}

			DashBoard_VM DashboardData = new DashBoard_VM();

			DashboardData.MobileRND_QcTaskHead_List = _dataAccessService.TaskHeadList(DateTime.Today, "");
			DashboardData.sequential = _dataAccessService.TaskHeadList(DateTime.Today, "sequential");
			DashboardData.random = _dataAccessService.TaskHeadList(DateTime.Today, "random");

			DashboardData.TodayTotal = _dataAccessService.DashBoardHeaderData().TodayTotal;
			DashboardData.LastDayTotal = _dataAccessService.DashBoardHeaderData().LastDayTotal;
			DashboardData.TodayInComplete = _dataAccessService.DashBoardHeaderData().TodayInComplete;
			DashboardData.TodayComplete = _dataAccessService.DashBoardHeaderData().TodayComplete;
			return View(DashboardData);
		}
		//[Authorize("Authorization")]
		public IActionResult Privacy()
		{
			return View();
		}
		//[Authorize("Authorization")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}
}