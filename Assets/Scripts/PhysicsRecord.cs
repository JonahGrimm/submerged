using UnityEngine;
using System;
using System.Collections;

public enum ForceType
{
    Explosion,
    Force,
    ForceAtPosition
}
public class PhysicsRecord : MonoBehaviour
{
    public static bool start = false;
    public bool checkToStart = false;
    public static bool restart = false;
    public bool checkToRestart = false;
    public ForceType type;
    public bool isOffSetRelative = true;
    public Vector3 offSet;
    public Vector3 direction;
    public float recordTime = 5f;
    public float startSecondDelay = 0;
    public float strength;
    public float radius;
    [Range(1, 10)]
    public int recordingResolution = 1; //1 is every frame is keyframed, 2 is every other frame, etc.

    private Rigidbody rb;   
    private bool isRecording;
    private int currentTime;            //The current time (in frames) that the animation is being recorded at out of recordTime*60
    private int recordTimeFrames;
    private bool hasStartedRecording = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    //Animation
    public AnimationClip clip;
    private AnimationCurve xPosCurve;
    private AnimationCurve yPosCurve;
    private AnimationCurve zPosCurve;
    private AnimationCurve xRotCurve;
    private AnimationCurve yRotCurve;
    private AnimationCurve zRotCurve;
    private AnimationCurve wRotCurve;
    private Keyframe[] xPosKeys;
    private Keyframe[] yPosKeys;
    private Keyframe[] zPosKeys;
    private Keyframe[] xRotKeys;
    private Keyframe[] yRotKeys;
    private Keyframe[] zRotKeys;
    private Keyframe[] wRotKeys;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (checkToStart)
            checkToStart = false;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (checkToStart && !isRecording && !hasStartedRecording)
        {
            Debug.Log("Start Checked!");
            start = true;
            checkToStart = false;
        }

        if (checkToRestart && !isRecording && !hasStartedRecording)
        {
            Debug.Log("Restart Checked!");
            restart = true;
            checkToRestart = false;
        }

        if (restart && !isRecording && !hasStartedRecording)
        {
            Debug.Log("Restarted positions!");
            transform.SetPositionAndRotation(originalPosition, originalRotation);
            StartCoroutine("BriefPause");
        }
    }

    void FixedUpdate()
    {
        if (isRecording)
        {
            currentTime++;

            Debug.Log("Current Time is " + currentTime);

            if (currentTime >= recordTimeFrames)
            {
                xPosCurve = new AnimationCurve(xPosKeys);
                yPosCurve = new AnimationCurve(yPosKeys);
                zPosCurve = new AnimationCurve(zPosKeys);
                xRotCurve = new AnimationCurve(xRotKeys);
                yRotCurve = new AnimationCurve(yRotKeys);
                zRotCurve = new AnimationCurve(zRotKeys);
                wRotCurve = new AnimationCurve(wRotKeys);

                clip.SetCurve("", typeof(Transform), "localPosition.x", xPosCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.y", yPosCurve);
                clip.SetCurve("", typeof(Transform), "localPosition.z", zPosCurve);
                clip.SetCurve("", typeof(Transform), "localRotation.x", xRotCurve);
                clip.SetCurve("", typeof(Transform), "localRotation.y", yRotCurve);
                clip.SetCurve("", typeof(Transform), "localRotation.z", zRotCurve);
                clip.SetCurve("", typeof(Transform), "localRotation.w", wRotCurve);

                Debug.Log("Done!");
                isRecording = false;
                clip.legacy = false;
                start = false;
                hasStartedRecording = false;
                return;
            }

            if (currentTime % recordingResolution == 0)
            {
                Debug.Log("Keyed.");
                KeyPosRot();
            }
        }

        if (start && !isRecording && !hasStartedRecording)
        {
            hasStartedRecording = true;
            StartCoroutine("StartAction");
        }
    }

    IEnumerator StartAction()
    {
        Debug.Log("Starting recording... waiting on Delay.");

        yield return new WaitForSeconds(startSecondDelay);

        Debug.Log("Delay is over. Applying force.");

        switch (type)
        {
            case ForceType.Explosion:
                if(isOffSetRelative)
                    rb.AddExplosionForce(strength, transform.position + offSet, radius);
                else
                    rb.AddExplosionForce(strength, offSet, radius);
                break;

            case ForceType.Force:
                rb.AddForce(offSet * strength); 
                //Maybe use a different Vector3
                //Add more options
                break;

            case ForceType.ForceAtPosition:
                if (isOffSetRelative)
                    rb.AddForceAtPosition(direction * strength, transform.position + offSet);
                else
                    rb.AddForceAtPosition(direction * strength, offSet);
                break;
        }

        Debug.Log("Setting up vars for recording...");

        isRecording = true;                         //Set the status to being recorded
        currentTime = 0;                            //Start at frame 0
        recordTimeFrames = (int)recordTime * 60;    //Convert to frames

        clip.ClearCurves();                         //Clear whatever was in this animtion before
        clip.legacy = false;                         //Set it to legacy so this method will work

        //Create enough keyframes for ourselves
        xPosKeys = new Keyframe[recordTimeFrames/recordingResolution];
        yPosKeys = new Keyframe[recordTimeFrames/recordingResolution];
        zPosKeys = new Keyframe[recordTimeFrames/recordingResolution];
        xRotKeys = new Keyframe[recordTimeFrames/recordingResolution];
        yRotKeys = new Keyframe[recordTimeFrames/recordingResolution];
        zRotKeys = new Keyframe[recordTimeFrames/recordingResolution];
        wRotKeys = new Keyframe[recordTimeFrames/recordingResolution];

        KeyPosRot();

        Debug.Log("Setup done. Initial keys set. Recording beginning...");
    }

    IEnumerator BriefPause()
    {
        yield return null;
        restart = false;
    }

    private void KeyPosRot()
    {
        Debug.Log("Current time is " + currentTime + " and xPosKeys.Length is " + xPosKeys.Length);
        xPosKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localPosition.x);
        yPosKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localPosition.y);
        zPosKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localPosition.z);
        xRotKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localRotation.x);
        yRotKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localRotation.y);
        zRotKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localRotation.z);
        wRotKeys[currentTime / recordingResolution] = new Keyframe(currentTime/60f, transform.localRotation.w);
    }
}