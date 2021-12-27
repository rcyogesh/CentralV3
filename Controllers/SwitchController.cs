using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace CentralV3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SwitchController : ControllerBase
    {
        static Dictionary<string, int> locations = new Dictionary<string, int>();

        private readonly ILogger<SwitchController> _logger;
        private readonly IHttpClientFactory clientFactory;

        public static DateTime NextStateChangeAt { get; set; }

        public SwitchController(ILogger<SwitchController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            this.clientFactory = clientFactory;
        }

        [HttpGet("{location}")]
        public ActionResult<int> Get(string location)
        {
            int result;
            locations.TryGetValue(location, out result);
            return result;
        }

        [HttpGet("{location}/{value}")]
        public ActionResult<int> Get(string location, int value)
        {
            locations[location] = value;
            return Get(location);
        }

        [HttpGet("statechange")]
        public DateTime GetNextStateChangeTime()
        {
            return NextStateChangeAt;
        }

        [HttpGet("state")]
        public SwitchState GetState()
        {
            bool state = GetValue();
            return new SwitchState { CurrentState = state, NextChangeAt = NextStateChangeAt };
        }

        [HttpGet("value")]
        public bool GetValue()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.228/value");
            var client = clientFactory.CreateClient();
            var response = client.Send(request);
            //return response.Content. == "1";
            //var response = WebRequest.Create("http://192.168.1.228/value").GetResponse();
            string s;
            using (var stream = response.Content.ReadAsStream())
            {
                StreamReader reader = new StreamReader(stream);
                s = reader.ReadToEnd();
            }
            return s == "1";
        }

        [HttpGet("on")]
        public ActionResult<bool> SwitchOn()
        {
            SwitchController._SwitchOn();
            return false;
        }

        public static void _SwitchOn()
        {
            WebRequest.Create("http://192.168.1.228/on").GetResponse();
        }

        [HttpGet("off")]
        public void SwitchOff()
        {
            SwitchController._SwitchOff();
        }

        public static void _SwitchOff()
        {
            WebRequest.Create("http://192.168.1.228/off").GetResponse();
        }

    }
}