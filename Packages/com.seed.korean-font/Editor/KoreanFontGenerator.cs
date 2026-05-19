using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Seed.KoreanFont.Editor
{
    internal static class KoreanFontGenerator
    {
        private const string TTF_PATH = "Packages/com.seed.korean-font/Runtime/Fonts/Pretendard-Regular.ttf";
        private const string OUT_PATH = "Packages/com.seed.korean-font/Runtime/Fonts/PretendardSDF.asset";

        [MenuItem("Tools/Seed/Korean Font/Generate && Apply", priority = 0)]
        public static void Generate()
        {
            var ttf = AssetDatabase.LoadAssetAtPath<Font>(TTF_PATH);
            if (ttf is null)
            {
                EditorUtility.DisplayDialog(
                    "Seed Korean Font",
                    $"TTF 파일이 없습니다.\n다음 위치에 'Pretendard-Regular.ttf'를 넣어주세요:\n\n{TTF_PATH}",
                    "OK");
                return;
            }

            if (File.Exists(OUT_PATH))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Seed Korean Font",
                    $"이미 SDF 에셋이 존재합니다:\n{OUT_PATH}\n\n덮어쓸까요?",
                    "덮어쓰기", "취소");
                if (!overwrite) return;
                AssetDatabase.DeleteAsset(OUT_PATH);
            }

            const int samplingPointSize = 90;
            const int atlasPadding = 9;
            const int atlasSize = 1024;

            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                ttf,
                samplingPointSize,
                atlasPadding,
                GlyphRenderMode.SDFAA,
                atlasSize,
                atlasSize,
                AtlasPopulationMode.Dynamic,
                enableMultiAtlasSupport: true);

            fontAsset.name = "PretendardSDF";

            AssetDatabase.CreateAsset(fontAsset, OUT_PATH);

            if (fontAsset.atlasTextures is { Length: > 0 } && fontAsset.atlasTextures[0] is not null)
            {
                fontAsset.atlasTextures[0].name = "PretendardSDF Atlas";
                AssetDatabase.AddObjectToAsset(fontAsset.atlasTextures[0], fontAsset);
            }

            if (fontAsset.material is not null)
            {
                fontAsset.material.name = "PretendardSDF Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(OUT_PATH, ImportAssetOptions.ForceUpdate);

            Debug.Log($"[Seed.KoreanFont] Pretendard SDF 에셋을 생성했습니다: {OUT_PATH}");

            KoreanFontInstaller.ApplyMenu();

            EditorGUIUtility.PingObject(fontAsset);
            Selection.activeObject = fontAsset;
        }
    }
}