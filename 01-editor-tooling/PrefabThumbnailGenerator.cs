#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class PrefabThumbnailGenerator
{
    public static Texture2D RenderSpritePrefab(GameObject prefab, int size = 512)
    {
        // create isolated camera
        var camGO = new GameObject("ThumbnailCam", typeof(Camera));
        var cam = camGO.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.orthographic = true;
        cam.orthographicSize = 1f;
        cam.enabled = false;
        cam.allowHDR = false;
        cam.allowMSAA = false;

        // render texture (no mipmaps)
        var rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 1;
        cam.targetTexture = rt;

        // instantiate prefab off-scene
        var instance = Object.Instantiate(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;
        instance.transform.position = Vector3.zero;

        // isolate layer
        int isolatedLayer = 31;
        SetLayerRecursively(instance, isolatedLayer);
        cam.cullingMask = 1 << isolatedLayer;

        // fit camera to bounds
        var bounds = GetRendererBounds(instance);
        float extent = Mathf.Max(bounds.extents.x, bounds.extents.y);
        cam.orthographicSize = extent * 1.2f;
        cam.transform.position = bounds.center + Vector3.back * 10f;

        // clear + render
        RenderTexture.active = rt;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        cam.Render();

        // copy to Texture2D
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply(false, false);

        // cleanup
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGO);
        Object.DestroyImmediate(instance);

        return tex;
    }

    static Bounds GetRendererBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers)
            b.Encapsulate(r.bounds);
        return b;
    }

    static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    // ---- helper for import ----
    public static void ForceSpriteImportSettings(string assetPath)
    {
        var ti = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        if (ti == null) return;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single; // <-- force single
        ti.alphaIsTransparency = true;
        ti.mipmapEnabled = false;
        ti.npotScale = TextureImporterNPOTScale.None;
        ti.SaveAndReimport();
    }
}
#endif
