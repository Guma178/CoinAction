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

        #region 
        private short collectedValues;

        public short CollectedValues => collectedValues;

        private NetworkConnection owner;

        public void Init(NetworkConnection owner)
        {
            this.owner = owner;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            Collectable collectable = collision.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectedValues += collectable.Collect();
                ValuesChange(owner, collectedValues);
                CollectedValuesChanged?.Invoke(collectedValues);
            }
        }
        #endregion

        #region Client

        [TargetRpc]
        private void ValuesChange(NetworkConnection target, short collectedValuesNew)
        {
            CollectedValuesChanged?.Invoke(collectedValuesNew);
        }
        #endregion
    }
}
