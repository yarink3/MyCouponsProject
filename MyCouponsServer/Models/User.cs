using Google.Cloud.Firestore;
using System.Collections;

namespace MyCouponsServer.Models
{

    public class User
    {

        public string? id { get; set; }
        public string? displayName { get; set; }
        public string? email { get; set; }
        public string? emailText { get; set; }

        public string? verificationCode { get; set; }
        public string[]? familyMembersIds { get; set; }


    }

    
}