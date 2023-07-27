using CoinAction.Lobby;
using CoinAction.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        MissilesObjectsPool misilesObjectsPool;

        [SerializeField]
        CollectablesObjectsPool collectablesObjectsPool;

        [SerializeField]
        Competitor competitorPrefab;

        [SerializeField]
        float collectableSpawnPeriod;

        [SerializeField]
        Bounds spawnArea;

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
        private int randSeed = DateTime.Now.Millisecond;
        private List<Competitor> competitors = new List<Competitor>();

        public override void OnStartServer()
        {
            base.OnStartServer();

            CoinAction.NetworkManager.Instance.ClientDisconnected += Kick;
        }

        [Command(requiresAuthority = false)]
        private void CmdPlayerReady(NetworkConnectionToClient sender = null)
        {
            Competitor competitor;
            Competitor.Data data;

            data = new Competitor.Data
            {
                MatchId = NetworkMatch.matchId,
                CompetitorColor = colors[colorPointer],
                Owner = sender,
                MissilesPool = misilesObjectsPool
            };

            colorPointer++;
            if (colorPointer >= colors.Length)
            {
                colorPointer = 0;
            }

            competitor = Instantiate(competitorPrefab, GetRandomPositionOnField(), Quaternion.identity, this.transform);
            competitor.Init(data);
            NetworkServer.Spawn(competitor.gameObject);

            competitor.Victim.Died += delegate ()
            {
                competitor.IsActive = false;
                NetworkServer.UnSpawn(competitor.gameObject);
            };

            competitors.Add(competitor);
        }

        private void Kick(NetworkConnectionToClient conn)
        {
            Competitor competitor;

            competitor = competitors.FirstOrDefault();
            if (competitor != null)
            {
                competitor.IsActive = false;
                NetworkServer.UnSpawn(competitor.gameObject);
            }
        }

        private IEnumerator CollectablesSpawning()
        {
            Collectable collectable;

            while (true)
            {
                yield return new WaitForSeconds(collectableSpawnPeriod);

                collectable = collectablesObjectsPool.Pop(delegate (Collectable col) { col.Init(NetworkMatch.matchId, GetRandomPositionOnField()); });
                collectable.Collected += OnCollectableCollected;
            }
        }

        private void OnCollectableCollected(Collectable collectable)
        {
            collectablesObjectsPool.Push(collectable);
            collectable.Collected -= OnCollectableCollected;
        }

        private Vector2 GetRandomPositionOnField()
        {
            Vector2 position;
            UnityEngine.Random.InitState(randSeed);
            randSeed++;
            position = new Vector2(UnityEngine.Random.Range(spawnArea.min.x, spawnArea.max.x), UnityEngine.Random.Range(spawnArea.min.y, spawnArea.max.y));
            return position;
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
