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
using System.Text;
using System.Net;
using Google.Type;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace MyCouponsServer.Controllers
{
    [EnableCors("corspolicy")]

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public FirestoreDb db;//= Database.Instance.db;
        public string BucketName;//= "mycoupons-6058d.appspot.com";
        public StorageClient storage;// = StorageClient.Create();
        SmtpClient smtpClient;// =
        //new SmtpClient
        //{
        //    Host = "Smtp.Gmail.com",
        //    Port = 587,
        //    EnableSsl = true,
        //    Timeout = 10000,
        //    DeliveryMethod = SmtpDeliveryMethod.Network,
        //    UseDefaultCredentials = false,
        //    Credentials = new NetworkCredential("mycouponsorg@Gmail.com", "thnszmwogbnizqis")
        //};

        public UsersController()
        {
            //_context = context;
            db = Database.Instance.db;
            BucketName = "mycoupons-6058d.appspot.com";
            storage = StorageClient.Create();
            smtpClient =
            new SmtpClient
            {
                Host = "Smtp.Gmail.com",
                Port = 587,
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mycouponsorg@Gmail.com", "thnszmwogbnizqis")
            };
    }

        
         //GET: api/Coupons/username
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<String>>> GetUser(string id)
        {
            try
            {
                

                DocumentReference docRef = db.Collection($"Users/{id}/Profile").Document("SimpleDetails");
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                Dictionary<string, object> dbUserData = snapshot.ToDictionary();


                    string jsonString = JsonConvert.SerializeObject(dbUserData);
                
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
        public async Task<IActionResult> ChangeCoupon([FromForm] User data)
        {
            

            return BadRequest("Couldnt change");
        }

        //public Timestamp getTimestamp(string datetimeStr)
        //{ 
        //    return Timestamp.FromDateTime(DateTime.SpecifyKind(Convert.ToDateTime(datetimeStr), DateTimeKind.Utc));
        //}

        // POST: api/Users/sendVerificationMail
        [HttpPost("sendVerificationMail")]
        public async Task<IActionResult> sendVerificationMail([FromForm] User user)
        {

            try
            {
                smtpClient.Send(new MailMessage
                {
                    From = new MailAddress("mycouponsorg@Gmail.com", "My Coupon Organizer"),
                    To = { user.email },
                    Subject = "Verification Mail",
                    Body =
                            "Hi :)\n\n" +

                            $"We are happy that you choose our service.\n\n" +

                            $"Please enter/paste this value as confirmation code: {user.verificationCode}. \n\n " +

                            "Have a nice day, \n\n" +

                            "My Coupons Organizer team.",

                    BodyEncoding = Encoding.UTF8
                });

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("support")]
        public async Task<IActionResult> sendSupportMail([FromForm] User user)
        {

            try
            {
                smtpClient.Send(new MailMessage
                {
                    From = new MailAddress("mycouponsorg@Gmail.com", "My Coupon Organizer"),
                    To = { "mycouponsorg@Gmail.com" },
                    Subject = "Support Mail",
                    Body =
                            "Hi :)\n\n" +

                            $"User {user.id} having probles, here is the details he sent:\n\n" +

                            $"{user.emailText} \n\n"+

                            $"You can return him in email: {user.email}. \n\n " +

                            "Have a nice day, \n\n" +

                            "My Coupons Organizer team.",

                    BodyEncoding = Encoding.UTF8
                });

                smtpClient.Send(new MailMessage
                {
                    From = new MailAddress("mycouponsorg@Gmail.com", "My Coupon Organizer"),
                    To = { user.email },
                    Subject = "Support Mail",
                    Body =
                            $"Hi {user.displayName} :)\n\n" +

                            $"We got your support ticket , and will try to solve your issue soon.\n\n" +  

                            "Have a nice day, \n\n" +

                            "My Coupons Organizer team.",

                    BodyEncoding = Encoding.UTF8
                });

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("newuser")]
        public async Task<IActionResult> AddUser([FromForm] User user)
        {
            try
            {
                DocumentReference userDoc = db.Collection($"Users/{user.id}/Profile").Document("SimpleDetails");
                DocumentSnapshot snapshot = await userDoc.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    return Ok("User already exist");
                }

                Dictionary<string, object> dbUser = new Dictionary<string, object>
                {
                    { "id",user.id },
                    { "email",user.email },
                    { "displayName",user.displayName }
                };


                await userDoc.SetAsync(dbUser);

                return Ok(dbUser);
            }
            catch
            {
                return BadRequest();
            }



        }
         // DELETE: api/Users
        [HttpDelete]
        public async Task<IActionResult> DeleteCoupon([FromForm] User user)
        {

            


               return Ok("Deleted");
          
            

            return BadRequest("Couldn't delete");
        }

    }
}
