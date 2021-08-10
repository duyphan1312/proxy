using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrongProxyAPI.Models
{
    public class StaticIP
    {
        public string PCName { get; set; }
        public string IP { get; set; }
        public string SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DNS1 { get; set; }
        public string DNS2 { get; set; }
    }
}
