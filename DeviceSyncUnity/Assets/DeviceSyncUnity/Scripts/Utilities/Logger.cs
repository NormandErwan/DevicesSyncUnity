using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceSyncUnity
{
    public class Logger : Singleton<Logger>
    {
        public Text text;

        private List<string> lines = new List<string>();

        public static void Log(string message)
        {
            Debug.Log(message);

            lock (Instance.lines)
            {
                Instance.lines.Add(message + "\n");
            }
        }

        private void Start()
        {
            text.text = "";
        }

        private void Update()
        {
            lock (Instance.lines)
            {
                foreach (var line in lines)
                {
                    Instance.text.text += line;
                }
                lines.Clear();
            }
        }
    }
}
