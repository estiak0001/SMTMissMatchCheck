using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAppEs.Models;
using System.Diagnostics;
using WebAppEs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;

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
            
			return View();
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