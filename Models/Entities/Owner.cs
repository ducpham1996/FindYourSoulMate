using FindYourSoulMate.Models.Manager;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Owner
    {

        //[BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonIgnore]
        public string user_name { get; set; }

        [BsonIgnore]
        public string user_picture { get; set; }

        [BsonElement("email")]
        public string email { get; set; }
    }
}
