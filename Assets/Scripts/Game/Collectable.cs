using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoinAction.Game
{
    public class Collectable : NetworkBehaviour
    {
        #region Server
        [SerializeField]
        short value;

        public event System.Action<Collectable> Collected;

        public void Init(Guid matchId, Vector2 position)
        {
            NetworkMatch.matchId = matchId;
            ThisTransform.position = position;
        }

        public short Collect()
        {
            Collected?.Invoke(this);
            return value;
        }

        private System.Tuple<bool, NetworkMatch> networkMatch = System.Tuple.Create<bool, NetworkMatch>(false, null);
        public NetworkMatch NetworkMatch
        {
            get
            {
                if (!networkMatch.Item1)
                {
                    networkMatch = System.Tuple.Create<bool, NetworkMatch>(true, this.GetComponent<NetworkMatch>());
                }

                return networkMatch.Item2;
            }
        }
        #endregion

        #region Client
        #endregion

        Transform thisTransform;
        private Transform ThisTransform
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
    }
}
