using System;
using System.Collections.Generic;

namespace VK.WindowsPhone.SDK
{
    public class VKObject
    {
        private static readonly Dictionary<long, VKObject> RegisteredObjects = new Dictionary<long, VKObject>();
        private long _registeredObjectId;

        /// <summary>
        /// Returns object saved in local cache.
        /// </summary>
        /// <param name="registeredObjectId">Registered object id</param>
        /// <returns>Object saved via RegisterObject() method</returns>
        public static VKObject GetRegisteredObject(long registeredObjectId)
        {
            VKObject obj = null;
            RegisteredObjects.TryGetValue(registeredObjectId, out obj);
            return obj;
        }

        internal Random Rand = new Random();

        /// <summary>
        /// Saves object in local cache for future use. Always call UnregisterObject() after use.
        /// </summary>
        /// <returns></returns>
        public long RegisterObject()
        {
            while (true)
            {
                var buffer = new byte[sizeof(Int64)];
                Rand.NextBytes(buffer);
                long nextRand = BitConverter.ToInt64(buffer, 0);

                if (RegisteredObjects.ContainsKey(nextRand) || nextRand == 0)
                    continue;

                RegisteredObjects.Add(nextRand, this);
                _registeredObjectId = nextRand;

                return nextRand;
            }
        }


        public void UnregisterObject()
        {
            RegisteredObjects.Remove(_registeredObjectId);
            _registeredObjectId = 0;
        }
    }
}
