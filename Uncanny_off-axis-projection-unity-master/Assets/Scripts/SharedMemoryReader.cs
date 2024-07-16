using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class SharedMemoryReader : MonoBehaviour
{
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;

    public bool toggleKalman = true;

    private KalmanFilter kfX;
    private KalmanFilter kfY;
    private KalmanFilter kfZ;

    void Start()
    {
        // Open the existing memory-mapped file
        mmf = MemoryMappedFile.OpenExisting("Local\\memfile");
        accessor = mmf.CreateViewAccessor(0, 1024, MemoryMappedFileAccess.Read);

        // Initialize Kalman filters
        kfX = new KalmanFilter(0.3f, 100f, 1023f, 0f);
        kfY = new KalmanFilter(0.3f, 100f, 1023f, 0f);
        kfZ = new KalmanFilter(0.2f, 150f, 1023f, 0f);

        //// Initialize Kalman filters
        //kfX = new KalmanFilter(0.125f, 32f, 1023f, 0f);
        //kfY = new KalmanFilter(0.125f, 32f, 1023f, 0f);
        //kfZ = new KalmanFilter(0.125f, 32f, 1023f, 0f);
    }

    void Update()
    {
        // Read data from shared memory
        float x = accessor.ReadSingle(0);
        float y = accessor.ReadSingle(4);
        float z = accessor.ReadSingle(8); 

        if (y < 0)
        {
            return;
        }

        if (toggleKalman)
        {
            // Apply Kalman filter to each coordinate
            x = kfX.Update(x);
            y = kfY.Update(y);
            z = kfZ.Update(z);
        }

        BinocularCameraManager.Instance.target_position_real.x = - x;
        BinocularCameraManager.Instance.target_position_real.y = - y * 2;
        BinocularCameraManager.Instance.target_position_real.z = - z;

        Debug.Log($"Coordinates from Python: ({x}, {y}, {z})");
    }

    void OnDestroy()
    {
        // Clean up resources
        accessor.Dispose();
        mmf.Dispose();
    }
}
