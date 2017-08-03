using System;
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
            Instance.LogInternal(message);
        }

        public static void LogError(string message)
        {
            Instance.LogErrorInternal(message);
        }

        protected virtual void Start()
        {
            text.text = "";
        }

        protected virtual void Update()
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

        protected virtual void LogInternal(string message)
        {
            Debug.Log(message);
            AddLine(message);
        }

        protected virtual void LogErrorInternal(string message)
        {
            Debug.LogError(message);
            AddLine(message, true);
        }

        protected void AddLine(string message, bool isError = false)
        {
            lock (Instance.lines)
            {
                message = DateTime.Now + " - " + message;
                if (isError)
                {
                    message = "<color=red>" + message + "</color>";
                }
                Instance.lines.Add(message + "\n");
            }
        }
    }
}
