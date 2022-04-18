using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using VRC.Core;

namespace PendulumClient.ButtonAPIV2
{
    public static class APIV2_MainMenuButtons
    {
        public static void AvatarMenuButton(string txt, float xpos, float ypos, int xscale, int yscale, System.Action listener)
        {
            var ParentMenu = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Change Button").gameObject;
            var Btn = GameObject.Instantiate(BtnObj, BtnObj.transform, true).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(Btn.transform.localPosition.x - 275 + (xpos * 130), Btn.transform.localPosition.y - 50 + (ypos * 65), Btn.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.enabled = true; 
            Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = UnityEngine.Color.white;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(xscale, 0);
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(0, yscale);
            Btn.onClick.AddListener(listener);
            Btn.transform.SetParent(ParentMenu.transform);
        }
    }
}
