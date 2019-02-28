using org.apache.zookeeper;
using System.Linq;
using System.Threading.Tasks;
using static org.apache.zookeeper.ZooDefs;

namespace DistributedLock.ZooKeeper
{
    public class DistributedLock
    {
        private string frontLockNode;

        private MyWatch myWatch;

        public DistributedLock()
        {
            myWatch = new MyWatch();
        }

        public async Task<bool> TryLock()
        {
            try
            {
                var zooKeeper = new org.apache.zookeeper.ZooKeeper("127.0.0.1", 50000, new MyWatch());

                if (await zooKeeper.existsAsync("/Locks") == null)
                    await zooKeeper.createAsync("/Locks", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

                var lockNode = await zooKeeper.createAsync("/Locks/Lock_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);

                var num = int.Parse(lockNode.Split("_").Last());

                var lockNodes = await zooKeeper.getChildrenAsync("/Locks");

                lockNodes.Children.Sort();

                if (lockNode.Split("/").Last() == lockNodes.Children[0])
                    return true;
                else
                {
                    await zooKeeper.getDataAsync("/Locks/Lock_" + (num - 1).ToString().PadLeft(10, '0'), myWatch);
                    myWatch.AutoResetEvent.WaitOne();
                    return true;
                }
            }
            catch (KeeperException e)
            {
                throw e;
            }
        }

    }
}
