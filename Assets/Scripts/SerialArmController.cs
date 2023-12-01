using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JelleVer.ConnectionTools;

public class SerialArmController : MonoBehaviour
{
    [SerializeField]
    private SimpleIKController ikController;
    [SerializeField]
    private ArduinoConnector conn;
    [SerializeField]
    private bool sendSerial = false;
    [SerializeField]
    private bool receiveSerial = false;
    [SerializeField]
    private bool sendToServer = false;
    [SerializeField]
    private float sendInterval = 1;

    private float counter = 0;
    private RobotArmController cont;

    // Start is called before the first frame update
    void Start()
    {
        cont = GetComponent<RobotArmController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (sendSerial)
        {
            counter += Time.deltaTime;

            if (counter < sendInterval) return;

            counter = 0;
            string mess = "";

            foreach (var angle in ikController.angles)
            {
                mess +=  Mathf.RoundToInt(angle).ToString() + " ";
            }
            mess.Trim();
            Debug.Log("Sending value to the Arduino: " + mess);
            conn.SendString(mess);

            //conn.SendBytes(System.Text.Encoding.UTF8.GetBytes(mess));
        }
        if (receiveSerial)
        {
            float[] angles = new float[5];

            string[] splitString = conn.receivedValue.Trim().Split(" ");

            for (int i = 0; i < Mathf.Min(splitString.Length, angles.Length); i++)
            {
                float.TryParse(splitString[i], out angles[i]);
            }

            ikController.SetIKAngles(angles);

            if (sendToServer)
            {
                cont.SendAngles();
            }
        }
    }

    public void LogString(string str)
    {
        Debug.Log(str);
    }

    
}
