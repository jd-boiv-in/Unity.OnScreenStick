using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace EnhancedOnScreenControls
{
    public enum StickType
    {
        Fixed = 0,
        Floating = 1,
        Dynamic = 2
    }

    public enum AxisOptions
    {
        Both = 0,
        Horizontal = 1,
        Vertical = 2
    }

    [AddComponentMenu("Input/Enhanced On-Screen Stick")]
    [RequireComponent(typeof(RectTransform))]
    public class EnhancedOnScreenStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [InputControl(layout = "Vector2")]
        [FormerlySerializedAs("controlPath")]
        [SerializeField]
        string internalControlPath;

        [SerializeField] StickType stickType;
        [SerializeField] AxisOptions axisOptions = AxisOptions.Both;
        [SerializeField] float movementRange = 50f;
        [SerializeField, Range(0f, 1f)] float deadZone = 0f;
        [SerializeField] bool showOnlyWhenPressed;

        [SerializeField] Image background;
        [SerializeField] Image handle;

        protected override string controlPathInternal
        {
            get => internalControlPath;
            set => internalControlPath = value;
        }

        public StickType StickType
        {
            get => stickType;
            set => stickType = value;
        }

        public float MovementRange
        {
            get => movementRange;
            set => movementRange = value;
        }

        public float DeadZone
        {
            get => deadZone;
            set => deadZone = value;
        }

        RectTransform rectTransform;
        Canvas canvas;

        public bool Fade = true;
        public float Alpha = 1.0f;
        public float FadeDelay = 0.25f;
        public float FadeShowSpeed = 0.20f;
        public float FadeHideSpeed = 0.50f;
        public float LerpFactor = 0.000001f;
        private float _alpha = 0f;
        private float _delay = 0f;
        private bool _show;

        private Vector2 _desiredHandle;
        
        protected void Awake()
        {
            rectTransform = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>();

            if (showOnlyWhenPressed)
            {
                background.gameObject.SetActive(false);

                if (Fade)
                {
                    _alpha = 0f;
                    UpdateAlpha();
                }
            }
        }

        private void UpdateAlpha()
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, _alpha);
            handle.color = new Color(background.color.r, background.color.g, background.color.b, _alpha);
        }

        public void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            if (Fade)
            {
                if (_show)
                {
                    if (_alpha < Alpha)
                    {
                        _alpha += deltaTime / FadeShowSpeed;
                        if (_alpha > Alpha) _alpha = Alpha;
                        UpdateAlpha();
                    }
                }
                else if (_delay > 0f)
                {
                    _delay -= deltaTime;
                }
                else
                {
                    if (_alpha > 0f)
                    {
                        _alpha -= deltaTime / FadeHideSpeed;
                        if (_alpha < 0f) _alpha = 0f;
                        UpdateAlpha();
                    }
                }
            }

            handle.rectTransform.anchoredPosition = Vector2.Lerp(handle.rectTransform.anchoredPosition, _desiredHandle, 1f - Mathf.Pow(LerpFactor, deltaTime));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _show = true;
            background.gameObject.SetActive(true);

            if (stickType != StickType.Fixed)
            {
                var camera = canvas.worldCamera;
                var pos = new Vector2(eventData.position.x, eventData.position.y);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, eventData.position, camera, out var localPoint);
                bool inside = (localPoint.x > -background.rectTransform.sizeDelta.x / 2f) &&
                              (localPoint.x <  background.rectTransform.sizeDelta.x / 2f) && 
                              (localPoint.y > -background.rectTransform.sizeDelta.y / 2f) &&
                              (localPoint.y <  background.rectTransform.sizeDelta.y / 2f);
                
                if (_alpha <= 0 || !inside)
                {
                    _desiredHandle = handle.rectTransform.anchoredPosition = Vector2.zero;
                    background.rectTransform.localPosition = ScreenToAnchoredPosition(pos);
                    
                    if (Fade)
                    {
                        _alpha = 0f;
                        UpdateAlpha();
                    }
                }
            }

            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SentDefaultValueToControl();

            _desiredHandle = Vector2.zero;

            if (showOnlyWhenPressed)
            {
                if (!Fade) background.gameObject.SetActive(false);
                _show = false;
                _delay = FadeDelay;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            var camera = canvas.worldCamera;
            var position = RectTransformUtility.WorldToScreenPoint(camera, background.rectTransform.position);

            var input = (eventData.position - position) / (movementRange * canvas.scaleFactor) * EnabledAxis();
            var rawMagnitude = input.magnitude;
            var normalized = input.normalized;

            if (rawMagnitude < deadZone) input = Vector2.zero;
            else if (rawMagnitude > 1f) input = input.normalized;

            SendValueToControl(input);

            if (stickType == StickType.Dynamic && rawMagnitude > 1f)
            {
                var difference = movementRange * (rawMagnitude - 1f) * normalized;
                background.rectTransform.anchoredPosition += difference;
            }

            _desiredHandle = input * movementRange;
        }

        Vector2 ScreenToAnchoredPosition(Vector2 screenPosition)
        {
            var camera = canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, camera, out var localPoint))
            {
                var pivotOffset = rectTransform.pivot * rectTransform.sizeDelta;
                return localPoint - (background.rectTransform.anchorMax * rectTransform.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }

        Vector2 EnabledAxis()
        {
            if (axisOptions == AxisOptions.Horizontal)
                return Vector2.right;
            else if (axisOptions == AxisOptions.Vertical)
                return Vector2.up;
            return Vector2.one;
        }
    }
}