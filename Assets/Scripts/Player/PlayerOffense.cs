using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOffense : MonoBehaviour {

    //Modifiable Stats
    public string Melee = "MeleeP1";
    public string GroundPound = "GroundPoundP1";
    public string Ability1 = "Ability1P1";
    public string Ability2 = "Ability2P1";

    private float AttackSpeed;
    private float MultistrikeTimes;
    private int MultiStrikeLevel = 0;
    private float meleeDistance = 5f; //Test, will remove
    private float AoRRange = 0;
    private float EnergyCostReduction = 0;
    private float StatusEffectChance = 0;
    private float LifeSteal = 0;
    private float ProjectileSpeed = 0;
    private float DamageIgnoreChance = 0;
    


    //Damage Values
    private float GroundPoundDamage = 15f;
    public bool GroundPoundAttack = false;

    //Righeous fire
    public float RighteousFireDamage = 2f;
    public float RighteousFireMultiDamage = 0.2f;
    private float RighteousFireMultiDamageNode = 1.1f;
    public float RighteousFireIFrames = 0f;
    public float RighteousFireIFramesMax = 1f;
    private int RighteousFireMultiStrike = 4;

    private float righteousFireMultiHittimer = 0;
    private float righteousFireMultiHittimerMax = 1f;

    public Vector3 rotation;
    public Quaternion q;


    public int PlayerNumber = 1;

    private float attackPause = 0;
    private float attackPauseTimer = 0.5f;


    private GameObject MatchController;
    PlayerMovement movementScript;
    CharacterController Controller;
    PlayerHealth PlayerHP;
    PlayerOffense PlayerAttack;


    private float radius = 7.5f;

    // Use this for initialization
    void Start ()
    {
        Quaternion q = Quaternion.AngleAxis(100 * Time.time, Vector3.up);
        Vector3 Rotation = transform.forward * 20;
        movementScript = GetComponent<PlayerMovement>();
        Controller = GetComponent<CharacterController>();
        PlayerHP = GetComponent<PlayerHealth>();
        PlayerAttack = GetComponent<PlayerOffense>();
        MatchController = GameObject.FindWithTag("GameController");
        
    }

    // Update is called once per frame
    void Update()
    {

        


        if (RighteousFireIFrames < RighteousFireIFramesMax)
        {
            RighteousFireIFrames += 1 * Time.deltaTime;
        }
        if (Input.GetAxis(Ability1) > 0.5)
        {
            RighteousFire(transform.position, radius);
        }
        if (Input.GetButtonDown(Melee))
        {
            print("Attack");
            RaycastHit Attack;
            Ray rayForward = new Ray(transform.position, transform.forward);

            Debug.DrawRay(transform.position, transform.forward);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Physics.Raycast(rayForward, out Attack) && Attack.collider.CompareTag("Player") && !(Attack.collider.gameObject == gameObject) && Attack.distance < meleeDistance)
            {
                Attack.collider.GetComponent<PlayerHealth>().AddDamage(50, this.gameObject);
            }
        }

            if (Controller.isGrounded == false && Input.GetButtonDown(GroundPound))
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
            GroundPoundAction(transform.position, radius);
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
    void GroundPoundAction(Vector3 center, float radius)
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

                hitColliders[i].GetComponent<PlayerHealth>().transform.Translate(this.transform.forward);
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


    void RighteousFire(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Is the object another player and not the caster.
            if (hitColliders[i].CompareTag("Player") && !(hitColliders[i].gameObject == gameObject))
            {
                if (hitColliders[i].GetComponent<PlayerOffense>().RighteousFireIFrames >= hitColliders[i].GetComponent<PlayerOffense>().RighteousFireIFramesMax)
                {
                    
                    //Gonna Change to a less intensive method. needs to send a value as well.
                    hitColliders[i].GetComponent<PlayerHealth>().AddDamage(RighteousFireDamage, this.gameObject);
                    print("Hit Fire");
                    hitColliders[i].GetComponent<PlayerOffense>().RighteousFireIFrames = 0;

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
                                    MatchController.GetComponent<GameController>().UpdateScore(1, (1 * MatchController.GetComponent<GameController>().Score_Modifier));
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

            }
            //Next in Array of objects in Sphere Overlay
            i++;
        }
        //Every Second, Spawn a MultiHit
        if (righteousFireMultiHittimer >= righteousFireMultiHittimerMax)
        {
            righteousFireMultiHittimer = 0;
            if (RighteousFireMultiStrike == 4)
            {
                RighteousFireMultiHit();
                RighteousFireMultiHit();
                RighteousFireMultiHit();
                RighteousFireMultiHit();
            }
            else if (RighteousFireMultiStrike ==3)
            {
                RighteousFireMultiHit();
                RighteousFireMultiHit();
                RighteousFireMultiHit();
            }
            else if (RighteousFireMultiStrike == 2)
            {
                RighteousFireMultiHit();
                RighteousFireMultiHit();
            }
            else if (RighteousFireMultiStrike == 1)
            {
                RighteousFireMultiHit();
            }
        }
        else
        {
            righteousFireMultiHittimer += 1 * Time.deltaTime;
        }
    }
    //Handles the Righteous Fire Multihit. Spawns a seperate Hitbox around the player at random.
    void RighteousFireMultiHit()
    {
        //rotation = Random.rotation.eulerAngles;
        Debug.DrawRay(transform.position, q * rotation, Color.green);
        RaycastHit Attack1;
        q = Quaternion.AngleAxis(100 * Time.time, Vector3.up);
        q.w = Random.Range(-1f, 1f);
        q.y = Random.Range(-1f, 1f);
        rotation = transform.forward * 20;
        if (Physics.SphereCast(transform.position, radius, q * rotation, out Attack1, 64) && Attack1.collider.CompareTag("Player") && !(Attack1.collider.gameObject == gameObject))
        {
            print("Attack");
            //Gonna Change to a less intensive method. needs to send a value as well.
            Attack1.collider.GetComponent<PlayerHealth>().AddDamage(RighteousFireMultiDamage * RighteousFireMultiDamageNode, this.gameObject);
            print("Hit Fire");
            Attack1.collider.GetComponent<PlayerOffense>().RighteousFireIFrames = 0;
            if (Attack1.collider.GetComponent<PlayerHealth>().Health <= 0)
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
                            MatchController.GetComponent<GameController>().UpdateScore(1, (1 * MatchController.GetComponent<GameController>().Score_Modifier));
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

    }
    private void OnDrawGizmos()
    {
         Gizmos.DrawWireSphere(transform.position, radius); 
    }
}