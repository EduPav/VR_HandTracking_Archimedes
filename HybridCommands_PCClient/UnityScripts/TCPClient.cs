using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

public class TCPClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];
    private StringBuilder receivedData = new StringBuilder();
    // private string host="192.168.1.36";
    private string host="192.168.56.1";
    // private string host=InputController.ipServer?.Replace("\u200B", "");   //Here we access the previous scene class attribute
    //We needed to remove the invisible character that is added to the string
    public int port = 65432;
    private Texture2D texture;
    private byte[] encodedImage;
    private WebCamTexture webcamTexture;
    private int width = 1280;
    private int height = 720;
    private string dataRec;
    public string [] dataHands;

    private void Start()
    {
        ConnectToServer(host, port); // Replace with your server's IP address and port

        // Get the rear camera
        WebCamDevice[] devices = WebCamTexture.devices;
        string rearCameraName = "";
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                rearCameraName = devices[i].name;
                break;
            }
        }

        // Create the webcam texture object using the rear camera
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        webcamTexture = new WebCamTexture(rearCameraName, width, height);
        webcamTexture.Play();
    }

    private void ConnectToServer(string ip, int port)
    {
        client = new TcpClient();
        client.BeginConnect(ip, port, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        client.EndConnect(ar);
        stream = client.GetStream();
        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        int bytesRead = stream.EndRead(ar);
        if (bytesRead <= 0)
        {
            Debug.Log("Disconnected from server.");
            return;
        }

        byte[] data = new byte[bytesRead];
        Array.Copy(receiveBuffer, data, bytesRead);
        receivedData.Append(Encoding.ASCII.GetString(data));

        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
    }

    private void SendData(byte[] data)
    {
        if (client == null || !client.Connected)
        {
            Debug.Log("Not connected to server.");
            return;
        }

        // byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private string [] HandleData(string dataRaw)
    {
        string dataL="";
        string dataR="";
        string[] dataT= new string[2];
        Regex regexL = new Regex(@"'L'.*?('L'|'R'|])");
        Regex regexR = new Regex(@"'R'.*?('L'|'R'|])");
        Match matchL = regexL.Match(dataRaw);
        Match matchR = regexR.Match(dataRaw);
        if (matchL.Value.Length>10){ //10 is an arbitrary number to guarantee it's not empty
            if (matchL.Value[matchL.Value.Length - 1] == ']'){
                dataL = matchL.Value.Substring(0, matchL.Value.Length - 1); //If ending is ']', then we have to remove the last character
            }
            else{
                dataL = matchL.Value.Substring(0, matchL.Value.Length - 5); //If it's 'L' or 'R', then we have to remove the last 5 characters
            }
        }

        if (matchR.Value.Length>10){ //10 is an arbitrary number to guarantee it's not empty
            if (matchR.Value[matchR.Value.Length - 1] == ']'){
                dataR = matchR.Value.Substring(0, matchR.Value.Length - 1);
            }
            else{
                dataR = matchR.Value.Substring(0, matchR.Value.Length - 5);
            }
        }
        //Create a variable to access both dataR and dataL as its elements
        dataT[0]=dataR;
        dataT[1]=dataL;
        return dataT;
    }
    // Example usage
    private void Update()
    {
        if (webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame)
        {
            texture.SetPixels(webcamTexture.GetPixels());
            texture.Apply();
            encodedImage = texture.EncodeToJPG(20);

            // Debug.Log("Encoded image size: " + encodedImage.Length);
            SendData(encodedImage);
        }
        

        if (receivedData.Length > 0)
        {
            dataRec = receivedData.ToString();
            // Debug.Log(dataRec);
            dataHands=HandleData(dataRec);
            receivedData.Clear();
        }
    }

    private void OnDestroy()
    {
        if (client != null && client.Connected)
        {
            client.Close();
        }
    }
}

