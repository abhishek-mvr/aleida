using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aleida1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text.RegularExpressions;

namespace Aleida1.Controllers
{
    public class AnalyticsController : Controller
    {
        private DataServerContext ds = new DataServerContext();
        //Check whether the ip is suspected or not from SuspectedIPs.csv file
        bool IsSuspected(IPAddress ip)
        {
            var lines = System.IO.File.ReadLines("SuspectedIPs.csv");
            foreach (var line in lines)
            {
                if(line.Equals(ip))
                {
                    return true;
                }
            }
            return false;
        }

        [Authorize]
        public IActionResult Index()
        {
            Console.WriteLine("Index method in Analytics");
            return View(ds.Pcdetails.ToList());
        }

        [Authorize]
        public IActionResult Details(int id = 1)
        {
            Pcdetails pc = ds.Pcdetails.Find(id);
            return View(pc);
        }



    }
}