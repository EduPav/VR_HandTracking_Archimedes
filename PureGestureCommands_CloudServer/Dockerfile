# Use the official Python image as the base image
FROM python:3

# Install required packages
RUN apt-get update && apt-get install -y libopencv-dev &&  rm -rf /var/lib/apt/lists/*

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Install dependencies
RUN pip install numpy opencv-python mediapipe

# Make port 65432 available to the world outside this container
EXPOSE 65432

# Run application when the container launches
CMD ["python", "HandTracking.py"]

