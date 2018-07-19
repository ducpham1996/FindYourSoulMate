using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Like_Record
    {

        [BsonElement("emoji")]
        public string emoji { get; set; }

        [BsonIgnore]
        public string user_name { get; set; }

        [BsonIgnore]
        public string user_picture { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
    }
}
