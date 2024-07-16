using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class BinocularCameraManager : MonoBehaviour
{
    // Static instance to hold the singleton instance
    private static BinocularCameraManager _instance;

    // Public static property to access the instance
    public static BinocularCameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Find the instance in the scene if it's not already set
                _instance = FindObjectOfType<BinocularCameraManager>();

                if (_instance == null)
                {
                    Debug.LogError("No instance of SomeComponent found in the scene.");
                }
            }
            return _instance;
        }
    }


    public GameObject rightCamera;
    public GameObject leftCamera;

    public GameObject targetRepresentation;

    public Vector3 target_position_real = new Vector3(0, 0, 0);

    private Vector3 unit_vector;
    private Vector3 target_position_game = new Vector3(0, 0, 0);
    private Vector3 camera_center;


    // Start is called before the first frame update
    void Start()
    {
        calculateUnit();
    }

    void calculateUnit()
    {
        unit_vector = (rightCamera.transform.localPosition - leftCamera.transform.localPosition) / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // calc unit length
        calculateUnit(); 

        // multiply game coord scale
        target_position_game = target_position_real * unit_vector.magnitude;

        // get center of camera
        camera_center = (rightCamera.transform.localPosition + leftCamera.transform.localPosition) / 2;

        // calculate targets game world coord 
        target_position_game = camera_center + target_position_game;

        targetRepresentation.transform.localPosition = target_position_game;
    }
}
