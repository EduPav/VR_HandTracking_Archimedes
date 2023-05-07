> **Important Note:** These instructions are left here for future reference and there is no guarantee that they will work perfectly. If you encounter any issues or errors during the process, please let me know about them. It's always a good practice to read the documentation and do some research before attempting to deploy to the cloud.

## Building the Docker Image

1. First, create a Dockerfile in a folder that contains the HandTracking.py and HandTrackingModule.py scripts.
2. Run the following command to build the Docker image and tag it with a name (`epavez/ht` in this case): 

   ```
   docker build . -t epavez/ht
   ```

3. Wait for the Docker image to build. Once it's done, you should see a message confirming that the image has been successfully built.



## Pushing to Docker Hub
1. Create a Docker Hub account and login from the terminal.

2. Run the following command to push the Docker image to Docker Hub: 

   ```
   docker push epavez/ht
   ```

   This command will upload the Docker image to your Docker Hub account so that you can pull it later on any machine.


## Cloud Setup

1. Sign in to AWS.
2. Set the server location to São Paulo, which is the fastest region for me. You can check your fastest region [here](https://www.cloudping.info/).
3. Go to "Instances" and create a new instance.
4. Select "Amazon Linux 2 AMI (HVM), SSD Volume Type (Free tier)" as the machine image.
5. Select "t2.micro" as the instance type (which is free tier eligible) or "c6in.large" for a cheap, 2vCPU, 4GB RAM instance.
6. Launch the instance.
7. Select the instance you just created and navigate to the "Security Groups" tab.
8. Edit the inbound rules by adding a new rule for TCP traffic with a personalized port (in this case, `65432`) and the source set to "Anywhere IPv4".
9. Connect to the instance using SSH or directly from the webpage.

## Cloud Usage

1. Once you're connected to the instance, update the installed packages and package cache by running the following command:

   ```
   sudo yum update -y
   ```

2. Install the most recent Docker Community Edition package by running the following command:

   ```
   sudo yum install docker -y
   ```

3. Start the Docker service with the following command:

   ```
   sudo service docker start
   ```

4. Add the `ec2-user` to the Docker group so that you can execute Docker commands without using `sudo`:

   ```
   sudo usermod -a -G docker ec2-user
   ```

5. Verify that the `ec2-user` can run Docker commands without `sudo`:

   ```
   docker --version
   ```

   or

   ```
   docker info
   ```



6. If you've just installed Docker, reboot the instance and connect to it manually again:

   ```
   sudo reboot
   ```

7. Once you're connected again, start the Docker service once more:

   ```
   sudo service docker start
   ```

8. To pull the Docker image you uploaded to Docker Hub earlier, run the following command:

   ```
   docker pull epavez/ht
   ```

9. To run the container, use the following command:

   ```
   docker run -it -p 65432:65432 epavez/ht
   ```

   If you need to map a volume from your local machine to the container (for debugging or profiling), use the following command instead:

   ```
   docker run -it -p 65432:65432 -v "$(pwd)":/app epavez/ht
   ```


10. Ignore the private IP address and use the public IP address instead.
11. AFTER USING THE CONTAINER, STOP IT AND TURN OFF YOUR INSTANCE TO AVOID CHARGES

That's it! With these instructions, you should be able to easily build, push, and deploy your Docker container to the cloud.

