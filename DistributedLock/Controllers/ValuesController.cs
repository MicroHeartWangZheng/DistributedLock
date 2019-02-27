using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<String> Get()
        {
            DistributedLock distributedLock = new DistributedLock();
            await distributedLock.Test();
            return "Ok";
        }
    }
}
