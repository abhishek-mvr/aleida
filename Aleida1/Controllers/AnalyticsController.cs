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
    public class AnalyticsController : Controller
    {
        private DataServerContext ds = new DataServerContext();
        //[Authorize]

        public IActionResult Index()
        {
            Console.WriteLine("Index method in Analytics");
            return View(ds.Pcdetails.ToList());
        }

        public IActionResult Details(int id = 1)
        {
            Pcdetails pc = ds.Pcdetails.Find(id);
            return View(pc);
        }

    }
}