using CoinAction.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace CoinAction.Game
{
    [RequireComponent(typeof(NetworkTransform), typeof(Rigidbody2D))]
    public class Walker : NetworkBehaviour
    {
        Transform thisTransform;
        public Transform ThisTransform
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

        private System.Tuple<bool, NetworkTransform> networkTransform = System.Tuple.Create<bool, NetworkTransform>(false, null);
        public NetworkTransform NetworkTransform
        {
            get
            {
                if (!networkTransform.Item1)
                {
                    networkTransform = System.Tuple.Create<bool, NetworkTransform>(true, this.GetComponent<NetworkTransform>());
                }

                return networkTransform.Item2;
            }
        }

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

        #region Server
        [SerializeField]
        private float moveSpeed = 1.5f;

        [SerializeField]
        private float rotationSpeed = 15f;

        private Vector2 direction = Vector2.zero;

        private NetworkConnection owner;

        public void Init(NetworkConnection owner)
        {
            this.owner = owner;
        }

        [Command(requiresAuthority = false)]
        public void CmdMove(Vector2 direction, NetworkConnectionToClient sender = null)
        {
            if (sender == owner)
            {
                this.direction= direction;
            }
        }

        private void ServeFixedUpdate()
        {
            if(isServer)
            {
                if (direction != Vector2.zero)
                {
                    ThisRigidbody.MoveRotation(ThisRigidbody.rotation + Vector2.SignedAngle(transform.up, direction) * Time.fixedDeltaTime * rotationSpeed);
                    ThisRigidbody.MovePosition(ThisRigidbody.position + (Vector2)ThisTransform.up * direction.magnitude * moveSpeed * Time.fixedDeltaTime);
                }
            }
        }

        #endregion

        #region Client

        #endregion



        private void FixedUpdate()
        {
            ServeFixedUpdate();
        }
    }
}
