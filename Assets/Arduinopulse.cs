using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine.UI;
using System.Windows.Markup;
using Unity.VisualScripting;
using System.IO;
using System.Text;
using System;
using System.Collections;

public class Arduinopulse : MonoBehaviour
{
    public string portname = "COM3";
    public int baudrate = 600;
    public Text UI;
    private List<string[]> rowdata = new List<string[]>();
    private string[] rowDataTemp = new string[2];
    private string data;
    private InputField inputfield;
    private SerialPort serialPort;
     
    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort(portname,baudrate);
        serialPort.Open();
        serialPort.ReadTimeout = 1000;
        createdCSV();
        
    }

    // Update is called once per frame
    void Update()
    {
        StartSensor();
    }

    public void StartSensor()
    {
        if (serialPort.IsOpen)
        {
            try
             {
                data = serialPort.ReadLine();
                UI.text = "BPM:" + data;
                SaveData();
                CollectDataLoop();
                Debug.Log(data);
            }
            catch (System.Exception)
            {
                Debug.Log("error");
            }
        }

        void OnApplicationQuit()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }

    private void createdCSV()
    {
        List<string[]> rowdata =  new List<string[]>();
        string[] columnname = new string[2];

        columnname[0] = "Date";
        columnname[1] = "BPM";
        rowdata.Add(columnname);

        string[][] output = new string[rowdata.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowdata[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        string filePath = Application.dataPath + "/CSV/" + "Bpmfile_" + ".csv";
        // Debug.Log(filePath);

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.Write(sb);
        outStream.Close();

    }

    private void SaveData()
    {
        rowdata = new List<string[]>();

        DateTime serverTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)serverTime).ToUnixTimeMilliseconds();

        rowDataTemp[0] = unixTime.ToString();
        rowDataTemp[1] = data.ToString();
        rowdata.Add(rowDataTemp);

        string[][] output = new string[rowdata.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowdata[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = Application.dataPath + "/CSV/" + "Bpmfile_"  + ".csv";

        StreamWriter outStream = System.IO.File.AppendText(filePath);
        outStream.Write(sb);
        outStream.Close();
    }

    private IEnumerator CollectDataLoop()
    {
        while (true)
        {
            SaveData();
            yield return new WaitForSeconds(0.08f);
        }
    }
}
