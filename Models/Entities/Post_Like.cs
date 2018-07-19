using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Post_Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string post_id { get; set; }

        [BsonElement("like_records")]
        public List<Like_Record> like_Records { get; set; }
    }
}
