# VR_HandTracking_Archimedes

**Authors:** [_Eduardo Martin Pavez Fabriani_](https://www.linkedin.com/in/eduardo-pavez/) and [_Martin Tous_](https://martintous.net/).

This repository contains the Archimedes project.

The functionality demo is available in [this video](https://youtu.be/Dqqu_yh6Uw0).
[![Watch the video](https://img.youtube.com/vi/Dqqu_yh6Uw0/maxresdefault.jpg)](https://youtu.be/Dqqu_yh6Uw0)

Unity files can be found [here](https://drive.google.com/drive/folders/1AGd0dnqLf6OTxdLysQvqMhknENuwqijQ?usp=sharing).

In case they are not available there, contact me at edumarpav@yahoo.com.ar

---

## Description

The Archimedes project aims to provide an intuitive adaptable remote control system for exploratory robots.
We modelled a virtual reality version of a robot in Unity. This simulation could also be used to train a robot operator without risking real equipment.

The project consists of 3 scenes designed in 2 versions.
Some features that you can find in these 2 versions are:

1. Welcome: Starting scene for a computer or a mobile phone.
2. Register: You can register using an RFID card, or an augmented reality marker with a password.
3. Town: You can control the robot using either a combination of gestures and a joystick or a full gesture control.

The Handtracking script is shown to run in a Cloud server and your local machine, but any other alternative via TCP/IP is possible.

### Mobile Version

![Mobile Version](/Full%20report%2C%20images%20and%203D%20Model/MobileVersionCommunications.png)

### PC Version

![PC Version](/Full%20report%2C%20images%20and%203D%20Model/PCVersionCommunications.png)

### PC Version Hardware requirements

![PC Version Hardware](/Full%20report%2C%20images%20and%203D%20Model/PCVersionHardware.png)

---

## Usage

A detailed explanation about the project development can be found in the [project report](/Full%20report%2C%20images%20and%203D%20Model/ArchimedesReport.pdf). Currently it is only available in Spanish.

### Mobile Version

1. Download Unity project files [here](https://drive.google.com/drive/folders/1AGd0dnqLf6OTxdLysQvqMhknENuwqijQ?usp=sharing). You will want to set your own credential's in the AR scene "Register". It was done using Vuforia package.
2. Build into an Android device.
3. Run the server application either in the cloud or any alternative. You can refer to the [instructions file](/PureGestureCommands_CloudServer/Docker_and_Cloud_usage.md) on this repository for a tutorial.
4. Run the Mobile Client in your android device and set the server ip in the "Welcome" Scene.
5. Proceed to register with your credential and password in the "Register Scene".
6. When the last scene loads, touch the screen ideally looking to a not so heterogeneous background. Program will be initialized and you will be ready to put on the VR headset and start interacting with the robot. Handtracking server should print the fps in the console if everything is working properly.

### PC Version

1. Download Unity project files [here](https://drive.google.com/drive/folders/1AGd0dnqLf6OTxdLysQvqMhknENuwqijQ?usp=sharing).
2. Build a compatible hardware device. You can find the 3D model design [here](/Full%20report%2C%20images%20and%203D%20Model)
3. Load the [arduino script](/HybridCommands_PCClient/ArduinoScript/HardwareCode.ino) in a compatible hardware device. You will want to define your own RFID devices in the desiredCode variable on the Arduino File.
4. Run the Python server locally.
5. Run the Unity project locally.
6. When succesfully registered in the second scene you will be able to interact with the robot using the hardware device and the hand tracking capabilities. FPS will be printed in the server console if everything is working properly.

---

## License

This project is licensed under the MIT License.

---

## Special Thanks

Special thanks to [Murtaza's Workshop - Robotics and AI](https://www.youtube.com/@murtazasworkshop/videos) YouTube channel, which provided the cvzone library, and the base ideas behind the handtracking and the depth estimation, along with the Unity Hands game object design. Link to cvzone repository [here](https://www.youtube.com/watch?v=RQ-2JWzNc6k&t=3175s)
