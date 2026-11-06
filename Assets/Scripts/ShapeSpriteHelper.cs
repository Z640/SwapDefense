using System.Collections.Generic;
using UnityEngine;

public static class ShapeSpriteHelper
{
    private static readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private static Material defaultLineMaterial;

    public static Material GetLineMaterial()
    {
        if (defaultLineMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            }
            defaultLineMaterial = new Material(shader);
        }
        return defaultLineMaterial;
    }

    public static Sprite GetSquareSprite()
    {
        string key = "Square";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] colors = new Color[16];
        for (int i = 0; i < 16; i++)
        {
            colors[i] = Color.white;
        }
        tex.SetPixels(colors);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        spriteCache[key] = sprite;
        return sprite;
    }

    public static Sprite GetCircleSprite(int resolution = 128)
    {
        string key = $"Circle_{resolution}";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float center = (resolution - 1) * 0.5f;
        float maxRadius = center - 1.5f;
        Color[] colors = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(maxRadius - dist + 0.5f);
                colors[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
        spriteCache[key] = sprite;
        return sprite;
    }

    public static Sprite GetRingSprite(int resolution = 128, float thicknessRatio = 0.14f)
    {
        string key = $"Ring_{resolution}_{thicknessRatio:F2}";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float center = (resolution - 1) * 0.5f;
        float outerRadius = center - 1.5f;
        float innerRadius = outerRadius * (1f - thicknessRatio);
        Color[] colors = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float outerAlpha = Mathf.Clamp01(outerRadius - dist + 0.5f);
                float innerAlpha = Mathf.Clamp01(dist - innerRadius + 0.5f);
                float alpha = outerAlpha * innerAlpha;
                colors[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
        spriteCache[key] = sprite;
        return sprite;
    }

    public static Sprite GetDiamondSprite(int resolution = 128, bool isOutlineOnly = false, float thicknessRatio = 0.16f)
    {
        string key = $"Diamond_{resolution}_{isOutlineOnly}_{thicknessRatio:F2}";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float center = (resolution - 1) * 0.5f;
        float maxDist = center - 2.0f;
        float innerDist = maxDist * (1f - thicknessRatio);
        Color[] colors = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = Mathf.Abs(x - center);
                float dy = Mathf.Abs(y - center);
                float manhattan = dx + dy;
                float outerAlpha = Mathf.Clamp01(maxDist - manhattan + 0.7f);
                float alpha = outerAlpha;
                if (isOutlineOnly)
                {
                    float innerAlpha = Mathf.Clamp01(manhattan - innerDist + 0.7f);
                    alpha = outerAlpha * innerAlpha;
                }
                colors[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
        spriteCache[key] = sprite;
        return sprite;
    }

    public static Sprite GetHexagonSprite(int resolution = 128, bool isOutlineOnly = false, float thicknessRatio = 0.14f)
    {
        string key = $"Hexagon_{resolution}_{isOutlineOnly}_{thicknessRatio:F2}";
        if (spriteCache.TryGetValue(key, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float center = (resolution - 1) * 0.5f;
        float maxRadius = (center - 2.0f) * 0.8660254f; // cos(30 deg)
        float innerRadius = maxRadius * (1f - thicknessRatio);
        Color[] colors = new Color[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float px = Mathf.Abs(x - center);
                float py = Mathf.Abs(y - center);
                // Flat-top or pointy-top hexagon distance function
                float hexDist = Mathf.Max(px * 0.8660254f + py * 0.5f, py);
                float outerAlpha = Mathf.Clamp01(maxRadius - hexDist + 0.7f);
                float alpha = outerAlpha;
                if (isOutlineOnly)
                {
                    float innerAlpha = Mathf.Clamp01(hexDist - innerRadius + 0.7f);
                    alpha = outerAlpha * innerAlpha;
                }
                colors[y * resolution + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), resolution);
        spriteCache[key] = sprite;
        return sprite;
    }
}
