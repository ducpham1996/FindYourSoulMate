using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class VideoImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("location")]
        public string location { get; set; }

        [BsonElement("type")]
        public string type { get; set; }

        [BsonElement("link")]
        public string link { get; set; }

        [BsonElement("like")]
        public int like { get; set; }

        [BsonElement("love")]
        public int love { get; set; }

        [BsonElement("angry")]
        public int angry { get; set; }

        [BsonElement("scare")]
        public int scare { get; set; }

        [BsonElement("haha")]
        public int haha { get; set; }

        [BsonElement("sad")]
        public int sad { get; set; }

        [BsonElement("amaze")]
        public int amaze { get; set; }

        [BsonElement("suprise")]
        public int suprise { get; set; }

        [BsonElement("comments")]
        public int comments { get; set; }

        [BsonElement("sub_comments")]
        public int sub_comments { get; set; }
    }
}
