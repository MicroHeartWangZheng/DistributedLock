using org.apache.zookeeper;
using System.Threading;
using System.Threading.Tasks;
using static org.apache.zookeeper.Watcher.Event;

namespace DistributedLock.ZooKeeper
{
    public class MyWatch : Watcher
    {

        public AutoResetEvent AutoResetEvent;

        public MyWatch()
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
