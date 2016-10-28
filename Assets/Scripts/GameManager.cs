using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject mananger = new GameObject("GameManager");
                mananger.AddComponent<GameManager>();
            }
            return instance;
        }

    }


    public float timer;
    public float angerLevel;
    public int pointBuff;
    private System.TimeSpan finalTime;

    public float playerDistanceTravelled;
    public bool hasWon;

    private GameObject player;
     


	// Use this for initialization
	void Awake () {
        instance = this;
        timer = 0;
        pointBuff = 0;
        hasWon = false;
        playerDistanceTravelled = 0;
	}

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (hasWon == false)
        {
            timer += Time.deltaTime;
        }
    }

    public void DisplayWinState()
    {
        hasWon = true;   
        Debug.Log("Your time was: " + System.TimeSpan.FromSeconds(timer));
        int score = (int)(1000000 / timer);
        Debug.Log("Your score was: " + score);
        float bonus = (-2 * player.GetComponent<AngerScript>().m_anger) + 2;
        bonus *= 200;
        Debug.Log("Final anger bonus: " + (int)bonus);
        Debug.Log("Bonus points for staying happy: " + pointBuff);

        score += (int)bonus;
        score += pointBuff;
        Debug.Log("Final score: " + score);

    }
	
	
}
