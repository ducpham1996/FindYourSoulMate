using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class Comment
    {
        [BsonElement("user")]
        public string user { get; set; }
        [BsonElement("message")]
        public string message { get; set; }
        
        [BsonElement("dateCreated")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime date_created { get; set; }
        [BsonElement("like")]
        public double like { get; set; }
    
    }
}
