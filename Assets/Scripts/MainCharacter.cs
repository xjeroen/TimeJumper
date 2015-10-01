﻿using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System.Linq;
using UnityEngine.UI;

public class MainCharacter : MonoBehaviour {

    float previousClickTime = 0;
    public float clickRate = 0.40f;
    public float speedModifier = 1f;
    float lastButtonPresstime = 0;
    int currentTime = 0;

    public int defaultJumps;
    private int jumps;
    private float downTime;
    private Swipe currentSwipe = new Swipe();
    Rigidbody2D rigid;
    Animator animator;

    public void ResetJumps()
    {
        jumps = defaultJumps;
    }

	// Use this for initialization
	void Start ()
    {
        Input.simulateMouseWithTouches = false;
        jumps = defaultJumps;
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void processInput()
    {

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                currentSwipe.Enabled = true;

                Vector3 test = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));


                currentSwipe.xDirectionStart = test.x;
                currentSwipe.yDirectionStart = test.y;
                currentSwipe.xDirectionEnd = test.x;
                currentSwipe.yDirectionEnd = test.y;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 test = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

                currentSwipe.xDirectionEnd = test.x;
                currentSwipe.yDirectionEnd = test.y;

                currentSwipe.xDirectionDelta = currentSwipe.xDirectionEnd - currentSwipe.xDirectionStart;
                currentSwipe.yDirectionDelta = currentSwipe.yDirectionEnd - currentSwipe.yDirectionStart;

                if (currentSwipe.xDirectionDelta < 0.25f && currentSwipe.xDirectionDelta > -0.25f && currentSwipe.yDirectionDelta < 0.25f && currentSwipe.yDirectionDelta > -0.25f)
                {
                    currentSwipe.xDirectionDelta = 0;
                    currentSwipe.yDirectionDelta = 0;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector3 test = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

                currentSwipe.xDirectionEnd = test.x;
                currentSwipe.yDirectionEnd = test.y;

                currentSwipe.xDirectionDelta = currentSwipe.xDirectionEnd - currentSwipe.xDirectionStart;
                currentSwipe.yDirectionDelta = currentSwipe.yDirectionEnd - currentSwipe.yDirectionStart;

                if (currentSwipe.xDirectionDelta < 0.25f && currentSwipe.xDirectionDelta > -0.25f && currentSwipe.yDirectionDelta < 0.25f && currentSwipe.yDirectionDelta > -0.25f)
                {
                    currentSwipe.Tap = true;
                    currentSwipe.xDirectionDelta = 0;
                    currentSwipe.yDirectionDelta = 0;
                }
            }
        }

        if (Input.touches.Length == 0)
        {
            currentSwipe.Reset();
        }
    }

    void processHorizontalPerspective()
    {
        if (rigid.transform.localPosition.y >= 0.765f - float.Epsilon && rigid.transform.localPosition.y <= 0.765f + float.Epsilon)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            jumps = defaultJumps;
        }
        
        if (!GameOverAnimation.GetInstance().m_fAnimationInProgress)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            if ((Input.GetMouseButtonDown(0) || (currentSwipe.Enabled && currentSwipe.Tap)) && (Time.time > previousClickTime + (clickRate / Global.Instance.speed)))
            {
                if (jumps > 0 || jumps == -1)
                {
                    previousClickTime = Time.time;
                    rigid.velocity = new Vector2(0, 0);
                    rigid.AddForce(new Vector2(0, 150.0f * 1.5f));

                    if (jumps >= 0)
                    {
                        if ((!GameObject.Find("Resource Manager").GetComponent<ResourceManager>().inVehicle) && ((GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MainCharacter")) || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Jump")))
                            GetComponent<Animator>().Play("Jump", 0, 0);

                        jumps--;
                    }
                }
            }
        }
    }

    void processVerticalPerspective()
    {
        if (!GameOverAnimation.GetInstance().m_fAnimationInProgress)
        {
            if (!(currentSwipe.Enabled || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
                return;

            if (currentSwipe.Enabled)
            {
                rigid.velocity = new Vector2(0, 0);
                if (currentSwipe.yDirectionEnd < 3.1f)
                {
                    rigid.AddForce(new Vector2(0, -100f));
                }
                else
                {
                    rigid.AddForce(new Vector2(0, 100f));
                }
            }
            else
            {
                rigid.velocity = new Vector2(0, 0);
                rigid.AddForce(new Vector2(0, (Input.GetMouseButtonDown(0) ? 1 : -1) * 100.0f));
            }
        }
    }

    void keepWithinScreen()
    {
        if (GameOverAnimation.GetInstance().m_fAnimationInProgress)
        {
            if (Global.Instance.orientation == 0)
                rigid.transform.localPosition = new Vector3(Mathf.Clamp(rigid.transform.localPosition.x, -6.35f, 4.1f), Mathf.Clamp(rigid.transform.localPosition.y, 0.4f, 6.45f), rigid.transform.localPosition.z);
            else
                rigid.transform.localPosition = new Vector3(Mathf.Clamp(rigid.transform.localPosition.x, -4.35f, 4.1f), Mathf.Clamp(rigid.transform.localPosition.y, -0.19f, 6.45f), rigid.transform.localPosition.z);
        }
        else
        {
            if (Global.Instance.orientation == 0)
                rigid.transform.localPosition = new Vector3(Mathf.Clamp(rigid.transform.localPosition.x, -4.35f, 4.1f), Mathf.Clamp(rigid.transform.localPosition.y, 0.765f, 6.45f), rigid.transform.localPosition.z);
            else
                rigid.transform.localPosition = new Vector3(Mathf.Clamp(rigid.transform.localPosition.x, -4.35f, 4.1f), Mathf.Clamp(rigid.transform.localPosition.y, -0.19f, 6.45f), rigid.transform.localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.Instance == null)
            return;

        if (animator != null)
            animator.speed = (Global.Instance.speed < 0.0f ? 0.0f : Global.Instance.speed / speedModifier);

        processInput();

        SpawnManager.Instance.platformManager.UpdateActive(currentSwipe, transform, Input.GetKey(KeyCode.DownArrow));

        currentTime++;
        if (currentTime > int.MaxValue - short.MaxValue)
        {
            currentTime = 0;
            lastButtonPresstime = 0;
        }
        
        if(lastButtonPresstime + 10 < currentTime)
        {
            if (currentSwipe.Enabled && currentSwipe.xDirectionEnd > 12.0f && currentSwipe.xDirectionEnd < 12.8f && currentSwipe.yDirectionEnd > 6.45f)
            {
                lastButtonPresstime = currentTime;
                GameObject.Find("Global").GetComponent<Global>().PauseButton();
            }
            else if (currentSwipe.Enabled && currentSwipe.xDirectionEnd > 12.8f && currentSwipe.yDirectionEnd > 6.45f)
            {
                lastButtonPresstime = currentTime;
                GameObject.Find("Global").GetComponent<Global>().AudioButton();
            }
        }

        if (Global.Instance.orientation == 0)
            processHorizontalPerspective();
        else
            processVerticalPerspective();

        keepWithinScreen();
    }
}
