using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi
{
    public class Coupon
    {
        public string company { get; set; }
        public long sum { get; set; }
        public Timestamp expireDate { get; set; }
        public string serialNumber { get; set; }




       
        
    }
}
