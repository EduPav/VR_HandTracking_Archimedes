import socket
import cv2 
import numpy as np
import time
from HandTrackingModule import HandDetector, PALM, THUMB, INDEX, MIDDLE, RING, PINKY

#INITIALIZATIONS
HOST=socket.gethostbyname(socket.gethostname())
# HOST="127.0.0.1"
PORT = 65432        # Port to listen on (non-privileged ports are > 1023)
lm_data_wcommands=[]
pTime=0
fpslist=[]
dataReceived=True
detector=HandDetector(maxHands=2,detectionCon=0.8) 


def print_fps(pTime):
    """Prints the FPS in the console. Average last 10 frames to show a more stable value."""
    cTime=time.time()
    fps=1/(cTime-pTime)
    pTime=cTime
    #average of last 10 frames
    fpslist.append(fps)
    if fpslist.__len__()>10:
        fpslist.pop(0)
    fps_avg=int(sum(fpslist)/fpslist.__len__())
    print("FPS: "+str(fps_avg))
    return pTime
    
def get_data2send(img):
    """Gets the image and returns the list of hand landmarks with the commands to send to the client.
    Output structure: ['L','command',63 coordinates(x,y,z of 21 landmarks), 'R', 'command', 63 coordinates(x,y,z of 21 landmarks)]
    In case of go or back commands in right hand, it adds 'percentage' at the end of the 'R' list, which is the distance between thumb and index tips 
    Sends an empty list if no hand is found or a single hand list if only one hand is found."""
    #Get img size
    hcam, wcam, c = img.shape

    commandL='none'
    commandR='none'
    
    hands=detector.findHands(img,draw=False,flipType=False) #Hands store both hands info: landmarks, handtype, etc
    lm_data,LeftLmList,RightLmList,LeftFingers,RightFingers=detector.getHandsInfo(hands,hcam) #Extracts info from hands into lists
    
    #GESTURE SELECTION LEFT HAND- CAMERA CONTROL
    if LeftFingers.count(1)==0:
        commandL='stop'
    elif LeftFingers==[1,0,0,0,0]:
        if detector.findDistance(THUMB,PALM,LeftLmList,distType='x')<0:
            commandL='left'
        else:
            commandL='right'
    elif LeftFingers.count(1)==5:
        if detector.findDistance(PINKY,PALM,LeftLmList,distType='y')>0:
            commandL='up'
        else:
            commandL='down'


    #GESTURE SELECTION RIGHT HAND-ROBOT DISPLACEMENT CONTROL
    if RightFingers.count(1)==0:
        commandR='stop'
    elif RightFingers==[1,0,0,0,0]:
        if detector.findDistance(THUMB,PALM,RightLmList,distType='x')<0:
            commandR='left'
        else:
            commandR='right'
    elif RightFingers[2:5]==[1,1,1]:
        if detector.findDistance(THUMB,PALM,RightLmList,distType='x')<0:
            commandR='go'
        else:
            commandR='back'
    #We calculate the distance between the tips of the index and thumb to control the speed of the robot
    #The distance is referred to the distance between the palm and the index base, so it doesn't depend on how close the hand is
    #to the camera.
    if commandR=='go' or commandR=='back':        
        distance=detector.findDistance(THUMB,INDEX,RightLmList)
        reference=detector.findDistance(PALM,INDEX-3,RightLmList)
        minDist=0.15
        maxDist=1.5
        try:
            dist_referred=distance/reference 
        except ZeroDivisionError:
            dist_referred=maxDist    
        
        if dist_referred<minDist:
            dist_referred=minDist
        elif dist_referred>maxDist:
            dist_referred=maxDist
        percentage=100-round((dist_referred-minDist)/(maxDist-minDist)*100) #Inverse to the amplitude, Slow when completely open.
    
    try:
        #Get the index of the R and L letters in the list
        indexRcommand=lm_data.index('R')
        #Insert the command after its letter in the list    
        lm_data.insert(indexRcommand+1,commandR)
        if commandR=='go' or commandR=='back':   
            #Add the percentage at the end of the handtype section in the list (Length='R'+'command'+63 coordinates=65)
            lm_data.insert(indexRcommand+65,percentage)     
    except ValueError: #In case there is no 'R' in the list
        pass
    try:
        indexLcommand=lm_data.index('L')
        lm_data.insert(indexLcommand+1,commandL)
    except ValueError:
        pass
    return lm_data


print(f'Host IP={HOST}; PORT={PORT}')

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()

    while True: #Check if works to accept new connections in case of disconnection. 
        conn, addr = s.accept()
        with conn:
            print('Connected by', addr)
            while True:
                #receive image length. 
                dataLength=conn.recv(4) #First 4 bytes are the length of the data
                #DATACHECKER
                if not dataLength:
                    print("Expecting new connection")
                    conn,addr=s.accept()
                    print('Connected by', addr)
                    continue
                print(len(dataLength))
                #Datalength is a byte array, we convert it to int
                dataLength=int.from_bytes(dataLength,byteorder='little')
                # print(dataLength)
                #Receive image
                data=conn.recv(40000) #Data average size around 30k   
                #DATACHECKER
                if not data:
                    print("Expecting new connection")
                    conn,addr=s.accept()
                    print('Connected by', addr)
                    continue
                if dataLength>70000:
                    continue

                while len(data)<dataLength:
                    datapiece=conn.recv(8192)
                    dataReceived=True
                    #DATACHECKER
                    if not datapiece:
                        dataReceived=False
                        print("Expecting new connection")
                        conn,addr=s.accept()
                        print('Connected by', addr)
                        break
                    data+=datapiece
                    
                if dataReceived==False:
                    continue
                
                img = cv2.imdecode(np.frombuffer(data, np.uint8), cv2.IMREAD_COLOR)
                #Flip image horizontally and vertically
                img=cv2.flip(img,2) 
                if img is not None:
                    lm_data_wcommands=get_data2send(img)
                    #FPS count
                    pTime=print_fps(pTime)
                    #Show image received in server
                    # cv2.imshow('frame',img)
                    # cv2.waitKey(1)
                conn.sendall(str(lm_data_wcommands).encode('utf-8'))



    
    