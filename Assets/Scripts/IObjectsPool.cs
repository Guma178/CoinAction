using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction
{
    public interface IObjectsPool<T> : IEnumerable<T> where T : Component
    {
        T Pop();
        void Push(T obj);
    }
}
