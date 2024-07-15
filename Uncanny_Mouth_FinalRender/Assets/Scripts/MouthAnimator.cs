using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthAnimator : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // Reference to the SkinnedMeshRenderer for mouth animation
    public bool lipSyncToggle = false;
    public AudioSource audioSource; // Reference to the AudioSource for getting loudness

    private float lerpSpeed = 10f; // Speed of linear interpolation
    private float currentBlendShapeValue = 0.0f; // Current blend shape value

    private void Update()
    {
        if (lipSyncToggle)
        {
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is not assigned.");
                return;
            }

            // Retrieve the loudness from the AudioSource
            float loudness = GetCurrentLoudness();

            // Update mouth shape based on the loudness
            UpdateMouthShape(loudness);
        }
    }

    private void UpdateMouthShape(float loudness)
    {
        // Debug.Log("µË´Ï´ç");
        // Calculate target blend shape value based on loudness
        float targetBlendShapeValue = Mathf.Clamp(loudness * 1000, 0, 100); // Scale the loudness to blend shape range

        // Interpolate towards the target blend shape value
        currentBlendShapeValue = Mathf.Lerp(currentBlendShapeValue, targetBlendShapeValue, lerpSpeed * Time.deltaTime);

        // Update the blend shape weight for the mouth
        skinnedMeshRenderer.SetBlendShapeWeight(0, currentBlendShapeValue); // Assuming 'mouth' blend shape is at index 0
    }

    private float GetCurrentLoudness()
    {
        // Calculate loudness by averaging the absolute values of the samples
        float[] samples = new float[256];
        audioSource.GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }
        return sum / samples.Length;
    }
}