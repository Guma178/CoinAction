using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoinAction.UI
{
    public class Joystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        float xSensitivity, ySensitivity;

        [SerializeField]
        RectTransform stick, platform;

        Vector2 stickNormal, value;
        float absoluteMaximal;

        Coroutine broadcasting;

        public Vector2 Value => value;

        public event System.Action<Vector2> ValueChanged;
        public event System.Action Begin, End;

        private void Start()
        {
            value = Vector2.zero;
            absoluteMaximal = (platform.sizeDelta.y - stick.sizeDelta.y) / 2;
            stickNormal = stick.anchoredPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Begin?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            value = eventData.position - new Vector2(platform.position.x, platform.position.y);
            stick.anchoredPosition = Vector2.ClampMagnitude(value, absoluteMaximal);
            value = new Vector2(stick.anchoredPosition.x / absoluteMaximal * xSensitivity, stick.anchoredPosition.y / absoluteMaximal * ySensitivity);
            ValueChanged?.Invoke(value);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            value = Vector2.zero;
            ValueChanged?.Invoke(value);
            stick.anchoredPosition = stickNormal;
            End?.Invoke();
        }
    }
}
