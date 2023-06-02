using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pooling
{
    /// <summary>
    /// TODO: This is really old Pooling System implementation that I didn't update. Does not support package pool increasing and max pool size.
    /// 
    /// Pooling implementation.
    ///
    /// You can create pool by object type. In this case, type name will be used as pool name.
    /// If you use same types in different context, consider to use pooling with specifying pool name.
    ///
    /// </summary>
    ///
    /// <example>
    /// <para>
    /// Step-by-step:
    /// 
    /// <para>
    /// 1. Create pool once by calling <see cref="CreatePool"/>.
    ///     You can check if pool exist by using <see cref="IsPoolForObjectsExist"/>) if you have pool with specified name,
    ///     or <see cref="IsPoolForObjectsExist{T}"/> if you are using pooling by object type.
    /// </para>
    /// 
    /// <para>
    /// 2. Use <see cref="Get{T}"/> to get game object for your needs.
    ///     It will instantiate object if there is no such objects in pool yet, by specified original object.
    /// </para>
    ///
    /// <para>
    /// 3. Return object in pool by calling <see cref="Return"/>.
    /// </para>
    ///
    /// <para>
    /// 4. Remove pool by calling <see cref="RemovePool"/> when you are sure that you don't need it anymore.
    /// </para>
    /// </para>
    /// </example>
    public static class SimplePool
    {
        /// <summary>
        /// Dictionary with pools.
        /// </summary>
        /// 
        /// <remarks> key - MonoBehavior object type </remarks>
        /// <remarks> value - queue with pool objects </remarks>
        private static readonly Dictionary<string, Queue<MonoBehaviour>> ObjectsDictionary =
            new Dictionary<string, Queue<MonoBehaviour>>();

        // Contains game objects that represents pools
        private static readonly Dictionary<string, GameObject> PoolDictionary = new Dictionary<string, GameObject>();

        private const string
            POOLS_CONTAINER_NAME =
                "NotDestroyablePools"; // Name of the game object that will be created and contain pools

        private static GameObject _globalPoolContainer;

        /// <summary>
        /// Gets object of specified type from pool. Instantiates it if necessarily.
        /// </summary>
        /// <param name="original">Original object that will be used to instantiate new object in case there is no objects in pool</param>
        /// <param name="parent">Transform where object needs to be placed after getting from pool.</param>
        /// <param name="poolName">Pool name where object is pooled.</param>
        /// <typeparam name="T">Type of the object that you need.</typeparam>
        /// 
        /// <returns>
        /// Object from pool.
        /// </returns>
        /// 
        /// <exception cref="KeyNotFoundException">
        /// If pool was not created yet.
        /// </exception>
        public static T Get<T>([NotNull] T original, [CanBeNull] Transform parent, [NotNull] string poolName)
            where T : MonoBehaviour
        {
            if (ObjectsDictionary.TryGetValue(poolName, out var monoBehaviours))
            {
                if (monoBehaviours.Count == 0)
                {
                    return Object.Instantiate(original, parent);
                }

                T toReturn = (T) monoBehaviours.Dequeue();
                toReturn.transform.SetParent(parent, true);
                toReturn.gameObject.SetActive(true);
                return toReturn;
            }

            throw new KeyNotFoundException($"There is no such pool names '{poolName}' exist.");
        }

        /// <summary>
        /// Returns object in pool
        /// </summary>
        /// <param name="obj">Object to return</param>
        /// <param name="poolName">Use to return object in custom named pool.</param>
        /// <exception cref="KeyNotFoundException">
        /// If there is no pool with such objects exist.
        /// </exception>
        public static void Return([NotNull] MonoBehaviour obj, [NotNull] string poolName)
        {
            if (ObjectsDictionary.ContainsKey(poolName))
            {
                obj.gameObject.SetActive(false);
                ObjectsDictionary[poolName].Enqueue(obj);
                obj.transform.SetParent(PoolDictionary[poolName].transform, false);
            }
            else
            {
                throw new KeyNotFoundException($"There is no pool with such objects exist: '{poolName}'");
            }
        }

        /// <summary>
        /// Creates new pool.
        /// Pool name can be custom or determined by object type name.
        /// </summary>
        /// 
        /// <param name="poolName">Use to create custom named pool.</param>
        /// <param name="suppressException">Suppress <see cref="DuplicateNameException"/> throw.</param>
        /// <exception cref="DuplicateNameException">
        /// If there is already exist pool with objects of this type.
        /// </exception>
        public static void CreatePool([NotNull] string poolName, bool suppressException = true)
        {
            if (!ObjectsDictionary.ContainsKey(poolName))
            {
                var poolObject = CreatePoolContainer("Pool of " + poolName + " ");

                ObjectsDictionary.Add(poolName, new Queue<MonoBehaviour>());
                PoolDictionary.Add(poolName, poolObject.gameObject);

                Debug.Log($"Pool named '{poolName}' created.");
            }
            else if(!suppressException)
            {
                throw new DuplicateNameException(
                    $"There is already such pool named '{poolName}' exist. No swimming today :C");
            }
        }


        /// <summary>
        /// Removes existing pool, destroying all objects in it. 
        /// </summary>
        /// <param name="poolName">Use to remove custom named pool</param>
        public static void RemovePool([NotNull] string poolName)
        {
            if (ObjectsDictionary.ContainsKey(poolName))
            {
                foreach (var objectInPool in ObjectsDictionary[poolName])
                {
                    Object.Destroy(objectInPool);
                }

                Object.Destroy(PoolDictionary[poolName].gameObject);
                ObjectsDictionary.Remove(poolName);
                PoolDictionary.Remove(poolName);

                Debug.Log($"Pool named '{poolName}' destroyed.");
                return;
            }

            Debug.LogError($"There is no such pool named '{poolName}' to destroy!");
        }

        /// <summary>
        /// Checks if pool with specified objects exist
        /// </summary>
        public static bool IsPoolForObjectsExist<T>() where T : Object
        {
            return ObjectsDictionary.ContainsKey(typeof(T).Name);
        }
        
        /// <summary>
        /// Checks if pool with specified objects exist
        /// </summary>
        /// <param name="poolName">Use to check custom named pool</param>
        public static bool IsPoolForObjectsExist([NotNull] string poolName)
        {
            return ObjectsDictionary.ContainsKey(poolName);
        }

        private static GameObject CreatePoolContainer([NotNull] string poolName)
        {
            if (ReferenceEquals(_globalPoolContainer, null))
            {
                _globalPoolContainer = new GameObject(POOLS_CONTAINER_NAME);
                _globalPoolContainer.SetActive(false);
                Object.DontDestroyOnLoad(_globalPoolContainer);
            }

            GameObject pool = new GameObject(poolName);
            pool.transform.SetParent(_globalPoolContainer.transform);
            return pool;
        }
    }
}