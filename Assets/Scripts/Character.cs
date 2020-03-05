using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float XAxis = 0f;
    private float YAxis = 0f;
    private float walkingspeed = 1.5f;


    Animator anim;


    /// <summary>
    /// This class will be for managing the character movement
    /// The character can move only forward or rotate, Otherwise it's too eas
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        this.transform.Translate(XAxis * Time.deltaTime * walkingspeed, 0, YAxis * Time.deltaTime * walkingspeed);
        //transform.position += transform.forward * velocity * Time.deltaTime;
    }


    public void GetInput()
    {
        //XAxis = Input.GetAxis("Horizontal");
        YAxis = Input.GetAxis("Vertical");
        if(YAxis < 0)
        {
            this.transform.Rotate(0, 3, 0);
        }

        //anim.SetFloat("BlendX", XAxis);
        anim.SetFloat("BlendY", YAxis);
    }





}
