using UnityEngine;
using System.Collections;

public class DestroyGameobjects : MonoBehaviour {

	void OnTriggerEnter(Collider col)
    {
        Destroy(col.gameObject);
    }
}
