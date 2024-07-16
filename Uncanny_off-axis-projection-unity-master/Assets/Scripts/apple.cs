using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GaussianBlurEffect : MonoBehaviour
{
    public Material gaussianBlurMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (gaussianBlurMaterial != null)
        {
            Graphics.Blit(source, destination, gaussianBlurMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
