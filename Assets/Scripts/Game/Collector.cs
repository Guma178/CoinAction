using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.Game
{
    public class Collector : NetworkBehaviour
    {
        public event System.Action<short> CollectedValuesChanged;

        #region Server
        [SyncVar(hook = nameof(ValuesChange))]
        private short collectedValues;

        public short CollectedValues => collectedValues;


        public void OnTriggerEnter2D(Collider2D collision)
        {
            Collectable collectable = collision.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectedValues += collectable.Collect();
                CollectedValuesChanged(collectedValues);
            }
        }
        #endregion

        #region Client
        private void ValuesChange(short collectedValuesOld, short collectedValuesNew)
        {
            CollectedValuesChanged?.Invoke(collectedValuesNew);
        }
        #endregion
    }
}
