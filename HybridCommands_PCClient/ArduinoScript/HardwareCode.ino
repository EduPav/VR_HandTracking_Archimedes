#include <SPI.h>
#include <MFRC522.h>

#define RST_PIN  9       // Pin 9 for RC522 reset
#define SS_PIN_ENB  10   // Pin 10 for RC522 SS (SDA)

int analog_x;
int analog_y;
int button;
int button_array[4] = {1, 1, 1, 1}; // Initialize the array with four ones

MFRC522 MyReaderRF(SS_PIN_ENB, RST_PIN); // Create the object for RC522

const byte desiredCode[] = {0x6A, 0xB5, 0xEA, 0x81}; // Desired code
const byte green = 3; // Pin for green LED
const byte red = 4; // Pin for red LED

void setup() {
  Serial.begin(9600); // Start serial communication
  SPI.begin();        // Start SPI bus
  MyReaderRF.PCD_Init(); // Initialize MyReaderRF
  pinMode(3,OUTPUT);
  pinMode(green, OUTPUT); // Initialize green LED as output
  pinMode(red, OUTPUT); // Initialize red LED as output
  pinMode(7, INPUT_PULLUP); // Initialize joystick button as input with internal pull-up resistor
}

void loop() {
  analog_x = analogRead(A1);
  if (analog_x == 0){
    analog_x = 1;
  }
  analog_y = analogRead(A0);
  if (analog_y == 0){
    analog_y = 1;
  }
  button = digitalRead(7);
  Serial.print(analog_x);
  Serial.print(",");
  Serial.print(analog_y);
  Serial.print(",");

  // Add the current button value to the end of the array and remove the first element
  for (int i = 0; i < 3; i++) {
    button_array[i] = button_array[i+1];
  }
  button_array[3] = button;

  // Check if all elements of the array are equal to 1, except the last one
  bool button_pressed = true;
  for (int i = 0; i < 3; i++) {
    if (button_array[i] != 1) {
      button_pressed = false;
      break;
    }
  }
  if (button_pressed && button_array[3] == 0) {
    Serial.print("0");
    Serial.print(",");
  } else {
    Serial.print("1");
    Serial.print(",");
  }
  
  // Check if a card has been detected
  if (MyReaderRF.PICC_IsNewCardPresent()) {  
    // Determine the card code
    if (MyReaderRF.PICC_ReadCardSerial()) {
      // Retrieve the ID of the card
      for (byte i = 0; i < MyReaderRF.uid.size; i++) {
        Serial.print(MyReaderRF.uid.uidByte[i] < 0x10 ? " 0" : " ");
        Serial.print(MyReaderRF.uid.uidByte[i], HEX);
      }
        
      Serial.println();
      // Finish reading the current card
      MyReaderRF.PICC_HaltA();

      // Compare the read code with the desired code
      bool correctCode = true;
      for (byte i = 0; i < sizeof(desiredCode); i++) {
        if (MyReaderRF.uid.uidByte[i] != desiredCode[i]) {
          correctCode = false;
          break;
        }
      }

      // Turn on the green or red LED accordingly
      if (correctCode) {
        digitalWrite(green, HIGH);
        digitalWrite(red, LOW);
      } else {
        digitalWrite(green, LOW);
        digitalWrite(red, HIGH);
      }
    }
  } else {
    Serial.print("nothing");
    Serial.println();
  }
  delay(40);
}
