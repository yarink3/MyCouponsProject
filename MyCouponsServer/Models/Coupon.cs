using Google.Cloud.Firestore;
using System.Collections;

namespace MyCouponsServer.Models
{

    public class Coupon
    {

        public string? id { get; set; }
        public string? creator { get; set; }
        public string? company { get; set; }
        public long? ammount { get; set; }
        public string? expireDateStr { get; set; }
        public DateTime? expireDate { get; set; }
        public string? fullyUsedDateStr { get; set; }
        public DateTime? fullyUsedDate { get; set; }
        public string? serialNumber { get; set; }
        public string? imageUrl { get; set; }
        public IFormFile? Image { get; set; }


        public object? getValue(string key)
        {
            switch (key)
            {
                case "company":
                    return company;
                case "ammount":
                    return ammount;
                case "expireDateStr":
                    return expireDateStr;
                case "serialNumber":
                    return this.serialNumber;
                case "imageUrl":
                    return imageUrl;
                case "Image":
                    return Image;
            }

            return null;
        }

        public ArrayList getChangedKeys()
        {
            var keys = new ArrayList();

            if(company != null)
            {
                keys.Add("company");
            }
            if(ammount != null)
            {
                keys.Add("ammount");
            }
            if (expireDateStr != null)
            {
                keys.Add("expireDateStr");
            }
            if (serialNumber != null)
            {
                keys.Add("serialNumber");
            }

            if (Image != null)
            {
                keys.Add("Image");
            }

            return keys;

        } 

    }

    
}