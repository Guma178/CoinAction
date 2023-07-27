    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.UI
{
    public class MatchMenu : Menu
    {
        [SerializeField]
        Joystick moveStick;

        public Joystick MoveStick => moveStick;
    }
}
