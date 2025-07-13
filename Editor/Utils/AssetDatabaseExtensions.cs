using System.IO;
using UnityEditor;

namespace Polymorphism4Unity.Editor.Utils
{
    public static class AssetDatabaseExtensions
    {
        public static T LoadPackageAssetAtPath<T>(string relativePath) where T : UnityEngine.Object =>
            AssetDatabase.LoadAssetAtPath<T>(Path.Join(Constants.AssemblyPath, relativePath));
    }
}