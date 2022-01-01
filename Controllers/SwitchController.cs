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
        private readonly ILogger<SwitchController> _logger;
        private readonly SwitchModel switchModel;

        public SwitchController(ILogger<SwitchController> logger, SwitchModel switchModel)
        {
            _logger = logger;
            this.switchModel = switchModel;
        }

        [HttpGet("{location}")]
        public ActionResult<int> Get(string location)
        {
            return switchModel.Get(location);
        }

        [HttpGet("{location}/{value}")]
        public ActionResult<int> Get(string location, int value)
        {
            return switchModel.Get(location, value);
        }

        [HttpGet("state")]
        public async Task<SwitchState> GetState()
        {
            bool val = await Task.Run(() => switchModel.GetValue());
            return new SwitchState { CurrentState = val, NextChangeAt = switchModel.NextStateChangeAt };
        }

        [HttpGet("on")]
        public void SwitchOn()
        {
            switchModel.Switch(true);
        }

        [HttpGet("off")]
        public void SwitchOff()
        {
            switchModel.Switch(false);
        }

        [HttpPost("start")]
        public void StartCycle(PatternArg arg)
        {
            int[] pattern = arg.PatternBlob.Trim().Split(',').Select(s=>s.Trim()).Select(int.Parse).ToArray();
            switchModel.StartCycle(pattern);
        }
    }
}