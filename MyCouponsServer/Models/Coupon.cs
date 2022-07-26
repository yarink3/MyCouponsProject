using Google.Cloud.Firestore;

namespace MyCouponsServer.Models
{

    public class Coupon
    {

        public string? id { get; set; }
        public string? company { get; set; }
        public long ammount { get; set; }
        public string? expireDateStr { get; set; }
        public DateTime? expireDate { get; set; }
        public string? fullyUsedDateStr { get; set; }
        public DateTime? fullyUsedDate { get; set; }
        public string? serialNumber { get; set; }
        public string? imageUrl { get; set; }
        public IFormFile? Image { get; set; }

    }
}