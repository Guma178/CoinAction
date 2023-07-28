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

        public event System.Action Finished;
        public event System.Action<NetworkConnectionToClient> UserLeft;

        private bool isFinished;
        private int colorPointer = 0;
        private int randSeed = DateTime.Now.Millisecond;
        private List<Competitor> competitors = new List<Competitor>();


        public override void OnStartServer()
        {
            base.OnStartServer();

            StartCoroutine(CollectablesSpawning());

            isFinished = false;

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
                MissilesPool = misilesObjectsPool,
                Parrent = this.netIdentity
            };

            colorPointer++;
            if (colorPointer >= colors.Length)
            {
                colorPointer = 0;
            }

            Vector3 pos = GetRandomPositionOnField(); 

            competitor = Instantiate(competitorPrefab, pos, Quaternion.identity, this.transform);
            competitor.Init(data);
            competitor.Victim.Died += delegate ()
            {
                if (competitor.IsActive)
                {
                    competitor.IsActive = false;
                    NetworkServer.UnSpawn(competitor.gameObject);
                    CheackWinCondition();
                }
            };
            NetworkServer.Spawn(competitor.gameObject);

            competitors.Add(competitor);

            SendCameraPosition(sender, this.transform.position);
        }

        private void MatchLeave(NetworkConnectionToClient sender = null)
        {
            Competitor competitor;

            competitor = competitors.FirstOrDefault(cmp => cmp.Owner == sender);

            if (competitor != null)
            {
                competitor.IsInGame = false;
                if (sender.isReady)
                {
                    UserLeft?.Invoke(sender);
                }
            }

            if (competitors.Count(c => c.IsInGame) <= 0)
            {
                NetworkServer.UnSpawn(this.gameObject);
            }
        }

        [Command(requiresAuthority = false)]
        private void LeaveMatch(NetworkConnectionToClient sender = null)
        {
            MatchLeave(sender);
        }

        private void Kick(NetworkConnectionToClient conn)
        {
            Competitor competitor;

            competitor = competitors.FirstOrDefault(cmp => cmp.Owner == conn && cmp.IsActive);
            if (competitor != null)
            {
                competitor.IsActive = false;
                NetworkServer.UnSpawn(competitor.gameObject);
                CheackWinCondition();
                competitor.IsInGame = false;
            }
            MatchLeave(conn);
        }

        private void CheackWinCondition()
        {
            Rezults rez;
            IEnumerable<Competitor> alive = competitors.Where(c => c.IsActive);
            Competitor winner;

            if (!isFinished)
            {
                if (alive.Count() <= 1)
                {
                    isFinished = true;
                    Finished?.Invoke();
                    winner = alive.FirstOrDefault();
                    if (winner != null)
                    {
                        foreach (Competitor c in competitors)
                        {
                            if (c.Owner.isReady)
                            {
                                rez = new Rezults
                                {
                                    IsWon = c.IsActive,
                                    WinnerCoinsAmount = winner.Collector.CollectedValues,
                                    WinnerColor = winner.Color,
                                    PlayerColor = c.Color,
                                    PlayerCoinsAmount = c.Collector.CollectedValues
                                };
                                SendRezults(c.Owner, rez);
                            }
                            if (c.IsActive)
                            {
                                NetworkServer.UnSpawn(c.gameObject);
                            }
                        }
                    }
                }

            }
        }

        private IEnumerator CollectablesSpawning()
        {
            Collectable collectable;

            while (true)
            {
                collectable = collectablesObjectsPool.Pop(delegate (Collectable col) { col.Init(NetworkMatch.matchId, GetRandomPositionOnField()); });
                collectable.Collected += OnCollectableCollected;

                yield return new WaitForSeconds(collectableSpawnPeriod);
            }
        }

        private void OnCollectableCollected(Collectable collectable)
        {
            collectablesObjectsPool.Push(collectable);
            collectable.Collected -= OnCollectableCollected;
        }

        private Vector2 GetRandomPositionOnField()
        {
            Vector2 position, rez;
            UnityEngine.Random.InitState(randSeed);
            randSeed++;
            position = new Vector2(UnityEngine.Random.Range(spawnArea.min.x, spawnArea.max.x), UnityEngine.Random.Range(spawnArea.min.y, spawnArea.max.y));
            rez = (Vector2)ThisTransform.position + position;
            return rez;
        }
        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdPlayerReady();

            Menus.Instance.Activate(Menus.Instance.MatchMenu);
            Menus.Instance.MatchMenu.RezultsPopUp.OkButttonClick += delegate () 
            { 
                LeaveMatch();
            };
        }

        [TargetRpc]
        private void SendRezults(NetworkConnection target, Rezults rezults)
        {
            Menus.Instance.MatchMenu.RezultsPopUp.LoadRezults(rezults);
        }

        [TargetRpc]
        private void SendCameraPosition(NetworkConnection target, Vector3 position)
        {
            Transform cameraTransform = Camera.main.transform;
            Vector3 height = Vector3.ProjectOnPlane(cameraTransform.position, Vector3.up);
            cameraTransform.position = position + height;
        }
        #endregion

        [System.Serializable]
        public class Rezults
        {
            public bool IsWon;
            public Color WinnerColor;
            public short WinnerCoinsAmount;
            public Color PlayerColor;
            public short PlayerCoinsAmount;
        }

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
