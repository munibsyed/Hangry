using UnityEngine;
using System.Collections;

public class GenerateTrack : MonoBehaviour {

    [Tooltip("Maximum total length of the track")]
    public int trackMaxLength; //maximum length of the track
    [Tooltip("How far ahead the player the tracks are being instantiated in advance")]
    public int forwardDistance;
    [Tooltip("Determines how likely the track generator is to spawn a turning track segment.")]
    public int turnFrequency;

    public GameObject trackSegment10;
    public GameObject trackSegment100;
   
    public GameObject trackSegment100Fork; //left and right path, 90 degree angle
    public GameObject trackSegment100ForkLeftOnly; //left path only, 90 degree angle
    public GameObject trackSegment100ForkRightOnly; //right path only, 90 degree angle

    public GameObject trackSegmentTurnThreeLanesRight;
    public GameObject trackSegmentTurnThreeLanesLeft;


    private Vector3 trackInitialDirection;
    private Vector3 trackGenerationStartPos; //currentPos + half the z value of the first track piece
    private Vector3 nextTrackPos;

    private bool hasWon;

    private int trackCurrentLength;

    private bool generateStraightTracks;

    public bool GenerateStraightTracks
    {
        get
        {
            return generateStraightTracks;
        }

        set
        {
            generateStraightTracks = value;
        }
    }

    public Vector3 NextTrackPos
    {
        get
        {
            return nextTrackPos;
        }

        set
        {
            nextTrackPos = value;
        }
    }

    private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        trackInitialDirection = player.transform.forward;

        //first piece is 100 units long so it must be shifted 50 units in the direction that the player is facing
        trackCurrentLength = 100;
        trackGenerationStartPos = transform.position + transform.TransformPoint((player.transform.forward * 50));
        nextTrackPos = trackGenerationStartPos;
        Instantiate(trackSegment100, nextTrackPos, Quaternion.LookRotation(trackInitialDirection, Vector3.up)); //1.13
        generateStraightTracks = true;
        hasWon = false;
	}
	
	// Update is called once per frame
	void Update () {
        //If we are still underneath the max length
        if (trackCurrentLength < trackMaxLength)
        //if (GameManager.Instance.playerDistanceTravelled+200 < trackMaxLength)
        {
            //this block generates turns. 
            if (Random.Range(0, 1000) < turnFrequency && generateStraightTracks == true) //generateStraightTracks will stop the game from generating extra tracks perpendicular to the turn 
            {
                generateStraightTracks = false;
              
                Quaternion currentRot = Quaternion.LookRotation(player.transform.forward, Vector3.up); 
                Vector3 shiftBy = player.transform.forward * 100;
                nextTrackPos += (shiftBy);
                Instantiate(ChooseNextTrackSegment(), nextTrackPos, currentRot);
            }

            else if ((nextTrackPos - player.transform.position).magnitude < forwardDistance && generateStraightTracks == true) //when the player is less than the forwardDistance behind the next track segment (one that they are not currently on)
            {
                trackCurrentLength += 100;

                //for a straight segment, the rotation of the piece will equal to player's current forward vector
                Quaternion currentRot = Quaternion.LookRotation(player.transform.forward, Vector3.up);
                
                Vector3 shiftBy = player.transform.forward * 100;
                nextTrackPos += shiftBy;
                //Debug.Log(shiftBy);

                Instantiate(trackSegment100, nextTrackPos, currentRot);
            }

        }

        else
        {
            GameManager.Instance.DisplayWinState();
        }

    }

    GameObject ChooseNextTrackSegment()
    {
        int randInt = Random.Range(0, 5);
                                                          
        if (randInt == 0)
        {
            return trackSegment100Fork; //left and right path, 90 degree angle
        }

        if (randInt == 1)
        {
            return trackSegment100ForkLeftOnly;
        }

        if (randInt == 2)
        {
            return trackSegment100ForkRightOnly; //right path only, 90 degree angle
        }

        if (randInt == 3)
        {
            return trackSegmentTurnThreeLanesLeft;

        }

        else
        {
            return trackSegmentTurnThreeLanesRight;
        }


    }

    GameObject ChooseNextTrackSegmentNoBacktracking()
    {
        Vector3 playerForward = GameObject.FindGameObjectWithTag("PlayerTransforms").transform.forward;
        int sign = Vector3.Cross(Vector3.forward, playerForward).y < 0 ? -1 : 1;
        float rot = Vector3.Angle(Vector3.forward, playerForward);

        rot *= sign;
        //sign does not seem to be preserved

        if ((Mathf.Round(rot) == -90))
        {
            int randomInt = Random.Range(0, 2);

            if (randomInt == 1)
            {
                trackCurrentLength += 200;               
                return trackSegment100ForkRightOnly;
            }

            else
            {
                trackCurrentLength += 200;
                return trackSegmentTurnThreeLanesRight;
            }

        }

        if ((Mathf.Round(rot) == 90))
        {
            int randomInt = Random.Range(0, 2);

            if (randomInt == 1)
            {
                trackCurrentLength += 200;
                return trackSegment100ForkLeftOnly;
            }

            else
            {
                trackCurrentLength += 200;
                return trackSegmentTurnThreeLanesLeft;
            }

        }

        else
        {
            int randomInt = Random.Range(0, 5);


            if (randomInt == 0)
            {
                trackCurrentLength += 200;
                return trackSegment100Fork;
            }
            if (randomInt == 1)
            {
                trackCurrentLength += 200;
                return trackSegment100ForkLeftOnly;
            }

            if (randomInt == 2)
            {
                trackCurrentLength += 200;
                return trackSegment100ForkRightOnly;
            }

            if (randomInt == 3)
            {
                trackCurrentLength += 200;
                return trackSegmentTurnThreeLanesRight;
            }

            else
            {
                trackCurrentLength += 200;
                return trackSegmentTurnThreeLanesLeft;
            }
        }
    }
}
