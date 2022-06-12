using System;
using System.Linq;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using ReMod.Core.UI;

namespace PendulumClient.UI
{
    internal static class AvatarPreviewScroller
    {
        internal static float SENSITIVITY = 3f;
        internal static ReUiButton _autoRotateButton = null;
        internal static bool AutoRotate = true;
        internal static Vector3 autoRotateAmount = Vector3.zero;
        internal static void SetupPreviewScroller()
        {
            GameObject scrollerContainer = new GameObject("ScrollerContainer", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[2] { Il2CppType.Of<RectMask2D>(), Il2CppType.Of<RectTransform>() }));
            RectTransform scrollerContainerRect = scrollerContainer.GetComponent<RectTransform>();
            scrollerContainerRect.SetParent(GameObject.Find("UserInterface/MenuContent/Screens/Avatar").transform);
            scrollerContainerRect.anchoredPosition3D = new Vector3(-565, 20, 1);
            scrollerContainerRect.localScale = Vector3.one;
            scrollerContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 400);
            scrollerContainerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 650);

            GameObject scrollerContent = new GameObject("ScrollerContent", new UnhollowerBaseLib.Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[2] { Il2CppType.Of<Image>(), Il2CppType.Of<RectTransform>() }));
            RectTransform scrollerContentRect = scrollerContent.GetComponent<RectTransform>();
            scrollerContentRect.SetParent(scrollerContainerRect);

            scrollerContentRect.anchoredPosition3D = Vector3.zero;
            scrollerContentRect.localScale = Vector3.one;
            scrollerContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 800);
            scrollerContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1300);
            scrollerContentRect.GetComponent<Image>().color = new Color(0, 0, 0, 0);

            ScrollRect scrollRect = scrollerContainer.AddComponent<ScrollRect>();
            scrollRect.content = scrollerContentRect;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            scrollRect.decelerationRate = 0.03f;
            scrollRect.scrollSensitivity = 0; //:moyai:
            scrollRect.onValueChanged = new ScrollRect.ScrollRectEvent();
            GameObject pedestal = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase/MainRoot/MainModel");
            var autoTurn = pedestal.GetComponent<UnityStandardAssets.Utility.AutoMoveAndRotate>();

            //ok thanks loukylor for showin how to do this properly but ur original drag func is complete shit so i had to remake it
            float lastposx = 0f;
            scrollRect.onValueChanged.AddListener(new Action<Vector2>((pos) =>
            {
                var rotamt = 0f;
                if (pos.x < lastposx)
                {
                    scrollRect.horizontalNormalizedPosition = 0f;
                    rotamt = -37f / Math.Abs(pos.x);
                    lastposx = pos.x;
                }
                else if (pos.x > lastposx)
                {
                    scrollRect.horizontalNormalizedPosition = 0f;
                    rotamt = 37f / Math.Abs(pos.x);
                    lastposx = pos.x;
                }
                pedestal.transform.Rotate(new Vector2(0, rotamt), SENSITIVITY * 375 * Time.deltaTime);
            }));

            //i still want the auto rotate thing cuz idk sometimes im too lazy to rotate it

            var parent = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Favorite Button").transform.parent;
            _autoRotateButton = new ReUiButton("Auto Rotate: <color=#00FF00>ON</color>", new Vector2(-875f, 315f), new Vector2(250f, 80f), () =>
            {
                if (AutoRotate)
                {
                    _autoRotateButton.Text = "Auto Rotate: <color=#FF0000>OFF</color>";
                    AutoRotate = false;

                    if (autoRotateAmount == Vector3.zero)
                        autoRotateAmount = new Vector3(0f, autoTurn.rotateDegreesPerSecond.value.y, 0f); //they will probably never change the speed of this but who knows what they gon do when the new mm comes out

                    autoTurn.rotateDegreesPerSecond.value = Vector3.zero;
                }
                else
                {
                    _autoRotateButton.Text = "Auto Rotate: <color=#00FF00>ON</color>";
                    AutoRotate = true;
                    autoTurn.rotateDegreesPerSecond.value = autoRotateAmount;
                }
            }, parent);
            _autoRotateButton.GameObject.SetActive(true);
        }
    }
}
