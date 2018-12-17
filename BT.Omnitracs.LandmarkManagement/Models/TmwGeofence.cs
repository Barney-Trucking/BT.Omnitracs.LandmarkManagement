using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT.Omnitracs.LandmarkManagement.Models
{
    public class TmwGeofence
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double Radius { get; set; }
        public string Id { get; set; }
        public string AtHomeLoc { get; set; }
    }
}
