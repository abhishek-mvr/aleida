using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace AleidaConsole
{
    
    public static class Connections
    {
        public static List<Connection> list;

        public static bool ContainsConn(string lanip, string wanip)
        {
            bool has = list.Any(conn => conn.LanIp == lanip && conn.WanIp == wanip);
            return has;
        }


    }

    //Class for info on each connections
    public class Connection
    {
        public string LanIp { get; set; }
        public string WanIp { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public bool Failure { get; set; }
        public int[] ActHours = new int[24];
        public List<int> Ports;
        public Connection(string lanip, string wanip, int time, int port , bool failure)
        {
            this.LanIp = lanip;
            this.WanIp = wanip;
            this.Start = time;
            this.End = time;
            this.Failure = failure;
            this.ActHours[time] = 1;
            Ports.Add(port);

    }

        public void AddConn(int time, int port)
        {
            ActHours[time]++;
            if (time < Start)
                Start = time;
            if (time > End)
                End = time;
            if (!Ports.Contains(port))
                Ports.Add(port);
        }

    }


    public class LanIP
    {
        string ip;
        int end;
        int start;
        int Null;

        public float ActHour
        {
            get
            {
                var x = from conn in Connections.list where conn.LanIp == ip select conn.ActHours.Sum();
                float y = x.Max();
                return y;
            }
        }

        public float ActRate
        {
            get
            {
                int t = 0;
                int[] ah = new int[24];
                List<float> ar = new List<float>();
                var iend = from conn in Connections.list where conn.LanIp == ip select conn.End;
                end = iend.Max();
                var istart = from conn in Connections.list where conn.LanIp == ip select conn.Start;
                start = istart.Min();
                var ilist = from conn in Connections.list where conn.LanIp == ip select conn.ActHours;
                Null = 0;
                int[] arr = Enumerable.Repeat(1, 24).ToArray();
                foreach(var conn in ilist)
                {
                    int i = start;
                    t = 0;
                    while(i<end)
                    {
                        if(conn[i]!=0)
                        {
                            ah[t]++;
                            if(arr[i]!=0)
                            {
                                arr[i] = 0;
                            }
                        }
                        i++;
                    }
                    t++;
                }
                Null = arr.Sum();
                t = 0;
                foreach(var start in istart)
                {
                    ar[t] = ah[t] / (((end - start) + 1) - Null);
                }
                return ar.Min();
            }
        }

        public float ActWeight
        {
            get
            {

            }

        }
    }


    static class Program
        {
        //Dictionary for all connection_info

        class DPLayerItem
        {
            public string connection, lanip;
            public float ActHour(string iconnection)
            {
                connection = iconnection;
                lanip = ExtractIP(connection)[0];
                var xconn = from conn in Connections where ExtractIP(conn.Key)[0] == lanip select conn;
                return xconn.Max(x=>x.Value.acthours.Sum());
            }
                //, ActRate, ActWeight, FailHour, FailRate, FailWeight, FailFlow, FailMatch, NoExist, DPortSum;

        }

        static void PrintCollections()
        {
            DPLayerItem dPItem = new DPLayerItem();
            Console.WriteLine("Printing complete connections...");
            foreach(var item in Connections)
            {
                Console.WriteLine(item.Key + " : "+dPItem.ActHour(item.Key));
            }
        }

        static void Main()
        {
//          DPLayerItem dPLayerItem;
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



        private static void ReadRawData()
        {
            int hour;
            StreamReader readFile = new StreamReader("RawData.csv");
            string line;
            string[] row;
            while ((line = readFile.ReadLine()) != null)
            {
                row = line.Split(',');
                hour = Convert.ToInt32(row[0]);

                if (IsSuspected(row[2]))
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                else
                    Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(line);

                var item = from i in connections.list where i.LanIp==row[1] && i.WanIp==row[2] select i ;
                if(item!=null)
                {
                    item.ToList()[0].AddConn(Convert.ToInt32(row[0]), Convert.ToInt32(row[3]));
                }
                else
                {
                    connections.list.Add(new Connection(row[1], row[2], Convert.ToInt32(row[0]), Convert.ToInt32(row[3]), Convert.ToBoolean(row[4])));
                }
            }

        readFile.Close();
        Console.ForegroundColor = ConsoleColor.White;

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
    }

}

