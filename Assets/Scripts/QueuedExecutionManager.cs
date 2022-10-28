using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum QueuedAction
    {
        None = 0,
        PlayerReceiveHmCommand,
        PlayerReceiveHmCommandPredefined,
        OnConnectionStatusChanged
    }
    public class QueuedExecutionManager
    {
        List<(QueuedAction, object)> actions = new();
        readonly object lck = new();
        bool hasItems;

        public QueuedExecutionManager()
        {

        }

        public void Enqueue(QueuedAction type, object model)
        {
            lock (lck)
            {
                actions.Add((type, model));
                hasItems = true;
            }
        }

        public List<(QueuedAction, object)> Dequeue()
        {
            // Don't care about race condition as dequeue will be called *many* times (called every update frame)
            if (!hasItems)
                return null;

            lock (lck)
            {
                var temp = actions;
                actions = new();
                hasItems = false;
                return temp;
            }
        }
    }
}
