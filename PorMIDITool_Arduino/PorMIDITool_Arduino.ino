#include<EEPROM.h>
int addpstatus[10] ={0,0,0,0,0,0,0,0,0,0};
bool bstatus[16] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
int colors[7][3]={{0,1023,1023},{1023,0,1023},{1023,1023,0},{0,0,1023},{1023,0,0},{0,1023,0},{0,0,0}};
int colorData[16][2]={{1,0},{0,1},{0,2},{1,3},{0,4},{0,5},{0,6},{0,0},{1,1},{0,2},{0,3},{0,4},{0,5},{0,6},{0,0},{0,0}};
String* midiMode_info = new String[59];
bool midiMode = true;
#define pulse 8//SCL
#define data 9//SDO
#define savedatavol 59

bool GetBit()
{
  digitalWrite(pulse, LOW);
  delayMicroseconds(2); // 500KHz
  bool retVal = !digitalRead(data);
  digitalWrite(pulse, HIGH);
  delayMicroseconds(2); // 500KHz
  return retVal;
}

void midi_Send(String str, String byte1, int byte2) {
  char str1[4]; str.toCharArray(str1, str.length());
  Serial.write(str1);
  char str2[3]; byte1.toCharArray(str2, byte1.length());
  Serial.write(str2);
  char str3[3]; String(byte2).toCharArray(str3, String(byte2).length());
  Serial.write(str3);
}

void setup() {
  //Get midi_byte1_data from 50-27 EEPROM
  for (int i = 0; i < savedatavol; i++) {
    midiMode_info[i] = EEPROM.read(i);
  }
  Serial.begin(38400);
  Serial.setTimeout(50);
  //pinMode(10, INPUT_PULLUP);
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);
  pinMode(pulse, OUTPUT);
  pinMode(data, INPUT);
  if (digitalRead(10) == LOW) {
    midiMode = false;
    String str = "";
    while (1) {
      str = "";
      Serial.println("I'm PMT!");
      if (Serial.available()) {
        str = Serial.readString();
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
  }
}
int datacol = 0;
bool status = 0;
void loop() {
  if ((Serial.available()) && (midiMode == false)) {
    String strread = Serial.readString();
    if (strread.substring(0,16) == "write_midiinfoA1") {
        Serial.println(strread);
        //memcpy(midiMode_info, Split(strread.substring(16,59), '_'), 59);
        //String *saveData=Split(strread.substring(16,strread.length()), '_');
        //for (int i = 0; i < 59; i++)Serial.println(saveData[i]);
          //EEPROM.write(i, midiMode_info[i].toInt());
      }
     else if (strread.substring(0,11) == "ColorChange") {
      String *colorDataGet=Split(strread.substring(11,strread.length()),'_');
      for(int i=0;i<16;i++){
        colorData[i][0]=colorDataGet[i].substring(0,1).toInt();
        colorData[i][1]=colorDataGet[i].substring(1,2).toInt()-1;
      }
      delete[]colorDataGet;
    } else if (strread == "I'm PMTM!") {
      Serial.print(midiMode);
    }
  }
  /*for (byte cnt = 0; cnt < 16; cnt++)
  {
    if (GetBit()) {

      if (bstatus[cnt] == 0) {
        bstatus[cnt] = 1;
        if (midiMode) {
          midi_Send("0x9" + String(midiMode_info[29].toInt(), HEX), midiMode_info[cnt], 127);
        } else {
          Serial.print("A1 "); Serial.print("b "); Serial.print(cnt + 1); Serial.println(" on");
        }
      }
    }
    else {
      if (bstatus[cnt] == 1) {
        bstatus[cnt] = 0;
        if (midiMode) {
          midi_Send("0x8" + String(midiMode_info[29].toInt(), HEX), midiMode_info[cnt], 127);
        } else {
          Serial.print("A1 "); Serial.print("b "); Serial.print(cnt + 1); Serial.println(" off");
        }

      }
    }
  }
*/
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
        if (j < 5) {
          if ((analRead > 125)&&(addpstatus[j]< 125)) {
            addpstatus[j]=analRead;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17], 127);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(127);
            }
          }else if((analRead < 125)&&(addpstatus[j]> 125)){
            addpstatus[j]=analRead;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17], 0);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(0);
            }
          }
        } else {
          if ((addpstatus[j] - analRead >= 2) || (addpstatus[j] - analRead <= -2)) {
            addpstatus[j] = analRead;if(addpstatus[j]==126)addpstatus[j] = analRead = 127;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[0].toInt(), HEX), midiMode_info[j + 17], analRead);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(analRead);
            }
          }
        }
      } else if (i > 2) {
        int analRead = map(analogRead(A0), 0, 1023, 0, 127);
        if ((addpstatus[5 + i] - analRead >= 2) || (addpstatus[5 + i] - analRead <= -2)) {
          addpstatus[5 + i] = analRead;if(addpstatus[5 + i]==126)addpstatus[5 + i] = analRead = 127;
          if (midiMode) {
            midi_Send("0xB" + String(midiMode_info[0].toInt(), HEX), midiMode_info[22 + i], analRead);
          } else {
            Serial.print("A1 "); Serial.print("cc "); Serial.print(String(6 + i)+" "); Serial.println(analRead);
          }
        }
      } else{
        if (midiMode)if(((bstatus[j + (i - 1) * 8] == 1)&&(midiMode_info[(j + (i - 1) * 8)*2+27]=="1"))or(midiMode_info[(j + (i - 1) * 8)*2+27]=="0")){
          pinMode(A0,OUTPUT);analogWrite(A0,1023);analogWrite(A1,colors[colorData[j + (i - 1) * 8][1]][0]);analogWrite(A2,colors[colorData[j + (i - 1) * 8][1]][1]);analogWrite(A3,colors[colorData[j + (i - 1) * 8][1]][2]);analogWrite(A0,0);pinMode(A0,INPUT);
        }
        if (!midiMode)if(((bstatus[j + (i - 1) * 8] == 1)&&(colorData[j + (i - 1) * 8][0]==1))or(colorData[j + (i - 1) * 8][0]==0)){
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
