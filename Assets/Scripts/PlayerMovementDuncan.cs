using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerMovementDuncan : MonoBehaviour {

    // Use this for initialization

    public GameObject middleTransform;
    public GameObject leftTransform;
    public GameObject rightTransform;
    public GameObject verticalTransform;
    public GameObject verticalLeftTransform;
    public GameObject verticalRightTransform;
    public GameObject allTransforms;
    public GameObject transformForward;
    public float playerStartingY;
    public ResolveMiddleLaneChange middleLaneChange;
    

    public bool changeLaneWhenHit;

    [Tooltip("When the difference between the player's y-position and a vertical transform target's y-position is less than this threshold, the player will start falling.")]
    public float jumpThreshold;
    
    private GameObject targetTransform;
    private GameObject player;



    private Vector3 pushbackPos;
    private Vector3 local;

    public float speed;
    public float pushbackRecoverySpeed;

    private float pushbackTimer;
    

    private MovementStates movementState;
   

    public enum MovementStates
    {
        MOVING_STRAIGHT,
        MOVING_RIGHT,
        MOVING_LEFT,
        PUSHED_BACK,
        JUMPING_UP,
        FALLING_DOWN,
        DUCKING
    }
    public enum ResolveMiddleLaneChange
    {
        CHOOSE_RANDOM_LANE,
        CHOOSE_FURTHEST_OBSTACLE,
        CHOOSE_CLOSEST_GOOD_FOOD,
        MAKE_SMART_CHOICE
    }

    public MovementStates MovementState
    {
        get
        {
            return movementState;
        }

        set
        {
            movementState = value;
        }
    }

    public Vector3 PushbackPos
    {
        get
        {
            return pushbackPos;
        }

        set
        {
            pushbackPos = value;
        }
    }

   
    public GameObject AllTransforms
    {
        get
        {
            return allTransforms;
        }
    }

    void Start () {
        
        movementState = MovementStates.MOVING_STRAIGHT;
        targetTransform = middleTransform;
        local = allTransforms.transform.localPosition;
        pushbackTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {

        switch (movementState)
        {
            case MovementStates.MOVING_STRAIGHT:
                Debug.Log("Moving Forward!");
                break;

            case MovementStates.PUSHED_BACK:
                Debug.Log("Pushed Back!");
                break;

            case MovementStates.MOVING_LEFT:
                Debug.Log("Moving Left!");
                break;

            case MovementStates.MOVING_RIGHT:
                Debug.Log("Moving Right!");
                break;

            case MovementStates.FALLING_DOWN:
                Debug.Log("Falling Down!");
                break;

            case MovementStates.JUMPING_UP:
                Debug.Log("Jumping Up!");
                break;

            case MovementStates.DUCKING:
                Debug.Log("Ducking!");
                break;

            default:
                Debug.Log("Default");
                break;
        }


        if (player.transform.position.y - playerStartingY < 0.1f && movementState == MovementStates.FALLING_DOWN)
        {
            movementState = MovementStates.MOVING_STRAIGHT;
            player.transform.position = new Vector3(player.transform.position.x, playerStartingY, player.transform.position.z);

            //re-align left-rights
            GameObject[] leftMiddleRightTransforms = { leftTransform, middleTransform, rightTransform };
            leftMiddleRightTransforms = leftMiddleRightTransforms.OrderBy(x => (x.transform.position - transform.position).magnitude).ToArray();
            transform.position = leftMiddleRightTransforms[0].transform.position;
        }


        if (movementState == MovementStates.JUMPING_UP)
        {
            if (Mathf.Abs(transform.position.y - verticalTransform.transform.position.y) < jumpThreshold)
            //if ((transform.position - targetTransform.transform.position).magnitude < 0.2f)
            {
                movementState = MovementStates.FALLING_DOWN;

                if (targetTransform == verticalLeftTransform)
                {
                    targetTransform = leftTransform;
                }

                if (targetTransform == verticalTransform)
                {
                    targetTransform = middleTransform;
                }

                if (targetTransform == verticalRightTransform)
                {
                    targetTransform = rightTransform;
                }
            }
        }

        if (movementState != MovementStates.PUSHED_BACK)
        {
            Vector3 newPos = new Vector3(0, 0, speed * Time.deltaTime);
            allTransforms.transform.position = allTransforms.transform.TransformPoint(newPos);
        }

        else
        {
            pushbackTimer += pushbackRecoverySpeed * Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(allTransforms.transform.position, pushbackPos, pushbackTimer);
            float playerNewY = Mathf.Lerp(player.transform.position.y, playerStartingY, pushbackTimer);
            //transform.position = newPos;
            allTransforms.transform.position = newPos;
            player.transform.position = new Vector3(player.transform.position.x, playerNewY, player.transform.position.z);

            if (pushbackTimer >= 0.5f)
            {
                pushbackTimer = 0;
                movementState = MovementStates.MOVING_STRAIGHT;
                //re-align forward 
                if (transform.forward == Vector3.forward || transform.forward == -Vector3.forward)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, allTransforms.transform.position.z);
                }

                else
                {
                    transform.position = new Vector3(allTransforms.transform.position.x, transform.position.y, transform.position.z);
                }

                //re-align left-rights
                GameObject[] leftMiddleRightTransforms = { leftTransform, middleTransform, rightTransform };
                leftMiddleRightTransforms = leftMiddleRightTransforms.OrderBy(x => (x.transform.position - transform.position).magnitude).ToArray();
                transform.position = leftMiddleRightTransforms[0].transform.position;


                if (changeLaneWhenHit == true)
                {
                    if (targetTransform == leftTransform || targetTransform == verticalLeftTransform)
                    {
                        targetTransform = middleTransform;
                        movementState = MovementStates.MOVING_RIGHT;
                    }

                    else if (targetTransform == rightTransform || targetTransform == verticalRightTransform)
                    {
                        targetTransform = middleTransform;
                        movementState = MovementStates.MOVING_LEFT;
                    }

                    else //if in middle lane
                    {
                        if (middleLaneChange == ResolveMiddleLaneChange.CHOOSE_RANDOM_LANE)
                        {
                            int randInt = Random.Range(0, 2);
                            if (randInt == 0)
                            {
                                targetTransform = leftTransform;
                                movementState = MovementStates.MOVING_LEFT;
                            }

                            else
                            {
                                targetTransform = rightTransform;
                                movementState = MovementStates.MOVING_RIGHT;
                            }
                        }

                        if (middleLaneChange == ResolveMiddleLaneChange.CHOOSE_FURTHEST_OBSTACLE)
                        {
                            float rightDistance = 0;
                            float leftDistance = 0;

                            RaycastHit hit;
                            if (Physics.Raycast(new Ray(rightTransform.transform.position, transform.forward), out hit))
                            {
                                if (hit.collider.gameObject.CompareTag("Obstacle"))
                                {
                                    rightDistance = (hit.transform.position - transform.position).magnitude;
                                }
                            }

                            if (Physics.Raycast(new Ray(leftTransform.transform.position, transform.forward), out hit))
                            {
                                if (hit.collider.gameObject.CompareTag("Obstacle"))
                                {
                                    leftDistance = (hit.transform.position - transform.position).magnitude;
                                }
                            }

                            if (rightDistance == 0)
                            {
                                rightDistance = System.Int32.MaxValue;
                            }

                            if (leftDistance == 0)
                            {
                                leftDistance = System.Int32.MaxValue;
                            }

                            if (rightDistance > leftDistance)
                            {
                                targetTransform = rightTransform;
                                movementState = MovementStates.MOVING_RIGHT;
                            }

                            else
                            {
                                targetTransform = leftTransform;
                                movementState = MovementStates.MOVING_LEFT;
                            }
                        }

                        else if (middleLaneChange == ResolveMiddleLaneChange.CHOOSE_CLOSEST_GOOD_FOOD)
                        {
                            float rightDistance = 0;
                            float leftDistance = 0;

                            RaycastHit hit;
                            if (Physics.Raycast(new Ray(rightTransform.transform.position, transform.forward), out hit))
                            {
                                if (hit.collider.gameObject.CompareTag("GoodFood"))
                                {
                                    rightDistance = (hit.transform.position - transform.position).magnitude;
                                }
                            }

                            if (Physics.Raycast(new Ray(leftTransform.transform.position, transform.forward), out hit))
                            {
                                if (hit.collider.gameObject.CompareTag("GoodFood"))
                                {
                                    leftDistance = (hit.transform.position - transform.position).magnitude;
                                }
                            }

                            if (rightDistance == 0)
                            {
                                rightDistance = System.Int32.MaxValue;
                            }

                            if (leftDistance == 0)
                            {
                                leftDistance = System.Int32.MaxValue;
                            }



                            if (rightDistance > leftDistance)
                            {
                                targetTransform = rightTransform;
                                movementState = MovementStates.MOVING_RIGHT;
                            }

                            else
                            {
                                targetTransform = leftTransform;
                                movementState = MovementStates.MOVING_LEFT;
                            }
                        }

                        else if (middleLaneChange == ResolveMiddleLaneChange.MAKE_SMART_CHOICE)
                        {
                            float rightDistance = 0;
                            float leftDistance = 0;

                            //right 
                            RaycastHit hitRight;
                            RaycastHit hitLeft;
                            if (Physics.Raycast(new Ray(rightTransform.transform.position, transform.forward), out hitRight) &&
                                Physics.Raycast(new Ray(leftTransform.transform.position, transform.forward), out hitLeft))
                            {
                                string leftTag = hitLeft.collider.gameObject.tag;
                                string rightTag = hitRight.collider.gameObject.tag;

                                rightDistance = (hitRight.collider.transform.position - rightTransform.transform.position).magnitude;
                                leftDistance = (hitLeft.collider.transform.position - leftTransform.transform.position).magnitude;

                                //choose closest
                                if (leftTag == "GoodFood" && rightTag == "GoodFood")
                                {
                                    if (leftDistance < rightDistance)
                                    {
                                        targetTransform = leftTransform;
                                        movementState = MovementStates.MOVING_LEFT;
                                    }

                                    else
                                    {
                                        targetTransform = rightTransform;
                                        movementState = MovementStates.MOVING_RIGHT;
                                    }
                                }

                                //choose left lane
                                else if (leftTag == "GoodFood" && (rightTag == "BadFood" || rightTag == "Obstacle"))
                                {
                                    targetTransform = leftTransform;
                                    movementState = MovementStates.MOVING_LEFT;
                                }

                                //choose right lane
                                else if (leftTag == "BadFood" && rightTag == "GoodFood")
                                {
                                    targetTransform = rightTransform;
                                    movementState = MovementStates.MOVING_RIGHT;
                                }

                                //choose furthest
                                else if ((leftTag == "BadFood" || leftTag == "Obstacle") && (rightTag == "BadFood" || rightTag == "Obstacle"))
                                {
                                    if (leftDistance < rightDistance)
                                    {
                                        targetTransform = rightTransform;
                                        movementState = MovementStates.MOVING_RIGHT;
                                    }

                                    else
                                    {
                                        targetTransform = leftTransform;
                                        movementState = MovementStates.MOVING_LEFT;
                                    }
                                }

                                //choose right lane
                                else if (leftTag == "Obstacle" && rightTag == "GoodFood")
                                {
                                    targetTransform = rightTransform;
                                    movementState = MovementStates.MOVING_RIGHT;
                                }

                                //choose right lane
                                else if (leftTag == "Untagged" && rightTag == "GoodFood")
                                {
                                    targetTransform = rightTransform;
                                    movementState = MovementStates.MOVING_RIGHT;
                                }

                                //choose left lane
                                else if (leftTag == "Untagged" && (rightTag == "BadFood" || rightTag == "Obstacle"))
                                {
                                    targetTransform = leftTransform;
                                    movementState = MovementStates.MOVING_LEFT;
                                }

                                //choose left lane
                                else if (leftTag == "GoodFood" && rightTag == "Untagged")
                                {
                                    targetTransform = leftTransform;
                                    movementState = MovementStates.MOVING_LEFT;
                                }

                                //choose right lane
                                else if ((leftTag == "BadFood" || leftTag == "Obstacle") && rightTag == "Untagged")
                                {
                                    targetTransform = rightTransform;
                                    movementState = MovementStates.MOVING_RIGHT;
                                }

                                //go left
                                else if (leftTag == "Untagged" && rightTag == "Untagged")
                                {
                                    targetTransform = leftTransform;
                                    movementState = MovementStates.MOVING_LEFT;
                                }

                                else
                                {
                                    Debug.Log("Oops!");
                                }
                            }

                            else
                            {
                                if (rightDistance == 0)
                                {
                                    rightDistance = System.Int32.MaxValue;
                                }

                                if (leftDistance == 0)
                                {
                                    leftDistance = System.Int32.MaxValue;
                                }



                                if (rightDistance > leftDistance)
                                {
                                    targetTransform = rightTransform;
                                    movementState = MovementStates.MOVING_RIGHT;
                                }

                                else
                                {
                                    targetTransform = leftTransform;
                                    movementState = MovementStates.MOVING_LEFT;
                                }
                            }
                        }

                        else
                        {
                            Debug.Log("Error: Reached the else");
                        }

                    }
                }
            }
        }

        if (movementState != MovementStates.MOVING_STRAIGHT)
        {
            if (movementState == MovementStates.MOVING_LEFT || movementState == MovementStates.MOVING_RIGHT)
            {
                transform.position = Vector3.Lerp(transform.position, targetTransform.transform.position, Time.deltaTime * speed);
            }

            if (movementState == MovementStates.JUMPING_UP || movementState == MovementStates.FALLING_DOWN)
            {
                transform.position = Vector3.Lerp(transform.position, targetTransform.transform.position, Time.deltaTime * speed * 0.25f);

            }


            if ((transform.position).magnitude < 0.1f)
            {
                transform.position = targetTransform.transform.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (movementState != MovementStates.PUSHED_BACK)
            {
                if (movementState == MovementStates.JUMPING_UP)
                {

                    if (targetTransform == verticalTransform)
                    {
                        targetTransform = verticalRightTransform;
                    }

                    if (targetTransform == verticalLeftTransform)
                    {
                        targetTransform = verticalTransform;
                    }


                }

                else if (movementState == MovementStates.FALLING_DOWN)
                {

                    if (targetTransform == middleTransform)
                    {
                        targetTransform = rightTransform;
                    }

                    if (targetTransform == leftTransform)
                    {
                        targetTransform = middleTransform;
                    }
                }

                else
                {
                    movementState = MovementStates.MOVING_RIGHT;

                    if (targetTransform == middleTransform)
                    {
                        targetTransform = rightTransform;
                    }

                    if (targetTransform == leftTransform)
                    {
                        targetTransform = middleTransform;
                    }
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (movementState != MovementStates.PUSHED_BACK)
            {
                if (movementState == MovementStates.JUMPING_UP)
                {
                    //Debug.Log((transform.position - targetTransform.transform.position).magnitude);
                    //if ((transform.position - targetTransform.transform.position).magnitude > 0.2f)
                    {
                        if (targetTransform == verticalTransform)
                        {
                            targetTransform = verticalLeftTransform;
                        }

                        if (targetTransform == verticalRightTransform)
                        {
                            targetTransform = verticalTransform;
                        }
                    }


                }

                else if (movementState == MovementStates.FALLING_DOWN)
                {
                    if (targetTransform == middleTransform)
                    {
                        targetTransform = leftTransform;
                    }

                    if (targetTransform == rightTransform)
                    {
                        targetTransform = middleTransform;
                    }
                }

                else
                {
                    movementState = MovementStates.MOVING_LEFT;

                    if (targetTransform == middleTransform)
                    {
                        targetTransform = leftTransform;
                    }

                    if (targetTransform == rightTransform)
                    {
                        targetTransform = middleTransform;
                    }
                }


            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (movementState != MovementStates.PUSHED_BACK && movementState != MovementStates.JUMPING_UP && movementState != MovementStates.FALLING_DOWN)
            {
                movementState = MovementStates.JUMPING_UP;

                if (targetTransform == middleTransform)
                {
                    targetTransform = verticalTransform;
                }

                if (targetTransform == leftTransform)
                {
                    targetTransform = verticalLeftTransform;
                }

                if (targetTransform == rightTransform)
                {
                    targetTransform = verticalRightTransform;
                }
            }
        }

        if (movementState != MovementStates.FALLING_DOWN && movementState != MovementStates.JUMPING_UP)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movementState = MovementStates.DUCKING;
            }
        }

        if (movementState == MovementStates.DUCKING && Input.GetKeyUp(KeyCode.DownArrow))
        {
            movementState = MovementStates.MOVING_STRAIGHT;
        }




    }
}
