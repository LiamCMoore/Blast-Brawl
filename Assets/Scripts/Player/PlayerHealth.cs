using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    public float MaxHealth = 100;
    public float Health = 100;
    public float Armour = 0;
    public float ArmourMax = 200;
    public float Mana = 100;
    public float ManaMax = 100;
	
	// Update is called once per frame
	void Update ()
    {
        //If Health is Larger than the maximum amount of Health, deplete health slowly back to the max
        if (Health > MaxHealth)
        {
            Health -= 1 * Time.deltaTime;
        }
        //If Armour is Larger than the maximum amount of Armour, deplete Armour slowly back to the max
        if (Armour > ArmourMax)
        {
            Armour -= 1 * Time.deltaTime;
        }
        //If Mana is Larger than the maximum amount of Mana, deplete Mana slowly back to the max
        if (Mana > ManaMax)
        {
            Mana -= 1 * Time.deltaTime;
        }

    }
    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <=0)
        {
            Destroy(this);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Health")
        {
            Health += (other.gameObject.GetComponent<HealthBoost>().HealthGain);
        }

        if (other.gameObject.tag == "Armour")
        {
            Armour += (other.gameObject.GetComponent<ArmourBoost>().ArmourGain);
        }

        //Call Damage Check Here. 
        //If HP <= 0, Player Die.
        //Give killer point
    }
}
