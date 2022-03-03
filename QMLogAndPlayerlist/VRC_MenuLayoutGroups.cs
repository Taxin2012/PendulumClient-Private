using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PendulumClient.QMLogAndPlayerlist
{
    public class VRC_PLScrollRect
    {
        public GameObject Scroll;
        public GameObject BG;
        public GameObject Viewport;
        public static GameObject VerticalLayout;
        public static GameObject CloneableText;
        public static GameObject TempText;
        public static TextMeshProUGUI CloneableText_T;
        public static TextMeshProUGUI TempText_T;
        public static GameObject PlayerCount;
        public static GameObject QuestCount;
        public static TextMeshProUGUI PlayerCount_T;
        public static TextMeshProUGUI QuestCount_T;
        public string name { get; private set; }
        public VRC_PLScrollRect(string filtername, string internalname, float x, float y, Transform parent)
        {
            Scroll = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect").gameObject, parent);
            //BG = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/BackgroundLayer01").gameObject, lable.transform.Find("Viewport/VerticalLayoutGroup"));
            //BG.name = "NewBGLayer";
            Scroll.name = internalname;
            name = filtername;
            Scroll.transform.localPosition = new Vector3(x, y, 0);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickLinks").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject);
            Viewport = Scroll.transform.Find("Viewport").gameObject;
            VerticalLayout = Viewport.transform.Find("VerticalLayoutGroup").gameObject;
            Viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(4096f, 0f);
            Viewport.GetComponent<RectMask2D>().enabled = true;
            Scroll.GetComponent<ScrollRect>().enabled = true;
            ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window").GetComponent<BoxCollider>().size = new Vector3(4096f, 1024f, 1f);
            Scroll.AddComponent<Button>();
            Scroll.AddComponent<VRC.UI.Core.Styles.StyleElement>();
            VerticalLayout.GetComponent<RectTransform>().localPosition += new Vector3(-40f, 0f);


            var tempqa = VerticalLayout.transform.Find("Header_QuickActions").gameObject;
            tempqa.name = "PC_CloneablePlayerListText";
            tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            tempqa.SetActive(false);
            CloneableText = tempqa;
            CloneableText_T = tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
            CloneableText_T.enableWordWrapping = false;
            CloneableText_T.fontSizeMin = 30;
            CloneableText_T.fontSizeMax = 30;
            CloneableText_T.alignment = TMPro.TextAlignmentOptions.Left;
            CloneableText_T.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
            CloneableText_T.color = Color.white;
            var temptext = VerticalLayout.transform.Find("Header_QuickLinks").gameObject;
            temptext.name = "DontDestroy";
            temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            TempText = temptext;
            TempText_T = temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
            TempText_T.enableWordWrapping = false;
            TempText_T.fontSizeMin = 30;
            TempText_T.fontSizeMax = 30;
            TempText_T.alignment = TMPro.TextAlignmentOptions.Left;
            TempText_T.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
            TempText_T.color = Color.white;

            VerticalLayout.GetComponent<VerticalLayoutGroup>().childControlHeight = true;

            Scroll.gameObject.SetActive(true);

            SetupPlayerCount(CloneableText, parent);
        }

        private void SetupPlayerCount(GameObject textobj, Transform parent)
        {
            var PlayerCountText = GameObject.Instantiate(textobj, parent);
            PlayerCountText.name = "PC_PlayerCounter";
            PlayerCountText.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "Players: NULL";
            PlayerCountText.transform.localPosition = new Vector3(594.5f, 600f, 0);
            PlayerCountText.SetActive(true);

            var QuestCountText = GameObject.Instantiate(textobj, parent);
            QuestCountText.name = "PC_QuestCounter";
            QuestCountText.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "Quest Users: NULL";
            QuestCountText.transform.localPosition = new Vector3(594.5f, 550f, 0);
            QuestCountText.SetActive(true);

            PlayerCount = PlayerCountText;
            PlayerCount_T = PlayerCountText.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
            QuestCount = QuestCountText;
            QuestCount_T = QuestCountText.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    public class VRC_DebugScrollRect
    {
        public GameObject Scroll;
        public GameObject BG;
        public GameObject Viewport;
        public static GameObject VerticalLayout;
        public static GameObject CloneableText;
        public static GameObject TempText;
        public static TextMeshProUGUI CloneableText_T;
        public static TextMeshProUGUI TempText_T;
        public string name { get; private set; }
        public VRC_DebugScrollRect(string filtername, string internalname, float x, float y, Transform parent)
        {
            Scroll = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect").gameObject, parent);
            //BG = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/BackgroundLayer01").gameObject, lable.transform.Find("Viewport/VerticalLayoutGroup"));
            //BG.name = "NewBGLayer";
            Scroll.name = internalname;
            name = filtername;
            Scroll.transform.localPosition = new Vector3(x, y, 0);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickLinks").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject);
            Viewport = Scroll.transform.Find("Viewport").gameObject;
            VerticalLayout = Viewport.transform.Find("VerticalLayoutGroup").gameObject;
            Viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(4096f, 0f);
            Viewport.GetComponent<RectMask2D>().enabled = true;
            Scroll.GetComponent<ScrollRect>().enabled = true;
            ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window").GetComponent<BoxCollider>().size = new Vector3(4096f, 1024f, 1f);
            Scroll.AddComponent<Button>();
            Scroll.AddComponent<VRC.UI.Core.Styles.StyleElement>();
            VerticalLayout.GetComponent<RectTransform>().localPosition += new Vector3(3722f, 0f);


            var tempqa = VerticalLayout.transform.Find("Header_QuickActions").gameObject;
            tempqa.name = "PC_CloneableDebugLogText";
            tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            tempqa.SetActive(false);
            CloneableText = tempqa;
            CloneableText_T = tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
            CloneableText_T.enableWordWrapping = false;
            CloneableText_T.fontSizeMin = 30;
            CloneableText_T.fontSizeMax = 30;
            CloneableText_T.alignment = TMPro.TextAlignmentOptions.Right;
            CloneableText_T.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
            CloneableText_T.color = Color.white;
            var temptext = VerticalLayout.transform.Find("Header_QuickLinks").gameObject;
            temptext.name = "DontDestroy";
            temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            TempText = temptext;
            TempText_T = temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
            TempText_T.enableWordWrapping = false;
            TempText_T.fontSizeMin = 30;
            TempText_T.fontSizeMax = 30;
            TempText_T.alignment = TMPro.TextAlignmentOptions.Right;
            TempText_T.verticalAlignment = TMPro.VerticalAlignmentOptions.Top;
            TempText_T.color = Color.white;

            VerticalLayout.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
            VerticalLayout.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;

            Scroll.gameObject.SetActive(true);
        }
    }
}
