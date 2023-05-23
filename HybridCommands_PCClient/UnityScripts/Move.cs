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

    public ReceiveData receiveData;
    private float speed = 10f;
    int rotate=0;
    int quadrant=3;
    float t=20f;
    float rotationY;
    private string[] rightHand, leftHand;
    private string commandR, commandL, commandRlast, commandLlast;
    static float timeOut=0.1f;
    private float tL=timeOut, tR=timeOut;
    private int percentage;




    // Update is called once per frame
    void Update()
    {
        commandRlast=commandR;
        commandLlast=commandL;
        int valueX = receiveData.x;
        int valueY = receiveData.y;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //This portion of code keeps the last command for 200ms if no new command is received
        //Stores received data appropriately
        if(tcpClient.dataHands.Length>0){
            //Get handpoints
            rightHand=tcpClient.dataHands[0].Split(',');
            leftHand=tcpClient.dataHands[1].Split(',');
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

        if (commandR=="'go'"|| commandR=="'back'"|| commandR=="'left'"|| commandR=="'right'"){
            commandL="'none'";
        }

        //RIGHT HAND COMMAND
        if(commandR == "'stop'"){
            // Debug.Log("Robot Stop");
        }
        else if(commandR == "'go'"){
            percentage= Convert.ToInt32(rightHand[^1]);
            // rb.AddForce(rb.transform.right * RobotForce*percentage/100);
            rb.transform.Translate(3*Time.deltaTime*percentage/100,0, 0);
            Debug.Log("Move forward");
            Debug.Log("Velocity percentage="+percentage);
        }
        else if(commandR == "'back'"){
            percentage= Convert.ToInt32(rightHand[^1]);
            // rb.AddForce(rb.transform.right * -RobotForce*percentage/100);
            rb.transform.Translate(-3*Time.deltaTime*percentage/100,0, 0);   
            Debug.Log("Move backward");      
            Debug.Log("Velocity percentage="+percentage);          
        }
        else if(commandR == "'left'"){
            // rb.transform.Rotate(0, -30 * Time.deltaTime, 0);
            rb.transform.Rotate(0, -30 * Time.deltaTime, 0);
        }
        else if(commandR == "'right'"){
            // rb.transform.Rotate(0, 30 * Time.deltaTime, 0);
            rb.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        else{
            Debug.Log("Unrecognized command in Right Hand");
            Debug.Log(commandR);
        }

        //LEFT HAND COMMAND 
        if(commandL == "'stop'"){
            // Debug.Log("Robot Stop");
        }
        else if (commandL == "'go'"){
            percentage= Convert.ToInt32(leftHand[^1]);
            // rb.AddForce(rb.transform.right * RobotForce*percentage/100);
            rb.transform.Translate(3*Time.deltaTime*percentage/100,0, 0);
            Debug.Log("Move forward");
            Debug.Log("Velocity percentage="+percentage);
        }
        else if(commandL == "'back'"){
            percentage= Convert.ToInt32(leftHand[^1]);
            // rb.AddForce(rb.transform.right * -RobotForce*percentage/100);
            rb.transform.Translate(-3*Time.deltaTime*percentage/100,0, 0);
            Debug.Log("Move backward");      
            Debug.Log("Velocity percentage="+percentage);          
        }
        else if(commandL == "'left'"){
            // rb.transform.Rotate(0, -30 * Time.deltaTime, 0);
            rb.transform.Rotate(0, -30 * Time.deltaTime, 0);
        }
        else if(commandL == "'right'"){
            // rb.transform.Rotate(0, 30 * Time.deltaTime, 0);
            rb.transform.Rotate(0, 30 * Time.deltaTime, 0);
            
        }
        else{
            Debug.Log("Unrecognized command in Left Hand");
            Debug.Log(commandL);
        }
        


        //Joystick analog control CAMERA
        //Move down
        if(valueY<400 && valueY!=0){
            cameraParent.transform.Rotate( 0,0, -30* Time.deltaTime);
        }
        //Move up
        if(valueY>600){
            cameraParent.transform.Rotate( 0,0, 30* Time.deltaTime);
        }
        //Turn left
        if(valueX<400 && valueX!=0){
            armParent.transform.Rotate(0, -30* Time.deltaTime, 0);
        }
        //Turn right
        if(valueX>600){
            armParent.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        }
}