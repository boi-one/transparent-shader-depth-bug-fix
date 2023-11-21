using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Gets a component attached to the given GameObject.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>Previously or newly attached component.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.HasComponent<T>())
            {
                return gameObject.GetComponent<T>();
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }

        /// <summary>
        /// Gets a component attached to the given GameObject parent.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>Previously or newly attached component.</returns>
        public static T GetOrAddComponentToParent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.transform.parent.HasComponent<T>())
            {
                return gameObject.transform.parent.GetComponent<T>();
            }

            return gameObject.transform.parent.AddComponent<T>();
        }

        /// <summary>
        /// checks whether a GameObject has the given component or not.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// checks if the given GameObject parent has the given type 
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponentInParent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponentInParent<T>() != null;
        }

        public static void RemoveComponent<T>(this GameObject obj, bool immediate = false) where T : Component
        {
            if (!(obj.HasComponent<T>())) return;
            if (immediate) Object.DestroyImmediate(obj.GetComponent<T>(), true);
            else Object.Destroy(obj.GetComponent<T>());
        }
        
        public static int GetDirection(this GameObject obj)
        {
            return Math.Sign(obj.transform.localScale.x);
        }

        public static void Flip(this GameObject obj, Vector3 direction, float treshold = 0f)
        {
            if (Mathf.Abs(direction.x) < treshold) return;
            
            var flipDirection = Math.Sign(direction.x);
            var currentDirection = Math.Sign(obj.transform.localScale.x);
            if (flipDirection == 0 || flipDirection == currentDirection) return;

            var localScale = obj.transform.localScale;
            localScale.x = Math.Abs(localScale.x) * flipDirection;
            obj.transform.localScale = localScale;
        }
    }
}