using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace AleidaConsole
{


    //Class for each ip connections
    public class Connection
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
            return lanip + " " + swanip;
        }

    }

    //Class for info on each connections
    public class ConnectionInfo
    {
        public int start;
        public int end;
        public int[] acthours = new int[24];
        public ConnectionInfo()
        { }
    }



    static class Program
    {

        //Dictionary for all connection_info
        static Dictionary<string, ConnectionInfo> Connections;

        public static String[] extractIP(string con)
        {
            string[] res = con.Split(',');
            return res;
        }
        class DPLayerItem
        {
            public Connection con { get; set; }

            public DPLayerItem(Connection con)
            {
                this.con =con;
            }
            public float ActHour()
            {
                var xconn = from conn in Connections where extractIP(conn.Key)[0] == con.lanip select conn;
                return xconn.Max(x=>x.Value.acthours.Sum());
            }
                //, ActRate, ActWeight, FailHour, FailRate, FailWeight, FailFlow, FailMatch, NoExist, DPortSum;

        }

        static void PrintCollections()
        {
            Console.WriteLine("Printing complete connections...");
            foreach(var item in Connections)
            {
                Console.WriteLine(item.Key);
            }
        }

        static void Main()
        {
//          DPLayerItem dPLayerItem;
            Connections = new Dictionary<string, ConnectionInfo>();
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
            Console.WriteLine("Reading RawData....");
            Thread.Sleep(1000);
            ReadRawData();
            Console.WriteLine("Done.");
            Console.WriteLine("Processing LanIP Layer.....");
            Thread.Sleep(1000);

            PrintCollections();
            //foreach(var item in Connections)
            //{
            //    dPLayerItem = new DPLayerItem(item.Key);
            //    Console.WriteLine(dPLayerItem.con.lanip + " => " + dPLayerItem.con.swanip + " | " + dPLayerItem.ActHour());
            //}

            Console.WriteLine("Done.");

        }

        //Check whether the ip is suspected or not from SuspectedIPs.csv file
        private static bool IsSuspected(String ip)
        {
            IEnumerable<string> lines;
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            try
            {
                lines = File.ReadLines("ClearIPs.csv");
                foreach (var line in lines)
                {
                    if (line.Equals(ip))
                    {
                        return false;
                    }
                }
            }
            catch(IOException)
            {
                Console.WriteLine("The process cannot access the file 'ClearIPs.csv' because its being used by another process");
            }
        return true;
        }


        private static void ReadRawData()
        {
//            Connection ikey;
            string key;
            ConnectionInfo value;
            int hour;
            //String pattern = "s*,s*,s*,s*,s*";
            //String[] token = new String[5];
            //var lines = System.IO.File.ReadLines("RawData.csv");
            //foreach (var line in lines)
            //{
            //    token = Regex.Split(line, pattern);
            //    Console.WriteLine(token[0] + " : " + " source : " + token[0] + " dest : " + token[0] + "\n");

            StreamReader readFile = new StreamReader("RawData.csv");
            string line;
            string[] row;
            while ((line = readFile.ReadLine()) != null)
            {
                row = line.Split(',');
                hour = Convert.ToInt32(row[0]);

                //ikey = new Connection(row[1], row[2]);

                key = row[1] + "," + row[2];
                if (IsSuspected(row[2]))
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                else
                    Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(line);

                if (Connections.ContainsKey(key))
                {
                    value = Connections[key];
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
        Console.ForegroundColor = ConsoleColor.White;

        }
    }

}

