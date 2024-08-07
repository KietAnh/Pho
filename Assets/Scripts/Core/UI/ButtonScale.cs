
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[AddComponentMenu("Button Scale")]
public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform tweenTarget;

    [Range(0, 2f)] public float pressScale = 1.1f;
    public float pressedDuration = 0.2f;

    private Vector3 _baseScale = Vector3.one;

    private void OnPress(bool isPressed)
    {
        Vector3 endValue = isPressed ? pressScale * Vector3.one : _baseScale;
        tweenTarget.DOScale(endValue, pressedDuration);
    }

    public void OnPointerDown(PointerEventData eventData) { OnPress(true); }

    public void OnPointerUp(PointerEventData eventData) { OnPress(false); }
}