using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class User_Authorization
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonElement("authorization")]
        public Models.Authorization authorization { get; set; }

        [BsonElement("user_details")]
        public User user { get; set; }
    }
}
