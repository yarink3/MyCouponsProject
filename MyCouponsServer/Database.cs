using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace MyCouponsServer
{
    public class Database
    {
        private Database() { }
        private static Database instance = null;
        public FirestoreDb db;
        public static Database Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Database();
                    string path = AppDomain.CurrentDomain.BaseDirectory + @"FirestoreKey.json";
                    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                    instance.db = FirestoreDb.Create("mycoupons-6058d");
                    
                }
                return instance;
            }
        }
    }

}
