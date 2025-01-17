using Elarion.Attributes;
using Elarion.Extensions;
using Elarion.UI.Helpers.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI.Helpers {
    [UIComponentHelper]
    [RequireComponent(typeof(RectTransform))]
    public class UIResizable : BaseUIBehaviour {
        
        // TODO the same clamping as in UI draggable (same inheritor)

        [SerializeField]
        protected bool limitSize = false;

        [SerializeField]
        [ConditionalVisibility("limitSize")]
        private Vector2 _minSize = new Vector2(100, 100);
        
        [SerializeField]
        [ConditionalVisibility("limitSize")]
        private Vector2 _maxSize = new Vector2(1000, 1000);

        [Tooltip("Save the position if an animator is present.")]
        public bool savePosition = true;
        
        private UIAnimator _animator;

        private ResizeDirection _resizeDirection;

        public bool Resizing { get; private set; }

        public bool LimitSize {
            get { return limitSize; }
            set { limitSize = value; }
        }

        public Vector2 MinSize {
            get { return _minSize; }
            set { _minSize = value; }
        }

        public Vector2 MaxSize {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        protected override void Awake() {
            base.Awake();
            _animator = GetComponent<UIAnimator>();
        }

        internal void AddHandle(UIResizeHandle handle) {
            handle.eventTrigger.AddEventTrigger(
                eventData => {
                    _resizeDirection = handle.resizeDirection;
                    OnBeginDrag(eventData);
                },
                EventTriggerType.BeginDrag);

            handle.eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
            handle.eventTrigger.AddEventTrigger(OnEndDrag, EventTriggerType.EndDrag);
        }

        protected virtual void OnBeginDrag(BaseEventData data) {
            Resizing = true;
        }

        protected virtual void OnDrag(BaseEventData data) {
            var delta = ((PointerEventData)data).delta;

            Resize(delta);
        }

        protected virtual void OnEndDrag(BaseEventData data) {
            Resizing = false;

            if(!savePosition || !_animator) {
                return;
            }
            
            _animator.SizeTweener.SaveProperty();
            _animator.PositionTweener.SaveProperty();
        }

        public void Resize(Vector2 amount) {
            float pivotX = 0.5f;
            float pivotY = 0.5f;
            
            var resize = Vector2.zero;
            
            if(_resizeDirection.HasFlag(ResizeDirection.Right)) {
                pivotX = 0;
                resize.x = amount.x;
            } else if(_resizeDirection.HasFlag(ResizeDirection.Left)) {
                pivotX = 1;
                resize.x = -amount.x;
            }
            
            if(_resizeDirection.HasFlag(ResizeDirection.Up)) {
                pivotY = 0;
                resize.y = amount.y;
            } else if(_resizeDirection.HasFlag(ResizeDirection.Down)) {
                pivotY = 1;
                resize.y = -amount.y;
            }

            var width = Transform.sizeDelta.x + resize.x;
            var height = Transform.sizeDelta.y + resize.y;

            if(LimitSize) {
                width = Mathf.Clamp(width, MinSize.x, MaxSize.x);
                height = Mathf.Clamp(height, MinSize.y, MaxSize.y);
            } 

            var pivot = Transform.pivot;
            
            Transform.SetPivot(new Vector2(pivotX, pivotY));
            
            Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            
            Transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            
            Transform.SetPivot(pivot);
        }
    }
}