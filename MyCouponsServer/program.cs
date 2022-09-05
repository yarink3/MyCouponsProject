using Microsoft.EntityFrameworkCore;
using MyCouponsServer.Models;
using Microsoft.Net.Http.Headers;
using System.Threading;
using Google.Cloud.Firestore;
using MyCouponsServer;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Diagnostics;
using MyCouponsServer.Controllers;

var MyAllowSpecificOrigins = "_MyAllowSubdomainPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corspolicy",
                      build =>
                      {
                          build.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<CouponContext>(opt =>
    opt.UseInMemoryDatabase("CouponsList"));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseCors("corspolicy");
app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

SmtpClient smtpClient = 
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


async void checkCouponsForMails()
{
    FirestoreDb db = Database.Instance.db;
    while (true)
    {
        try
        {
            Query allCoupons = db.Collection($"Coupons");
            QuerySnapshot allCouponsSnapshot = await allCoupons.GetSnapshotAsync();
            DateTime now = DateTime.Now;
            while (now.Hour != 9) {
                int hours = 9-now.Hour;
                int minutes = 60-now.Minute;


                Thread.Sleep(1000 * 60 * minutes * hours);
            }
            
            foreach (DocumentSnapshot snapshotFromUser in allCouponsSnapshot.Documents)
            {
                string couponId = snapshotFromUser.Id;

                DocumentReference coupon = db.Collection("Coupons").Document(couponId);
                DocumentSnapshot snapshot = await coupon.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Dictionary<string, object> dbCouponData = snapshot.ToDictionary();
                    DocumentSnapshot userDetails = await db.Collection($"Users/{dbCouponData["creator"]}/Profile").Document("SimpleDetails").GetSnapshotAsync();
                    if (userDetails.Exists)
                    {
                        string userMail = (string)userDetails.ToDictionary()["email"];
                        DateTime expire = ((Timestamp)dbCouponData["expireDate"]).ToDateTime();
                        int daysToExpire = (expire - now).Days;

                        if (daysToExpire < 7)
                        {
                            //var apiKey = Environment.GetEnvironmentVariable("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\");
                            //var client = new SendGridClient("kDuppEZyToaaxF4EtvIYzw");
                            try
                            {

                                smtpClient.Send(new MailMessage
                                {
                                    From = new MailAddress("mycouponsorg@Gmail.com", "My Coupin Organizer"),
                                    To = { userMail },
                                    Subject = "Your coupon is about to expire",
                                    Body =
                                    "Hi :)\n\n" +

                                    $"We noticed that your coupon to {dbCouponData["company"]} is about to expire in {daysToExpire} days.\n\n" +

                                    $"Did you use it already? edit / delete the coupon from your list here - TODO: add url. \n\n " +

                                    "Have a nice day, \n\n" +

                                    "My Coupons Organizer team.",

                                    BodyEncoding = Encoding.UTF8
                                });

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("error: " + ex.Message);

                            }
                        }
                        int y = 8;
                    }
                }
            }

        }
        catch { }
        Thread.Sleep(1000*60*60*24);

    }
}

Thread t = new Thread(new ThreadStart(checkCouponsForMails));
t.Start();
app.Run();
