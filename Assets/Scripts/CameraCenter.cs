using UnityEngine;
using System.Collections;

public class CameraCenter : MonoBehaviour {

    public GameObject target;
    private Vector3 boom;


   
	// Use this for initialization
	void Start () {
        boom = (target.transform.position - transform.position);
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = target.transform.position - boom;
	}
}
