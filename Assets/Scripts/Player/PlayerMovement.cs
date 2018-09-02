using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Control Set
    public string Horizontal = "HorizontalP1";
    public string Vertical = "VerticalP1";
    public string Parkour = "ParkourP1";
    public string Jump = "JumpP1";
    //public string Horizontal = "HorizontalP1";

    //Public Variables - Easy to edit stats in editor.
    public float Forward_speed = 100.0f;
    public float MaxRun_Speed = 6.0f;
    public float Acceleration = 0.5f;
    public float wallRunMax = 100;
    public Transform CameraT;
    public float gravity = 14.0f;
    public float jumpForce = 10.0f;
    public float fallMultiplier = 2.5f;
    public float lowjumpMultiplier = 2f;


    public int speed;

    //Private Variabler
    private Vector3 inputControls;
    private float wallRunTimer = 0;
    private bool DJump = false;
    private float JumpPadJump;
    private float VerticalVelocity;
    private bool WallRun = false;
    private Vector3 movement = Vector3.zero;



    //Player Stat Changes
    private float JumpAccend = 0;
    private float GravityNegation = 0;
    



    float turnSmoothVelocity;
    CharacterController Controller;

    private void Start()
    {
        inputControls = new Vector3(0, 0, 0);
        //CameraT = Camera.main.transform;
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButton(Parkour) || Input.GetAxis(Parkour) > 0.5)
        {
            MaxRun_Speed = 20.0f;
            print("Parkour Held");
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
                DJump = false;
                WallRun = true;
            }
            else if (Physics.Raycast(rayLeft, out hit) && hit.collider.CompareTag("Floor") && hit.distance < 1 && VerticalVelocity <= 0)
            {
                //movement = transform.right;
                WallRun = true;
                DJump = false;
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
            MaxRun_Speed = 15.0f;
        }
        //Is the character grounded////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        if (Controller.isGrounded)
        {
            WallRun = false;
            DJump = false;
            //apply some gravity to ensure player sticks to grond
            VerticalVelocity = -gravity * Time.deltaTime;
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

        //Jump
        JumpAction(jumpForce);


        //Character Movement//////////////////////////////////////////////////////////

        Vector3 input = new Vector3(Input.GetAxis(Horizontal), 0, Input.GetAxis(Vertical));
        

        if (input != Vector3.zero)
        {
            inputControls = input;
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
            if (Forward_speed > 0)
            {
                Forward_speed -= Acceleration;
            }
            else
            {
                //Else limit speed to max speed
                Forward_speed = 0;
            }
        }
        movement = inputControls.normalized;
        movement = transform.TransformDirection(movement);
        //If no Input?
        if (movement != Vector3.zero)
        {
            
           
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
            if (Input.GetButtonDown(Jump))
            {
                WallRun = false;
                wallRunTimer = 0;
                Launch(jumpForce);
            }
            if (Input.GetButtonUp(Parkour) || Input.GetAxis(Parkour) < 0.5)
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
            Launch(other.gameObject.GetComponent<JumpPadValues>().JumpPadForce);
        }
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }     
    }


    private void OnCollisionStay(Collision collision)
    {
        
    }

    //Jump Code
    void JumpAction(float JumpVal)
    {
        if (Input.GetButtonDown(Jump) && Controller.isGrounded == true)
        {
            VerticalVelocity = JumpVal;
        }
        if (Input.GetButtonDown(Jump) && (DJump == false) && Controller.isGrounded == false)
        {
            DJump = true;
            VerticalVelocity = JumpVal;
        }

    }

    void Launch(float JumpVal)
    {
        VerticalVelocity = JumpVal * (lowjumpMultiplier);
        UpdateMovement();
    }
}