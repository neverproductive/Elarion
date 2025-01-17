using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Elarion.UI {

    public class UIDialog : UIPanel, ISubmitHandler, ICancelHandler {

        // TODO use Unity events
        public event Action SubmitActions = () => { };
        public event Action CancelActions = () => { };

        [Serializable]
        protected enum DeselectAction {
            None,
            Submit,
            Cancel
        }
        
        // TODO automatically add Submit/Cancel handlers to child input components
        
        // TODO cache the object that was last focused before opening this and focus it back when closing
        
        // dialog submit button; is it necessary?

        [SerializeField]
        protected Button submitButton;

        [SerializeField]
        protected DeselectAction deselectAction;
        
        // close/submit dialogs when the scene changes/gets deselected
        
        // TODO Timeout component (as in graphic settings) - executes an action when the timer expires (also updates a UI element)
        
        // TODO scene changed event

        /// <summary>
        /// OnBlurred callback. Activate it only on the dialog 
        /// </summary>
        protected virtual void OnBlurred() {
            if(!IsOpened || ActiveDialogs.Peek() != this) {
                return;
            }
            
            switch(deselectAction) {
                case DeselectAction.None:
                    break;
                case DeselectAction.Submit:
                    Submit();
                    break;
                case DeselectAction.Cancel:
                    Cancel();
                    break;
                default:
                    goto case DeselectAction.None;
            }
        }

        protected override void BeforeOpen(bool skipAnimation) {
            base.BeforeOpen(skipAnimation);
            
            ActiveDialogs.Push(this);

            Canvas.overrideSorting = true;
            Canvas.sortingOrder = ActiveDialogs.Count;
        }

        protected override void AfterClose() {
            ActiveDialogs.Pop();

            Canvas.overrideSorting = false;
        }

        public virtual void Submit() {
            if(!IsOpened) {
                return;
            }
            
            // TODO cache the submitted/canceled state and reset it when opened (to preven submitting multiple times) 
            Debug.Log(name + "Submitting", gameObject);

            SubmitActions();
            
            Close();
        }

        public virtual void Cancel() {
            if(!IsOpened) {
                return;
            }
            
            Debug.Log(name + "Canceling", gameObject);
            
            CancelActions();
            
            Close();
        }

        public void OnSubmit(BaseEventData eventData) {
            Submit();
        }

        public void OnCancel(BaseEventData eventData) {
            Cancel();
        }

        private static Stack<UIDialog> _activeDialogs;

        protected static Stack<UIDialog> ActiveDialogs {
            get {
                if(_activeDialogs == null) {
                    _activeDialogs = new Stack<UIDialog>();
                }                
                return _activeDialogs;
            }
        }
    }
}