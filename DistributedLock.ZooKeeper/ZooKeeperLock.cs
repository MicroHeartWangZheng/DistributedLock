using org.apache.zookeeper;
using System.Linq;
using System.Threading.Tasks;
using static org.apache.zookeeper.ZooDefs;

namespace DistributedLock.ZooKeeper
{
    public class ZooKeeperLock
    {
        private MyWatcher myWatcher;

        private string lockNode;

        private org.apache.zookeeper.ZooKeeper zooKeeper;

        public ZooKeeperLock()
        {
            myWatcher = new MyWatcher();
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
                zooKeeper = new org.apache.zookeeper.ZooKeeper("127.0.0.1", 50000, new MyWatcher());

                //创建锁节点
                if (await zooKeeper.existsAsync("/Locks") == null)
                    await zooKeeper.createAsync("/Locks", null, Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);

                //新建一个临时锁节点
                lockNode = await zooKeeper.createAsync("/Locks/Lock_", null, Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);

                //获取锁下所有节点
                var lockNodes = await zooKeeper.getChildrenAsync("/Locks");

                lockNodes.Children.Sort();

                //判断如果创建的节点就是最小节点 返回锁
                if (lockNode.Split("/").Last() == lockNodes.Children[0])
                    return true;
                else
                {
                    //当前节点的位置
                    var location = lockNodes.Children.FindIndex(n => n == lockNode.Split("/").Last());
                    //获取当前节点 前面一个节点的路径
                    var frontNodePath = lockNodes.Children[location - 1];
                    //在前面一个节点上加上Watcher ，当前面那个节点删除时，会触发Process方法
                    await zooKeeper.getDataAsync("/Locks/" + frontNodePath, myWatcher);

                    //如果时间为0 一直等待下去
                    if (millisecondsTimeout == 0)
                        myWatcher.AutoResetEvent.WaitOne();
                    else //如果时间不为0 等待指定时间后，返回结果
                    {
                        var result = myWatcher.AutoResetEvent.WaitOne(millisecondsTimeout);

                        if (result)//如果返回True，说明在指定时间内，前面的节点释放了锁(但是)
                        {
                            //获取锁下所有节点
                            lockNodes = await zooKeeper.getChildrenAsync("/Locks");
                            //判断如果创建的节点就是最小节点 返回锁
                            if (lockNode.Split("/").Last() == lockNodes.Children[0])
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;

                    }
                }
            }
            catch (KeeperException e)
            {
                await UnLock();
                throw e;
            }
            return false;
        }

        /// <summary>
        /// 释放锁
        /// </summary>
        /// <returns></returns>
        public async Task UnLock()
        {
            try
            {
                myWatcher = null;
                await zooKeeper.deleteAsync(lockNode);
            }
            catch (KeeperException e)
            {
                throw e;
            }
        }

    }
}
