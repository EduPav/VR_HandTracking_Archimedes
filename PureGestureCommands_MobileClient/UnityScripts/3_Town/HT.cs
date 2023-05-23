using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HT : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject[] handPoints;
    Vector3 pulsar=new Vector3(0,0,0);

    private string[] rightHand, leftHand;

    // Update is called once per frame
    void Update()
    {
        if (tcpClient.dataHands.Length==0){
            for(int i=0;i<42;i++)
                handPoints[i].transform.localPosition=new Vector3(100,100,100); //Send hands away if no data is received
            return;
        }
        rightHand=tcpClient.dataHands[0].Split(',');
        leftHand=tcpClient.dataHands[1].Split(',');

        if(rightHand.Length==1 || leftHand.Length==1) //Fill with zeros in case any of the hands is empty.
        {    for(int i=0;i<42;i++)
                handPoints[i].transform.localPosition=new Vector3(0,0,0);
        }

        if(rightHand.Length>1){//Right Hand data received
            if (rightHand[1][1..]=="'go'" || rightHand[1][1..]=="'back'"){
                //Drop last rightHand element. It's the velocity and is not relevant to position hands. 
                rightHand=rightHand[0..^1];
            }
            //drop first element ('R' and 'L' letters) and second element (command)
            rightHand=rightHand[2..];
        }

        if(leftHand.Length>1){//Left Hand data received
            //drop first element ('R' and 'L' letters) and second element (command)
            leftHand=leftHand[2..];
        }
        
        
        //Combine both hands in points array
        string[] points;
        if(rightHand.Length>1 && leftHand.Length>1){
            points = new string[leftHand.Length + rightHand.Length];
            Array.Copy(leftHand, points, leftHand.Length);
            Array.Copy(rightHand, 0, points, leftHand.Length, rightHand.Length);
        }
        else if(rightHand.Length>1){
            points = new string[rightHand.Length];
            Array.Copy(rightHand, points, rightHand.Length);
        }
        else if(leftHand.Length>1){
            points = new string[leftHand.Length];
            Array.Copy(leftHand, points, leftHand.Length);
        }
        else{
            points = new string[0];
        }
        
        for (int i=0;i<points.Length/3;i++)
        {
            float x=-float.Parse(points[3*i])/50; //Parse needed because they were still strings
            float y=float.Parse(points[3*i+1])/50;
            float z=float.Parse(points[3*i+2])/50;

            handPoints[i].transform.localPosition=new Vector3(x,y,z);
        }
        
    }
}

