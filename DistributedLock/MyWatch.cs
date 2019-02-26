using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributedLock
{
    public class MyWatch : Watcher
    {
        public override Task process(WatchedEvent @event)
        {
            return null;
        }
    }
}
