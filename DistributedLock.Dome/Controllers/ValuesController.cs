using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedLock.ZooKeeper;
using Microsoft.AspNetCore.Mvc;

namespace Dome.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get()
        {
            ZooKeeperLock zooKeeperLock = new ZooKeeperLock();

            string result = null;
            if (await zooKeeperLock.TryLock(6000))
                result = "Success";
            else
                result = "Fail";
            await zooKeeperLock.UnLock();

            return result;
        }
    }
}
