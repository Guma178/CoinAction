using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.Game
{
    public class Shooter : NetworkBehaviour
    {
        #region Server

        [SerializeField]
        Transform gun;

        [SerializeField]
        float reloadTime = 2f;


        private Color color;
        private float lastShootTime;
        private NetworkConnection owner;
        private MirrorObjectsPool<Missile> missiles;

        public void Init(NetworkConnection owner, Color color, MirrorObjectsPool<Missile> missiles)
        {
            this.missiles = missiles;
            this.owner = owner;
            this.color = color;
        }

        [Command(requiresAuthority = false)]
        public void CmdShoot(NetworkConnectionToClient sender = null)
        {
            Missile missile; 

            if (sender == owner && (Time.time - lastShootTime) >= reloadTime)
            {
                lastShootTime = Time.time;
                missile = missiles.Pop(delegate(Missile mis) { mis.Init(NetworkMatch.matchId, gun.position, ThisTransform.rotation, color); });
                missile.OnHit += OnMissileHit;
            }
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

        private void OnMissileHit(Missile missile, GameObject gameObject)
        {
            missiles.Push(missile);
            Victim victim = gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                VictimHited(owner, victim.ActualHealth, victim.MaximalHealth, victim.Color);
            }
            missile.OnHit -= OnMissileHit;
        }
        #endregion

        #region Client

        public event System.Action<float, float, Color> OnVictimHited;

        [TargetRpc]
        private void VictimHited(NetworkConnection target, float healthActual, float healthMaximal, Color victimColor)
        {
            OnVictimHited?.Invoke(healthActual, healthMaximal, victimColor);
        }
        #endregion

        private System.Tuple<bool, Rigidbody2D> thisRigidbody = System.Tuple.Create<bool, Rigidbody2D>(false, null);
        public Rigidbody2D ThisRigidbody
        {
            get
            {
                if (!thisRigidbody.Item1)
                {
                    thisRigidbody = System.Tuple.Create<bool, Rigidbody2D>(true, this.GetComponent<Rigidbody2D>());
                }

                return thisRigidbody.Item2;
            }
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
