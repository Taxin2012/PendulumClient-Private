using System;
using PendulumClient.UI;
using PendulumClient.UnityExtensions;
using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.Reflection;
using MelonLoader;
using TMPro;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using Object = UnityEngine.Object;

using ToggleIcon = MonoBehaviourPublicToStUnique;//VRC.UI.Elements.Controls.ToggleIcon;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_ToggleButton : APIV2_MenuElement
    {
        private readonly Toggle _toggleComponent;

        public static Color ToggleDiabled = new Color(1f, 1f, 1f, 1f);
        public static Color ToggleEnabled = new Color(1f, 1f, 1f, 1f);
        public bool Interactable
        {
            get => _toggleComponent.interactable;
            set
            {
                _toggleComponent.interactable = value;

                if (_toggleStyleElement != null)
                    _toggleStyleElement.OnEnable();
            }
        }

        private bool _valueHolder;

        private StyleElement _toggleStyleElement;

        private object _toggleIcon;

        private TextMeshProUGUI _textComponent;
        public string Text
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public APIV2_ToggleButton(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultvalue = false, Sprite enabledicon = null, Sprite disabledicon = null) : base(NewQuickMenu.TogglePrefab, parent, $"Button_Toggle{text}")
        {
            var iconOn = RectTransform.Find("Icon_On").GetComponent<Image>();
            var iconOff = RectTransform.Find("Icon_Off").GetComponent<Image>();
            if (enabledicon == null)
            {
                iconOn.sprite = NewQuickMenu.OnIconSprite;
                iconOn.gameObject.GetComponent<RectTransform>().sizeDelta -= new Vector2(18f, 18f);
                iconOn.gameObject.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 6f, 0f);
            }
            else
            {
                iconOn.sprite = enabledicon;
            }

            if (disabledicon != null)
            {
                iconOff.sprite = disabledicon;
            }

            iconOff.color = ToggleDiabled;
            iconOn.color = ToggleEnabled;

            _toggleIcon = GameObject.GetComponent<ToggleIcon>();

            _toggleComponent = GameObject.GetComponent<Toggle>();
            _toggleComponent.onValueChanged = new Toggle.ToggleEvent();
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(OnValueChanged));
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(onToggle));

            _toggleStyleElement = GameObject.GetComponent<StyleElement>();

            _textComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _textComponent.text = text;
            _textComponent.richText = true;
            _textComponent.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            _textComponent.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            _textComponent.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiToggleTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            Toggle(defaultvalue, false);

            if (defaultvalue)
            {
                onToggle.Invoke(true);
                OnValueChanged(true);
            }

            /*EnableDisableListener.RegisterSafe();
            var listener = GameObject.AddComponent<EnableDisableListener>();
            listener.OnEnableEvent += UpdateToggleIfNeeded;*/
        }


        public void Toggle(bool value, bool callback = true, bool updateVisually = false)
        {
            _valueHolder = value;
            _toggleComponent.Set(value, callback);
            if (updateVisually)
            {
                UpdateToggleIfNeeded();
            }
        }

        private void UpdateToggleIfNeeded()
        {
            OnValueChanged(_valueHolder);
        }

        private void FindToggleIcon()
        {
            var components = new Il2CppSystem.Collections.Generic.List<Component>();
            GameObject.GetComponents(components);

            foreach (var c in components)
            {
                var il2CppType = c.GetIl2CppType();
                var il2CppFields = il2CppType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (il2CppFields.Count != 1)
                    continue;

                if (!il2CppFields.Any(t => t.IsPublic && t.FieldType == Il2CppType.Of<Toggle>()))
                    continue;

                var realType = GetUnhollowedType(il2CppType);
                if (realType == null)
                {
                    MelonLogger.Error("SHITS FUCKED!");
                    break;
                }
                _toggleIcon = Activator.CreateInstance(realType, c.Pointer);
                break;
            }
        }

        private List<Action> _onValueChanged;

        private void OnValueChanged(bool arg0)
        {
            if (_onValueChanged == null)
            {
                _onValueChanged = new List<Action>();
                foreach (var methodInfo in _toggleIcon.GetType().GetMethods().Where(m =>
                             m.Name.StartsWith("Method_Public_Void_") && XrefUtils.CheckMethod(m, "Toggled")))
                {
                    _onValueChanged.Add((Action)Delegate.CreateDelegate(typeof(Action), _toggleIcon, methodInfo));
                }
            }

            foreach (var onValueChanged in _onValueChanged)
            {
                onValueChanged();
            }
        }

        public static IEnumerable<Type> TryGetTypes(System.Reflection.Assembly asm)
        {
            try
            {
                return asm.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                try
                {
                    return asm.GetExportedTypes();
                }
                catch
                {
                    return e.Types.Where(t => t != null);
                }
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }

        }
        private static readonly Dictionary<string, Type> DeobfuscatedTypes = new Dictionary<string, Type>();
        private static readonly Dictionary<string, string> ReverseDeobCache = new Dictionary<string, string>();

        private static void BuildDeobfuscationCache()
        {
            if (DeobfuscatedTypes.Count > 0)
                return;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in TryGetTypes(asm)) TryCacheDeobfuscatedType(type);
            }
        }

        private static void TryCacheDeobfuscatedType(Type type)
        {
            try
            {
                if (!type.CustomAttributes.Any()) return;
                foreach (var att in type.CustomAttributes)
                {
                    if (att.AttributeType == typeof(ObfuscatedNameAttribute))
                    {
                        string obfuscatedName = att.ConstructorArguments[0].Value.ToString();
                        DeobfuscatedTypes.Add(obfuscatedName, type);
                        ReverseDeobCache.Add(type.FullName, obfuscatedName);
                    }
                }
            }
            catch
            {
            }

        }
        public static Type GetUnhollowedType(Il2CppSystem.Type cppType)
        {
            if (DeobfuscatedTypes.Count == 0)
            {
                BuildDeobfuscationCache();
            }

            var fullname = cppType.FullName;

            if (DeobfuscatedTypes.TryGetValue(fullname, out var deob))
                return deob;

            if (fullname.StartsWith("System."))
                fullname = $"Il2Cpp{fullname}";

            return null;
        }
    }
}
