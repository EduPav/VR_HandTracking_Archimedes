using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    public Rigidbody rb;
    public Transform armParent;
    public Transform cameraParent;
    public GameObject[] handPoints;
    public TCPClient tcpClient;

    private string commandR, commandL, commandRlast, commandLlast;
    private string [] rightHandList, leftHandList;
    static float timeOut=0.1f;
    private float tL=timeOut, tR=timeOut;
    private int percentage;


    //Create a function that manages left hand command:
    private void CameraCommands(string command){
        if(command == "'stop'"){
            // Debug.Log("Robot Stop");
        }
        else if (command == "'up'"){
            cameraParent.transform.Rotate( 0,0, 30* Time.deltaTime);
        }
        else if(command == "'down'"){
            cameraParent.transform.Rotate( 0,0, -30* Time.deltaTime);
        }
        else if(command == "'left'"){
            armParent.transform.Rotate(0, -30* Time.deltaTime, 0);
        }
        else if(command == "'right'"){
            armParent.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        else{
            Debug.Log("Unrecognized command for Camera");
            Debug.Log(command);
        }
    }

    private void RobotCommands(string command,string[] Handlist){
        if(command == "'stop'"){
            // Debug.Log("Robot Stop");
        }
        else if(command == "'go'"){
            percentage= Convert.ToInt32(Handlist[^1]);
            rb.transform.Translate(3*Time.deltaTime*percentage/100,0, 0);
            Debug.Log("Move forward");
            Debug.Log("Velocity percentage="+percentage);
        }
        else if(command == "'back'"){
            percentage= Convert.ToInt32(Handlist[^1]);
            rb.transform.Translate(-3*Time.deltaTime*percentage/100,0, 0);   
            Debug.Log("Move backward");      
            Debug.Log("Velocity percentage="+percentage);          
        }
        else if(command == "'left'"){
            rb.transform.Rotate(0, -30 * Time.deltaTime, 0);
        }
        else if(command == "'right'"){
            rb.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        else{
            Debug.Log("Unrecognized command for robot displacement");
            Debug.Log(command);
        }
    }

    private (string, string, string[], string[]) ExtractCommands(TCPClient tcp, string commandLlast, string commandRlast){
        string[] rightHand=default(string[]), leftHand=default(string[]);
        string commandR, commandL;
        //Command extraction
        if(tcp.dataHands.Length>0){
            //Get handpoints
            rightHand=tcp.dataHands[0].Split(',');
            leftHand=tcp.dataHands[1].Split(',');
            if (rightHand.Length>1){
                commandR=rightHand[1][1..]; //Take out the starting element of the command, which is invisible
                rightHand=rightHand[1..]; //drop first element ('R' and 'L' letters)
            }
            else {//If not enough data is received, command is assigned to 'none'
                commandR="'none'";
            }
            if (leftHand.Length>1){
                commandL=leftHand[1][1..];
                leftHand=leftHand[1..];
            }
            else {//If not enough data is received, command is assigned to 'none'
                commandL="'none'";
            }
        }
        else {
            commandR="'none'";
            commandL="'none'";
        }

        //Command stays the same for 200ms if no new command is received
        if(commandR =="'none'"){
            tR-=Time.deltaTime;
            commandR=commandRlast;
        }
        else{
            tR=timeOut;
        }
        if(commandL == "'none'"){
            tL-=Time.deltaTime;
            commandL=commandLlast;
        }
        else{
            tL=timeOut;
        }

        if(tR<0){
            commandR="'stop'";
        }
        if(tL<0){
            commandL="'stop'";
        }

        return (commandR, commandL,rightHand,leftHand);
    }
    //Receives camera commands and assigns them

    private void Start(){
        commandR="'stop'";
        commandL="'stop'";
    }
    // Update is called once per frame
    private void Update()
    {
        commandRlast=commandR;
        commandLlast=commandL;
        
        // Current position of the robot
        Vector3 currentPosition = rb.position;

        //extract Commands and keeps last when no new has arrived
        (commandR, commandL, rightHandList, leftHandList)=ExtractCommands(tcpClient,commandRlast,commandLlast);

        //RIGHT HAND COMMAND
        RobotCommands(commandR, rightHandList);

        //LEFT HAND COMMAND 
        CameraCommands(commandL); 
        
    }
}
