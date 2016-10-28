using UnityEngine;
using System.Collections;

public class PushbackTransform : MonoBehaviour {

    private Vector3 position; //the position the player will be taken to
    private float playerSpeed;

    public Vector3 Position
    {
        get
        {
            return position;
        }
    }

	// Use this for initialization
	void Start () {
        position = transform.position;
        playerSpeed = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementDuncan>().speed;
    }
	
	// Update is called once per frame
	void Update () {
	    //I want to always keep track of the correct x position (locally)
	}
}
