using org.apache.zookeeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static org.apache.zookeeper.ZooDefs;

namespace DistributedLock
{
    public class DistributedLock
    {

        public async Task Test()
        {
            var zooKeeper = new ZooKeeper("118.24.96.212", 50000, new MyWatch());

            if (await zooKeeper.existsAsync("/Locks") == null)
                await zooKeeper.createAsync("/Locks", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

            var lockNode = await zooKeeper.createAsync("/Locks/Lock_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);

            var num = int.Parse(lockNode.Split("_").Last());

            var lockNodes = await zooKeeper.getChildrenAsync("/Locks");



        }
    }
}
