using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // this enables the IO port namespace
using System;
using Unity.VisualScripting;
using System.Threading.Tasks;

// ************* This script manages the Arduino Communication ******************* //
// tutorial I learned this from: https://playground.arduino.cc/Main/MPU-6050/#measurements //
// arduino script at the bottom //

public class ArduinoConnector : MonoBehaviour
{
    [Header("ConnectionSettings")]
    public bool useArduino;
    public string IOPort = "/dev/cu.HC05-SPPDev"; // Change this to whatever port your Arduino is connected to, this is the port for the specefic bluetooth adaptor used (HC-05 Wireless Bluetooth RF Transceiver)
    public int baudeRate = 9600; //this must match the bauderate of the Arduino script
    public bool receive = false;
    [HideInInspector]
    public SerialPort sp;
    public string receivedValue;



    // Start is called before the first frame update
    void Start()
    {
        if (useArduino)
        {
            ActivateSP();
        }

    }

    private void Update()
    {
        if(receive) CheckSerial();

    }

    private async void CheckSerialAsync()
    {
        try
        {
            receivedValue = await Task.Run(sp.ReadLine);
            Debug.Log(receivedValue);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

    }

    private void CheckSerial()
    {
        try
        {
            receivedValue = sp.ReadLine();
            Debug.Log(receivedValue);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

    }

    private void OnApplicationQuit()
    {
        DeactivateSP();
    }

    void ActivateSP()
    {
        sp = new SerialPort(IOPort, baudeRate, Parity.None, 8, StopBits.One);

        sp.Open();
        sp.DtrEnable = true;
        sp.RtsEnable = true;
        sp.ReadTimeout = 50;
    }

    void DeactivateSP()
    {
        if (sp == null) return;
        if(sp.IsOpen) sp.Close();
    }

    /// <summary>
    /// Sends a message to the activate serial port
    /// </summary>
    /// <param name="value">the value to send</param>
    public void SendBytes(byte[] value)
    {
        if (sp == null)
        {
            Debug.LogWarning("Cant send data, because th eport is not available");
            return;
        }

        if (sp.IsOpen)
        {
            sp.Write(value, 0, value.Length);

        }

    }

    public void SendString(string value)
    {
        if (sp == null)
        {
            Debug.LogWarning("Cant send data, because th eport is not available");
            return;
        }

        if (sp.IsOpen)
        {
            sp.WriteLine(value);
            Debug.Log("Writing string to arduino:" + value);

        }
    }


}
