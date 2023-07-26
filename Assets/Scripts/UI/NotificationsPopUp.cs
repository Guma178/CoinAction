using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoinAction.UI
{
    public class NotificationsPopUp : MonoBehaviour
    {
        [SerializeField]
        RectTransform notificationPanel;

        [SerializeField]
        TMP_Text notificationMessageLable;

        [SerializeField]
        Button closeButton, approveButtton, declineButton;

        private static NotificationsPopUp instance;

        private System.Action approve, decline;

        private void Start()
        {
            instance = this;

            closeButton.onClick.AddListener(delegate () { notificationPanel.gameObject.SetActive(false); });
            approveButtton.onClick.AddListener(delegate () { approve?.Invoke(); notificationPanel.gameObject.SetActive(false); });
            declineButton.onClick.AddListener(delegate () { decline?.Invoke(); notificationPanel.gameObject.SetActive(false); });

        }

        public static void ShowMessage(string messageText)
        {
            if (instance != null)
            {
                instance.notificationPanel.gameObject.SetActive(true);
                instance.closeButton.gameObject.SetActive(true);
                instance.approveButtton.gameObject.SetActive(false);
                instance.declineButton.gameObject.SetActive(false);
                instance.notificationMessageLable.text = messageText;
            }
        }

        public static void ShowMessage(string messageText, System.Action approveAction, System.Action declineAction)
        {
            if (instance != null)
            {
                instance.approve = approveAction;
                instance.decline = declineAction;
                instance.notificationPanel.gameObject.SetActive(true);
                instance.closeButton.gameObject.SetActive(false);
                instance.approveButtton.gameObject.SetActive(true);
                instance.declineButton.gameObject.SetActive(true);
                instance.notificationMessageLable.text = messageText;
            }
        }
    }
}
