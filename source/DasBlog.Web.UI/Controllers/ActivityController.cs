using System;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class ActivityController : Controller
	{
		private ILoggingManager loggingManager;

		public ActivityController(ILoggingManager loggingManager)
		{
			this.loggingManager = loggingManager;
		}
		// GET
		public IActionResult List()
		{
			var events = loggingManager.GetEventsForDay(DateTime.UtcNow);
			return View(events);
		}
	}
}
