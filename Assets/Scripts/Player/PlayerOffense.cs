using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOffense : MonoBehaviour {

    //Modifiable Stats
    private float AttackSpeed;
    private float MultistrikeTimes;
    private bool MultiStrike = false;
    private float AoRRange = 0;
    private float EnergyCostReduction = 0;
    private float StatusEffectChance = 0;
    private float LifeSteal = 0;
    private float ProjectileSpeed = 0;
    private float DamageIgnoreChance = 0;
    private bool GroundPound = false;

    PlayerMovement movementScript;
    CharacterController Controller;

    private float radius = 7.5f;

    // Use this for initialization
    void Start () {
        movementScript = GetComponent<PlayerMovement>();
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementScript.DJump == true && Input.GetButtonDown(movementScript.Jump))
        {
            GroundPound = true;
        }

        if (GroundPound == true && Controller.isGrounded == true)
        {

        }
        if (Input.GetButtonDown(movementScript.Jump))
        {
            GroundPoundDamage(transform.position, radius);
        }
    }
    //Ground Pound Attack
    void GroundPoundDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        while (i < hitColliders.Length)
        {

            if (hitColliders[i].CompareTag("Player") && !(hitColliders[i].gameObject == gameObject))
            {
                //Gonna Change to a less intensive method. needs to send a value as well.
                hitColliders[i].SendMessage("AddDamage");
            }
            
            i++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
