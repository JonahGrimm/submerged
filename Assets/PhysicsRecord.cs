using UnityEngine;
using System;
using System.Collections;

public enum ForceType
{
    Explosion,
    Regular
}
public class PhysicsRecord : MonoBehaviour
{
    public static bool start = false;
    public ForceType type;
    public bool isOffSetRelative = true;
    public Vector3 offSet;
    public float recordTime = 5f;
    public float startSecondDelay = 0;
    public float strength;
    public float radius;
    [Range(1, 10)]
    public int recordingResolution = 1; //1 is every frame is keyframed, 2 is every other frame, etc.

    private Rigidbody rb;   
    private bool isRecording;
    private float currentRecordTime;
    private int currentResolutionTracker;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (start)
        {
            StartAction();
            start = false;
        }

        if (isRecording)
        {
            currentResolutionTracker--;
            if (currentResolutionTracker == 0)
            {
                currentResolutionTracker = recordingResolution;
                //TODO Figure out a way to specify a file path and generate an animation file
                //TODO Make sure its name does not conflict with an existing file
                //TODO Record key frames in that file...
            }
        }

    }

    IEnumerator StartAction()
    {
        yield return new WaitForSeconds(startSecondDelay);

        switch (type)
        {
            case ForceType.Explosion:
                if(isOffSetRelative)
                    rb.AddExplosionForce(strength, transform.position + offSet, radius);
                else
                    rb.AddExplosionForce(strength, offSet, radius);
                break;

            case ForceType.Regular:
                rb.AddForce(offSet * strength); 
                //Maybe use a different Vector3
                //Add more options
                break;
        }

        isRecording = true;
        currentRecordTime = recordTime * 60; //Convert to frames
        currentResolutionTracker = recordingResolution;
    }

}