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
using Microsoft.AspNetCore.Cors;
//using System.Web.Http.Cors;
using Firebase.Storage;


namespace MyCouponsServer.Controllers
{
    [EnableCors("corspolicy")]

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
        public async Task<ActionResult<IEnumerable<String>>> GetCouponsList(string username)
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
                            imageUrl = (string)couponData["imageUrl"],
                            id = ((string)couponData["company"] + (string)couponData["serialNumber"])

                        };
                        couponsOutput.Add(couponToAdd);

                    }
                    Console.WriteLine("");
                }

               string jsonString = JsonConvert.SerializeObject(couponsOutput);
                
                return Ok(jsonString);
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


        public class Image
        {

            public int Id { get; set; }
            public string FileName { get; set; }
            public byte[] Picture { get; set; }

        }

        // POST: api/Coupons/newcoupon/username
        [HttpPost("newcoupon/{username}")]
        public async Task<IActionResult> AddCoupon(string username, [FromForm] Coupon data)
        {
            string imageUrl;
            try
            {
                var stream = data.Image.OpenReadStream();
                // Construct FirebaseStorage with path to where you want to upload the file and put it there
                var task = new FirebaseStorage("mycoupons-6058d.appspot.com")
                 .Child("Users")
                 .Child($"{username}")
                 .Child("CouponsList")
                 .Child($"{data.id}")
                 .PutAsync(stream);

                 imageUrl = await task;

            }
            catch
            {
                imageUrl = "";
            }


            try
            {
                DocumentReference docRef = db.Collection("Users").Document($"{username}/CouponsList/{data.id}");
                Timestamp expireDate = Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(data.expireDateStr), DateTimeKind.Utc));
                Timestamp fullyUsedDate = new Timestamp();
                Dictionary<string, object> coupon = new Dictionary<string, object>
            {
                    { "id",data.id },
                    { "company",data.company },
                    { "ammount",data.ammount },
                    { "originalAmmount",data.ammount },
                    { "expireDate",expireDate },
                    { "serialNumber",data.serialNumber },
                    { "imageUrl",imageUrl },
                    { "fullyUsedDate",fullyUsedDate }
            };

                Dictionary<string, object> couponToAdd = new Dictionary<string, object>
            {
                { data.id,coupon }
            };
                docRef.SetAsync(couponToAdd);



                return Ok(coupon);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/Coupons/use/{username}/{id}
        [HttpDelete ("delete/{username}/{id}")]
        public async Task<IActionResult> DeleteCoupon(string username,string id)
        {
            DocumentReference docRef = db.Collection($"Users/{username}/CouponsList").Document(id);
            DocumentReference archive = db.Collection($"Archive/CouponsList/{username}").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> docDict = snapshot.ToDictionary();
                docDict.Add("dateDeleted", Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(DateTime.Now), DateTimeKind.Utc)));
                archive.SetAsync(docDict);

                await docRef.DeleteAsync();

                return Ok("Deleted");
            }
            

            return BadRequest("Couldn't delete");
        }

    }
}
