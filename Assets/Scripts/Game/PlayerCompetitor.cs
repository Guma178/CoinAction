using CoinAction.Game;
using CoinAction.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
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
            Menus.Instance.MatchMenu.MoveStick.ValueChanged += delegate (Vector2 dir) { moveDirection = dir; };
            Menus.Instance.MatchMenu.Colorize(state.Color);
            Menus.Instance.MatchMenu.ShootClick += delegate () { Shooter.CmdShoot(); };
            Shooter.OnVictimHited += delegate (float actualHealth, float maximalHealtj, Color color) 
            {
                Menus.Instance.MatchMenu.DisplayEnemyHealth(actualHealth / maximalHealtj, color);
            };
            Victim.HealthChanged += delegate (float actual, float maximal) { Menus.Instance.MatchMenu.PlayerHealthSlider.value = actual / maximal; };
            Collector.CollectedValuesChanged += delegate (short val) { Menus.Instance.MatchMenu.CollectValuesLable.text = val.ToString(); };
            if (movementSendingProcess != null)
            {
                StopCoroutine(movementSendingProcess);
            }
            movementSendingProcess = StartCoroutine(MovementSending());
        }
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
