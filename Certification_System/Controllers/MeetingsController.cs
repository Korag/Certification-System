using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.Controllers
{
    public class MeetingsController : Controller
    {
        // GET: AddNewMeetingPartial
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewMeetingPartial(int MeetingIdentificator)
        {
            ViewBag.Id = MeetingIdentificator;
            return PartialView();
        }
    }
}