using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.Lobby
{
    [RequireComponent(typeof(NetworkMatch))]
    public class User : NetworkBehaviour
    {
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
    }
}
