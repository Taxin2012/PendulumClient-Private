using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC;
using VRC.Core;
using VRC.UI.Core;
using VRC.UI.Elements;
using System.Reflection;
using HarmonyLib;
using UnhollowerRuntimeLib.XrefScans;

using ToggleIcon = MonoBehaviourPublicToStUnique;//VRC.UI.Elements.Controls.ToggleIcon;

namespace PendulumClient.UI
{
    public static class AvatarUtils
    {
        private delegate void PedestalRefreshDelegate(SimpleAvatarPedestal @this, ApiAvatar avatar);
        private static PedestalRefreshDelegate ourPedestalRefreshDelegate;
        public static SimpleAvatarPedestal Refresh(this SimpleAvatarPedestal pedestal, ApiAvatar avatar)
        {
            if (ourPedestalRefreshDelegate == null)
            {
                var target = typeof(SimpleAvatarPedestal)
                    .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Single(
                        it =>
                        {
                            if (it.ReturnType != typeof(void)) return false;
                            var parameters = it.GetParameters();
                            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(ApiAvatar))
                                return false;

                            var strings = XrefScanner.XrefScan(it)
                                .Select(jt => jt.Type == XrefType.Global ? jt.ReadAsObject()?.ToString() : null)
                                .Where(jt => jt != null).ToHashSet();
                            return strings.Contains("Refreshing with : ");
                        });

                ourPedestalRefreshDelegate =
                    (PedestalRefreshDelegate)Delegate.CreateDelegate(typeof(PedestalRefreshDelegate), target);
            }

            ourPedestalRefreshDelegate(pedestal, avatar);

            return pedestal;
        }
    }
    public static class XrefUtils
    {
        public static bool CheckMethod(MethodInfo method, string match)
        {
            try
            {
                foreach (var instance in XrefScanner.XrefScan(method))
                {
                    if (instance.Type == XrefType.Global && instance.ReadAsObject().ToString().Contains(match))
                        return true;
                }

                return false;
            }
            catch
            {
            }

            return false;
        }
        public static bool CheckUsedBy(MethodInfo method, string methodName, Type type = null)
        {
            foreach (var instance in XrefScanner.UsedBy(method))
            {
                if (instance.Type == XrefType.Method)
                {
                    try
                    {
                        if ((type == null || instance.TryResolve().DeclaringType == type) && instance.TryResolve().Name.Contains(methodName))
                            return true;
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }
        public static bool CheckUsing(MethodInfo method, string methodName, Type type = null)
        {
            foreach (var instance in XrefScanner.XrefScan(method))
            {
                if (instance.Type == XrefType.Method)
                {
                    try
                    {
                        if ((type == null || instance.TryResolve().DeclaringType == type) && instance.TryResolve().Name.Contains(methodName))
                            return true;
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }
        public static void DumpXRefs(this Type type)
        {
            PendulumLogger.Log($"{type.Name} XRefs:");
            foreach (var m in AccessTools.GetDeclaredMethods(type))
            {
                m.DumpXRefs(1);
            }
        }
        public static void DumpXRefs(this MethodInfo method, int depth = 0)
        {
            var indent = new string('\t', depth);
            PendulumLogger.Log($"{indent}{method.Name} XRefs:");
            foreach (var x in XrefScanner.XrefScan(method))
            {
                if (x.Type == XrefType.Global)
                {
                    PendulumLogger.Log($"\tString = {x.ReadAsObject()?.ToString()}");
                }
                else
                {
                    var resolvedMethod = x.TryResolve();
                    if (resolvedMethod != null)
                    {
                        PendulumLogger.Log($"{indent}\tMethod -> {resolvedMethod.DeclaringType?.Name}.{resolvedMethod.Name}");
                    }
                }
            }
        }
    }

    public static class NewUIExtensions
    {
        public static void QueueHudMessage(this VRCUiManager uiManager, string notification, float duration = 5f, float delay = 0f, Color color = default)
        {
            if (color == default)
            {
                color = ColorModule.ColorModule.CachedColor;
            }
            uiManager.field_Public_Text_0.color = color; // DisplayTextColor
            uiManager.field_Public_Text_0.text = string.Empty;
            uiManager.field_Private_Single_0 = 0f; // HudMessageDisplayTime
            uiManager.field_Private_Single_1 = duration; // HudMessageDisplayDuration
            uiManager.field_Private_Single_2 = delay; // DelayBeforeHudMessage

            uiManager.field_Private_List_1_String_0.Add(notification);
        }

        private delegate void OnValueChangedDelegate(ToggleIcon toggleIcon, bool arg0);
        private static OnValueChangedDelegate _onValueChanged;

        public static void OnValueChanged(this ToggleIcon toggleIcon, bool arg0)
        {
            if (_onValueChanged == null)
            {
                _onValueChanged = (OnValueChangedDelegate)Delegate.CreateDelegate(typeof(OnValueChangedDelegate),
                    typeof(ToggleIcon).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Private_Void_Boolean_PDM_") && XrefUtils.CheckMethod(m, "Toggled")));
            }

            _onValueChanged(toggleIcon, arg0);
        }

        private delegate void PushPageDelegate(MenuStateController menuStateCtrl, string pageName, UIContext uiContext,
            bool clearPageStack, UIPage.TransitionType transitionType);
        private static PushPageDelegate _pushPage;

        public static void PushPage(this MenuStateController menuStateCtrl, string pageName, UIContext uiContext = null,
            bool clearPageStack = false, UIPage.TransitionType transitionType = UIPage.TransitionType.Right)
        {
            if (_pushPage == null)
            {
                _pushPage = (PushPageDelegate)Delegate.CreateDelegate(typeof(PushPageDelegate),
                    typeof(MenuStateController).GetMethods().FirstOrDefault(m => m.GetParameters().Length == 4 && m.Name.StartsWith("Method_Public_Void_String_UIContext_Boolean_TransitionType_") && XrefUtils.CheckMethod(m, "No page named")));
            }

            _pushPage(menuStateCtrl, pageName, uiContext, clearPageStack, transitionType);
        }

        private delegate void SwitchToRootPageDelegate(MenuStateController menuStateCtrl, string pageName, UIContext uiContext,
            bool clearPageStack, bool inPlace);
        private static SwitchToRootPageDelegate _switchToRootPage;

        public static void SwitchToRootPage(this MenuStateController menuStateCtrl, string pageName, UIContext uiContext = null,
            bool clearPageStack = false, bool inPlace = false)
        {
            if (_switchToRootPage == null)
            {
                _switchToRootPage = (SwitchToRootPageDelegate)Delegate.CreateDelegate(typeof(SwitchToRootPageDelegate),
                    typeof(MenuStateController).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Public_Void_String_UIContext_Boolean_") && XrefUtils.CheckMethod(m, "UIPage not in root page list:")));
            }

            _switchToRootPage(menuStateCtrl, pageName, uiContext, clearPageStack, inPlace);
        }

        private delegate void CloseMenuDelegate(VRCUiManager uiManager, bool what, bool what2);
        private static CloseMenuDelegate _closeMenu;

        public static void CloseMenu(this VRCUiManager uiManager)
        {
            if (_closeMenu == null)
            {
                _closeMenu = (CloseMenuDelegate)Delegate.CreateDelegate(typeof(CloseMenuDelegate),
                    typeof(VRCUiManager).GetMethods().FirstOrDefault(m => m.Name.StartsWith("Method_Public_Void_Boolean_Boolean") && XrefUtils.CheckUsing(m, "TrimCache")));
            }

            _closeMenu(uiManager, true, false);
        }

        public static void StartRenderElementsCoroutine(this UiVRCList instance, Il2CppSystem.Collections.Generic.List<ApiAvatar> avatarList, int offset = 0, bool endOfPickers = true, VRCUiContentButton contentHeaderElement = null)
        {
            if (!instance.gameObject.activeInHierarchy || !instance.isActiveAndEnabled || instance.isOffScreen ||
                !instance.enabled)
                return;

            if (instance.scrollRect != null)
            {
                instance.scrollRect.normalizedPosition = new Vector2(0f, 0f);
            }
            instance.Method_Protected_Void_List_1_T_Int32_Boolean_VRCUiContentButton_0(avatarList, offset, endOfPickers, contentHeaderElement);
        }
        public static GameObject FindInactiveObjectInActiveRoot(string path)
        {
            var split = path.Split(new char[] { '/' }, 2);
            var rootObject = GameObject.Find($"/{split[0]}")?.transform;
            if (rootObject == null) return null;
            return Transform.FindRelativeTransformWithPath(rootObject, split[1], false)?.gameObject;
        }
    }
}
