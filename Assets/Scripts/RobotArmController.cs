using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RobotArmController : MonoBehaviour
{
    [SerializeField]
    private SimpleIKController ikController;

    [SerializeField]
    private string url = "http://192.168.0.182/sliders";

    [SerializeField]
    private bool update = false;
    [SerializeField]
    private bool smooth = true;

    private bool sending = false;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetExtremityPitch(float value)
    {
        ikController.extremityRotation = new Vector2(value, 0);
    }

    public void SetIp(string value)
    {
        url = value;
    }

    public void SetUpdate(bool value)
    {
        update = value;
    }

    public void SetSmooth(bool value)
    {
        smooth = value;
    }


    public void SendAngles()
    {
        if (sending) return;

        StartCoroutine(Upload());

    }

    /// <summary>
    /// converts a string of angles separated by spaces into an array of floats and send it to the IK controller
    /// </summary>
    /// <param name="values"></param>
    public void ConvertSerialAngles(string values)
    {
        string[] splitString = values.Split(" ");

        float[] angles = new float[5];


        for (int i = 0, j = 0; (i < splitString.Length) && (j < 5); i++)
        {
            if(float.TryParse(splitString[i], out angles[j])) j++;
        }

        Debug.Log(angles);
        ikController.SetIKAngles(angles);
    }

    IEnumerator Upload()
    {
        sending = true;

        WWWForm form = new WWWForm();
        form.AddField("BaseYaw", ikController.angles[0].ToString());
        form.AddField("BasePitch", ikController.angles[1].ToString());
        form.AddField("ElbowPitch", ikController.angles[2].ToString());
        form.AddField("WristPitch", ikController.angles[3].ToString());
        form.AddField("WristRoll", ikController.angles[4].ToString());
        form.AddField("smooth", smooth.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                if(update) StartCoroutine(Upload());
            }
        }
        sending = false;
    }
}
