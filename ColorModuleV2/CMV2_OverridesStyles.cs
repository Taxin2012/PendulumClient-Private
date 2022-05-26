using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using VRC.UI.Core.Styles;
using UnityEngine;
using UnhollowerRuntimeLib;
using Object = UnityEngine.Object;

namespace PendulumClient.ColorModuleV2
{
    public class CMV2_OverridesStyles
    {
        private readonly Dictionary<string, string> myStyleOverrides = new Dictionary<string, string>();
        private readonly Dictionary<string, List<ElementStyle>> myStylesCache = new Dictionary<string, List<ElementStyle>>();
        private readonly List<(int styleIndex, ulong selectorPriority, Selector selector, List<(int, StyleElement.PropertyValue)> Properties)> myOriginalStylesBackup = new List<(int styleIndex, ulong selectorPriority, Selector selector, List<(int, StyleElement.PropertyValue)> Properties)>();

        private readonly Dictionary<string, Sprite> myOriginalSprites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> myOriginalSpritesByLowercaseShortKey = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> myOriginalSpritesByLowercaseFullKey = new Dictionary<string, Sprite>();
        private readonly Dictionary<Il2CppSystem.Tuple<string, Il2CppSystem.Type>, Object> myOriginalResourcesInAllResourcesMap = new Dictionary<Il2CppSystem.Tuple<string, Il2CppSystem.Type>, Object>();
        private readonly Dictionary<string, string> myNormalizedToActualSpriteNames = new Dictionary<string, string>();
        private readonly Dictionary<string, Sprite> myOverrideSprites = new Dictionary<string, Sprite>();

        private static List<string> SpritesToGrayscale = new List<string>();

        private readonly Il2CppSystem.Collections.Generic.Dictionary<Sprite, Sprite> mySpriteOverrideDict = new Il2CppSystem.Collections.Generic.Dictionary<Sprite, Sprite>();

        public readonly StyleEngine myStyleEngine;

        public readonly string Name;

        public CMV2_OverridesStyles(string name, StyleEngine engine)
        {
            Name = name;
            myStyleEngine = engine;
        }

        public static CMV2_OverridesStyles ParseFrom(string name, IEnumerable<string> lines, StyleEngine engine)
        {
            var result = new CMV2_OverridesStyles(name, engine);

            var inComment = false;
            var inBody = false;
            var lastSelectorText = "";
            var bodyText = new StringBuilder();
            int lineNumber = 0;
            foreach (var lineRaw in lines)
            {
                lineNumber++;
                var line = lineRaw.Trim();
                if (line.Length == 0)
                    continue;

                if (inComment)
                {
                    if (line.EndsWith("*/"))
                        inComment = false;
                    else if (line.Contains("*/"))
                        throw new ArgumentException($"Multi-line comments mixed into line are not supported (at line {lineNumber})");

                    continue;
                }

                if (line.StartsWith("//")) continue;
                if (line.StartsWith("/*"))
                {
                    inComment = true;
                    continue;
                }
                else if (line.Contains("/*"))
                    throw new ArgumentException($"Multi-line comments mixed into line are not supported (at line {lineNumber})");

                if (inBody)
                {
                    if (line == "}")
                    {
                        inBody = false;
                        result.ParseOverride(lastSelectorText, bodyText.ToString());
                        lastSelectorText = "";
                        bodyText.Clear();
                    }
                    else if (line.Contains("}"))
                    {
                        throw new ArgumentException($"Mid-line closing braces are not supported (at line {lineNumber})");
                    }
                    else
                    {
                        bodyText.AppendLine(line);
                    }
                }
                else
                {
                    var openBraceIndex = line.IndexOf('{');
                    if (openBraceIndex != -1 && openBraceIndex != line.Length - 1)
                        throw new ArgumentException($"Mid-line opening braces are not supported (at line {lineNumber})");
                    if (line.Length > 1 && openBraceIndex != 0)
                        lastSelectorText = openBraceIndex == -1 ? line : line.Substring(0, openBraceIndex);
                    inBody = openBraceIndex >= 0;
                }
            }

            return result;
        }

        public void ApplyOverrides(CMV2_ColorManager colorizer)
        {
            foreach (var keyValuePair in myStyleOverrides)
            {
                var baseStyles = TryGetBySelector(keyValuePair.Key);
                if (baseStyles.Count < 1)
                {
                    PendulumLogger.Log($"Selector {keyValuePair.Key} overrides nothing in default style");
                    continue;
                }

                var style = new ElementStyle();
                myStyleEngine.Method_Public_Void_ElementStyle_String_0(style, colorizer.ReplacePlaceholders(keyValuePair.Value));

                foreach (var newStylePair in style.field_Public_Dictionary_2_Int32_PropertyValue_0)
                    foreach (var baseStyle in baseStyles)
                        baseStyle.field_Public_Dictionary_2_Int32_PropertyValue_0[newStylePair.Key] = newStylePair.Value;
            }

            PendulumLogger.Log($"Applied {myStyleOverrides.Count} overrides");
        }

        public void ParseOverride(string lastSelectorText, string bodyText)
        {
            try
            {
                var selector = Selector.Method_Public_Static_Selector_String_0(lastSelectorText);

                var selectorNormalized = selector.ToStringNormalized();

                if (myStyleOverrides.ContainsKey(selectorNormalized))
                    PendulumLogger.LogWarning($"Style sheet override {Name} contains duplicate selector {selectorNormalized}");

                myStyleOverrides[selectorNormalized] = bodyText;
            }
            catch (Exception ex)
            {
                PendulumLogger.LogWarning($"Error while parsing override style {Name}: {ex}");
            }
        }
        public Sprite TryFindOriginalSprite(string key)
        {
            return myOriginalSpritesByLowercaseFullKey.TryGetValue(key, out var result) ? result : null;
        }

        public void ApplySpriteOverrides(List<string> grayimages)
        {
            SpritesToGrayscale = grayimages;
            foreach (var spriteName in SpritesToGrayscale)
            {
                var originalSprite = TryFindOriginalSprite(spriteName);
                if (originalSprite == null) continue;

                var grayscaled = CMV2_SpriteUtil.GetGrayscaledSprite(originalSprite, true);
                OverrideSprite(spriteName, grayscaled);
            }

            foreach (var keyValuePair in myOverrideSprites)
                OverrideSprite(keyValuePair.Key, keyValuePair.Value);
        }

        public void OverrideSprite(string key, Sprite sprite)
        {
            var StyleEngine = myStyleEngine;
            var keyLastPart = key + ".png";
            var actualKey = myNormalizedToActualSpriteNames.TryGetValue(keyLastPart, out var normalized) ? normalized : keyLastPart;

            var originalSprite = TryFindOriginalSprite(key);
            if (originalSprite != null) mySpriteOverrideDict[originalSprite] = sprite;

            StyleEngine.field_Private_Dictionary_2_String_Sprite_0[actualKey] = sprite;
            StyleEngine.field_Private_Dictionary_2_Tuple_2_String_Type_Object_0[
                new Il2CppSystem.Tuple<string, Il2CppSystem.Type>(key.ToLower(), Il2CppType.Of<Sprite>())] = sprite;
        }


        public void UpdateStylesForSpriteOverrides()
        {
            var StyleEngine = myStyleEngine;
            var writeAccumulator = new List<(int, StyleElement.PropertyValue)>();

            foreach (var elementStyle in StyleEngine.field_Private_List_1_ElementStyle_0)
            {
                foreach (var keyValuePair in elementStyle.field_Public_Dictionary_2_Int32_PropertyValue_0)
                {
                    var styleProperty = keyValuePair.Value;
                    var maybeSprite = styleProperty.field_Public_Object_0?.TryCast<Sprite>();
                    if (maybeSprite == null || !mySpriteOverrideDict.ContainsKey(maybeSprite)) continue;

                    styleProperty.field_Public_Object_0 = mySpriteOverrideDict[maybeSprite];
                    writeAccumulator.Add((keyValuePair.Key, styleProperty));
                }

                // ah yes, ConcurrentModificationException
                foreach (var (k, v) in writeAccumulator)
                    elementStyle.field_Public_Dictionary_2_Int32_PropertyValue_0[k] = v;

                writeAccumulator.Clear();
            }
        }

        public void BackupDefaultStyle()
        {
            var StyleEngine = myStyleEngine;
            foreach (var elementStyle in StyleEngine.field_Private_List_1_ElementStyle_0)
            {
                var innerList = new List<(int, StyleElement.PropertyValue)>();
                foreach (var keyValuePair in elementStyle.field_Public_Dictionary_2_Int32_PropertyValue_0)
                    innerList.Add((keyValuePair.Key, keyValuePair.Value));
                myOriginalStylesBackup.Add((elementStyle.field_Public_Int32_0, elementStyle.field_Public_UInt64_0, elementStyle.field_Public_Selector_0, innerList));

                var normalizedSelector = elementStyle.field_Public_Selector_0.ToStringNormalized();

                if (myStylesCache.TryGetValue(normalizedSelector, out var existing))
                    existing.Add(elementStyle);
                else
                    myStylesCache[normalizedSelector] = new List<ElementStyle>() { elementStyle };
            }

            foreach (var keyValuePair in StyleEngine.field_Private_Dictionary_2_String_Sprite_0)
            {
                var key = keyValuePair.Key;
                var normalizedKey = key.ToLower();
                var sprite = keyValuePair.Value;

                myOriginalSprites[key] = sprite;
                myOriginalSpritesByLowercaseShortKey[normalizedKey] = sprite;
                myNormalizedToActualSpriteNames[normalizedKey] = key;
            }

            foreach (var keyValuePair in StyleEngine.field_Private_Dictionary_2_Tuple_2_String_Type_Object_0)
            {
                myOriginalResourcesInAllResourcesMap[keyValuePair.Key] = keyValuePair.Value;
                if (keyValuePair.Key.Item2 == Il2CppType.Of<Sprite>())
                    myOriginalSpritesByLowercaseFullKey[keyValuePair.Key.Item1] = keyValuePair.Value.Cast<Sprite>();
            }

            PendulumLogger.Log($"Stored default style: {myOriginalStylesBackup.Count} styles, {myOriginalSprites.Count} sprites");
        }

        public List<ElementStyle> TryGetBySelector(string normalizedSelector)
        {
            return myStylesCache.TryGetValue(normalizedSelector, out var result) ? result : new List<ElementStyle>();
        }
    }
}
