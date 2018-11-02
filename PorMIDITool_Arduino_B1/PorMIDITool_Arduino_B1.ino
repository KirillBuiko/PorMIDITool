#include <ardumidi.h>
#include <EEPROM.h>

int ledPin = 13;
bool but_on[5] = {0, 0, 0, 0, 0};
int mode_active = 0;
int midiMode = false;
int midiMode_info[4];
int channel;
int mode = 0;

void setup()
{
  Serial.begin(90000);
  Serial.setTimeout(2);
  pinMode(13, OUTPUT);

  pinMode(2, INPUT_PULLUP);
  pinMode(3, INPUT_PULLUP);
  pinMode(4, INPUT_PULLUP);
  pinMode(5, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);

  pinMode(7, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(9, OUTPUT);
  pinMode(10, OUTPUT);

  digitalWrite(10, HIGH);
  delay(100);
  digitalWrite(9, HIGH);
  delay(100);
  digitalWrite(8, HIGH);
  delay(100);
  digitalWrite(7, HIGH);
  delay(100);
  digitalWrite(7, LOW);
  delay(100);
  digitalWrite(8, LOW);
  delay(100);
  digitalWrite(9, LOW);
  delay(100);
  digitalWrite(10, LOW);
  digitalWrite(13, HIGH);
  delay(50);
  digitalWrite(13, LOW);
  delay(300);
  mode = EEPROM.read(5);
  for (int i = 0; i < 5; i++)midiMode_info[i] = EEPROM.read(i);
  while (1) {
    if (Serial.available()) {
    String strread=Serial.readString();
    if(strread.substring(0,14)=="I hear you, M!"){
      if(strread.substring(14,15)=="0"){
        midiMode=0;
      }else{
        channel = midiMode_info[0];
        midiMode = 1;
      }
      break;
    }
    midiMode = Serial.readString().toInt();
  }

    Serial.print("I'm PMTM!B1FS");
    delay(2000);
  }
  if(mode==1){
      mode_active=0;
      digitalWrite(mode_active+7, HIGH);
    }

}
void loop() {
  if ((Serial.available()) && (midiMode == false)) {
    String strread = Serial.readString();
    if (strread == "write_midiinfoB1FS") {
      int num=0;
      while(!Serial.available());
      int count = Serial.readString().toInt();
      for(int j=0;j<count;j++){
        while(!Serial.available());
        strread = Serial.readString();
        if(j==0){
          EEPROM.write(0, strread.toInt());
          continue;
        }
        String *saveData=Split(strread, '_');
        if(saveData[0].substring(0,2)=="tx")
        EEPROM.write(saveData[0].substring(2,3).toInt(), saveData[1].toInt());
        delete[]saveData;
      }
    }
  }
  //////////////////////////////////////////////////1////////////////////////
  if (digitalRead(2) == false) {
    if (but_on[0] == false) {
      but_on[0] = true;
      //Serial.print("1 on, ");Serial.println(mode);
    }
  } else if (but_on[0] == true) {
    but_on[0] = false;
    //Serial.print("1 off, ");Serial.println(mode);
    digitalWrite(7, LOW);
    digitalWrite(8, LOW);
    digitalWrite(9, LOW);

    mode = 1 - mode;
    EEPROM.write(5, mode);
    if (mode == 0) {
      for (int i = 0; i < 5; i++) {
        but_on[i] = 0;
      }
    }else digitalWrite(mode_active+7, HIGH);
  }
  for (int j = 0; j < 3; j++) {
    //////////////////////////////////////////////////2////////////////////////
    if (digitalRead(j + 3) == false) {
      if (but_on[j + 1] == false) {
        //Serial.print("2 on, ");Serial.println(mode);
        but_on[j + 1] = true;
        if (mode == 0) {
          digitalWrite(j + 7, HIGH);
          if (midiMode == 0) {
            Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 127");
          } else
          midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 127);
          if (midiMode == 0){
            Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 127");
          }
          else
            midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 127);
        } else {
          if (mode_active != j) {
            digitalWrite(7, LOW);
            digitalWrite(8, LOW);
            digitalWrite(9, LOW);
            digitalWrite(j + 7, HIGH);
            mode_active = j;
            if (midiMode == 0) {
              Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 127");
              Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 0");
            } else {
              midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 127);
              midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 0);
            }
          }
        }
      }
    } else if (but_on[j + 1] == true) {
      but_on[j + 1] = false;
      //Serial.print("2 off, ");Serial.println(mode);
      if (mode == 0) {
        digitalWrite(j + 7, LOW);
        if (midiMode == 0) {
          Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 0");
        } else
        midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 0);
      } else {
        if (mode_active != j) {
          digitalWrite(8, LOW);
          digitalWrite(9, LOW);
          digitalWrite(10, LOW);
          digitalWrite(j + 7, HIGH);
          mode_active = j;
          if (midiMode == 0) {
            Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 127");
            Serial.print("B1FS "); Serial.print("cc tx"); Serial.println(String(j + 1) + " 0");
          } else {
            midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 127);
            midi_Send("B" + String(channel, HEX), midiMode_info[j+1], 0);
          }
        }
      }
    }
  }
  //////////////////////////////////////////////////5////////////////////////
  if (digitalRead(6) == false) {
    if (but_on[4] == false) {
      //Serial.print("5 on, ");Serial.println(mode);
      but_on[4] = true;
      digitalWrite(10, HIGH);
      if (midiMode == 0){
        Serial.print("B1FS "); Serial.println("cc tx4 127");
      }
      else
        midi_Send("B" + String(channel, HEX), midiMode_info[4], 0);
    }
  } else if (but_on[4] == true) {
    but_on[4] = false;
    //Serial.print("5 off, ");Serial.println(mode);
    digitalWrite(10, LOW);
    if (midiMode == 0){
      Serial.print("B1FS "); Serial.println("cc tx4 0");
    }
    else
      midi_Send("B" + String(channel, HEX), midiMode_info[4], 127);
  }
}
