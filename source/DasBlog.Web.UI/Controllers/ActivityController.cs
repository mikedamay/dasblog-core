using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class ActivityController : Controller
	{
		// GET
		public IActionResult List()
		{
			return View();
		}
	}
}
