using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyTank : MonoBehaviour {

    public float EnergyGain = 50;
    public float RespawnTimeMax = 400;

    private float RespawnTime = 1200;
    private Vector3 StartPosition;
    // Use this for initialization
    void Start()
    {
        StartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (RespawnTime < RespawnTimeMax)
        {
            RespawnTime++;
        }
        else
        {
            transform.position = StartPosition;
            RespawnTime = RespawnTimeMax;
        }
    }
    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.position = new Vector3(9990.0f, 9999.0f, 9990.0f);
            RespawnTime = 0;
        }
    }
}
