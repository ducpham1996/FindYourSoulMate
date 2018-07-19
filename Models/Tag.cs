using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class Tag
    {
        [BsonElement("title")]
        public string Title { get; set; }
    }
}
