// Based on https://stackoverflow.com/a/41333540

using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceSyncUnity
{
    public class UnityThread : Singleton<UnityThread>
    {
        // Variables

        ///<summary>Holds actions received from another Thread. Will be coped to actionCopiedQueueUpdateFunc then executed from there</summary>
        private static List<Action> actionQueuesUpdateFunc = new List<Action>();

        ///<summary>Holds Actions copied from actionQueuesUpdateFunc to be executed</summary>
        List<Action> actionCopiedQueueUpdateFunc = new List<Action>();

        ///<summary>Used to know if whe have new Action function to execute. This prevents the use of the lock keyword every frame</summary>
        private volatile static bool noActionQueueToExecuteUpdateFunc = true;

        // Methods

        public static void ExecuteCoroutine(IEnumerator action)
        {
            ExecuteInUpdate(() => Instance.StartCoroutine(action));
        }

        public static void ExecuteInUpdate(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            lock (actionQueuesUpdateFunc)
            {
                actionQueuesUpdateFunc.Add(action);
                noActionQueueToExecuteUpdateFunc = false;
            }
        }

        protected void Update()
        {
            if (noActionQueueToExecuteUpdateFunc)
            {
                return;
            }

            actionCopiedQueueUpdateFunc.Clear();
            lock (actionQueuesUpdateFunc)
            {
                actionCopiedQueueUpdateFunc.AddRange(actionQueuesUpdateFunc);
                actionQueuesUpdateFunc.Clear();
                noActionQueueToExecuteUpdateFunc = true;
            }

            for (int i = 0; i < actionCopiedQueueUpdateFunc.Count; i++)
            {
                actionCopiedQueueUpdateFunc[i].Invoke();
            }
        }
    }
}
