using System;
using System.Collections.Generic;

namespace Aleida1.Models
{
    public partial class Pcdetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int? Affected { get; set; }
    }
}
