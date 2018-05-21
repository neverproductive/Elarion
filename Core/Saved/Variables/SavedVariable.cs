﻿using System;
using Elarion.Attributes;
using Elarion.Saved.Events;
using UnityEngine;

namespace Elarion.Saved.Variables {
    [SavedVariable]
    public abstract class SavedVariable<TVariable> : SavedEvent<TVariable> {
        
        [SerializeField, HideInInspector]
        private TVariable _value;
        
        public TVariable Value {
            get { return _value; }
            set {
                if(value.Equals(_value)) {
                    return;
                }
                
                _value = value;
                Raise(_value);
            }
        }
        
        public override void Subscribe(Action<TVariable> onValueChanged) {
            base.Subscribe(onValueChanged);
        }

        public override void Unsubscribe(Action<TVariable> onValueChanged) {
            base.Unsubscribe(onValueChanged);
        }
        
        public static implicit operator TVariable(SavedVariable<TVariable> savedFloat) {
            return savedFloat == null ? default(TVariable) : savedFloat.Value;
        }
    }
}