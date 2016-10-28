using UnityEngine;
using System.Collections;

public class SpawnFood : MonoBehaviour {

    public GameObject goodFood;
    public GameObject badFood;
    public int foodSpawnFrequency;
    public int coolDownDistance;
    public float foodSpawnHeight;
    private Transform playerLeftTransform;
    [Tooltip("If anger is at the max, spawn more good food")]
    public bool balanceHighAnger;
    public bool balanceLowAnger;
    private Transform playerMiddleTransform;
    private Transform playerRightTransform;

    private float angerMax;

    private GameObject lastFood;
    private Vector3 lastPos;

    private float boomDistance;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        angerMax = player.GetComponent<AngerScript>().m_angerMax;
        playerLeftTransform = GameObject.FindGameObjectWithTag("PlayerLeftTransform").transform;
        playerMiddleTransform = GameObject.FindGameObjectWithTag("PlayerMiddleTransform").transform;
        playerRightTransform = GameObject.FindGameObjectWithTag("PlayerRightTransform").transform;

        boomDistance = (transform.position - playerMiddleTransform.position).magnitude;
        int randomInt = Random.Range(0, 3);
        Vector3 foodPos = Vector3.zero;
        if (randomInt == 0)
        {
            foodPos = playerLeftTransform.position;
        }

        if (randomInt == 1)
        {
            foodPos = playerMiddleTransform.position;
        }

        if (randomInt == 2)
        {
            foodPos = playerRightTransform.position;
        }

        foodPos += transform.forward * boomDistance;
        lastFood = (GameObject)Instantiate(goodFood, foodPos, Quaternion.identity);
        lastPos = lastFood.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.hasWon == false)
        {
            //Debug.DrawRay(lastBlock.transform.position, Vector3.up*100, Color.red);

            //if distance between spawner and last obstacle is greater than the minimum cool down distance and if the spawner is in front of the last obstacle (for when you get pushed back significantly)
            if ((lastPos - transform.position).magnitude >= coolDownDistance &&
                (transform.position - playerMiddleTransform.transform.position).magnitude > (lastPos - playerMiddleTransform.transform.position).magnitude)
            {
                int randomInt = Random.Range(0, 3);
                Vector3 foodPos = Vector3.zero;

                if (randomInt == 0)
                {
                    foodPos = playerLeftTransform.position;
                }

                if (randomInt == 1)
                {
                    foodPos = playerMiddleTransform.position;
                }

                if (randomInt == 2)
                {
                    foodPos = playerRightTransform.position;
                }

                foodPos += transform.forward * boomDistance;
                foodPos = new Vector3(foodPos.x, foodSpawnHeight, foodPos.z);

                RaycastHit[] hits = Physics.SphereCastAll(new Ray(foodPos+new Vector3(0,5,0), Vector3.down), 0.5f);
                if (ArrayContains("TrackSegment", hits) == true && ArrayContains("Obstacle", hits) == false)
                {

                    if (Random.Range(0, 1000) < foodSpawnFrequency)
                    {
                        if (balanceHighAnger == true && (angerMax - player.GetComponent<AngerScript>().m_anger < 0.01f))
                        {
                            lastFood = (GameObject)Instantiate(goodFood, foodPos, Quaternion.identity);
                            lastPos = lastFood.transform.position;
                        }

                        else
                        {
                            int randomFood = Random.Range(0, 2);

                            if (randomFood == 0)
                            {
                                lastFood = (GameObject)Instantiate(goodFood, foodPos, Quaternion.identity);
                                lastPos = lastFood.transform.position;
                            }

                            if (randomFood == 1)
                            {

                                lastFood = (GameObject)Instantiate(badFood, foodPos, Quaternion.identity);
                                lastPos = lastFood.transform.position;
                            }
                        }
                    }
                }
            }
        }
    }

    bool ArrayContains(string target, RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(target))
            {
                return true;
            }
        }

        return false;
    }
}
