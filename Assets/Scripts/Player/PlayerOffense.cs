using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOffense : MonoBehaviour {

    //Modifiable Stats
    public string Melee = "MeleeP1";
    public string Ability1 = "Ability1P1";
    public string Ability2 = "Ability2P1";

    private float AttackSpeed;
    private float MultistrikeTimes;
    private bool MultiStrike = false;
    private float AoRRange = 0;
    private float EnergyCostReduction = 0;
    private float StatusEffectChance = 0;
    private float LifeSteal = 0;
    private float ProjectileSpeed = 0;
    private float DamageIgnoreChance = 0;
    public bool GroundPoundAttack = false;
    private float GroundPoundDamage = 15f;
    public int PlayerNumber = 1;

    private float attackPause = 0;
    private float attackPauseTimer = 0.5f;


    private GameObject MatchController;
    PlayerMovement movementScript;
    CharacterController Controller;
    PlayerHealth PlayerHP;


    private float radius = 7.5f;

    // Use this for initialization
    void Start () {
        movementScript = GetComponent<PlayerMovement>();
        Controller = GetComponent<CharacterController>();
        PlayerHP = GetComponent<PlayerHealth>();
        MatchController = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.isGrounded == false && Input.GetButtonDown(Melee))
        {
            GroundPoundAttack = true;
            attackPause = 0;
            GetComponent<PlayerMovement>().GroundPoundactive = true;
        }
        if (GroundPoundAttack == true && GetComponent<PlayerMovement>().GroundPoundMove == false)
        {
            if (attackPause <= attackPauseTimer)
            {
                attackPause += 1 * Time.deltaTime;
                GetComponent<PlayerMovement>().VerticalVelocity = 0;
                GetComponent<PlayerMovement>().StopFall = true;

            }
            else
            {
                GetComponent<PlayerMovement>().GroundPoundMove = true;
                GetComponent<PlayerMovement>().StopFall = false;
               
            }
        }
        if (GroundPoundAttack == true && Controller.isGrounded == true)
        {
            GroundPound(transform.position, radius);
            GroundPoundAttack = false;
        }

        if (GetComponent<PlayerMovement>().GroundPoundactive == true && GetComponent<PlayerMovement>().StopFall == false)
        {
            attackPause = 0;
            GetComponent<PlayerMovement>().VerticalVelocity -= 2f;
            CencelGroundPound();
            RaycastHit hit;
            Ray rayForward = new Ray(transform.position, transform.forward);

            Debug.DrawRay(transform.position, transform.forward);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Physics.Raycast(rayForward, out hit) && hit.collider.CompareTag("Floor") && hit.distance < 1 && Controller.isGrounded == false)
            {
                GetComponent<PlayerMovement>().GroundPoundMove = false;
            }
        }
        
    }
    //Ground Pound Attack
    void GroundPound(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Is the object another player and not the caster.
            if (hitColliders[i].CompareTag("Player") && !(hitColliders[i].gameObject == gameObject))
            {
                //Gonna Change to a less intensive method. needs to send a value as well.
                hitColliders[i].GetComponent<PlayerHealth>().AddDamage(GroundPoundDamage,this.gameObject);

                hitColliders[i].GetComponent<PlayerHealth>().transform.Translate(Vector3.forward);
                if (hitColliders[i].GetComponent<PlayerHealth>().Health <= 0)
                {
                    switch (PlayerNumber)
                    {
                        //If Game is 2 Player
                        default:
                            {
                                break;
                            } 
                        case 1:
                            {
                                MatchController.GetComponent<GameController>().UpdateScore(1,(1* MatchController.GetComponent<GameController>().Score_Modifier));
                                break;
                            } 
                        case 2:
                            {
                                MatchController.GetComponent<GameController>().UpdateScore(2, (1 * MatchController.GetComponent<GameController>().Score_Modifier));
                                break;
                            }
                        case 3:
                            {
                                MatchController.GetComponent<GameController>().UpdateScore(3, (1 * MatchController.GetComponent<GameController>().Score_Modifier));
                                break;
                            }
                        case 4:
                            {
                                MatchController.GetComponent<GameController>().UpdateScore(4, (1 * MatchController.GetComponent<GameController>().Score_Modifier));
                                break;
                            } 
                    }
                    
                }

            }
            //Next in Array of objects in Sphere Overlay
            i++;
        }
    }
    //Ground Pound Attack
    void CencelGroundPound()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Is the object another player and not the caster.
            if (hitColliders[i].CompareTag("Floor") && Controller.isGrounded == false)
            {
                GetComponent<PlayerMovement>().GroundPoundMove = false;
                GetComponent<PlayerMovement>().StopFall = false;
                attackPause = 0;
                GroundPoundAttack = false;
            }
            //Next in Array of objects in Sphere Overlay
            i++;
        }
    }
    private void OnDrawGizmos()
    {
         Gizmos.DrawWireSphere(transform.position, radius); 
    }
}