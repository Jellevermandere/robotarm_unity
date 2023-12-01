using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to the upper limb
/// </summary>
public class SimpleIKController : MonoBehaviour
{
    [SerializeField]
    private bool autoUpdate = true;
    [SerializeField]
    private bool controlextremityRotation = false;


    [SerializeField]
    private Transform lowerLimb;
    [SerializeField]
    private Transform upperLimb;
    [SerializeField]
    private Transform extremity;
    [SerializeField]
    private Transform extremityEnd;

    [SerializeField]
    private float extremityOffset = 0f;



    [SerializeField]
    public Vector2 extremityRotation = Vector2.zero;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform pole;

    private float upperLimbLength;
    private float lowerLimbLength;
    [SerializeField]
    public float[] angles = { 0f, 0f, 0f, 0f, 0f };


    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
       if(autoUpdate) SolveAngleIk();

    }

    void Setup()
    {
        upperLimbLength = Vector3.Distance(upperLimb.position, lowerLimb.position);
        lowerLimbLength = Vector3.Distance(lowerLimb.position, extremity.position);
    }

    bool SolveAngleIk()
    {

        Vector3 targetPosition = target.position - transform.position;

        //Check if position is valid (e.g. if the Y position > 0)
        if (Vector3.Dot(transform.up, targetPosition) < 0)
        {
            return false;
        }
        //check if the point is too close to the shoulder
        if (targetPosition.magnitude < (upperLimbLength - lowerLimbLength))
        {
            return false;
        }

        //Offset the target position with the end offset & rotation
        Vector3 groundDirection = Vector3.Scale(new Vector3(1,0,1),targetPosition);
        float endRad = -extremityRotation.x * Mathf.Deg2Rad;
        Vector3 offsetDirection = Vector3.Normalize(groundDirection) * Mathf.Cos(endRad) + Vector3.up * Mathf.Sin(endRad);
        Vector3 wristTargetPos = targetPosition - Vector3.Normalize(offsetDirection) * extremityOffset;
        groundDirection = Vector3.Scale(new Vector3(1, 0, 1), wristTargetPos);

        if (wristTargetPos.magnitude < (upperLimbLength - lowerLimbLength))
        {
            return false;
        }

        //Step 1: aim the base (0 deg is right, rotating counter clockwise)
        float baseYaw = Mathf.Atan2(-targetPosition.x, targetPosition.z);

        // Step 2: Determine the distance to the target point
        //if further than the lenght: go straight
        //else: calculate the elbow and base angle
        float targetDistance = wristTargetPos.magnitude;
        //float basePitch = groundDirection.magnitude == 0 ? (Mathf.PI / 2f) : Vector3.SignedAngle(transform.forward, wristTargetPos, -transform.right) * Mathf.Deg2Rad; //(Mathf.Atan(wristTargetPos.y / groundDirection.magnitude));
        float basePitch = groundDirection.magnitude == 0 ? (Mathf.PI / 2f) : Vector3.Angle(transform.forward, wristTargetPos) * Mathf.Deg2Rad; //(Mathf.Atan(wristTargetPos.y / groundDirection.magnitude));
        float elbowPitch = Mathf.PI;

        if ((upperLimbLength + lowerLimbLength) > targetDistance) {
            //the target is withing reach of the robot arm
            //print("The distance is within reach")
            float a = upperLimbLength;
            float b = lowerLimbLength;
            float c = targetDistance;
            basePitch += Mathf.Acos((Mathf.Pow(b, 2) - Mathf.Pow(c, 2) - Mathf.Pow(a, 2)) / (-2 * a * c));
            elbowPitch = Mathf.Acos((Mathf.Pow(c, 2) - Mathf.Pow(a, 2) - Mathf.Pow(b, 2)) / (-2 * a * b));
        }
        // Step 3: aim the wrist at the target
        // first set the wrist horizontal
        // then add the endrotation
        float wristPitch = -basePitch - elbowPitch + Mathf.PI;
        //print("wristPitch:", math.degrees(wristPitch))
        //if(wristPitch < 0): wristPitch = -wristPitch
        wristPitch += -extremityRotation.x * Mathf.Deg2Rad + Mathf.PI / 2f;

        // Step 4: Convert the angles to degrees in the right direction and starting point
        angles[0] = baseYaw * Mathf.Rad2Deg ;
        angles[1] = rad_to_servo(basePitch, true);
        angles[2] = rad_to_servo(elbowPitch);
        angles[3] = rad_to_servo(wristPitch, true);
        angles[4] = extremityRotation.y;

        SetAngles(angles);

        return true;
    }

    private void SetAngles(float[] _angles)
    {
        transform.rotation = Quaternion.Euler(0, -_angles[0], 0);
        upperLimb.localRotation = Quaternion.Euler(_angles[1] - 180, 0, 0);
        lowerLimb.localRotation = Quaternion.Euler(-_angles[2], 0, 0);
        extremity.localRotation = Quaternion.Euler(_angles[3] - 180, 0, 0);
        extremityEnd.localRotation = Quaternion.Euler(0, 0, _angles[4]);
    }

    float rad_to_servo(float rad, bool sup = false) {
        if (sup) return Mathf.Max(0, Mathf.Min(180, 180 - rad * Mathf.Rad2Deg));

        else return Mathf.Max(0, Mathf.Min(180, rad * Mathf.Rad2Deg));
    }

    /// <summary>
    /// Sets the angles of the arm directly
    /// </summary>
    /// <param name="_angles">the array of length 5 where each element is the following joint</param>
    public void SetIKAngles(float[] _angles)
    {
        if (autoUpdate)
        {
            Debug.Log("AutoUpdate is on, Received angles are ignored.");
            return;
        }
        angles = _angles;
        SetAngles(_angles);
    }



}
