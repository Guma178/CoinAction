using CoinAction.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CoinAction.Game.Match;

namespace CoinAction.Game
{
    [RequireComponent(typeof(SpriteRenderer), typeof(NetworkMatch), typeof(Walker))]
    public abstract class Competitor : NetworkBehaviour
    {
        private System.Tuple<bool, SpriteRenderer> sprite = System.Tuple.Create<bool, SpriteRenderer>(false, null);
        public SpriteRenderer Sprite
        {
            get
            {
                if (!sprite.Item1)
                {
                    sprite = System.Tuple.Create<bool, SpriteRenderer>(true, this.GetComponent<SpriteRenderer>());
                }

                return sprite.Item2;
            }
        }

        private System.Tuple<bool, Victim> victim = System.Tuple.Create<bool, Victim>(false, null);
        public Victim Victim
        {
            get
            {
                if (!victim.Item1)
                {
                    victim = System.Tuple.Create<bool, Victim>(true, this.GetComponent<Victim>());
                }

                return victim.Item2;
            }
        }

        private System.Tuple<bool, Shooter> shooter = System.Tuple.Create<bool, Shooter>(false, null);
        public Shooter Shooter
        {
            get
            {
                if (!shooter.Item1)
                {
                    shooter = System.Tuple.Create<bool, Shooter>(true, this.GetComponent<Shooter>());
                }

                return shooter.Item2;
            }
        }

        private System.Tuple<bool, Collector> collector = System.Tuple.Create<bool, Collector>(false, null);
        public Collector Collector
        {
            get
            {
                if (!collector.Item1)
                {
                    collector = System.Tuple.Create<bool, Collector>(true, this.GetComponent<Collector>());
                }

                return collector.Item2;
            }
        }

        private System.Tuple<bool, Walker> walker = System.Tuple.Create<bool, Walker>(false, null);
        public Walker Walker
        {
            get
            {
                if (!walker.Item1)
                {
                    walker = System.Tuple.Create<bool, Walker>(true, this.GetComponent<Walker>());
                }

                return walker.Item2;
            }
        }

        #region Server
        private Color color;
        private NetworkConnection owner;
        private NetworkIdentity pattent;

        public NetworkConnection Owner => owner;
        public Color Color => color;

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

        public bool IsInGame { get; set; }
        public bool IsActive { get; set; }

        public void Init(Data data)
        {
            NetworkMatch.matchId = data.MatchId;
            this.color = data.CompetitorColor;
            this.owner = data.Owner;
            this.pattent = data.Parrent;

            IsInGame = true;
            IsActive = true;
            Walker.Init(data.Owner);
            Shooter.Init(data.Owner, data.CompetitorColor, data.MissilesPool);
            Victim.Init(data.Owner, data.CompetitorColor);
            Collector.Init(data.Owner);
        }

        [Command(requiresAuthority = false)]
        private void CmdStateSynchronization(NetworkConnectionToClient sender = null)
        {
            State state = new State
            {
                IsOwner = sender == owner,
                Color = color,
                Parrent = pattent,
                Position = transform.position,
            };

            StateSynchronize(sender, state);
        }

        public class Data
        {
            public Guid MatchId;
            public Color CompetitorColor;
            public MissilesObjectsPool MissilesPool;
            public NetworkConnection Owner;
            public NetworkIdentity Parrent;
        }
        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdStateSynchronization();
        }

        [TargetRpc]
        private void StateSynchronize(NetworkConnection target, State state)
        {
            Fetch(state);
        }

        protected virtual void Fetch(State state)
        {
            Sprite.color = state.Color;
            ThisTransform.position = state.Position;
            ThisTransform.parent = state.Parrent.transform;
        }
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

        [System.Serializable]
        public class State
        {
            public bool IsOwner;
            public Color Color;
            public NetworkIdentity Parrent;
            public Vector3 Position;
        }
    }
}
