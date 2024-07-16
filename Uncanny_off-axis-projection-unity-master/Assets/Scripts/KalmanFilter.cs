using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KalmanFilter
{
    private float Q; // Process noise covariance
    private float R; // Measurement noise covariance
    private float P; // Estimate error covariance
    private float X; // Value

    public KalmanFilter(float q, float r, float p, float initialValue)
    {
        Q = q;
        R = r;
        P = p;
        X = initialValue;
    }

    public float Update(float measurement)
    {
        // Prediction update
        P = P + Q;

        // Measurement update
        float K = P / (P + R);
        X = X + K * (measurement - X);
        P = (1 - K) * P;

        return X;
    }

    public float Predict()
    {
        // Simple prediction step
        P = P + Q;
        return X;
    }
}
