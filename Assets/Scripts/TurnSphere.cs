using UnityEngine;
using System.Collections;

public class TurnSphere : MonoBehaviour {

    public float yRot;
    public float nextTrackDistance = 50;

    private bool hasRotated;

    void Start() {
        hasRotated = false;
    }

	void OnTriggerEnter(Collider col) {

        if (hasRotated == false)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                hasRotated = true;
                col.gameObject.GetComponent<PlayerMovementDuncan>().AllTransforms.transform.position = transform.position;
                col.gameObject.GetComponent<PlayerMovementDuncan>().AllTransforms.transform.Rotate(new Vector3(0, yRot, 0));
                GameObject.Find("TrackGenerator").GetComponent<GenerateTrack>().NextTrackPos = transform.position + (col.gameObject.transform.forward * nextTrackDistance);
                GameObject.Find("TrackGenerator").GetComponent<GenerateTrack>().GenerateStraightTracks = true;

            }
        }

    }
}
