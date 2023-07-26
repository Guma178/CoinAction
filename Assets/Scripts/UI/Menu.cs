using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CoinAction.UI
{
    public class Menu : MonoBehaviour
    {
        public void Turn(bool val)
        {
            gameObject.SetActive(val);
        }
    }
}
