using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MediaPortal.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound404()
        {
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult InternalServerError500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}