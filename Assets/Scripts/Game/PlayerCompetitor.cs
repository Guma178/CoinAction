using CoinAction.Game;
using CoinAction.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompetitor : Competitor
{
    #region Server
    #endregion


    #region Client

    float prevSendTime;

    Vector2 moveDirection, prevDirection;

    public override void OnStartClient()
    {
        base.OnStartClient();

        Menus.Instance.MatchMenu.MoveStick.ValueChanged += delegate (Vector2 dir) { moveDirection = dir; };
    }

    private void ClientUpdate()
    {
        if (isClient)
        {
            if (Time.time - prevSendTime > syncInterval && moveDirection != prevDirection)
            {
                prevSendTime = Time.time;
                prevDirection = moveDirection;
                Walker.CmdMove(moveDirection);
            }
        }
    }
    #endregion

    private void Update()
    {
        ClientUpdate();
    }

}
