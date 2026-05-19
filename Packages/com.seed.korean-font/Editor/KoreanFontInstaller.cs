using TMPro;
using UnityEditor;
using UnityEngine;

namespace Seed.KoreanFont.Editor
{
    [InitializeOnLoad]
    internal static class KoreanFontInstaller
    {
        private const string FONT_ASSET_PATH = "Packages/com.seed.korean-font/Runtime/Fonts/PretendardSDF.asset";
        private const string UNITY_DEFAULT_FONT_NAME = "LiberationSans SDF";
        private const string MARKER_KEY_PREFIX = "Seed.KoreanFont.AutoInstalled.";

        static KoreanFontInstaller()
        {
            EditorApplication.delayCall += AutoInstall;
        }

        [MenuItem("Tools/Seed/Korean Font/Apply as TMP Default", priority = 1)]
        public static void ApplyMenu() => Install(forceSetDefault: true, showLog: true);

        [MenuItem("Tools/Seed/Korean Font/Add as Fallback Only", priority = 2)]
        public static void FallbackMenu() => Install(forceSetDefault: false, showLog: true);

        [MenuItem("Tools/Seed/Korean Font/Remove from TMP Settings", priority = 20)]
        public static void RemoveMenu() => Remove();

        private static void AutoInstall()
        {
            string markerKey = MARKER_KEY_PREFIX + Application.dataPath.GetHashCode();
            if (EditorPrefs.GetBool(markerKey, false)) return;

            if (Install(forceSetDefault: false, showLog: true))
                EditorPrefs.SetBool(markerKey, true);
        }

        private static bool Install(bool forceSetDefault, bool showLog)
        {
            TMP_Settings settings = TMP_Settings.instance;
            if (settings is null)
            {
                if (showLog) Debug.LogWarning("[Seed.KoreanFont] TMP_Settings를 찾을 수 없습니다. TMP Essentials를 먼저 임포트하세요.");
                return false;
            }

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_ASSET_PATH);
            if (fontAsset is null)
            {
                if (showLog) Debug.LogWarning($"[Seed.KoreanFont] 폰트 에셋이 없습니다: {FONT_ASSET_PATH}\nREADME의 'Font Asset 준비' 단계를 참고해 PretendardSDF.asset을 생성하세요.");
                return false;
            }

            var so = new SerializedObject(settings);
            var defaultProp = so.FindProperty("m_defaultFontAsset");
            var currentDefault = defaultProp?.objectReferenceValue as TMP_FontAsset;

            bool canReplaceDefault = forceSetDefault
                                     || currentDefault is null
                                     || currentDefault.name == UNITY_DEFAULT_FONT_NAME;

            if (canReplaceDefault && defaultProp is not null)
            {
                defaultProp.objectReferenceValue = fontAsset;
                if (showLog) Debug.Log("[Seed.KoreanFont] Pretendard SDF를 TMP Default Font Asset으로 설정했습니다.");
            }
            else
            {
                AddToFallback(so, fontAsset, showLog);
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            return true;
        }

        private static void AddToFallback(SerializedObject so, TMP_FontAsset fontAsset, bool showLog)
        {
            var fallbackList = so.FindProperty("m_fallbackFontAssets");
            if (fallbackList is null) return;

            for (int i = 0; i < fallbackList.arraySize; i++)
            {
                if (fallbackList.GetArrayElementAtIndex(i).objectReferenceValue == fontAsset)
                {
                    if (showLog) Debug.Log("[Seed.KoreanFont] 이미 Fallback에 포함되어 있어 건너뜁니다.");
                    return;
                }
            }

            int idx = fallbackList.arraySize;
            fallbackList.InsertArrayElementAtIndex(idx);
            fallbackList.GetArrayElementAtIndex(idx).objectReferenceValue = fontAsset;
            if (showLog) Debug.Log("[Seed.KoreanFont] 기존 Default 폰트는 유지하고 Pretendard SDF를 Fallback에 추가했습니다.");
        }

        private static void Remove()
        {
            TMP_Settings settings = TMP_Settings.instance;
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_ASSET_PATH);
            if (settings is null || fontAsset is null) return;

            var so = new SerializedObject(settings);
            var defaultProp = so.FindProperty("m_defaultFontAsset");
            if (defaultProp is not null && defaultProp.objectReferenceValue == fontAsset)
                defaultProp.objectReferenceValue = null;

            var fallbackList = so.FindProperty("m_fallbackFontAssets");
            if (fallbackList is not null)
            {
                for (int i = fallbackList.arraySize - 1; i >= 0; i--)
                {
                    if (fallbackList.GetArrayElementAtIndex(i).objectReferenceValue == fontAsset)
                        fallbackList.DeleteArrayElementAtIndex(i);
                }
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
            EditorPrefs.DeleteKey(MARKER_KEY_PREFIX + Application.dataPath.GetHashCode());
            Debug.Log("[Seed.KoreanFont] TMP Settings에서 Pretendard SDF 참조를 제거했습니다.");
        }
    }
}
