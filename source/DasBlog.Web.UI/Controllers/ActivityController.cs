using System;
using System.Net.Http;
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

		[HttpGet]
		public IActionResult ActivityList()
		{
			return EventsByDate(DateTime.UtcNow);
		}
		[HttpGet(Name="/Activity/ActivityList/date")]
		public IActionResult EventsByDate(DateTime date)
		{
			var events = loggingManager.GetEventsForDay(date);
			ViewBag.Date = date.ToString("yyyy-MM-dd");
			ViewBag.NextDay = (date + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd");
			ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
			return View("ActivityList", events);
			
		}
	}
}
