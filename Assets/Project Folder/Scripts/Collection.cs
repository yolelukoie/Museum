using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Collection : MonoBehaviour
{
    public float fadeDuration = 1.0f;

    void Start()
    {
        // Example usage: Fade out on start
        //StartCoroutine(FadeAllChildren(false));
        //SetAlphaImmediate(0f);
    }

    public void FadeIn()
    {
        TriggerFade(true);
    }

    public void FadeOut()
    {
        TriggerFade(false);
    }

    public void TriggerFade(bool fadeIn)
    {
        StartCoroutine(FadeAllChildren(fadeIn));
    }

    IEnumerator FadeAllChildren(bool fadeIn)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsedTime = 0f;

        // Set initial alpha based on fade direction
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        foreach (Graphic graphic in graphics)
        {
            SetAlpha(graphic, startAlpha);
        }

        foreach (Renderer renderer in renderers)
        {
            SetMaterialAlpha(renderer, startAlpha);
        }

        // Fade in or out over time
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            foreach (Graphic graphic in graphics)
            {
                SetAlpha(graphic, alpha);
            }

            foreach (Renderer renderer in renderers)
            {
                SetMaterialAlpha(renderer, alpha);
            }

            yield return null;
        }

        // Ensure final alpha is set correctly
        foreach (Graphic graphic in graphics)
        {
            SetAlpha(graphic, endAlpha);
        }

        foreach (Renderer renderer in renderers)
        {
            SetMaterialAlpha(renderer, endAlpha);
        }
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }

    void SetMaterialAlpha(Renderer renderer, float alpha)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }

    public void SetAlphaImmediate(float alpha)
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Graphic graphic in graphics)
        {
            SetAlpha(graphic, alpha);
        }

        foreach (Renderer renderer in renderers)
        {
            SetMaterialAlpha(renderer, alpha);
        }
    }
}
