using CoinAction.Lobby;
using CoinAction.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoinAction.Game
{
    [RequireComponent(typeof(NetworkMatch))]
    public class Match : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Server

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

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        [Command(requiresAuthority = false)]
        private void CmdPlayerReady(NetworkConnectionToClient sender = null)
        {
            Joined(sender);
        }


        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdPlayerReady();
        }

        [TargetRpc]
        private void Joined(NetworkConnection target)
        {
            Menus.Instance.Activate(Menus.Instance.MatchMenu);
        }
        #endregion
    }
}
