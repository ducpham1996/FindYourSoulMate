using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class Authorization
    {
        [BsonElement("username")]
        public string username { get; set; }
        [BsonElement("salt")]
        public byte[] salt { get; set; }
        [BsonElement("key")]
        public byte[] key { get; set; }
    }
}
