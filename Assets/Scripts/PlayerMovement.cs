using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 fingerPos;
    private Vector3 direction;
    float roadSize;
    float sideSpeed;
    float forwardMaxSpeed;
    float forwardSlowedSpeed;
    float timeSlowed;
    private float inGamePos;
    private float forwardActualSpeed;


    private float screenWidth;
    private Rigidbody rb;

    private bool hitWall = false;

    private Animator anim;
    private float speedAnim = 1;

    private bool stopInput = false;

    private void Awake()
    {
        TextAsset file = Resources.Load<TextAsset>("PlayerAndEnemyData");
        string json = file.ToString();
        PlayerData loadedPlayerData = JsonUtility.FromJson<PlayerData>(json);
        roadSize = loadedPlayerData.roadSize;
        sideSpeed = loadedPlayerData.sideSpeed;
        forwardMaxSpeed = loadedPlayerData.forwardMaxSpeedPlayer;
        forwardSlowedSpeed = loadedPlayerData.forwardSlowedSpeedPlayer;
        timeSlowed = loadedPlayerData.timeSlowed;
    }

    private void Start()
    {
        screenWidth = Screen.width;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetFloat("Speed", speedAnim);
        forwardActualSpeed = forwardMaxSpeed;
    }


    void Update()
    {
        //if the game ends, stop input is true and will ignore everything
        if (stopInput)
            return;

        //for movement, see documentation Movement.txt
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchX = touch.position.x;
            inGamePos = Scale(0f, screenWidth, -roadSize, roadSize, touchX);

            if (touch.phase == TouchPhase.Ended)
            {
                inGamePos = 0f;
            }
        }

        // Mouse controller
        // --------------------------------------------------------------------

        if (Input.GetMouseButton(0))
        {
            fingerPos = Input.mousePosition;
            inGamePos = Scale(0f, screenWidth, -roadSize, roadSize, Input.mousePosition.x);            

        }

        if (Input.GetMouseButtonUp(0))
        {
            inGamePos = 0f;
        }

        // -----------------------------------------------------------

        //if the player hits a wall
        //slows him down and slows also the running animation
        if (hitWall)
        {

            hitWall = false;
            forwardActualSpeed = forwardSlowedSpeed;
            speedAnim = Scale(0, forwardMaxSpeed, 0, 1, forwardActualSpeed);
            anim.SetFloat("Speed", speedAnim);
            StartCoroutine(SpeedUp());
        }

    }

    private void FixedUpdate()
    {
        if (stopInput)
            return;

        
        Vector3 forwardMove = -transform.forward * forwardActualSpeed * Time.fixedDeltaTime;
        float horPos;
        if (inGamePos != 0)
        {
            horPos = inGamePos - transform.position.x;
        }
        else
        {
            horPos = 0;
        }
        Vector3 horizontalMove = (new Vector3(horPos, 0f, 0f)) * sideSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove + horizontalMove);
    }

    public float Scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

         float OldRange = (OldMax - OldMin);
         float NewRange = (NewMax - NewMin);
         float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

         return (NewValue);
    }

    IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(timeSlowed);
        forwardActualSpeed = forwardMaxSpeed;
        speedAnim = Scale(0, forwardMaxSpeed, 0, 1, forwardActualSpeed);
        anim.SetFloat("Speed", speedAnim);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            hitWall = true;
        }
    }

    //if the game ends, the player stops and change animation
    public void GameEndedPlayer(bool isWin)
    {
        stopInput = true;
        rb.MovePosition(transform.position);
        if (isWin)
        {
            anim.SetTrigger("Win");
        }
        else
        {
            anim.SetTrigger("IsDead");
        }
    }

    private class PlayerData
    {
        public float roadSize;
        public float sideSpeed;
        public float forwardMaxSpeedPlayer;
        public float forwardSlowedSpeedPlayer;
        public float timeSlowed;
    }
}
