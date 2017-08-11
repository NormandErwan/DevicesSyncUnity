using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceSyncUnity
{
    public class UnityThread : Singleton<UnityThread>
    {
        // Variables

        private Queue<IEnumerator> actions = new Queue<IEnumerator>();
        private volatile bool actionsToExecute = true;

        // Methods

        protected UnityThread()
        {
        }

        public static void Execute(IEnumerator coroutine)
        {
            lock (Instance.actions)
            {
                Instance.actions.Enqueue(coroutine);
                Instance.actionsToExecute = true;
            }
        }

        public static void Execute(Action action)
        {
            Execute(Instance.WrapActionInCoroutine(action));
        }

        protected IEnumerator WrapActionInCoroutine(Action action)
        {
            action();
            yield return null;
        }

        protected virtual void Update()
        {
            if (!actionsToExecute)
            {
                return;
            }
            
            lock (actions)
            {
                while (actions.Count > 0)
                {
                    StartCoroutine(actions.Dequeue());
                }
                actionsToExecute = false;
            }
        }
    }
}
