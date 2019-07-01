using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using System.Net.Http;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Hi Team, For testing I have used Postman with POST link https://localhost:5001/api/values/costs by putting the raw message in the body as a JSON. Also in Headers Content-Type is application/json. SSL certification verification is Off " };
        }

        // GET api/values/costs
        [HttpPost("costs")]
        public ActionResult<string> Costs( [FromBody] dynamic data)
        {
            string message = data.message;
            XMLCostAnalyzer analyzer = new XMLCostAnalyzer();
            string result = analyzer.ExtractCostAndCenter(message);
            if (result == "")
            {
                return analyzer.GetCosts();
            }

            return result;
        }

    }
}
