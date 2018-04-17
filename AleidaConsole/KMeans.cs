//Austin Henley

//
//Lots of optimizations to be done. Such as the duplicate array creations. Library could also be easier to use (put the loop INSIDE the library call).
//

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AleidaConsole
{
    class KMeans
    {
        public void ClusterData(List<LanIP> list)
        {
            List<Point3D> data = new List<Point3D>();
            for(int i=0;i<list.Count();i++)
            {
                data.Add(new Point3D(Convert.ToInt32(list[i].ActB), Convert.ToInt32(list[i].FailB), Convert.ToInt32(list[i].ScanB),(new Random()).Next(0,2),list[i].ip));
            }
            int k = 2;
            List<Point3D> centroids = new List<Point3D>();
            for (int i = 0; i < k; i++)
                centroids.Add(Point3D.RandomPoint(i));


            int round = 1;
            Point3D[] oldC;
            do
            {
                oldC = centroids.ToArray();
                data = Cluster(centroids, data);
                centroids = UpdateCentroids(centroids, data, k);
                PrintClusters(centroids, data, round);
                round++;
            } while (!CheckForConvergence(oldC, centroids.ToArray()));
            Console.WriteLine("Convergence after " + round + " rounds!");
            PrintFile(data, round, list);

            Console.ReadLine();
        }

        static List<Point3D> Cluster(List<Point3D> centroids, List<Point3D> data)
        {
            foreach (var p in data) //find closest centroid
            {
                int cent = -1;
                float dist = 9999999;
                foreach (var c in centroids)
                {
                    float d = p.Distance(c);
                    if (d < dist)
                    {
                        dist = d;
                        cent = c.cluster;
                    }
                }
                p.cluster = cent;
            }

            return data;
        }

        static bool CheckForConvergence(Point3D[] oldC, Point3D[] newC)
        {
            for (int i = 0; i < oldC.Length; i++)
            {
                if (!oldC[i].EqualTo(newC[i]))
                    return false;
            }

            return true;
        }

        static List<Point3D> UpdateCentroids(List<Point3D> centroids, List<Point3D> data, int k)
        {
            Point3D[] runningTotal = new Point3D[k];
            int[] counts = new int[k];
            for (int i = 0; i < k; i++) //initialize, maybe replace this with LINQ later?
            {
                runningTotal[i] = new Point3D(0, 0, 0, i,"");
                counts[i] = 0;
            }
            foreach (var p in data) //update centroids
            {
                runningTotal[p.cluster].Add(p);
                counts[p.cluster]++;
            }
            foreach (var total in runningTotal) //find averages
            {
                total.Divide(counts[total.cluster]);
                centroids[total.cluster] = new Point3D(total);
            }

            return centroids;
        }

        static void PrintFile(List<Point3D> data, int round,List<LanIP> lanip)
        {
            List<string> output = new List<string>();
            output.Add("ip,x,y,z,cluster");
            foreach (var p in data)
            {
                output.Add(p.ToCSV());
            }
            File.WriteAllLines("output-" + round + ".txt", output.ToArray());
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=DataServer;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                conn.Open();
                Console.WriteLine("Storing result to DB");
                for (int i=0;i<data.Count();i++)
                {
                    string cmd = "update PCDetails set Affected=" + data[i].cluster + " where IpAddress like '%" + data[i].ip + "%'";
                    SqlCommand command = new SqlCommand(cmd, conn);
                    int count = command.ExecuteNonQuery();
                    if(count>0)
                    //if (command.ExecuteReader().Read())
                    {
                        Console.WriteLine("table updated");
                    }
                    else
                    {
                        Console.WriteLine("no updation");
                    }
                    Console.WriteLine(cmd);
                    //Console.WriteLine(data[i].ip + " : " + data[i].cluster);
                }

            }
        }

        static void PrintClusters(List<Point3D> centroids, List<Point3D> data, int round)
        {
            Console.WriteLine("Round " + round);
            foreach (var c in centroids)
            {
                Console.Write("Cluster " + c.cluster + " " + c.ToString() + ":    ");
                foreach (var p in data)
                {
                    if (p.cluster == c.cluster)
                        Console.Write(p.ToString() + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }


    }

    public class Point3D
    {
        static Random r = new Random();
        public int x;
        public int y;
        public int z;
        public int cluster;
        public string ip;

        public Point3D(int a, int b, int c, int k,string ip)
        {

            x = a;
            y = b;
            z = c;

            cluster = k;
            this.ip = ip;
        }

        public Point3D(Point3D p)
        {
            x = p.x;
            y = p.y;
            z = p.z;
            cluster = p.cluster;
            ip = p.ip;
        }

        public override string ToString()
        {
            return "(" + ip + ")";
        }

        public string ToCSV()
        {
            return ip + "," + x + "," + y + "," + z + "," + cluster + "\r\n";
        }

        //randomly create a 3D point with values 0-255, k is the cluster (-1 means unclustered)
        public static Point3D RandomPoint(int k = -1)
        {
            return new Point3D(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256), k,"");
        }

        public bool EqualTo(Point3D p)
        {
            if (x == p.x && y == p.y && z == p.z)
                return true;
            else
                return false;
        }

        public float Distance(Point3D p)
        {
            float deltaX = x - p.x;
            float deltaY = y - p.y;
            float deltaZ = z - p.z;

            return (float) Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        public void Add(Point3D p)
        {
            x += p.x;
            y += p.y;
            z += p.z;
        }

        public void Divide(int div)
        {
            if (div == 0)
            {
                x = 0;
                y = 0;
                z = 0;
                return;
            }

            x /= div;
            y /= div;
            z /= div;
        }
    }
}

