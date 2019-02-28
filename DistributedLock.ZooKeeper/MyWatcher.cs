using org.apache.zookeeper;
using System.Threading;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;

namespace DistributedLock.ZooKeeper
{
    public class MyWatcher : Watcher
    {

        public AutoResetEvent AutoResetEvent;

        public MyWatcher()
        {
            this.AutoResetEvent = new AutoResetEvent(false);
        }

        public override Task process(WatchedEvent @event)
        {
            if (@event.get_Type() == EventType.NodeDeleted)
            {
                AutoResetEvent.Set();
            }
            return null;
        }
    }
}
