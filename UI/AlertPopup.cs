using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;

namespace PendulumClient.UI
{
    class AlertPopup
    {
        public static void SendAlertPopup(string text, float osdelay = 3f, float predelay = 0f, UnityEngine.Color color = default)
        {
            NewUIExtensions.QueueHudMessage(Main.PendulumClientMain.VRC_UIManager, "[PendulumClient]\n\n" + text, osdelay, predelay, color);
            //VRCUiManager.prop_VRCUiManager_0.field_Public_Text_0.color = new Color(0.8f, 0.8f, 0.8f);
            //VRCUiManager.prop_VRCUiManager_0.field_Private_Single_0 = 0f;
            //VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0("[PendulumClient]\n\n" + text);
        }

        public static void SendAlertPopup(string text, params object[] args)
        {
            string outputtext = string.Format(text, args);
            NewUIExtensions.QueueHudMessage(Main.PendulumClientMain.VRC_UIManager, "[PendulumClient]\n\n" + outputtext);
            //VRCUiManager.prop_VRCUiManager_0.field_Public_Text_0.color = new Color(0.8f, 0.8f, 0.8f);
            //VRCUiManager.prop_VRCUiManager_0.field_Private_Single_0 = 0f;
            //VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0("[PendulumClient]\n\n" + outputtext);
        }

        public static void SendAlertPopupNOPC(string text, float osdelay = 3f, float predelay = 0f, UnityEngine.Color color = default)
        {
            NewUIExtensions.QueueHudMessage(Main.PendulumClientMain.VRC_UIManager, text, osdelay, predelay, color);
            //VRCUiManager.prop_VRCUiManager_0.field_Public_Text_0.color = new Color(0.8f, 0.8f, 0.8f);
            //VRCUiManager.prop_VRCUiManager_0.field_Private_Single_0 = 0f;
            //VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0("[PendulumClient]\n\n" + text);
        }

        public static void SendAlertPopupNOPC(string text, params object[] args)
        {
            string outputtext = string.Format(text, args);
            NewUIExtensions.QueueHudMessage(Main.PendulumClientMain.VRC_UIManager, outputtext);
            //VRCUiManager.prop_VRCUiManager_0.field_Public_Text_0.color = new Color(0.8f, 0.8f, 0.8f);
            //VRCUiManager.prop_VRCUiManager_0.field_Private_Single_0 = 0f;
            //VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_String_0("[PendulumClient]\n\n" + outputtext);
        }

        public static void SendDefaultPopup(string text)
        {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_String_0(text);
        }
    }
    
    class InputPopup
    {
        /*VRCUiPopupManager.prop_VRCUiPopupManager_0.ShowInputPopupWithCancel("Set fly speed", FlySpeed.ToString(), InputField.InputType.Standard, false, "Submit",
                    (s, k, t) =>
                    {
                        if (string.IsNullOrEmpty(s))
                            return;

                        if (!float.TryParse(s, out var flySpeed))
                            return;

                        FlySpeed.SetValue(flySpeed);
                    }, null);
            }, ResourceManager.GetSprite("remodce.speed"));*/
        public static void ShowInputPopupWithCancel(VRCUiPopupManager popupManager, string title, string preFilledText, InputField.InputType inputType, bool useNumericKeypad, string submitButtonText, Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text> submitButtonAction, Action cancelButtonAction, string placeholderText = "Enter text....", bool hidePopupOnSubmit = true, Action<VRCUiPopup> additionalSetup = null)
        {
            popupManager.Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_Boolean_Int32_0(title, preFilledText, inputType, useNumericKeypad, submitButtonText, submitButtonAction, cancelButtonAction, placeholderText, hidePopupOnSubmit, additionalSetup);
        }

        public static int SendInputPopupReturnInt(string title, string prefill = "", string okbutton = "Submit")
        {
            var outputint = 0;
            ShowInputPopupWithCancel(VRCUiPopupManager.prop_VRCUiPopupManager_0, title, prefill, InputField.InputType.Standard, false, okbutton, (s, k, t) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    if (!int.TryParse(s, out var outnum))
                        return;

                    outputint = outnum;
                }, null);
            return outputint;
        }
    }
}
