using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "MapEditorModules", menuName = "ScriptableObjects/MapEditorModules")]
public class MapEditorModulesSO : ScriptableObject
{
    [SerializeField] public List<MapModuleBtnInfo> allMapEditorModuleInfos;

    [ContextMenu("Generate Thumbnails (Persistent, Build-Ready)")]
    public void GenerateThumbnails()
    {
        string outputFolder = "Assets/Sprites/UI/WallEditor/ModuleThumbnails";
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        // delete old .pngs
        foreach (string file in Directory.GetFiles(outputFolder, "*.png"))
            AssetDatabase.DeleteAsset(file);

        AssetDatabase.Refresh();

        foreach (var info in allMapEditorModuleInfos)
        {
            if (info == null || info.module == null) continue;

            // render prefab as transparent texture
            var tex = PrefabThumbnailGenerator.RenderSpritePrefab(info.module.gameObject, 512);
            if (tex == null) continue;

            // save png
            string safeName = Sanitize(info.module.name);
            string path = $"{outputFolder}/{safeName}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());

            // import as sprite
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            PrefabThumbnailGenerator.ForceSpriteImportSettings(path);

            // assign sprite back
            info.thumbnail = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Transparent prefab thumbnails regenerated and saved for build.");
    }

    public PlaceableWallModule GetModulePrefabFromGUID(string desiredGUID)
    {
        foreach (MapModuleBtnInfo btnInfo in allMapEditorModuleInfos) if (btnInfo.moduleGUID == desiredGUID) return btnInfo.module;

        Debug.LogError("ERROR: No module with GUID exists ->" + desiredGUID);
        return null;
    }

    [ContextMenu("Generate Missing Module GUIDs")]
    private void GenerateMissingModuleGUIDs()
    {
        if (allMapEditorModuleInfos == null)
            return;

        HashSet<string> usedGuids = new HashSet<string>();

        // Collect existing GUIDs and detect duplicates
        foreach (var info in allMapEditorModuleInfos)
        {
            if (info == null)
                continue;

            if (string.IsNullOrEmpty(info.moduleGUID))
                continue;

            if (!usedGuids.Add(info.moduleGUID))
            {
                Debug.LogError($"Duplicate module GUID detected: {info.moduleGUID}");
            }
        }

        // Assign GUIDs where missing
        foreach (var info in allMapEditorModuleInfos)
        {
            if (info == null || info.module == null)
                continue;

            if (!string.IsNullOrEmpty(info.moduleGUID))
                continue;

            string guid = Guid.NewGuid().ToString("N");

            while (usedGuids.Contains(guid))
            {
                guid = Guid.NewGuid().ToString("N");
            }

            info.moduleGUID = guid;
            usedGuids.Add(guid);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [ContextMenu("Export Module GUID Registry (.txt)")]
    public void ExportModuleGUIDRegistry()
    {
        // CHECK: make sure every module has a GUID already
        foreach (var info in allMapEditorModuleInfos)
        {
            if (info == null || info.module == null)
            {
                Debug.LogError("ERROR: A ModuleBtnInfo is null or module is null");
                return;
            }

            if (string.IsNullOrEmpty(info.moduleGUID))
            {
                Debug.LogError("ERROR: Missing module GUID -> " + info.module.name + ". Generate GUIDs before exporting.");
                return;
            }
        }

        #if UNITY_EDITOR
        // WRITE FILE (do not generate/modify GUIDs here)
        string outputFolder = "Assets/";
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        string path = $"{outputFolder}/ModuleGUIDRegistry.txt";

        List<string> lines = new List<string>();
        foreach (var info in allMapEditorModuleInfos)
        {
            if (info == null || info.module == null) continue;
            lines.Add(info.module.name + ":" + info.moduleGUID);
        }

        File.WriteAllLines(path, lines.ToArray());

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        #endif
    }

    static string Sanitize(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Replace(' ', '_');
    }
}

[System.Serializable]
public class MapModuleBtnInfo
{
    public string moduleGUID;
    public PlaceableWallModule module;
    public Sprite thumbnail;
}
