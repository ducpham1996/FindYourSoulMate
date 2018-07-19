using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Comment_Like
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string comment_id { get; set; }

        [BsonElement("like_records")]
        public List<Like_Record> like_Records { get; set; }
    }
}
