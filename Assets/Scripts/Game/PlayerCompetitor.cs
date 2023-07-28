using CoinAction.Game;
using CoinAction.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCompetitor : Competitor
{
    #region Server
    #endregion


    #region Client

    Vector2 moveDirection;
    Coroutine movementSendingProcess;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    protected override void Fetch(State state)
    {
        base.Fetch(state);

        if (state.IsOwner)
        {
            Menus.Instance.MatchMenu.Colorize(state.Color);

            Menus.Instance.MatchMenu.MoveStick.ValueChanged += Movement;
            Menus.Instance.MatchMenu.ShootClick += Shoot;
            Shooter.OnVictimHited += OnVictimHit;
            Victim.HealthChanged += OnHealthChange;
            Collector.CollectedValuesChanged += ValuesCollected;

            if (movementSendingProcess != null)
            {
                StopCoroutine(movementSendingProcess);
            }
            movementSendingProcess = StartCoroutine(MovementSending());
        }
    }

    private void Movement(Vector2 dir)
    {
        moveDirection = dir;
    }

    private void Shoot()
    {
        Shooter.CmdShoot();
    }

    private void OnVictimHit(float actualHealth, float maximalHealtj, Color color)
    {
        Menus.Instance.MatchMenu.DisplayEnemyHealth(actualHealth / maximalHealtj, color);
    }

    private void OnHealthChange(float actual, float maximal)
    {
        Menus.Instance.MatchMenu.PlayerHealthSlider.value = actual / maximal;
    }

    private void ValuesCollected(short val)
    { 
        Menus.Instance.MatchMenu.CollectValuesLable.text = val.ToString(); 
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        Menus.Instance.MatchMenu.MoveStick.ValueChanged -= Movement;
        Menus.Instance.MatchMenu.ShootClick -= Shoot;
        Shooter.OnVictimHited -= OnVictimHit;
        Victim.HealthChanged -= OnHealthChange;
        Collector.CollectedValuesChanged -= ValuesCollected;
    }

    private IEnumerator MovementSending()
    {
        Vector2 prevDirection = Vector2.zero;
        float prevSendTime = 0;

        if (isClient)
        {
            while (true)
            {
                if (Time.time - prevSendTime > syncInterval && moveDirection != prevDirection)
                {
                    prevSendTime = Time.time;
                    prevDirection = moveDirection;
                    Walker.CmdMove(moveDirection);
                }
                yield return null;
            }
        }
    }

    #endregion

}
