using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoinAction.UI
{
    public class LobbyMenu : Menu
    {
        [SerializeField]
        TMP_InputField createInput, joinInput;

        [SerializeField]
        Button joinButton, createButton;

        public event System.Action<string> JoinClick, CreateClick;

        public void Start()
        {
            joinButton.onClick.AddListener(delegate() { JoinClick?.Invoke(joinInput.text); });
            createButton.onClick.AddListener(delegate () { CreateClick?.Invoke(createInput.text); });
        }
    }
}
