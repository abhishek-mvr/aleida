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
                if (line.Equals(ip))
                {
                    return true;
                }
            }
            return false;
        }

        [Authorize]
        public IActionResult Index()
        {
            var activity = ds.Activity.ToList();
            int[] act = new int[24];
            foreach(var item in activity)
            {
                act[Convert.ToInt32(item.Hour)]+= Convert.ToInt32(item.Activity1);
            }
            string str = "[";
            for(int i=0;i<23;i++)
            {
                str = str + act[i]*2 + ",";
            }
            str = str + act[23]*2 + "]" ;
            ViewData["str"] = str;
            Console.WriteLine("Index method in Analytics");
            return View(ds.Pcdetails.ToList());
        }

        [Authorize]
        public IActionResult Details(int id = 1)
        {
            Pcdetails pc = ds.Pcdetails.Find(id);
            var activity = from item in ds.Activity where item.ip==pc.IpAddress select item;
            //            var activity = ds.Activity.ToList();
            int[] act = new int[24];
            foreach (var item in activity)
            {
                act[Convert.ToInt32(item.Hour)] += Convert.ToInt32(item.Activity1);
            }
            string str = "[";
            for (int i = 0; i < 23; i++)
            {
                str = str + act[i] * 2 + ",";
            }
            str = str + act[23] * 2 + "]";
            ViewData["str"] = str;
            return View(pc);
        }



    }
}