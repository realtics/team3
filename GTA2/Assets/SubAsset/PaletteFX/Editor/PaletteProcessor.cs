using UnityEngine;
using UnityEditor;
using Lunar.Utils;
using System.IO;

public class PaletteProcessor : EditorWindow
{
    Texture2D sourceTexture;
    Texture2D palTexture;
    string path = "";
    DitherMode ditherMode = DitherMode.None;

    [MenuItem("Tools/Palette Processor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PaletteProcessor window = (PaletteProcessor)EditorWindow.GetWindow(typeof(PaletteProcessor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Convert a texture to 8bits", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        palTexture = (Texture2D)EditorGUILayout.ObjectField("Palette Texture", palTexture, typeof(Texture2D), false);
        path = EditorGUILayout.TextField("Destination Path", path);
        ditherMode = (DitherMode)EditorGUILayout.EnumPopup("Dither Mode", ditherMode);


        if (GUILayout.Button("Convert"))
        {
            var palette = new ColorTable();
            palette.LoadFromTexture(palTexture);
            var indices = palette.ApplyToTextureAsIndices(sourceTexture, ditherMode);
            var tex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Point;

            var colors = new Color32[indices.Length];
            for (int i=0; i<colors.Length; i++)
            {
                var index = indices[i];
                if (index<0)
                {
                    colors[i] = Color.clear;
                }
                else
                {
                    float n = index / (float)(palTexture.width-1);
                    byte k = (byte)(n*255);                    
                    colors[i] = new Color32(k, k, k, 255);
                }
            }
            tex.SetPixels32(colors);
            tex.Apply();

            string targetPath = string.IsNullOrEmpty(path) ? Application.dataPath : Path.Combine(Application.dataPath, path);
            Directory.CreateDirectory(targetPath);

            //var targetPath = "Assets";

            string fileName = sourceTexture.name;

            string tag = "_" + palTexture.name;
            if (!fileName.Contains(tag))
            {
                fileName += tag;
            }
            fileName += ".png";

            targetPath = Path.Combine(targetPath, fileName);

            Debug.Log("Trying export 8bit texture to " + targetPath);

            /*AssetDatabase.CreateAsset(tex, targetPath);
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            AssetDatabase.CreateAsset(sprite, targetPath.Replace("_8bit", "_spr"));*/


            File.WriteAllBytes(targetPath, tex.EncodeToPNG());

            AssetDatabase.Refresh();

            /*Texture2D temp = (Texture2D)AssetDatabase.LoadAssetAtPath(targetPath, typeof(Texture2D));
            temp.filterMode = FilterMode.Point;

            AssetDatabase.SaveAssets();*/
        }
    }
}