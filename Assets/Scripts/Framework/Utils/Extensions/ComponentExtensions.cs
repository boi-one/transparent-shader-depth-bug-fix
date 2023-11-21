using UnityEngine;

namespace Extensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// adds the component to the given component's game object. 
        /// </summary>
        /// <param name="component">Component.</param>
        /// <returns>attached component</returns>
        public static T AddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddComponent<T>();
        }
        
        /// <summary>
        /// Gets a component attached to the given component's GameObject.
        /// If one isn't found, a new one is attached and returned.
        /// </summary>
        /// <param name="component">component.</param>
        /// <returns>the component we wanted to add or get</returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component.HasComponent<T>()) { return component.GetComponent<T>(); }
            else {return component.AddComponent<T>();}
        }

        /// <summary>
        /// checks whether a component's gameobject has the given component or not.
        /// </summary>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }
    }
}
