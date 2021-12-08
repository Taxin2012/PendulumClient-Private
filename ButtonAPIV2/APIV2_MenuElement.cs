﻿using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;


namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_MenuElement
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

        public APIV2_MenuElement(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : this(original, parent, name, defaultState)
        {
            GameObject.transform.localPosition = pos;
        }

        public APIV2_MenuElement(GameObject original, Transform parent, string name, bool defaultState = true)
        {
            GameObject = Object.Instantiate(original, parent);
            GameObject.name = CleanName(name);
            Name = GameObject.name;

            GameObject.SetActive(defaultState);
            RectTransform = GameObject.GetComponent<RectTransform>();
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }

        public static string CleanName(string name)
        {
            return Regex.Replace(Regex.Replace(name, "<.*?>", string.Empty), @"[^0-9a-zA-Z_]+", string.Empty);
        }
    }
}
