﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Crash : MonoBehaviour {

    private Rigidbody2D rigid;
    private Vector3 startLocation = new Vector3(3f, -0.215f, 1f);
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.transform.localPosition += Global.Instance.GlobalObject.transform.localPosition + Global.Instance.ForegroundObject.transform.localPosition;
        rigid.transform.parent = Global.Instance.ForegroundObject.transform;
    }

    // Update is called once per frame
    void Update () {
        if (rigid.transform.localPosition.x <= -8.0f)
        {
            rigid.velocity = new Vector2(0, 0);
            rigid.transform.localPosition = new Vector3(-12, 0, 0);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Main Character" && Time.time - Global.startTime > 30f)
        {
            Global.Instance.Gesture.SetActive(true);
            rigid.velocity = new Vector2(0, 0);
            rigid.transform.localPosition = new Vector3(-12, 0, 0);
            PerspectiveInitializer.s_Instance.CleanPerspective();
            Fader.s_Instance.InvokeMethod("Enable", 0.1f);
            PerspectiveInitializer.s_Instance.InvokeMethod("LoadVerticalPerspective", 0.1f);
            Fader.s_Instance.InvokeMethod("Disable", 1f);
            PerspectiveInitializer.s_Instance.Invoke("disableGesture", 2.5f);
            Global.Instance.IncreaseDistance(500);
        }
        else if(Time.time - Global.startTime > 30f)
        {
            if (rigid.transform.localPosition.x > -6.6f && other.gameObject.name != "Main Character" && other.gameObject.GetComponent<ObstacleBase>() != null)
            {
                other.gameObject.GetComponent<ObstacleBase>().Disable();
            }
        }
    }

    public void Trigger()
    {
        rigid.transform.localPosition = startLocation;
        rigid.velocity = new Vector2(-3.25f, 0);
        rigid.GetComponent<Animator>().Play("Crash");
    }
}
