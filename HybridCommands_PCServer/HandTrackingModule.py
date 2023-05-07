"""
Based on
Hand Tracking Module
By: Computer Vision Zone
Website: https://www.computervision.zone/
Modified by: Eduardo Martín Pavéz Fabriani
"""

import cv2
import mediapipe as mp
import math

PALM=0
THUMB=4
INDEX=8
MIDDLE=12
RING=16
PINKY=20

class HandDetector:
    """
    Finds Hands using the mediapipe library. Exports the landmarks
    in pixel format. Adds extra functionalities like finding how
    many fingers are up or the distance between two fingers. Also
    provides bounding box info of the hand found.
    """

    def __init__(self, mode=False, maxHands=2, detectionCon=0.5, minTrackCon=0.5):
        """
        :param mode: In static mode, detection is done on each image: slower
        :param maxHands: Maximum number of hands to detect
        :param detectionCon: Minimum Detection Confidence Threshold
        :param minTrackCon: Minimum Tracking Confidence Threshold
        """
        self.mode = mode
        self.maxHands = maxHands
        self.detectionCon = detectionCon
        self.minTrackCon = minTrackCon

        self.mpHands = mp.solutions.hands
        self.hands = self.mpHands.Hands(static_image_mode=self.mode, max_num_hands=self.maxHands,
                                        min_detection_confidence=self.detectionCon,
                                        min_tracking_confidence=self.minTrackCon)
        self.mpDraw = mp.solutions.drawing_utils
        self.fingers = []
        self.lmList = []

    def findHands(self, img, draw=True, flipType=True):
        """
        Finds hands in a BGR image.
        :param img: Image to find the hands in.
        :param draw: Flag to draw the output on the image.
        :return: Image with or without drawings
        """
        imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        self.results = self.hands.process(imgRGB)
        allHands = []
        h, w, c = img.shape
        if self.results.multi_hand_landmarks:
            for handType, handLms in zip(self.results.multi_handedness, self.results.multi_hand_landmarks):
                myHand = {}
                ## lmList
                mylmList = []
                xList = []
                yList = []
                for id, lm in enumerate(handLms.landmark):
                    px, py, pz = int(lm.x * w), int(lm.y * h), int(lm.z * w)
                    mylmList.append([px, py, pz])
                    xList.append(px)
                    yList.append(py)

                ## bbox
                xmin, xmax = min(xList), max(xList)
                ymin, ymax = min(yList), max(yList)
                boxW, boxH = xmax - xmin, ymax - ymin
                bbox = xmin, ymin, boxW, boxH
                cx, cy = bbox[0] + (bbox[2] // 2), \
                         bbox[1] + (bbox[3] // 2)

                myHand["lmList"] = mylmList
                myHand["bbox"] = bbox
                myHand["center"] = (cx, cy)

                if flipType:
                    if handType.classification[0].label == "Right":
                        myHand["type"] = "Left"
                    else:
                        myHand["type"] = "Right"
                else:
                    myHand["type"] = handType.classification[0].label
                allHands.append(myHand)

                ## draw
                if draw:
                    self.mpDraw.draw_landmarks(img, handLms,
                                               self.mpHands.HAND_CONNECTIONS)
                    cv2.rectangle(img, (bbox[0] - 20, bbox[1] - 20),
                                  (bbox[0] + bbox[2] + 20, bbox[1] + bbox[3] + 20),
                                  (255, 0, 255), 2)
                    cv2.putText(img, myHand["type"], (bbox[0] - 30, bbox[1] - 30), cv2.FONT_HERSHEY_PLAIN,
                                2, (255, 0, 255), 2)
        if draw:
            return allHands, img
        else:
            return allHands
    
    def getHandsInfo(self,hands,hcam):
        lm_data=[]
        LeftLmList=[]
        RightLmList=[]
        LeftFingers=[]
        RightFingers=[]
        for hand in hands:
            #HAND LANDMARKS
            lmList=hand["lmList"]

            #HANDEDNESS DETECTION
            if hand.get("type")=="Left":   
                lm_data.append('L')
                LeftLmList=lmList
                LeftFingers=self.fingersUp(LeftLmList)
                
            else:
                lm_data.append('R')
                RightLmList=lmList
                RightFingers=self.fingersUp(RightLmList)

            #DEPTH ESTIMATION
            w = self.findDistance(PALM, INDEX-3,lmList)  # with draw    
            W=8.0 #distance in cm from landmark 0 to 5 in my hand.
            #Finding the focal Length
            # d=50
            # f = (w * d) / W
            # print(f) #680
            #Finding the distance
            f=680
            d = (W * f) / w
            #print depth near the handtype
            # cv2.putText(img, str(int(d)), (info[4], info[5]), cv2.FONT_HERSHEY_PLAIN, 2, (255, 0, 0), 2)

            #Store data to send
            d=int(7.65*d) #We convert the cm depth into a pixel value according to the z values of the landmarks. This is a rough estimation.
            for lm in lmList:
                lm_data.extend([lm[0],hcam-lm[1],lm[2]+d])
        return lm_data,LeftLmList,RightLmList,LeftFingers,RightFingers

    def fingersUp(self, myLmList):
        """
        Finds how many fingers are open and returns in a list.
        :return: List of which fingers are up
        """
        fingers=[]
        if self.findDistance(PINKY-2,THUMB,myLmList)>self.findDistance(PINKY-2,THUMB-1,myLmList):
            fingers.append(1)
        else:
            fingers.append(0)
        
        for i in range(2, 6): #2,3,4,5
            if self.findDistance(PALM,i*4,myLmList)>max(self.findDistance(PALM,i*4-3,myLmList),self.findDistance(PALM,i*4-2,myLmList)):
                fingers.append(1)
            else:
                fingers.append(0)
        return fingers
    
    def findDistance(self,p1,p2,lmList,distType='Total'):
        """It returns the distance between two landmarks.
        Total distance is always positive, but the others aren't."""
        distance=0
        x1=lmList[p1][0]
        y1=lmList[p1][1]
        z1=lmList[p1][2]
        x2=lmList[p2][0]
        y2=lmList[p2][1]
        z2=lmList[p2][2]
        if distType=='Total':
            distance=math.hypot(x2-x1,y2-y1,z2-z1)
        elif distType=='x':
            distance=x2-x1
        elif distType=='y':
            distance=y2-y1
        elif distType=='z':
            distance=z2-z1
    
        return distance;   


def main():
    cap = cv2.VideoCapture(0)
    detector = HandDetector(detectionCon=0.8, maxHands=2)
    while True:
        # Get image frame
        success, img = cap.read()
        # Find the hand and its landmarks
        hands, img = detector.findHands(img)  # with draw
        # hands = detector.findHands(img, draw=False)  # without draw

        if hands:
            # Hand 1
            hand1 = hands[0]
            lmList1 = hand1["lmList"]  # List of 21 Landmark points
            bbox1 = hand1["bbox"]  # Bounding box info x,y,w,h
            centerPoint1 = hand1['center']  # center of the hand cx,cy
            handType1 = hand1["type"]  # Handtype Left or Right

            fingers1 = detector.fingersUp(hand1)

            if len(hands) == 2:
                # Hand 2
                hand2 = hands[1]
                lmList2 = hand2["lmList"]  # List of 21 Landmark points
                bbox2 = hand2["bbox"]  # Bounding box info x,y,w,h
                centerPoint2 = hand2['center']  # center of the hand cx,cy
                handType2 = hand2["type"]  # Hand Type "Left" or "Right"

                fingers2 = detector.fingersUp(hand2)

                # Find Distance between two Landmarks. Could be same hand or different hands
                length, info, img = detector.findDistance(lmList1[8][0:2], lmList2[8][0:2], img)  # with draw
                # length, info = detector.findDistance(lmList1[8], lmList2[8])  # with draw
        # Display
        cv2.imshow("Image", img)
        cv2.waitKey(1)


if __name__ == "__main__":
    main()
