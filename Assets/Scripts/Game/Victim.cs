using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CoinAction.Game
{
    public class Victim : NetworkBehaviour
    {
        public event System.Action Died;

        #region Server
        [SerializeField]
        [SyncVar]
        float maximalHealth;

        [SyncVar(hook = nameof(HealthChange))]
        private float actualHealth;
        private Color color;
        private NetworkConnection owner;

        public float MaximalHealth => maximalHealth;
        public float ActualHealth => actualHealth;
        public Color Color => color;

        public void Init(NetworkConnection owner, Color color)
        {
            this.owner = owner;
            this.color = color;
        }

        public void Damage(float damage)
        {
            actualHealth -= damage;
            if (actualHealth <= 0)
            {
                Died?.Invoke();
                Dead(owner);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            actualHealth = maximalHealth;
        }

        #endregion

        #region Client
        public event System.Action<float, float> HealthChanged;

        private void HealthChange(float actualHealthOld, float actualHealthNew)
        {
            HealthChanged?.Invoke(actualHealthNew, maximalHealth);
        }

        [TargetRpc]
        private void Dead(NetworkConnection target)
        {
            HealthChange(0, maximalHealth);
            Died?.Invoke();
        }
        #endregion
    }
}
