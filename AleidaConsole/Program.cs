using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace AleidaConsole
{

    static class Program
    {
        static void Main()
        {
            Console.Write("Starting Aleida\n");
            using (var progress = new ProgressBar())
            { 
                for (int i = 0; i <= 100; i++)
                {
                    progress.Report((double)i / 100);
                    Thread.Sleep(20);
                }
            }
            Console.WriteLine("Done.");
            ReadRawData();
        }

    //Check whether the ip is suspected or not from SuspectedIPs.csv file
    private static bool IsSuspected(String ip)
    {
        if (ip == null)
        {
            throw new ArgumentNullException(nameof(ip));
        }
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

        //Class for each ip connections
        private class Connection
        {
            public String lanip;
            public String swanip;
            public Connection(String lanip, String swanip)
            {
                this.lanip = lanip;
                this.swanip = swanip;
            }
            
            public override string ToString()
            {
                return lanip;
            }

        }

        //Class for info on each connections
        private class ConnectionInfo
        {
            public int start;
            public int end;
            public int[] acthours = new int[24];
            public ConnectionInfo()
            { }
        }

        //Dictionary for all connection_info
        static Dictionary<Connection, ConnectionInfo> Connections;
        static Dictionary<Connection, ConnectionInfo> temp;


        class DPLayer
        {
            Connection con { get; set; }
            public float ActHour()
            {
                var xconn = from conn in Connections where conn.Key.ToString() == con.lanip select conn;
                return xconn.Max(x=>x.Value.acthours.Sum());
            }
                //, ActRate, ActWeight, FailHour, FailRate, FailWeight, FailFlow, FailMatch, NoExist, DPortSum;

        }



        private static void ReadRawData()
        {
            Connection key;
            ConnectionInfo value;
            int hour;
            //String pattern = "s*,s*,s*,s*,s*";
            //String[] token = new String[5];
            //var lines = System.IO.File.ReadLines("RawData.csv");
            //foreach (var line in lines)
            //{
            //    token = Regex.Split(line, pattern);
            //    Console.WriteLine(token[0] + " : " + " source : " + token[0] + " dest : " + token[0] + "\n");

            StreamReader readFile = new StreamReader("RawData.cs");
            string line;
            string[] row;
            var tokens;
            readFile.ReadLine();
            while ((line = readFile.ReadLine()) != null)
            {
                row = line.Split(',');
            }
                hour = token[0]);
                key = new Connection(token[1], token[2]);
                if (Connections.TryGetValue(key, out value))
                {
                    value.acthours[hour]++;
                    if (value.start < hour)
                    {
                        value.start = hour;   
                    }
                    if(value.end > hour)
                    {
                        value.end = hour;
                    }
                    Connections[key] = value;
                }
                else
                {
                    value = new ConnectionInfo();
                    value.start = hour;
                    value.end = hour;
                    value.acthours[hour]++;
                    Connections.Add(key, value);
                }
            }
        readFile.Close();


    }

}

