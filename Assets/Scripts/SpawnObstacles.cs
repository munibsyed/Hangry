using UnityEngine;
using System.Collections;

public class SpawnObstacles : MonoBehaviour {

    public GameObject obstacle;
    public int obstacleSpawnFrequency;
    public int coolDownDistance;

    private Transform playerLeftTransform;
    private Transform playerMiddleTransform;
    private Transform playerRightTransform;

    private GameObject lastBlock;
    private Vector3 lastPos;

    private float boomDistance;


    // Use this for initialization
    void Start () {
       
        playerLeftTransform = GameObject.FindGameObjectWithTag("PlayerLeftTransform").transform;
        playerMiddleTransform = GameObject.FindGameObjectWithTag("PlayerMiddleTransform").transform;
        playerRightTransform = GameObject.FindGameObjectWithTag("PlayerRightTransform").transform;

        boomDistance = (transform.position - playerMiddleTransform.position).magnitude;
        int randomInt = Random.Range(0, 3);
        Vector3 obstaclePos = Vector3.zero;
        if (randomInt == 0)
        {
            obstaclePos = playerLeftTransform.position;
        }

        if (randomInt == 1)
        {
            obstaclePos = playerMiddleTransform.position;
        }

        if (randomInt == 2)
        {
            obstaclePos = playerRightTransform.position;
        }

        obstaclePos += transform.forward * boomDistance;
        lastBlock = (GameObject)Instantiate(obstacle, obstaclePos, Quaternion.identity);
        lastPos = lastBlock.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        if (GameManager.Instance.hasWon == false)
        {
            //Debug.DrawRay(lastBlock.transform.position, Vector3.up*100, Color.red);

            //if distance between spawner and last obstacle is greater than the minimum cool down distance and if the spawner is in front of the last obstacle (for when you get pushed back significantly)
            if ((lastPos - transform.position).magnitude >= coolDownDistance && 
                (transform.position - playerMiddleTransform.transform.position).magnitude > (lastPos - playerMiddleTransform.transform.position).magnitude)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit))
                {
                    if (hit.collider.gameObject.CompareTag("TrackSegment"))
                    {
                        if (Random.Range(0, 1000) < obstacleSpawnFrequency)
                        {
                            int randomInt = Random.Range(0, 3);
                            Vector3 obstaclePos = Vector3.zero;
                            if (randomInt == 0)
                            {
                                obstaclePos = playerLeftTransform.position;
                            }

                            if (randomInt == 1)
                            {
                                obstaclePos = playerMiddleTransform.position;
                            }

                            if (randomInt == 2)
                            {
                                obstaclePos = playerRightTransform.position;
                            }

                            obstaclePos += transform.forward * boomDistance;
                         
                            lastBlock = (GameObject)Instantiate(obstacle, obstaclePos, Quaternion.identity);
                            lastPos = lastBlock.transform.position;
                            

                        }
                    }
                }
            }
        }
	}
}
