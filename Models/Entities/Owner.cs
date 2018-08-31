using FindYourSoulMate.Models.Manager;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Owner
    {

        //[BsonElement("_id")]
        [JsonProperty("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [JsonProperty("user_name")]
        [BsonIgnore]
        public string user_name { get; set; }
        [JsonProperty("user_picture")]
        [BsonIgnore]
        public string user_picture { get; set; }
        [JsonProperty("email")]
        [BsonElement("email")]
        public string email { get; set; }
    }
}
