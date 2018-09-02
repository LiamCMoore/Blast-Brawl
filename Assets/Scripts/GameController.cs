using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public int P1_Score;
    public int P2_Score;
    public int P3_Score;
    public int P4_Score;

    public int Score_Modifier;
    public int Penalties;

    public float RespawnTimer;
    public float RespawnTimerMax;
    PreGameSetup playerCount;
    // Use this for initialization
    void Start () {
        playerCount = GetComponent<PreGameSetup>();
    }
	
    void UpdateScore()
    {

    }
	// Update is called once per frame
	void Update () {
		

    }
}
