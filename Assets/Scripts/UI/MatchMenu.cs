    using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoinAction.UI
{
    public class MatchMenu : Menu
    {
        [SerializeField]
        Graphic[] colorizable;

        [SerializeField]
        Graphic enemySliderBackground;

        [SerializeField]
        Slider playerHealthSlider, enemyHealthSlider;

        [SerializeField]
        Joystick moveStick;

        [SerializeField]
        RezultsPopUp rezultsPopUp;

        [SerializeField]
        Button shootButton;

        [SerializeField]
        TMP_Text collectValuesLable;

        [SerializeField]
        float enemyBarDisplayingTime = 2;

        public event System.Action ShootClick;

        public Slider PlayerHealthSlider => playerHealthSlider;
        public Slider EnemyHealthSlider => enemyHealthSlider;
        public Joystick MoveStick => moveStick;
        public RezultsPopUp RezultsPopUp => rezultsPopUp;

        public TMP_Text CollectValuesLable => collectValuesLable;

        private Coroutine displayingCoroutine;

        public void Colorize(Color color)
        {
            foreach (Graphic grp in colorizable)
            {
                grp.color = color;
            }
        }

        public void DisplayEnemyHealth(float healthRatio, Color color)
        {
            enemySliderBackground.color = color;
            enemyHealthSlider.value = healthRatio;
            if (displayingCoroutine != null) { StopCoroutine(displayingCoroutine); }
            displayingCoroutine = StartCoroutine(DiplayingEnemyHealthbar());
        }

        private IEnumerator DiplayingEnemyHealthbar()
        {
            enemyHealthSlider.gameObject.SetActive(true);
            yield return new WaitForSeconds(enemyBarDisplayingTime);
            enemyHealthSlider.gameObject.SetActive(false);
        }

        private void Start()
        {
            shootButton.onClick.AddListener(delegate() { ShootClick?.Invoke(); });
        }
    }
}
