﻿using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace PendulumClient.UnityExtensions
{
    [RegisterTypeInIl2Cpp]
    class EnableDisableListener : MonoBehaviour
    {
        [method: HideFromIl2Cpp]
        public event Action OnEnableEvent;
        [method: HideFromIl2Cpp]
        public event Action OnDisableEvent;

        public EnableDisableListener(IntPtr obj) : base(obj) { }
        public void OnEnable() => OnEnableEvent?.Invoke();
        public void OnDisable() => OnDisableEvent?.Invoke();

        private static bool _registered;
        [HideFromIl2Cpp]
        public static void RegisterSafe()
        {
            if (_registered) return;
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>();
                _registered = true;
            }
            catch (Exception)
            {
                _registered = true;
            }
        }
    }
}
