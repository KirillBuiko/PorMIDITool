#include<EEPROM.h>
#include<SoftwareSerial.h>
#include <Wire.h>
#define ttp229 (0xAF >> 1)

uint16_t data_out = 0;
uint16_t data1, data2;
int addpstatus[10] ={0,0,0,0,0,0,0,0,0,0};
bool bstatus[16] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
int colors[7][3]={{0,1023,1023},{1023,0,1023},{1023,1023,0},{0,0,1023},{1023,0,0},{0,1023,0},{0,0,0}};
int colorData[16][2]={{1,0},{0,1},{0,2},{1,3},{0,4},{0,5},{0,6},{0,0},{1,1},{0,2},{0,3},{0,4},{0,5},{0,6},{0,0},{0,0}};
String* midiMode_info = new String[59];
bool midiMode = true;
#define savedatavol 59
SoftwareSerial modSerial(8,9);

void setup() {
  //Get midi_byte1_data from 50-27 EEPROM
  modSerial.setTimeout(0);
  modSerial.begin(90000);
  Serial.setTimeout(2);
  pinMode(10, INPUT_PULLUP);
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);
  Wire.setTimeout(0);
  Wire.begin();
  if (digitalRead(10) == LOW) {
    Serial.begin(115200);
    midiMode = false;
    String str = "";
    while (1) {
      str = "";
      Serial.println("I'm PMT!");
      modSerial.println("I'm PMT!");
      if (Serial.available()||modSerial.available()) {
        if(Serial.available())str = Serial.readString();
        else str=modSerial.readString();
        if (str.substring(0,11) == "I hear you!")
        {
          String *colorDataGet=Split(str.substring(11,str.length()),'_');
          for(int i=0;i<16;i++){
            colorData[i][0]=colorDataGet[i].substring(0,1).toInt();
            colorData[i][1]=colorDataGet[i].substring(1,2).toInt()-1;
          }delete[]colorDataGet;
          break;
        }
      }
      delay(500);
    }
  }else{
    Serial.begin(31250);
    for (int i = 0; i < savedatavol; i++) {
      midiMode_info[i] = EEPROM.read(i);
    }
    for(int i=0;i<16;i++){
      colorData[i][0]=midiMode_info[27+i*2].toInt();
      colorData[i][1]=midiMode_info[27+i*2+1].toInt()-1;
    }

  }
}
int datacol = 0;
bool status = 0;
void loop() {
  if (modSerial.available()||Serial.available()) {
    String strread="";int serialMode=0;
    if(modSerial.available()){
      strread=modSerial.readString();
      serialMode=1;
    } else strread=Serial.readString();
    if (strread.substring(0,16) == "write_midiinfoA1") {
        //Serial.println(strread);
        //memcpy(midiMode_info, Split(strread.substring(16,59), '_'), 59);
          for(int j=0;j<15;j++){
            if(serialMode==0) {
              while(!Serial.available());
              strread = Serial.readString();
            }
            else {
              while(!modSerial.available());
              strread = modSerial.readString();
            }
            String *saveData=Split(strread, '_');
            for (int i = 0; i < 4; i++)//Serial.println(saveData[i].toInt());//1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_1_
            EEPROM.write(j*4+i, saveData[i].toInt());
            delete[]saveData;
          }
      }
     else if (strread.substring(0,11) == "ColorChange") {
      for(int i=0;i<16;i++){
        if(serialMode==0) {
          while(!Serial.available());
          strread = Serial.readString();
        }
        else {
          while(!modSerial.available());
          strread = modSerial.readString();
        }
        colorData[i][0]=strread.substring(0,1).toInt();
        colorData[i][1]=strread.substring(1,2).toInt()-1;
        }
    }else if (strread.substring(0,9) == "I'm PMTM!") {
      modSerial.print("I hear you, M!"+String(midiMode));
      if(serialMode==1)
      Serial.print("ModuleConnected "+strread.substring(9,strread.length())+'\n');
      else modSerial.print("ModuleConnected "+strread.substring(9,strread.length())+'\n');
    }else{
      Serial.print(strread);
      modSerial.print(strread);
    }
  }
  Wire.requestFrom(ttp229, 2, true);

  delay(1);
  while(Wire.available())
  {
    data1 = Wire.read();
    data2 = Wire.read();

    data_out = (data1 << 8) | data2;
  for (byte cnt = 0; cnt < 16; cnt++)
  {
  uint16_t contrast = 0x8000;
  if(data_out & contrast){

      if (bstatus[cnt] == 0) {
        bstatus[cnt] = 1;
        if (midiMode) {
          midi_Send("9" + String(midiMode_info[29].toInt(), HEX), midiMode_info[cnt+1].toInt(), 127);
        } else {
          Serial.print("A1 "); Serial.print("b "); Serial.print(cnt + 1); Serial.println(" on");
        }
      }
    }
    else {
      if (bstatus[cnt] == 1) {
        bstatus[cnt] = 0;
        if (midiMode) {
          midi_Send("9" + String(midiMode_info[29].toInt(), HEX), midiMode_info[cnt+1].toInt(), 0);
        } else {
          Serial.print("A1 "); Serial.print("b "); Serial.print(cnt + 1); Serial.println(" off");
        }

      }
    }
    data_out <<= 1;
  }
}
  for (byte i = 0; i <= 4 ; i++) {
    digitalWrite(2, bitRead(i, 0));
    digitalWrite(3, bitRead(i, 1));
    digitalWrite(4, bitRead(i, 2));
    for (byte j = 0; j <= 7 ; j++) {
      digitalWrite(5, bitRead(j, 0));
      digitalWrite(6, bitRead(j, 1));
      digitalWrite(7, bitRead(j, 2));
      if (i == 0) {
        int analRead = map(analogRead(A0), 0, 1023, 0, 127);
        if ((j<5)) {
          if ((analRead <125) && (addpstatus[j]>125)) {
            addpstatus[j]=analRead;
            if (midiMode) {
              midi_Send("B" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17].toInt(), 0);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(0);
              //delay(1000);
            }
          }else if ((analRead >125) && (addpstatus[j]<125)) {
              addpstatus[j]=analRead;
              if (midiMode) {
                midi_Send("B" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17].toInt(), 127);
              } else {
                Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(127);
              }
            }
        } else
        {
          if ((addpstatus[j] - analRead >= 2) || (addpstatus[j] - analRead <= -2)) {
            addpstatus[j] = analRead;if(addpstatus[j]==126)addpstatus[j] = analRead = 127;
            if(addpstatus[j]==1)addpstatus[j] = analRead = 0;
            if (midiMode) {
              midi_Send("B" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17].toInt(), analRead);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(analRead);
            }
          }
        }
      } else if (i > 2) {
        int analRead = map(analogRead(A0), 0, 1023, 0, 127);
        if ((addpstatus[5 + i] - analRead >= 2) || (addpstatus[5 + i] - analRead <= -2)) {
          addpstatus[5 + i] = analRead;if(addpstatus[5 + i]==126)addpstatus[5 + i] = analRead = 127;
          if(addpstatus[5 + i]==1)addpstatus[5 + i] = analRead = 0;
          if (midiMode) {
            midi_Send("B" + String(midiMode_info[0].toInt(), HEX), midiMode_info[22 + i].toInt(), analRead);
          } else {
            Serial.print("A1 "); Serial.print("cc "); Serial.print(String(6 + i)+" "); Serial.println(analRead);
          }
        }
      } else{
        if(((bstatus[j + (i - 1) * 8] == 1)&&(colorData[j + (i - 1) * 8][0]==1))or(colorData[j + (i - 1) * 8][0]==0)){
          pinMode(A0,OUTPUT);analogWrite(A0,1023);analogWrite(A1,colors[colorData[j + (i - 1) * 8][1]][0]);analogWrite(A2,colors[colorData[j + (i - 1) * 8][1]][1]);analogWrite(A3,colors[colorData[j + (i - 1) * 8][1]][2]);delay(1);analogWrite(A0,0);pinMode(A0,INPUT);
        }
      }
    }
  }
}
/*for (byte i = 0b001; i <= 0b010 ; i++) {
    digitalWrite(2, bitRead(i, 0));
    digitalWrite(3, bitRead(i, 1));
    digitalWrite(4, bitRead(i, 2));
    for (byte j = 0b000; j <= 0b111 ; j++) {
      digitalWrite(5, bitRead(j, 0));
      digitalWrite(6, bitRead(j, 1));
      digitalWrite(7, bitRead(j, 2));

      if ((s < 900) && (bstatus[(i - 1) * 8 + datacol]) == false) {
        Serial.print("b "); Serial.print((i - 1) * 8 + datacol + 1); Serial.println(" on");
        bstatus[(i - 1) * 8 + datacol] = true;
      }
      if ((s > 900) && (bstatus[(i - 1) * 8 + datacol]) == true) {
        Serial.print("b "); Serial.print((i - 1) * 8 + datacol + 1); Serial.println(" off");
        bstatus[(i - 1) * 8 + datacol] = false;
      }*/
