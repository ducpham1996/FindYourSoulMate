using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMateAngular.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMateAngular.Controllers
{
    //[Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        // GET: api/Test
        [HttpGet(Name = "GetTest")]
        public IActionResult GetTest()
        {
            return Json(new { data =  "Hello there"});
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(string id)
        {
            return "value = " + id;
        }

        // POST: api/Test
      
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Post([FromForm] List<IFormFile> files)
        {
            foreach (IFormFile file in files)
            {
                Debug.WriteLine(file.FileName);
            }
            //string imageName = "";
            //imageName = new String(Path.GetFileNameWithoutExtension(postedFiles.FileName).Take(10).ToArray()).Replace(" ", "-");
            //imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFiles.FileName);
            //var filePath = HttpContext.
            return Ok("Login fail") ;
        }
        
        // PUT: api/Test/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
