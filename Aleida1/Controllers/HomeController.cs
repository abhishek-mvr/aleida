using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aleida1.Models;
using Microsoft.AspNetCore.Authorization;

namespace Aleida1.Controllers
{
    public class HomeController : Controller
    {
        private DataServerContext ds = new DataServerContext();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

      //  [Authorize]
        public IActionResult Analytics()
        {
            return View(ds.Pcdetails.ToList());
        }

        public IActionResult Details(int id = 1)
        {
            Pcdetails pc = ds.Pcdetails.Find(id);
            return View(pc);
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
