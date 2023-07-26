using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoinAction.UI
{
    public class Menus : MonoBehaviour
    {
        [SerializeField]
        List<Menu> menus;

        [SerializeField]
        MatchMenu matchMenu;

        [SerializeField]
        LoadingMenu loadingMenu;

        [SerializeField]
        LobbyMenu lobbyMenu;

        public static Menus Instance;

        public MatchMenu MatchMenu => matchMenu;
        public LoadingMenu LoadingMenu => loadingMenu;
        public LobbyMenu LobbyMenu => lobbyMenu;

        public void Activate(Menu menu)
        {
            foreach (Menu m in menus)
            {
                m.Turn(m == menu);
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
