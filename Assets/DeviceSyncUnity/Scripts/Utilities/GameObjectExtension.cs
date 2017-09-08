using System;
using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    public static class GameObjecExtensions
    {
        public static GameObject AddChild(this GameObject gameObject)
        {
            var child = new GameObject();
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }

        public static GameObject AddChild(this GameObject gameObject, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }

        public static GameObject AddChild(this GameObject gameObject, string name, params Type[] components)
        {
            var child = new GameObject(name, components);
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }
    }
}
