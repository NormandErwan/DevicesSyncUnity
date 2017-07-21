using UnityEngine;

namespace DeviceSyncUnity
{
    public class ClientExample : ClientBase
    {
        protected override void Start()
        {
            base.Start();

            Proxy.Subscribe("AddMessage").Data += data =>
            {
                //var _first = data[0] as JToken;
                Debug.Log("Received: [" + data[1] as string + "] from " + data[0] as string);
            };

            Connected += (csender, ce) =>
            {
                Proxy.Invoke("Send", new object[] { "name1", "message content" }).Finished += (sender, e) =>
                {
                    Debug.Log("send done");
                };

                Proxy.Invoke("RequestReplyValueType").Finished += (sender, e) =>
                {
                    Debug.Log("RequestReplyValueType done: " + e.Result);
                };
            };
        }
    }
}
