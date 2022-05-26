using System;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using PendulumClient.UI;
using PendulumClient.UnityExtensions;
using AvatarList = Il2CppSystem.Collections.Generic.List<VRC.Core.ApiAvatar>;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using System.Linq;
using UnhollowerRuntimeLib.XrefScans;

//using UiAvatarList = UiAvatarList;

namespace ReMod.Core.UI
{
    public interface IAvatarListOwner
    {
        AvatarList GetAvatars(ReAvatarList avatarList);
        void Clear(ReAvatarList avatarList);
    }

    public class ReAvatarList : UiElement
    {
        private static GameObject _legacyAvatarList;
        private static GameObject LegacyAvatarList
        {
            get
            {
                if (_legacyAvatarList == null)
                {
                    _legacyAvatarList = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Legacy Avatar List");
                }

                return _legacyAvatarList;
            }
        }

        private readonly UiAvatarList _avatarList;

        private readonly bool _hasPagination;
        private readonly ReUiButton _refreshButton;
        private readonly ReUiButton _nextPageButton;
        private readonly ReUiButton _prevPageButton;
        private readonly ReUiText _pageCount;

        private int _currentPage;

        private int _maxAvatarsPerPage = 100;

        public SimpleAvatarPedestal AvatarPedestal => _avatarList.field_Public_SimpleAvatarPedestal_0;

        private readonly Text _textComponent;

        public string Title
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public event Action OnEnable;

        private readonly string _title;

        private readonly IAvatarListOwner _owner;
        public ReAvatarList(string title, IAvatarListOwner owner, bool addClearButton = true, bool addPagination = true) : base(
            LegacyAvatarList,
            LegacyAvatarList.transform.parent,
            $"{title}AvatarList",
            false)
        {
            _hasPagination = addPagination;
            _owner = owner;
            _title = title;

            _avatarList = GameObject.GetComponent<UiAvatarList>();
            _avatarList.clearUnseenListOnCollapse = false;
            _avatarList.field_Public_Category_0 = UiAvatarList.Category.SpecificList;

            GameObject.transform.SetSiblingIndex(0);

            EnableDisableListener.RegisterSafe();
            var enableDisableListener = GameObject.AddComponent<EnableDisableListener>();
            enableDisableListener.OnEnableEvent += () =>
            {
                OnEnable?.Invoke();
                RefreshAvatars();
            };

            var expandButton = GameObject.GetComponentInChildren<Button>(true);
            _textComponent = expandButton.GetComponentInChildren<Text>();
            _textComponent.supportRichText = true;
            Title = title;

            var offset = 0f;
            if (addClearButton)
            {
                var clearButton = new ReUiButton("Clear", new Vector3(975, 0f), new Vector2(101.7f, 80f), () => { _owner.Clear(this); }, expandButton.transform);
                offset = 85f;
            }

            _refreshButton = new ReUiButton("↻", new Vector3(980f - offset, 0f), new Vector2(84.75f, 80f), RefreshAvatars, expandButton.transform);
            if (_hasPagination)
            {
                _nextPageButton = new ReUiButton("→", new Vector2(900f - offset, 0f), new Vector2(84.75f, 80f), () =>
                {
                    _currentPage += 1;
                    RefreshAvatars();
                }, expandButton.transform);

                _prevPageButton = new ReUiButton("←", new Vector2(750f - offset, 0f), new Vector2(84.75f, 80f), () =>
                {
                    _currentPage -= 1;
                    RefreshAvatars();
                }, expandButton.transform);

                _pageCount = new ReUiText("0 / 0", new Vector2(825f - offset, 0f), new Vector2(0.25f, 1f), () =>
                {
                    ShowInputPopupWithCancel(VRCUiPopupManager.prop_VRCUiPopupManager_0, "Goto Page", string.Empty, InputField.InputType.Standard, true, "Submit",
                        (s, k, t) =>
                        {
                            if (string.IsNullOrEmpty(s))
                                return;

                            _currentPage = int.Parse(s) - 1;
                            RefreshAvatars();
                        }, null, "Enter page...");
                }, expandButton.transform);
            }
        }


        private delegate void ShowInputPopupWithCancelDelegate(VRCUiPopupManager popupManager, string title,
    string preFilledText,
    InputField.InputType inputType, bool useNumericKeypad, string submitButtonText,
    Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> submitButtonAction,
    Il2CppSystem.Action cancelButtonAction, string placeholderText = "Enter text....", bool hidePopupOnSubmit = true,
    Il2CppSystem.Action<VRCUiPopup> additionalSetup = null, bool param_11 = false, int param_12 = 0);

        private static ShowInputPopupWithCancelDelegate _showInputPopupWithCancelDelegate;

        private static ShowInputPopupWithCancelDelegate ShowInputPopupWithCancelFn
        {
            get
            {
                if (_showInputPopupWithCancelDelegate != null)
                    return _showInputPopupWithCancelDelegate;

                var method = typeof(VRCUiPopupManager).GetMethods().Single(m =>
                {
                    if (!m.Name.StartsWith(
                            "Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_Boolean_Int32_") ||
                        m.Name.Contains("PDM"))
                        return false;

                    return XrefScanner.XrefScan(m).Any(x => x.Type == XrefType.Global && x.ReadAsObject()?.ToString() ==
                        "UserInterface/MenuContent/Popups/InputKeypadPopup");
                });

                _showInputPopupWithCancelDelegate = (ShowInputPopupWithCancelDelegate)Delegate.CreateDelegate(typeof(ShowInputPopupWithCancelDelegate), method);

                return _showInputPopupWithCancelDelegate;
            }
        }

        public static void ShowInputPopupWithCancel(VRCUiPopupManager popupManager, string title, string preFilledText,
            InputField.InputType inputType, bool useNumericKeypad, string submitButtonText,
            Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> submitButtonAction,
            Action cancelButtonAction, string placeholderText = "Enter text....", bool hidePopupOnSubmit = true,
            Action<VRCUiPopup> additionalSetup = null)
        {
            ShowInputPopupWithCancelFn(popupManager,
                    title,
                    preFilledText,
                    inputType, useNumericKeypad, submitButtonText, submitButtonAction, cancelButtonAction, placeholderText, hidePopupOnSubmit, additionalSetup);
        }
        public void SetMaxAvatarsPerPage(int value)
        {
            _maxAvatarsPerPage = value;
            RefreshAvatars();
        }

        public void RefreshAvatars()
        {
            Refresh(_owner.GetAvatars(this));
        }

        public void Refresh(AvatarList avatars)
        {
            if (_hasPagination)
            {
                var pagesCount = 0;
                if (avatars.Count != 0)
                {
                    pagesCount = (avatars.Count - 1) / _maxAvatarsPerPage;
                }
                _currentPage = Mathf.Clamp(_currentPage, 0, pagesCount);

                _pageCount.Text = $"{_currentPage + 1} / {pagesCount + 1}";
                var cutDown = avatars.GetRange(_currentPage * _maxAvatarsPerPage,
                    Math.Abs(_currentPage * _maxAvatarsPerPage - avatars.Count));
                if (cutDown.Count > _maxAvatarsPerPage)
                {
                    cutDown.RemoveRange(_maxAvatarsPerPage, cutDown.Count - _maxAvatarsPerPage);
                }

                _prevPageButton.Interactable = _currentPage > 0;
                _nextPageButton.Interactable = _currentPage < pagesCount;

                Title = $"{_title} ({cutDown.Count * (_currentPage + 1)}/{avatars.Count})";

                _avatarList.StartRenderElementsCoroutine(cutDown);
            }
            else
            {
                _avatarList.StartRenderElementsCoroutine(avatars);
            }
        }
    }
    public class UiElement
    {
        public string Name { get; }
        public GameObject GameObject { get; }
        public RectTransform RectTransform { get; }

        public Vector3 Position
        {
            get => RectTransform.localPosition;
            set => RectTransform.localPosition = value;
        }

        public bool Active
        {
            get => GameObject.activeSelf;
            set => GameObject.SetActive(value);
        }

        public UiElement(Transform transform)
        {
            RectTransform = transform.GetComponent<RectTransform>();
            if (RectTransform == null)
                throw new ArgumentException("Transform has to be a RectTransform.", nameof(transform));

            GameObject = transform.gameObject;
            Name = GameObject.name;
        }

        public UiElement(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : this(original, parent, name, defaultState)
        {
            GameObject.transform.localPosition = pos;
        }

        public UiElement(GameObject original, Transform parent, string name, bool defaultState = true)
        {
            GameObject = Object.Instantiate(original, parent);
            GameObject.name = GetCleanName(name);
            Name = GameObject.name;

            GameObject.SetActive(defaultState);
            RectTransform = GameObject.GetComponent<RectTransform>();
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }

        public static string GetCleanName(string name)
        {
            return Regex.Replace(Regex.Replace(name, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
        }
    }

    public class ReUiButton : UiElement
    {
        private readonly Text _textComponent;

        public string Text
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        private readonly Button _buttonComponent;
        public bool Interactable
        {
            get => _buttonComponent.interactable;
            set => _buttonComponent.interactable = value;
        }

        public ReUiButton(string text, Vector2 pos, Vector2 scale, Action onClick, Transform parent = null) : base(
            GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Favorite Button"), parent, pos,
            $"{text}UiButton")
        {

            _buttonComponent = GameObject.GetComponentInChildren<Button>();
            _buttonComponent.onClick = new Button.ButtonClickedEvent();
            _buttonComponent.onClick.AddListener(new Action(onClick));

            _textComponent = GameObject.GetComponentInChildren<Text>();
            _textComponent.supportRichText = true;
            Text = text;

            var allTextComponents = GameObject.GetComponentsInChildren<Text>(true);
            foreach (var t in allTextComponents)
            {
                if (t.transform == _textComponent.transform)
                    continue;

                Object.DestroyImmediate(t);
            }

            RectTransform.sizeDelta = scale;
        }
    }

    public class ReUiText : UiElement
    {
        private readonly Text _textComponent;

        public string Text
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public ReUiText(string text, Vector2 pos, Vector2 scale, Action onClick = null, Transform parent = null) : base(
            GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Favorite Button"), parent, pos,
            $"{text}UiButton")
        {
            var button = GameObject.GetComponentInChildren<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new Action(() => onClick?.Invoke()));
            //Object.DestroyImmediate(GameObject.GetComponentInChildren<Button>());
            Object.DestroyImmediate(GameObject.GetComponentInChildren<Image>());

            _textComponent = GameObject.GetComponentInChildren<Text>();
            Text = text;

            var allTextComponents = GameObject.GetComponentsInChildren<Text>(true);
            foreach (var t in allTextComponents)
            {
                if (t.transform == _textComponent.transform)
                    continue;

                Object.DestroyImmediate(t);
            }

            RectTransform.sizeDelta *= scale;
        }
    }
}