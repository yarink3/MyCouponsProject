using MyCouponsServer.Controllers;
using MyCouponsServer.Models;
using Google.Cloud.Firestore;
using MyCouponsServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class BackendUnitTests
    {
        public FirestoreDb db = Database.Instance.db;
        public CouponsController controller = new CouponsController();
        public Coupon coupon = new Coupon
        {
            company = "company",
            ammount = 111,
            expireDate = DateTime.Now,
            fullyUsedDate = new DateTime(),
            serialNumber = "789",
            imageUrl = "imageUrl",
            id = "company789"

        };

        [TestMethod]
        public void AddCouponTest()
        {
            
            controller.AddCoupon("check1", coupon);
            
            Assert.IsNotNull(db.Collection($"Tests/check1/CouponsList").Document("company789"), "can not add user!");

        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [TestMethod]
        public async Task DeleteCouponTest()
        {
            string username = RandomString(3);
            DocumentReference docRef = db.Collection($"Tests/{username}/CouponsList").Document("company789");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Assert.IsFalse(snapshot.Exists, "can not delete user!");

            controller.AddCoupon(username, coupon);
            Assert.IsNotNull(db.Collection($"Tests/{username}/CouponsList").Document("company789"), "can not add user!");

            controller.DeleteCoupon(username, "company789");

        }

        [TestMethod]
        public async Task UseCouponTest()
        {
            
            controller.AddCoupon("check1", coupon);
            Assert.IsNotNull(db.Collection($"Tests/check1/CouponsList").Document("company789"), "can not add user!");

            long originalAmmount = coupon.ammount;
            coupon.ammount = 50;
            controller.ChangeCoupon("check1","edit", coupon);

            DocumentReference docRef = db.Collection($"Users/UserCheck/CouponsList").Document("company789");
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> etidedCoupon = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in etidedCoupon)
                {
                    Dictionary<string, object> value = (Dictionary<string, object>)pair.Value;
                    Assert.Equals(value["ammount"], originalAmmount+50);
                        
                }

            }

        }


    }
}