using CoinAction.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoinAction.UI
{
    public class RezultsPopUp : MonoBehaviour
    {
        [SerializeField]
        TMP_Text gameRezult, playerCoins, winnerCoins;

        [SerializeField]
        Button okButtton;

        public event System.Action OkButttonClick;

        private void Start()
        {
            okButtton.onClick.AddListener(delegate() { this.gameObject.SetActive(false); OkButttonClick?.Invoke(); });
        }

        public void LoadRezults(Match.Rezults rezults)
        {
            this.gameObject.SetActive(true);

            if (rezults.IsWon)
            {
                gameRezult.text = "You won!";
                winnerCoins.gameObject.SetActive(false);
            }
            else
            {
                gameRezult.text = "You loose!";
                winnerCoins.gameObject.SetActive(true);
                winnerCoins.text = $"Winner collected {rezults.WinnerCoinsAmount} coins.";
                winnerCoins.color = rezults.WinnerColor;
            }
            gameRezult.color = rezults.WinnerColor;

            playerCoins.text = $"You collected {rezults.PlayerCoinsAmount} coins.";
            playerCoins.color = rezults.PlayerColor;
        }
    }
}
