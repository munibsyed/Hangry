using UnityEngine;
using System.Collections;

public class FoodPickup : MonoBehaviour {

	public float m_angerAmount = 0.1f;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
            if (tag == "GoodFood")
			    other.GetComponent<AngerScript>().addAnger(-m_angerAmount);
            if (tag == "BadFood")
                other.GetComponent<AngerScript>().addAnger(m_angerAmount);

            Destroy(gameObject);
		}
	
	}
}
