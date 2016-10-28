using UnityEngine;
using System.Collections;
using System.Linq;

public class ObstaclePushback : MonoBehaviour {

    private Vector3 goalPos;
    public float pushbackDistance;
   
    
	// Use this for initialization
	void Start () {
        goalPos = transform.localPosition - new Vector3(0, 0, 15);
        goalPos = transform.TransformPoint(goalPos);
	}
	
	
    void OnCollisionEnter(Collision col) {

        if (col.gameObject.CompareTag("Player"))
        {
            //recalculate goalPos to always be on the ground (leftTransform, middleTransform or rightTransform)

            //goalPos = col.gameObject.transform.position - (col.gameObject.transform.forward * pushbackDistance);

            //goalPos = col.gameObject.GetComponent<PlayerMovementDuncan>().AllTransforms.transform.position - col.gameObject.GetComponent<PlayerMovementDuncan>().AllTransforms.transform.forward * pushbackDistance;
            //goalPos = new Vector3(goalPos.x, 0.0f, goalPos.z);
            //Debug.Log(goalPos);

            PlayerMovementDuncan playerMovement = col.gameObject.GetComponent<PlayerMovementDuncan>();

            playerMovement.PushbackPos = playerMovement.allTransforms.transform.position - ((col.transform.forward) * pushbackDistance);
            playerMovement.PushbackPos = new Vector3(playerMovement.PushbackPos.x, 0, playerMovement.PushbackPos.z);
            playerMovement.MovementState = PlayerMovementDuncan.MovementStates.PUSHED_BACK;

        }

    }
}
