#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class EditorResourcesMiner
    {
        [MenuItem("Unity Editor Resources/Export All Icons", priority = -1001)]
        private static void ExportIcons()
        {
            EditorUtility.DisplayProgressBar("Export Icons", "Exporting...", 0.0f);
            try
            {
                AssetBundle editorAssetBundle = GetEditorAssetBundle();
                string[] icons = EnumerateIcons(editorAssetBundle).ToArray();
                int count = 0;
                for (int i = 0; i < icons.Length; ++i)
                {
                    string assetName = icons[i];
                    Texture2D? icon = editorAssetBundle.LoadAsset<Texture2D>(assetName);
                    if (icon == null)
                    {
                        continue;
                    }
                    Texture2D readableTexture = new(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
                    Graphics.CopyTexture(icon, readableTexture);
                    string? folderPath = Path.GetDirectoryName(assetName);
                    folderPath = Path.Combine("Assets/Editor/Icons/", folderPath!);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string iconPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(assetName) + ".png");
                    readableTexture = Decompress(readableTexture);
                    File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());
                    count++;
                    EditorUtility.DisplayProgressBar("Export Icons", "Exporting...", i / (float)icons.Length);
                }
                Debug.Log($"{count} icons have been exported!");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Unity Editor Resources/Export All Stylesheets", priority = -1001)]
        private static void ExportStylesheets()
        {
            EditorUtility.DisplayProgressBar("Export Stylesheets", "Exporting...", 0.0f);
            try
            {
                AssetBundle editorAssetBundle = GetEditorAssetBundle();
                string[] stylesheets = EnumerateStylesheets(editorAssetBundle).ToArray();
                int count = 0;
                for (int i = 0; i < stylesheets.Length; ++i)
                {
                    string assetName = stylesheets[i];
                    StyleSheet styleSheet = editorAssetBundle.LoadAsset<StyleSheet>(assetName);
                    Debug.Log(styleSheet);
                    if (styleSheet == null)
                    {
                        continue;
                    }
                    string? folderPath = Path.GetDirectoryName(assetName);
                    folderPath = Path.Combine("Assets/Editor/Stylesheets/", folderPath!);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = Path.GetFileName(assetName);
                    string stylesheetPath = Path.Combine(folderPath, fileName + ".asset");
                    StyleSheet copy = Object.Instantiate(styleSheet);
                    AssetDatabase.CreateAsset(copy, stylesheetPath);
                    count++;
                    EditorUtility.DisplayProgressBar("Export Stylesheets", "Exporting...", i / (float)stylesheets.Length);
                }
                AssetDatabase.SaveAssets();
                Debug.Log($"{count} stylesheets have been exported!");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static Texture2D Decompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        private static IEnumerable<string> EnumerateStylesheets(AssetBundle editorAssetBundle)
        {
            foreach (string? assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (assetName.EndsWith(".uss", StringComparison.OrdinalIgnoreCase) == false &&
                    assetName.EndsWith(".tss", StringComparison.OrdinalIgnoreCase) == false)
                    continue;
                Debug.Log(assetName);
                yield return assetName;
            }
        }

        private static IEnumerable<string> EnumerateIcons(AssetBundle editorAssetBundle)
        {
            foreach (string? assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false &&
                    assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                yield return assetName;
            }
        }

        private static AssetBundle GetEditorAssetBundle()
        {
            Type editorGUIUtility = typeof(EditorGUIUtility);
            MethodInfo? getEditorAssetBundle = editorGUIUtility.GetMethod(
                "GetEditorAssetBundle",
                BindingFlags.NonPublic | BindingFlags.Static);

            return (AssetBundle)getEditorAssetBundle!.Invoke(null, new object[]
            {
            });
        }
    }
}