using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction
{
    public class MirrorObjectsPool<T> : NetworkBehaviour, IObjectsPool<T> where T : NetworkBehaviour
    {
        #region Server
        public T Pop()
        {
            T last;

            if (pool.Last != null)
            {
                last = pool.Last.Value;
                pool.RemoveLast();
                last.gameObject.SetActive(true);
                return last;
            }
            else
            {
                T newOne = Instantiate<T>(prefab, ThisTransform);
                NetworkServer.Spawn(newOne.gameObject);
                return newOne;
            }
        }

        public T Pop(System.Action<T> initializer)
        {
            T last;

            if (pool.Last != null)
            {
                last = pool.Last.Value;
                pool.RemoveLast();
                last.gameObject.SetActive(true);
                initializer(last);
                NetworkServer.Spawn(last.gameObject);
                return last;
            }
            else
            {
                T newOne = Instantiate<T>(prefab, ThisTransform);
                initializer(newOne);
                NetworkServer.Spawn(newOne.gameObject);
                return newOne;
            }
        }

        public void Push(T obj)
        {
            if (!pool.Contains(obj))
            {
                obj.gameObject.SetActive(false);
                pool.AddFirst(obj);
                NetworkServer.UnSpawn(obj.gameObject);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return pool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pool.GetEnumerator();
        }
        #endregion

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            NetworkClient.RegisterSpawnHandler(prefab.GetComponent<NetworkIdentity>().assetId, SpawnHandler, UnspawnHandler);
        }

        GameObject SpawnHandler(SpawnMessage msg) => Get(msg.position, msg.rotation);
        private GameObject Get(Vector3 position, Quaternion rotation)
        {
            T last;

            if (pool.Last != null)
            {
                last = pool.Last.Value;
                pool.RemoveLast();
            }
            else
            {
                last = Instantiate(prefab, ThisTransform);
            }

            // set position/rotation and set active
            last.transform.position = position;
            last.transform.rotation = rotation;
            last.gameObject.SetActive(true);
            return last.gameObject;
        }

        void UnspawnHandler(GameObject spawned) => Return(spawned);
        private void Return(GameObject spawned)
        {
            spawned.SetActive(false);
            pool.AddFirst(spawned.GetComponent<T>());
        }
        #endregion

        [SerializeField]
        private T prefab;

        Transform thisTransform;
        public Transform ThisTransform
        {
            get
            {
                if (thisTransform == null)
                {
                    thisTransform = this.transform;
                }

                return thisTransform;
            }
        }

        private LinkedList<T> pool = new LinkedList<T>();
    }
}
