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
        #region Server
        [SerializeField]
        Color[] colors;

        [SerializeField]
        Competitor competitorPrefab;

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

        private int colorPointer = 0;

        private List<Competitor> competitors = new List<Competitor>();

        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        [Command(requiresAuthority = false)]
        private void CmdPlayerReady(NetworkConnectionToClient sender = null)
        {
            Competitor competitor;

            competitor = Instantiate(competitorPrefab, this.transform);
            competitor.Init(NetworkMatch.matchId, Vector2.zero, colors[colorPointer], sender);
            NetworkServer.Spawn(competitor.gameObject);

            competitors.Add(competitor);

            colorPointer++;
            if (colorPointer >= colors.Length)
            {
                colorPointer = 0;
            }
        }


        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdPlayerReady();

            Menus.Instance.Activate(Menus.Instance.MatchMenu);
        }
        #endregion
    }
}
