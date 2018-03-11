using System;
using System.Collections.Generic;

namespace Aleida1.Models
{
    public partial class RawData
    {
        public string Lanip { get; set; }
        public string Wanip { get; set; }
        public int Time { get; set; }
        public int Port { get; set; }
        public int Failure { get; set; }
        public int Id { get; set; }
    }
}
