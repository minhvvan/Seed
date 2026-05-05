using UnityEngine;
using UnityEngine.EventSystems;

namespace Seed.Input
{
    public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float radius = 100f;

        private RectTransform _rectTransform;
        private Camera _uiCamera;

        public Vector2 InputDirection { get; private set; }

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas is not null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                _uiCamera = canvas.worldCamera;

            background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, eventData.position, _uiCamera, out Vector2 localPoint);

            background.anchoredPosition = localPoint;
            handle.anchoredPosition = Vector2.zero;
            background.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background, eventData.position, _uiCamera, out Vector2 offset))
                return;

            if (offset.magnitude > radius)
                offset = offset.normalized * radius;

            handle.anchoredPosition = offset;
            InputDirection = offset / radius;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            background.gameObject.SetActive(false);
            handle.anchoredPosition = Vector2.zero;
            InputDirection = Vector2.zero;
        }
    }
}
