using System;
using Elarion.Attributes;
using UnityEngine;

namespace Elarion.UI.Animation {

    // Animations modify the properties of an object
    [Serializable]
    public partial class UIAnimation : ScriptableObject {

        private const int DefaultAnimationPriority = 10; 
        
        // TODO slides have a higher animation priority than fades
        
        // TODO animation delay
        
        // TODO Animate interface (containing the animate method); maybe add a target field (useful for other scripts)

        // old
        [HideInInspector]
        public UITransitionType type = UITransitionType.AplhaFade;

        // old
        [HideInInspector]
        public SlideDirection slideDirection = SlideDirection.Left;

        [Header("Animation Configuration")]
        [SerializeField]
        private AnimationMovementPreset _movement = AnimationMovementPreset.NoMovement;
        [SerializeField]
        [ConditionalVisibility("_movement != AnimationMovementPreset.NoMovement || AnimationMovementPreset.Custom")]
        private AnimationPresetDirection _movementDirection = AnimationPresetDirection.From;
        [SerializeField]
        private AnimationFadePreset _fade = AnimationFadePreset.FadeIn;
        
        [Space(10)]
        [SerializeField]
        private UIAnimationDuration _duration = UIAnimationDuration.Normal;

        [SerializeField, ConditionalVisibility(
             "_duration == UIAnimationDuration.Custom")]
        private float _customDuration = .75f;

        [SerializeField]
        private UIAnimationEase _easeFunction = UIAnimationEase.Smooth; 
        
        [SerializeField, ConditionalVisibility(
             "_easeFunction == UIAnimationEase.Custom")]
        private Ease _customEaseFunction = Ease.Linear;

        // effects?

        [Header("Movement Options")]
        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animatePosition = false;

        [ConditionalVisibility("animatePosition", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection positionAnimationDirection = UIAnimationDirection.RelativeTo;

        [ConditionalVisibility("animatePosition", "_movement == AnimationMovementPreset.Custom", order = 1)]
        public Vector2 positionDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateAnchors = false;

        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection anchorsAnimationDirection = UIAnimationDirection.From;

        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 minAnchorDelta = Vector2.zero;
        
        [ConditionalVisibility("animateAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 maxAnchorDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateSize = false;
        
        [ConditionalVisibility("animateSize", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection sizeAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateSize", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 sizeDelta = Vector2.zero;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool animateRotation = false;
        
        [ConditionalVisibility("animateRotation", "_movement == AnimationMovementPreset.Custom")]
        public UIAnimationDirection rotationAnimationDirection = UIAnimationDirection.RelativeTo;
        
        [ConditionalVisibility("animateRotation", "_movement == AnimationMovementPreset.Custom")]
        public Vector3 rotationDelta = Vector3.zero;
        
        [Space(10)]
        [Header("Fade Options")]
        [ConditionalVisibility(enableConditions: "_fade == AnimationFadePreset.Custom")]
        public bool animateAlpha = false;
        
        [ConditionalVisibility("animateAlpha", "_fade == AnimationFadePreset.Custom")]
        public UIAnimationDirection alphaAnimationDirection = UIAnimationDirection.From;
        
        [ConditionalVisibility("animateAlpha", "_fade == AnimationFadePreset.Custom")]
        public float alphaDelta = 0;
        
        [Space(10)]
        [Header("Advanced Options")]
        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool overrideAnimationPriority = false;
        
        [ConditionalVisibility("overrideAnimationPriority", "_movement == AnimationMovementPreset.Custom")]
        public int animationPriority = DefaultAnimationPriority;

        [ConditionalVisibility(enableConditions: "_movement == AnimationMovementPreset.Custom")]
        public bool overrideParentAnchors = false;
        
        [ConditionalVisibility("overrideParentAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 overrideParentAnchorMin = new Vector2(0.5f, 0.5f);
        [ConditionalVisibility("overrideParentAnchors", "_movement == AnimationMovementPreset.Custom")]
        public Vector2 overrideParentAnchorMax = new Vector2(0.5f, 0.5f);
        
        [HideInInspector]
        [Tooltip("Save the position after the movement finishes.", order = 0)]
        [ConditionalVisibility("_movement == AnimationMovementPreset.Custom", order = 1)]
        public bool savePosition = false;


        private UIAnimationOptions AnimationOptions {
            get {
                return new UIAnimationOptions(savePosition, Duration == 0, _customEaseFunction, Duration);
            }
        }
        
        public float Duration {
            get {
                if(_duration == UIAnimationDuration.Custom) {
                    return _customDuration;
                }

                return (int) _duration / 100f;
            }
        }
        
        public Ease Ease {
            get {
                if(_easeFunction == UIAnimationEase.Custom) {
                    return _customEaseFunction;
                }

                return (Ease) _easeFunction;
            }
        }

        public void Animate(UIAnimator animator, Action callback = null) {
            // maybe animate to saved position?
            // animation delay?
            
            Action syncedCallback = null;

            if(callback != null) {
                var callbackFired = false;
                
                syncedCallback = () => {
                    if(!callbackFired) {
                        callback();
                        callbackFired = true;
                    }
                };    
            }
            
            if(animatePosition)
                animator.Move(positionDelta, positionAnimationDirection, syncedCallback, AnimationOptions);
            
            if(animateAnchors)
                animator.MoveAnchors(minAnchorDelta, maxAnchorDelta, anchorsAnimationDirection, syncedCallback, AnimationOptions);
            
            if(animateRotation)
                animator.Rotate(rotationDelta, rotationAnimationDirection, syncedCallback, AnimationOptions);
            
            if(animateSize)
                animator.Resize(sizeDelta, sizeAnimationDirection, syncedCallback, AnimationOptions);
            
            if(animateAlpha)
                animator.Fade(alphaDelta, alphaAnimationDirection, syncedCallback, AnimationOptions);
        }

        public void Stop(UIAnimator animator, bool reset = false) {
            if(animatePosition)
                animator.PositionTweener.StopTween(reset);
            
            if(animateAnchors)
                animator.AnchorsTweener.StopTween(reset);
            
            if(animateRotation)
                animator.RotationTweener.StopTween(reset);
            
            if(animateSize)
                animator.SizeTweener.StopTween(reset);
            
            if(animateAlpha)
                animator.AlphaTweener.StopTween(reset);
        }
    }
}