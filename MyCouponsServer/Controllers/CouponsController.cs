using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCouponsServer.Models;
using Google.Cloud.Firestore;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace MyCouponsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        public FirestoreDb db = Database.Instance.db;

        public CouponsController()
        {
            //_context = context;
        }

        //TODO: get all docs
         //GET: api/Coupons/username
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCouponsList(string username)
        {
            List<Coupon> couponsOutput = new List<Coupon>();
            try
            {
                Query allCoupons = db.Collection($"Users/{username}/CouponsList");
                QuerySnapshot allCouponsSnapshot = await allCoupons.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in allCouponsSnapshot.Documents)
                {
                    Dictionary<string, object> coupon = documentSnapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in coupon)
                    {
                        Dictionary<string, object> couponData = (Dictionary<string, object>)pair.Value;
                        DateTime expire = ((Timestamp)couponData["expireDate"]).ToDateTime();
                        DateTime used = ((Timestamp)couponData["fullyUsedDate"]).ToDateTime();

                        Coupon couponToAdd = new Coupon
                        {
                            company = (string)couponData["company"],
                            ammount = (long)couponData["ammount"],
                            expireDate = expire,
                            fullyUsedDate = used,
                            serialNumber = (string)couponData["serialNumber"],
                            imageString = (string)couponData["imageString"],
                            id = ((string)couponData["company"] + (string)couponData["serialNumber"])

                        };
                        couponsOutput.Add(couponToAdd);


                    }
                    Console.WriteLine("");
                }


                return Ok(couponsOutput);
            }
            catch
            {
                return BadRequest("couldnt get all coupons");
            }
        }



        // POST: api/Coupons/use/username/
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=

        [HttpPut("use/{username}/{type}")]
        public async Task<IActionResult> ChangeCoupon(string username,string type, [FromForm] Coupon data)
        {
            DocumentReference docRef = db.Collection($"Users/{username}/CouponsList").Document(data.id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> snapDict = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in snapDict)
                {
                    Dictionary<string, object> usageData = (Dictionary<string, object>) pair.Value;
                    if ((long) usageData["ammount"] < (long)data.ammount)
                    {
                        return BadRequest($"you only have {usageData["ammount"]} on this coupon");
                    }

                    if (type == "edit")
                    {
                        usageData["originalAmmount"] = usageData["ammount"];
                    }
                    else if (type == "use")
                    {
                        usageData["ammount"] = (long)usageData["ammount"] - (long)data.ammount;
                    }
                    

                    if ((long)usageData["ammount"] == 0)
                    {

                        usageData["fullyUsedDate"] = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
                    }

                    docRef.SetAsync(snapDict);
                }
                return Ok();
            }

            return BadRequest("Couldnt change");
        }

        //public Timestamp getTimestamp(string datetimeStr)
        //{ 
        //    return Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(datetimeStr), DateTimeKind.Utc));
        //}
        // POST: api/Coupons/newcoupon/username
        [HttpPost("newcoupon/{username}")]
        public string AddCoupon(string username, [FromForm] Coupon data)
        {
            DocumentReference docRef = db.Collection("Users").Document($"{username}/CouponsList/{data.id}");
            Timestamp expireDate = Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(data.expireDateStr), DateTimeKind.Utc));
            Timestamp fullyUsedDate = new Timestamp();
            Dictionary<string, object> coupon = new Dictionary<string, object>
            {
                    { "company",data.company },
                    { "ammount",data.ammount },
                    { "originalAmmount",data.ammount },
                    { "expireDate",expireDate },
                    { "serialNumber",data.serialNumber },
                    { "imageString",data.imageString },
                    { "fullyUsedDate",fullyUsedDate }
            };

            Dictionary<string, object> couponToAdd = new Dictionary<string, object>
            {
                { data.id,coupon }
            };
            docRef.SetAsync(couponToAdd);



            return JsonConvert.SerializeObject(coupon);
        }

        // DELETE: api/Coupons/use/{username}/{id}
        [HttpDelete ("delete/{username}/{id}")]
        public async Task<IActionResult> DeleteCoupon(string username,string id)
        {
            DocumentReference docRef = db.Collection($"Users/{username}/CouponsList").Document(id);
            DocumentReference archive = db.Collection($"Archive/CouponsList/{username}").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Dictionary<string, object> docDict = snapshot.ToDictionary();
            docDict["dateDeleted"]= Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(DateTime.Now), DateTimeKind.Utc));
            archive.SetAsync(docDict);

            await docRef.DeleteAsync();
            

            return Ok("deleted");
        }

    }
}
