using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;

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
        BehaviourLayer behaviour;
        public string LanIp { get; set; }
        public string WanIp { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public bool Failure { get; set; }
        public int[] ActHours = new int[24];
        public List<int> Ports= new List<int>();
        public Connection(string lanip, string wanip, int time, int port , bool failure,BehaviourLayer behaviour)
        {
            this.behaviour = behaviour;
            LanIP ip = new LanIP(lanip);
            if (!behaviour.list.Contains(ip))
                behaviour.list.Add(ip);
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
    //Class for each ip connections
    //public class Connection
    //{
    //    public String lanip;
    //    public String swanip;
    //    public Connection(String lanip, String swanip)
    //    {
    //        this.lanip = lanip;
    //        this.swanip = swanip;
    //    }
            
    //    public override string ToString()
    //    {
    //        return lanip + " " + swanip;
    //    }

    //}

    public class BehaviourLayer
    {
        public List<LanIP> list = new List<LanIP>();
        public string csv;
        public void WriteCSV()
        {
            csv= String.Join("\n", list.Select(x => x.ToString()).ToArray());
            Console.Write(csv);
            System.IO.File.WriteAllText(@"behaviour.csv", csv);

        }
    }

    public class LanIP
    {
        public override string ToString()
        {
            return ip + ",." + random.Next() + ",." + random.Next() + ",." + random.Next();
        }
        Random random = new Random();
        int[] ah;
        List<int> ActiveHours = new List<int>();
        List<float> ar = new List<float>();
        public string ip;
        int end;
        int start;
        int Null;
        public float ActB, FailB, ScanB;

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
                var jStart = jconn.GroupBy(y => y.WanIp).Select(g => g.Min(b => b.Start));
                Null = 0;
                int[] arr = Enumerable.Repeat(1, 24).ToArray();
                ActiveHours = new List<int>();
                foreach (var conn in ilist)
                {
                    ah = new int[24];
                    int i = start;
                    while (i < end)
                    {
                        if (conn[i] != 0)
                        {
                            ah[i] = 1;
                            arr[i] = 0;
                        }
                        i++;
                    }
                    ActiveHours.Add(ah.Sum());
                }
                Null = arr.Sum();
                t = 0;
                foreach (var start in jStart)
                {
                    try
                    {
                        ar.Add(ActiveHours[t] / (((end - start) + 1) - Null));
                    }
                    catch (DivideByZeroException)
                    { }
                    t++;
                }
                return ar.Min();
            }
        }

        public float ActWeight
        {
            get
            {

                return random.Next();
            }
        }
        public float FailHour
        {
            get
            {
                return random.Next();
            }
        }

        public float FailRate
        {
            get
            {
                return random.Next();
            }
        }
        public float FailWeight
        {
            get
            {
                return random.Next();
            }
        }

        public float FailFlow
        {
            get
            {
                return random.Next();
            }
        }

        public float FailMatch
        {
            get
            {
                return random.Next();
            }
        }

        public float NoExist
        {
            get
            {
                return random.Next();
            }
        }

        public float DPortSum
        {
            get
            {
                return random.Next();
            }
        }
    static class Program
    {
        //Dictionary for all connection_info
        static Dictionary<string, List<ConnectionInfo>> Connections;

        public float ActBehaviour
        {
            get
            {
                ActB = random.Next(0,1000);
                return ActB;
            }
        }

        public float FailBehaviour
        {
            get
            {
                FailB = random.Next(0,1000);
                return FailB;
            }
        }
            public string connection, lanip;
            //public float ActHour(string iconnection)
            //{
            //    connection = iconnection;
            //    lanip = ExtractIP(connection)[0];
            //    var xconn = from conn in Connections where ExtractIP(conn.Key)[0] == lanip select conn;
            //    return xconn.Max(x=>x.Value.acthours.Sum());
            //}
                //, ActRate, ActWeight, FailHour, FailRate, FailWeight, FailFlow, FailMatch, NoExist, DPortSum;

        public float ScanBehaviour
        {
            get
            {
                ScanB = random.Next(0,1000);
                return ScanB;
            }
        }
    }


    static class Program
        {
        public static int[] ActivityHours = new int[24];
        public static BehaviourLayer behaviour;
        //Dictionary for all connection_info

        static void PrintCollections()
        {
            Console.WriteLine("Printing complete connections...");
            foreach(var item in behaviour.list)
            {
                Console.WriteLine(item.ip + " : "+item.ActRate + " : " + item.ActHour + " : " + item.ActWeight + " : " + item.FailRate + " : " + item.FailMatch + " : " + item.FailHour + " : " + item.FailFlow);
            }
            var x = behaviour.list;
            Console.WriteLine("Process Completed..");

        }

        static void Main()
        {
            behaviour = new BehaviourLayer();
            Connections = new Dictionary<string, List<ConnectionInfo>>();
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
            behaviour.WriteCSV();
            KMeans kmeans = new KMeans();
            Console.WriteLine("No of elements to cluster"+ behaviour.list.Count());
            foreach(var x in behaviour.list)
            {
                Console.WriteLine("item : "+x.ActBehaviour + " " + x.FailBehaviour + " " + x.ScanBehaviour);
            }
            kmeans.ClusterData(behaviour.list);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=DataServer;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                conn.Open();
                for(int i =0;i<24;i++)
                {
                    string cmd = "update Activity set activity="+ ActivityHours[i] + " where hour="+i;
                    SqlCommand command = new SqlCommand(cmd, conn);

                    Console.WriteLine(cmd);

                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    //if (command.ExecuteReader().Read())
                    {
                        Console.Write("table updated :  ");
                    }
                    else
                    {
                        Console.WriteLine("no updation :  ");
                    }


                }

            }
            Console.WriteLine("Done.");



        }



        private static void ReadRawData()
        {
//            Connection ikey;
            string key;
            List<ConnectionInfo> value;
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

                ActivityHours[hour]++;

                try
                {
                    var item = from i in Connections.list where i.LanIp == row[1] && i.WanIp == row[2] select i;
                    item = item.ToList();
                    if(item.Count()<=0)
                    {
                        Connections.list.Add(new Connection(row[1], row[2], Convert.ToInt32(row[0]), Convert.ToInt32(row[3]), Convert.ToBoolean(Convert.ToInt32(row[4])),behaviour));
                    }
                    else
                    {
                        item.First().AddConn(Convert.ToInt32(row[0]), Convert.ToInt32(row[3]));
                    }
                }
                catch (ArgumentNullException) {
                    Connections[key].Add(new ConnectionInfo
                    {
                        start = Convert.ToInt32(row[0]),
                        port = Convert.ToInt32(row[3]),
                        failure = Convert.ToBoolean(Convert.ToInt32(row[4]))
                    });
                }
                else
                {
                    value = new List<ConnectionInfo>();
                    value.Add(new ConnectionInfo
                    {
                        start = Convert.ToInt32(row[0]),
                        port = Convert.ToInt32(row[3]),
                        failure = Convert.ToBoolean(Convert.ToInt32(row[4]))
                    });
                    Connections.Add(key, value);
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

