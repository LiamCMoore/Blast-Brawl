using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    //Public Variables - Easy to edit stats in editor.
    public float Forward_speed = 100.0f;
    public float MaxRun_Speed = 150.0f;
    public float Acceleration = 1f;
    public float SpeedSmoothTime = 0.1f;
    public float wallRunMax = 100;
    public Transform CameraT;

    public float gravity = 14.0f;
    public float jumpForce = 10.0f;
    public float fallMultiplier = 2.5f;
    public float lowjumpMultiplier = 2f;

    private float wallRunTimer = 0;
    
    //JumpPad jump Value
    private float JumpPadJump;



    private float VerticalVelocity;
    private bool hasJumped = false;
    private bool WallRun = false;
    private Vector3 movement = Vector3.zero;

    float turnSmoothVelocity;
    CharacterController Controller;

    private void Start()
    {
        //CameraT = Camera.main.transform;
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButton("Parkour") || Input.GetAxis("Parkour") > 0.5)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            RaycastHit hit;
            Ray rayRight = new Ray(transform.position, transform.right);
            Ray rayLeft = new Ray(transform.position, -transform.right);
            Debug.DrawRay(transform.position, transform.right);
            Debug.DrawRay(transform.position, -transform.right);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Physics.Raycast(rayRight, out hit) && hit.collider.CompareTag("Floor") && hit.distance < 1 && VerticalVelocity <= 0)
            {
                //movement = transform.right;
                WallRun = true;
            }
            else if (Physics.Raycast(rayLeft, out hit) && hit.collider.CompareTag("Floor") && hit.distance < 1 && VerticalVelocity <= 0)
            {
                //movement = transform.right;
                WallRun = true;
            }
            else
            {
                WallRun = false;
                print("No Hit");
                wallRunTimer = 0;
            }


        }
        else
        {
            WallRun = false;
            wallRunTimer = 0;
        }
        //Is the character grounded////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (Controller.isGrounded)
        {
            WallRun = false;
            hasJumped = false;
            //apply some gravity to ensure player sticks to grond
            VerticalVelocity = -gravity * Time.deltaTime;
            //Jump
            Jump(jumpForce);
        }
        else
        {
            if (WallRun == false)
            {

                if (VerticalVelocity < 0)
                {
                    VerticalVelocity -= (gravity * (fallMultiplier) * Time.deltaTime);
                }
                else
                {
                    VerticalVelocity -= gravity * Time.deltaTime;
                }
            }
            else
            {
                VerticalVelocity = 0;
            }
        
        }

        //if (Input.GetAxis("Parkour") > 0)
        //{
        //print(WallRun);
        //}


        //Character Movement//////////////////////////////////////////////////////////
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movement = input.normalized;

        movement = transform.TransformDirection(movement);
        //If no Input?
        if (movement != Vector3.zero)
        {
            //Increase acceleration for player. If smalller than max speed
            if (Forward_speed < MaxRun_Speed)
            {
                Forward_speed += Acceleration;
            }
            else
            {
                //Else limit speed to max speed
                Forward_speed = MaxRun_Speed;
            }
        }
        else
        {
            if (Forward_speed >0)
            {
                Forward_speed -= Acceleration;
            }
            else
            {
                //Else limit speed to max speed
                Forward_speed = 0;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //If WallRunning, Count down
        if (WallRun == true)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //print(wallRunTimer);
            if (wallRunTimer > wallRunMax)
            {
                WallRun = false;
                wallRunTimer = 0;
            }
            else
            {
                wallRunTimer += 1;
            }
            if (Input.GetButtonDown("Jump"))
            {
                WallRun = false;
                wallRunTimer = 0;
                Launch(jumpForce);
            }
            if (Input.GetButtonUp("Parkour"))
            {
                WallRun = false;
                wallRunTimer = 0;
            }
        }


        

        //Movement
        UpdateMovement();
    }
    //Updates Movement when called.
    void UpdateMovement()
    {
        if (WallRun == false)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, CameraT.localEulerAngles.y, transform.localEulerAngles.z);
        }
        //Get current Velocity from the current forward direction and verical movement
        //movement = transform.forward * Forward_speed + Vector3.up * VerticalVelocity;
        Vector3 NewMove =  movement * Forward_speed + Vector3.up * VerticalVelocity;
        //Move the player via Velocity
        Controller.Move(NewMove * Time.deltaTime);

    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }
    void OnTriggerStay(Collider other)
    {
        print("Collision");
        if (other.gameObject.tag == "JumpPad")
        {
            if (Controller.isGrounded)
            {
                hasJumped = false;
                Launch(other.gameObject.GetComponent<JumpPadValues>().JumpPadForce);
            }
        }
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
            
    }

    //Jump Code
    void Jump(float JumpVal)
    {
        if (Input.GetButtonDown("Jump"))
        {
            hasJumped = true;
            VerticalVelocity = JumpVal;
        }

    }

    void Launch(float JumpVal)
    {

        hasJumped = true;
        VerticalVelocity = JumpVal * (lowjumpMultiplier);
        UpdateMovement();
    }
}