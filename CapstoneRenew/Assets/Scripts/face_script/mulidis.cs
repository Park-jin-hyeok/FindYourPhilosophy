using UnityEngine;

public class ActivateAllDisplays : MonoBehaviour
{
    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);

        for (int i = 1; i < Display.displays.Length; i++)
        {
            //???¡À??? ?????? ?¡Æ? active
            Display.displays[i].Activate();
        }
    }

    void Update()
    {

    }
}