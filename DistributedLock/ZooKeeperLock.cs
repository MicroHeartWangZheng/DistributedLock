using org.apache.zookeeper;
using System.Linq;
using System.Threading.Tasks;
using static org.apache.zookeeper.ZooDefs;

namespace DistributedLock.ZooKeeper
{
    public class ZooKeeperLock
    {
        private MyWatch myWatch;

        private string lockNode;

        private org.apache.zookeeper.ZooKeeper zooKeeper;

        public ZooKeeperLock()
        {
            myWatch = new MyWatch();
        }

        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="millisecondsTimeout">等待时间</param>
        /// <returns></returns>
        public async Task<bool> TryLock(int millisecondsTimeout = 0)
        {
            try
            {
                zooKeeper = new org.apache.zookeeper.ZooKeeper("127.0.0.1", 50000, new MyWatch());

                if (await zooKeeper.existsAsync("/Locks") == null)
                    await zooKeeper.createAsync("/Locks", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

                lockNode = await zooKeeper.createAsync("/Locks/Lock_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);

                var num = int.Parse(lockNode.Split("_").Last());

                var lockNodes = await zooKeeper.getChildrenAsync("/Locks");

                lockNodes.Children.Sort();

                if (lockNode.Split("/").Last() == lockNodes.Children[0])
                    return true;
                else
                {
                    //当前节点的位置
                    var location = lockNodes.Children.FindIndex(n => n == lockNode.Split("/").Last());
                    //获取当前节点前面一个节点的路径
                    var frontNodePath = lockNodes.Children[location - 1];

                    await zooKeeper.getDataAsync("/Locks/" + frontNodePath, myWatch);
                    if (millisecondsTimeout == 0)
                        myWatch.AutoResetEvent.WaitOne();
                    else
                        return myWatch.AutoResetEvent.WaitOne(millisecondsTimeout);

                }
            }
            catch (KeeperException e)
            {
                await UnLock();
                throw e;
            }
            return false;
        }

        public async Task UnLock()
        {
            try
            {
                myWatch = null;
                await zooKeeper.deleteAsync(lockNode);
            }
            catch (KeeperException e)
            {
                throw e;
            }
        }

    }
}
