using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace CoinAction.Game
{
    [RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
    public class Missile : NetworkBehaviour
    {
        #region Server
        public event System.Action<Missile, GameObject> OnHit;

        [SerializeField]
        float speed;

        [SerializeField]
        float power;

        [SyncVar(hook = nameof(FetchColor))]
        public Color color;

        public void Init(Guid matchId, Vector2 position, Quaternion rotation, Color color)
        {
            NetworkMatch.matchId = matchId;
            ThisTransform.position = position;
            ThisTransform.rotation = rotation;
            this.color = color;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            Victim victim;

            OnHit?.Invoke(this, collision.gameObject);
            victim = collision.gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                victim.Damage(power);
            }
        }

        private void ServerFixedUpdate()
        {
            if (isServer)
            {
                ThisRigidbody.velocity = ThisTransform.up * speed;
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

        #endregion

        #region Client

        private void FetchColor(Color oldVal, Color newVal)
        {
            Sprite.color = newVal;
        }

        #endregion

        private System.Tuple<bool, SpriteRenderer> sprite = System.Tuple.Create<bool, SpriteRenderer>(false, null);
        private SpriteRenderer Sprite
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

        private System.Tuple<bool, Rigidbody2D> thisRigidbody = System.Tuple.Create<bool, Rigidbody2D>(false, null);
        private Rigidbody2D ThisRigidbody
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

        private void FixedUpdate()
        {
            ServerFixedUpdate();
        }
    }
}
