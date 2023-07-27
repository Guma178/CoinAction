using CoinAction.UI;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.Game
{
    [RequireComponent(typeof(SpriteRenderer), typeof(NetworkMatch), typeof(Walker))]
    public abstract class Competitor : NetworkBehaviour
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
        Color color;

        public void Init(Guid matchId, Vector2 position, Color color, NetworkConnection owner)
        {
            NetworkMatch.matchId = matchId;
            this.color = color;
            Walker.Init(position, owner);
        }

        [Command(requiresAuthority = false)]
        private void CmdSynchronization(NetworkConnectionToClient sender = null)
        {
            Synchronize(sender, color);
        }

        #endregion

        #region Client
        public override void OnStartClient()
        {
            base.OnStartClient();

            CmdSynchronization();
        }

        [TargetRpc]
        private void Synchronize(NetworkConnection target, Color color)
        {
            Sprite.color = color;
        }
        #endregion
    }
}
