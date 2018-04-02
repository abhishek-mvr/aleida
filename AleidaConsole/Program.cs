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
        public static List<Connection> list=new List<Connection>();

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
        public List<int> Ports= new List<int>();
        public Connection(string lanip, string wanip, int time, int port , bool failure)
        {
            LanIP ip = new LanIP(lanip);
            if (!LanIPs.list.Contains(ip))
                 LanIPs.list.Add(ip);
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

    public static class LanIPs
    {
        public static List<LanIP> list = new List<LanIP>();
    }

    public class LanIP
    {
        int[] ah;
        List<int> ActiveHours = new List<int>();
        List<float> ar = new List<float>();
        public string ip;
        int end;
        int start;
        int Null;

        public LanIP(string ip)
        {
            this.ip = ip;
        }

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
                var jconn = from conn in Connections.list where conn.LanIp == ip select conn;
                var istart = jconn.Select(x => x.Start);
                var iend = jconn.Select(x => x.End);
                var ilist = jconn.Select(x => x.ActHours);
                end = iend.Max();
                start = istart.Min();
                var jStart = jconn.GroupBy(y => y.WanIp).Select(g => g.Min(b=>b.Start));
                Null = 0;
                int[] arr = Enumerable.Repeat(1, 24).ToArray();
                ActiveHours = new List<int>();
                foreach (var conn in ilist)
                {
                    ah = new int[24];
                    int i = start;
                    while(i<end)
                    {
                        if(conn[i]!=0)
                        {
                            ah[i]=1;
                            arr[i] = 0;
                        }
                        i++;
                    }
                    ActiveHours.Add(ah.Sum());
                }
                Null = arr.Sum();
                t = 0;
                foreach(var start in jStart)
                {
                    try
                    {
                    ar.Add(ActiveHours[t] / (((end - start) + 1) - Null));
                    }
                    catch(DivideByZeroException)
                    {}
                    t++;
                }
                return ar.Min();
            }
        }
    }


    static class Program
        {
        //Dictionary for all connection_info

        static void PrintCollections()
        {
            Console.WriteLine("Printing complete connections...");
            foreach(var item in LanIPs.list)
            {
                Console.WriteLine(item.ip + " : "+item.ActRate + " : " +item.ActHour);
            }
            var x = LanIPs.list;
            Console.WriteLine("Process Completed..");

        }

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

                try
                {
                    var item = from i in Connections.list where i.LanIp == row[1] && i.WanIp == row[2] select i;
                    item = item.ToList();
                    if(item.Count()<=0)
                    {
                        Connections.list.Add(new Connection(row[1], row[2], Convert.ToInt32(row[0]), Convert.ToInt32(row[3]), Convert.ToBoolean(Convert.ToInt32(row[4]))));
                    }
                    else
                    {
                        item.First().AddConn(Convert.ToInt32(row[0]), Convert.ToInt32(row[3]));
                    }
                }
                catch (ArgumentNullException) {
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

