using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace PendulumClient.UI
{
	internal class NotificationV2
	{
		internal NotificationV2(GameObject obj, ImageThreeSlice background, TextMeshProUGUI tmp, Vector3 position, float deleteTime, Color outlinecolor, GameObject outline, GameObject bgobj)
        {
			Object = obj;
			BG = background;
			text = tmp;
			pos = position;
			timeUntilDelete = deleteTime;
			Outline = outline.GetComponent<ImageThreeSlice>();
			BGObj = bgobj;
			OutlineColor = outlinecolor;
		}
		internal float timeUntilDelete;
		internal TextMeshProUGUI text;
		internal GameObject Object;
		internal GameObject BGObj;
		internal ImageThreeSlice BG;
		internal ImageThreeSlice Outline;
		internal Color OutlineColor;
		internal Vector3 pos;
	}
	internal class NotificationsV2
    {
		internal static GameObject AlertTextParent;
		internal static GameObject NotificationTemplate;

		private static readonly List<NotificationV2> Notifications = new List<NotificationV2>();

		private static readonly float notificationPosX = 250f;
		private static readonly float notificationPosY = -345f;

		private static Color BGColor = new Color(0.15f, 0.15f, 0.15f, 1f);
		private static Color TextColor = new Color(1f, 1f, 1f, 1f);
		private static Color InvisColor = new Color(0f, 0f, 0f, 0f);

		private static Color PendColor = new Color(0.77f, 0.11f, 0.19f, 1f);

		internal static readonly ConcurrentQueue<Action> notificationQueue = new ConcurrentQueue<Action>();

		internal static void OnUI()
		{
			AlertTextParent = GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/AlertTextParent");
			NotificationTemplate = UnityEngine.Object.Instantiate(AlertTextParent.transform.Find("Capsule").gameObject, AlertTextParent.transform);
			NotificationTemplate.transform.Find("Text").gameObject.SetActive(true);
			NotificationTemplate.name = "PCNotificationTemplate";
			SendHudNotificationThreaded("Welcome to <color=#C51C31>Pendulum Client!</color>", PendColor);
		}
		internal static void OnUpdate()
        {
			if (NotificationTemplate == null)
				return;

			bool updatepos = false;
			float time = 10f * Time.deltaTime;
			for (int i = 0; i < Notifications.Count; i++)
			{
				Notifications[i].timeUntilDelete -= Time.deltaTime;
				if (Notifications[i].timeUntilDelete <= 0f)
				{
					UnityEngine.Object.Destroy(Notifications[i].BGObj.gameObject);
					Notifications.RemoveAt(i);
					updatepos = true;
					continue;
				}
				if (Notifications[i].timeUntilDelete <= 1f)
				{
					Notifications[i].BG.color = Color.Lerp(Notifications[i].BG.color, InvisColor, time);
					Notifications[i].text.faceColor = Color32.Lerp(Notifications[i].text.faceColor, InvisColor, time);
					Notifications[i].Outline.color = Color32.Lerp(Notifications[i].Outline.color, InvisColor, time);
				}
				else
				{
					Notifications[i].BG.color = Color.Lerp(Notifications[i].BG.color, BGColor, time);
					Notifications[i].text.faceColor = Color32.Lerp(Notifications[i].text.faceColor, TextColor, time);
					Notifications[i].Outline.color = Color32.Lerp(Notifications[i].Outline.color, Notifications[i].OutlineColor, time);
				}
				Vector3 localPosition = Vector3.Lerp(Notifications[i].BGObj.gameObject.transform.localPosition, Notifications[i].pos, time);
				Notifications[i].BGObj.gameObject.transform.localPosition = localPosition;
			}
			if (updatepos)
			{
				UpdateNotificationPositions();
			}
			Queue_1();
		}
		internal static void Queue_1()
		{
			try
			{
				if (notificationQueue.Count > 0 && notificationQueue.TryDequeue(out var result))
				{
					result();
				}
			}
			catch (Exception e)
			{
			}
		}

		internal static void UpdateNotificationPositions()
		{
			int num = 0;
			for (int num2 = Notifications.Count; num2 > 0; num2--)
			{
				Notifications[num2 - 1].pos = new Vector3(notificationPosX, 35f * num + notificationPosY, 0f);
				num++;
			}
		}
		internal static void ClearNotifications()
		{
			for (int i = 0; i < Notifications.Count; i++)
			{
				UnityEngine.Object.Destroy(Notifications[i].Object);
			}
			Notifications.Clear();
		}

		internal static void MoveInHierarchy(int delta, Transform org, Transform newt)
		{
			int index = org.GetSiblingIndex();
			newt.SetSiblingIndex(index + delta);
		}
		internal static void SendHudNotification(string text, Color color)
        {
			if (AlertTextParent == null || NotificationTemplate == null)
				return;

			try
			{
				AddNotification(text, color);
			}
			catch (System.Exception e)
			{
			}
		}
		internal static void SendHudNotificationThreaded(string text, Color color = default)
		{
			Color outline = Color.white;
			if (color == default)
				outline = new Color(ColorModule.ColorModule.CachedColor.r, ColorModule.ColorModule.CachedColor.g, ColorModule.ColorModule.CachedColor.b, 1f);
			else
				outline = color;

			notificationQueue.Enqueue(delegate
			{
				SendHudNotification(text, outline);
			});
		}
		internal static void AddNotification(string text, Color outlinecolor = default)
		{
			GameObject newslice = UnityEngine.Object.Instantiate(NotificationTemplate, AlertTextParent.transform);
			newslice.transform.localPosition = new Vector3(350f, -345f, 0f);
			newslice.transform.localScale = new Vector3(1f, 1f, 1f);
			newslice.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			newslice.name = "PCNotificationOutline";
			var outlineslice = newslice.GetComponent<ImageThreeSlice>();
			UnityEngine.Component.DestroyImmediate(newslice.GetComponent<UnityEngine.UI.ContentSizeFitter>());
			UnityEngine.Component.DestroyImmediate(newslice.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>());
			UnityEngine.Object.DestroyImmediate(newslice.GetComponentInChildren<TextMeshProUGUI>().gameObject);
			outlineslice._sprite = JoinNotifier.JoinNotifierMod.DevIconOutline;
			outlineslice.prop_Sprite_0 = JoinNotifier.JoinNotifierMod.DevIconOutline;

			//PendulumLogger.DebugLog("debug1");

			GameObject newbg = UnityEngine.Object.Instantiate(NotificationTemplate, AlertTextParent.transform);
			newbg.transform.localPosition = new Vector3(350f, -345f, 0f);
			newbg.transform.localScale = new Vector3(1f, 1f, 1f);
			newbg.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			newbg.name = "PCNotificationBG";
			var bgslice = newbg.GetComponent<ImageThreeSlice>();
			UnityEngine.Component.DestroyImmediate(newbg.GetComponent<UnityEngine.UI.ContentSizeFitter>());
			UnityEngine.Component.DestroyImmediate(newbg.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>());
			UnityEngine.Object.DestroyImmediate(newbg.GetComponentInChildren<TextMeshProUGUI>().gameObject);
			bgslice._sprite = JoinNotifier.JoinNotifierMod.DevOutline;
			bgslice.prop_Sprite_0 = JoinNotifier.JoinNotifierMod.DevOutline;

			//PendulumLogger.DebugLog("debug2");

			GameObject notifobj = UnityEngine.Object.Instantiate(NotificationTemplate, AlertTextParent.transform);
			notifobj.transform.localPosition = new Vector3(350f, -345f, 0f);
			notifobj.transform.localScale = new Vector3(1f, 1f, 1f);
			notifobj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			notifobj.name = "PCNotification";
			notifobj.GetComponent<ImageThreeSlice>().enabled = false;

			//PendulumLogger.DebugLog("debug3");

			//MoveInHierarchy(1, notifobj.transform, newslice.transform);
			newslice.transform.SetParent(newbg.transform);
			notifobj.transform.SetParent(newbg.transform);

			//PendulumLogger.DebugLog("debug4");

			Color outline = Color.white;
			if (outlinecolor == default)
				outline = new Color(ColorModule.ColorModule.CachedColor.r, ColorModule.ColorModule.CachedColor.g, ColorModule.ColorModule.CachedColor.b, 1f);
			else
				outline = outlinecolor;

			//PendulumLogger.DebugLog("debug5");

			NotificationV2 notification = new NotificationV2(notifobj, bgslice, notifobj.GetComponentInChildren<TextMeshProUGUI>(), new Vector3(), 10f, outline, newslice, newbg);

			//PendulumLogger.DebugLog("debug6");

			notification.BG.color = new Color(0f, 0f, 0f, 0f);
			notification.text.faceColor = new Color32(255, 255, 255, 0);
			notification.Outline.color = new Color(1f, 1f, 1f, 0f);
			notification.text.text = text;
			notification.text.richText = true;

			//PendulumLogger.DebugLog("debug7");

			notification.text.ComputeMarginSize();
			notification.text.ForceMeshUpdate();

			//PendulumLogger.DebugLog("debug8");

			try
			{
				newbg.SetActive(true);
				newslice.SetActive(true);
				notifobj.SetActive(true);
			}
			catch (Exception e)
            {
				//PendulumLogger.LogError("nigga nigger bruh: " + e.ToString());
            }

			//PendulumLogger.DebugLog("debug9");

			newslice.GetComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
			newbg.GetComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
			notification.text.gameObject.GetComponent<RectTransform>().ForceUpdateRectTransforms();
			newslice.GetComponent<RectTransform>().sizeDelta = new Vector2(notification.text.preferredWidth + 44f, 30f);
			newbg.GetComponent<RectTransform>().sizeDelta = new Vector2(notification.text.preferredWidth + 44f, 30f);
			Notifications.Add(notification);
			UpdateNotificationPositions();

			//PendulumLogger.DebugLog("debug10");
		}
	}	

}
