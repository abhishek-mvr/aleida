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
        private List<IPAddress> LanIP;

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

        //Class for each ip connections
        private class Connection
        {
            IPAddress lanip;
            IPAddress swanip;
        }

        //Class for info on each connections
        private class ConnectionInfo
        {
            int start;
            int end;
            int[] acthours = new int[24];

            public int acthour()
            {
                return acthours.Sum();
            }
        }

        class DPLayer
        {
            Connection LanIP2WanIP;
            float ActHour, ActRate, ActWeight, FailHour, FailRate, FailWeight, FailFlow, FailMatch, NoExist, DPortSum;
        }

        //Dictionary for all connection_info
        Dictionary<Connection, ConnectionInfo> Connections;

        private void ReadRawData()
        {
            String pattern = "s*,s*,s*,s*,s*";
            String[] tokens = new String[5];
            var lines = System.IO.File.ReadLines("RawData.csv");
            foreach (var line in lines)
            {
                tokens = Regex.Split(line,pattern);
            }

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