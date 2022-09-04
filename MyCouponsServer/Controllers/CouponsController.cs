using System;
using Microsoft.AspNetCore.Mvc;
using MyCouponsServer.Models;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Firebase.Storage;
using System.Collections;
using Google.Cloud.Storage.V1;
using System.Net.Mail;
using System.Net;

namespace MyCouponsServer.Controllers
{
    [EnableCors("corspolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        public FirestoreDb db = Database.Instance.db;
        //public string BucketName = "mycoupons-6058d.appspot.com";
        public string BucketName = "mycouponsstorage.appspot.com";
        public StorageClient storage = StorageClient.Create();
        public SmtpClient globalSmtpClient =
        new SmtpClient
        {
            Host = "Smtp.Gmail.com",
            Port = 587,
            EnableSsl = true,
            Timeout = 10000,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("mycouponsorg@Gmail.com", "thnszmwogbnizqis")
        } ;

        
        public CouponsController()
        {

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
                foreach (DocumentSnapshot snapshotFromUser in allCouponsSnapshot.Documents)
                {
                    string couponId = snapshotFromUser.Id;

                    DocumentReference coupon = db.Collection($"Coupons").Document(couponId);
                    DocumentSnapshot snapshot = await coupon.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Dictionary<string, object> dbCouponData = snapshot.ToDictionary();

                        DateTime expire = ((Timestamp)dbCouponData["expireDate"]).ToDateTime();
                        DateTime used = ((Timestamp)dbCouponData["fullyUsedDate"]).ToDateTime();

                        Coupon couponToAdd = new Coupon
                        {
                            company = (string)dbCouponData["company"],
                            ammount = (long)dbCouponData["ammount"],
                            expireDate = expire,
                            fullyUsedDate = used,
                            serialNumber = (string)dbCouponData["serialNumber"],
                            imageUrl = (string)dbCouponData["imageUrl"],
                            id = snapshot.Id

                        };
                        couponsOutput.Add(couponToAdd);
                    }
                }
                    

                    string jsonString = JsonConvert.SerializeObject(couponsOutput);
                
                    return Ok(jsonString);
                    
            }
            catch
            {
                return BadRequest("couldnt get all coupons");
            }
        }


        

        // PUT: api/Coupons/useOrEdit/username
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=

        [HttpPut]
        public async Task<IActionResult> ChangeCoupon([FromForm] Coupon data)
        {
            DocumentReference docRef = db.Collection("Coupons").Document(data.id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Dictionary<string, object> dbCouponData = snapshot.ToDictionary();

                    ArrayList changedKeys = data.getChangedKeys();

                    foreach (string key in changedKeys)
                    {
                        if (key == "id" || (key == "ammount" && data.ammount==-1))
                        {
                            continue;
                        }

                        else if (key == "Image")
                        {
                            var stream = data.Image.OpenReadStream();
                            string username = (string)dbCouponData["creator"];
                            string imagePath = $"Users/{username}/CouponsImages/{docRef.Id}";
                            storage.UploadObject(BucketName, imagePath, null, stream);

                        // need that?
                            dbCouponData["imageUrl"] = $"https://firebasestorage.googleapis.com/v0/b/{BucketName}/o/Users%2F{username}%2FCouponsImages%2F{docRef.Id}?alt=media";
                        
                        }
                        else
                        {
                            dbCouponData[key] = data.getValue(key);
                        }
                        
                    }
                    

                    if ((long)dbCouponData["ammount"] == 0)
                    {
                        dbCouponData["fullyUsedDate"] = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
                    }

                    docRef.SetAsync(dbCouponData);
                    
                    return Ok(dbCouponData);
                
                
            }

            return BadRequest("Couldnt change");
        }

        //public Timestamp getTimestamp(string datetimeStr)
        //{ 
        //    return Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(datetimeStr), DateTimeKind.Utc));
        //}

        // POST: api/Coupons/newcoupon/username
        [HttpPost("newcoupon")]
        public async Task<IActionResult> AddCoupon([FromForm] Coupon data)
        {

            try
            {
                Timestamp expireDate = Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(data.expireDateStr), DateTimeKind.Utc));
                Timestamp fullyUsedDate = new Timestamp();
                Dictionary<string, object> coupon = new Dictionary<string, object>
                {
                    { "creator",data.creator },
                    { "company",data.company },
                    { "ammount",data.ammount },
                    { "originalAmmount",data.ammount },
                    { "expireDate",expireDate },
                    { "serialNumber",data.serialNumber },
                    { "fullyUsedDate",fullyUsedDate }
                };

                DocumentReference docRef = await db.Collection("Coupons").AddAsync(coupon);

                string imageUrl;
                try
                {
                    var stream = data.Image.OpenReadStream();
                    string imagePath = $"Users/{data.creator}/CouponsImages/{docRef.Id}";
                    storage.UploadObject("mycouponsstorage.appspot.com", imagePath, null, stream);
                    imageUrl = $"https://firebasestorage.googleapis.com/v0/b/{BucketName}/o/Users%2F{data.creator}%2FCouponsImages%2F{docRef.Id}?alt=media";                
                }
                catch
                {
                    imageUrl = "";
                }

                coupon["imageUrl"] = imageUrl;
                await docRef.SetAsync(coupon);
                Dictionary<string, object> empty = new Dictionary<string, object>() ;
                await db.Collection("Users").Document($"{data.creator}/CouponsList/{docRef.Id}").SetAsync(empty);
                coupon["id"]=docRef.Id;

                return Ok(coupon);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/Coupons
        [HttpDelete]
        public async Task<IActionResult> DeleteCoupon([FromForm] Coupon data)
        {
            string id = data.id;
            string username = data.creator;

            DocumentReference userDoc = db.Collection($"Users/{username}/CouponsList").Document(id);
            DocumentReference couponDoc = db.Collection($"Coupons").Document(id);
            DocumentReference archive = db.Collection($"Archive/CouponsList/{username}").Document(id);
            DocumentSnapshot snapshot = await couponDoc.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                //move coupon to archive
                Dictionary<string, object> docDict = snapshot.ToDictionary();
                docDict.Add("dateDeleted", Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(DateTime.Now), DateTimeKind.Utc)));
                archive.SetAsync(docDict);

                await couponDoc.DeleteAsync();
                await userDoc.DeleteAsync();

                //move image to archive
                try
                {
                    
                    string sourceObjectName = $"Users/{username}/CouponsImages/{id}";
                    string destObjectName = $"Archive/Users/{username}/CouponsImages/{id}";
                    
                    storage.CopyObject(BucketName, sourceObjectName, BucketName, destObjectName);
                    storage.DeleteObject(BucketName, sourceObjectName);


                }
                catch
                {
                    Console.WriteLine("couldnt move image to archive");
                }


                return Ok("Deleted");
            }
            

            return BadRequest("Couldn't delete");
        }

    }
}
