using System;
using UnityEngine;

namespace DeviceSyncUnity.Utilities
{
    /// <summary>
    /// Extensions <see cref="GameObject"/> to create children.
    /// </summary>
    public static class GameObjecExtensions
    {
        /// <summary>
        /// Creates a new child with a reset transform.
        /// </summary>
        public static GameObject AddChild(this GameObject gameObject)
        {
            var child = new GameObject();
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }

        /// <summary>
        /// Creates a new child with a reset transform.
        /// </summary>
        /// <param name="name">The name of the child.</param>
        public static GameObject AddChild(this GameObject gameObject, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }

        /// <summary>
        /// Creates a new child with a reset transform.
        /// </summary>
        /// <param name="name">The name of the child.</param>
        /// <param name="components">A list of Components to add to the GameObject on creation</param>
        public static GameObject AddChild(this GameObject gameObject, string name, params Type[] components)
        {
            var child = new GameObject(name, components);
            child.transform.SetParent(gameObject.transform);
            child.transform.Reset();
            return child;
        }
    }
}
