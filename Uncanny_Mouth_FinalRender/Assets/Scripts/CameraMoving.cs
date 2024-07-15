using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    Animator animator;

    private float duringTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        IsStart(false);
        IsEnd(false);
    }

    public void IsStart(bool turth)
    {
        //a_Animator.SetBool("is_sleeping", turth);
        animator.SetBool("IsStart", turth);
    }

    public void IsEnd(bool turth)
    {
        //a_Animator.SetBool("is_sleeping", turth);
        animator.SetBool("IsEnd", turth);

    }
    // Update is called once per frame
    void Update()
    {
        duringTime += Time.deltaTime;
        if (duringTime > 3) {
            IsStart(true);
        }
    }
}
