using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteExporter
{
    [MenuItem("Assets/샘플 조각 PNG 저장")]
    public static void Export()
    {
        // 1. 처음 딱 한 번만 저장할 위치를 물어봅니다 (바탕화면이나 프로젝트 폴더 등 자유)
        string path = EditorUtility.SaveFolderPanel("저장할 폴더를 선택하세요", "Assets", "");

        if (string.IsNullOrEmpty(path)) return; // 취소하면 종료

        foreach (Object obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                Texture2D tex = sprite.texture;

                // [중요] Read/Write 체크 안되어있으면 패스
                if (!tex.isReadable)
                {
                    Debug.LogError($"{tex.name} 파일의 Read/Write를 체크해주세요!");
                    continue;
                }

                // 2. 조각 픽셀 추출
                Rect r = sprite.textureRect;
                Texture2D newTex = new Texture2D((int)r.width, (int)r.height);
                Color[] pixels = tex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
                newTex.SetPixels(pixels);
                newTex.Apply();

                // 3. 파일 쓰기
                byte[] bytes = newTex.EncodeToPNG();
                string finalPath = Path.Combine(path, sprite.name + ".png");
                File.WriteAllBytes(finalPath, bytes);

                Debug.Log($"[추출 완료] {finalPath}");
            }
        }
        AssetDatabase.Refresh();
    }
}