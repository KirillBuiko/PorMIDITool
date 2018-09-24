#include<EEPROM.h>
int addpstatus[10] ={0,0,0,0,0,0,0,0,0,0};
bool bstatus[16] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
String* midiMode_info = new String[30];
bool midiMode = true;
#define pulse 8//SCL
#define data 9//SDO
#define savedatavol 30

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
  for (int i = 50; i < 50 + savedatavol; i++) {
    midiMode_info[i - 50] = EEPROM.read(i);
  } analogWrite(A1, map(midiMode_info[26].toInt(), 0, 255, 0, 1023)); analogWrite(A2, map(midiMode_info[27].toInt(), 0, 255, 0, 1023)); analogWrite(A3, map(midiMode_info[28].toInt(), 0, 255, 0, 1023));
  Serial.begin(38400);
  Serial.setTimeout(100);
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
      delay(500);
      str = "";
      Serial.println("I'm PMT!");
      while (Serial.available()) {
        str = Serial.readString();
      }
      if (str == "I hear you!\n")break;
      else Serial.println(str);
    }
  }
  analogWrite(A1,1023);
}
int datacol = 0;
bool status = 0;
void loop() {
  if ((Serial.available()) && (midiMode == false)) {
    String strread = Serial.readString();
    if (strread == "write_midiinfoA1") {
      int i = micros();
      int flag = 0;
      while (!Serial.available()) {
        if (micros() - i > 2000) {
          flag = 1;
          Serial.println("error");
          break;
        }
      }
      if (flag == 0) {
        String readl = Serial.readString();
        memcpy(midiMode_info, Split(readl, '_'), sizeof(Split(readl, '_')));
        analogWrite(A1, map(midiMode_info[26].toInt(), 0, 255, 0, 1023)); analogWrite(A2, map(midiMode_info[27].toInt(), 0, 255, 0, 1023)); analogWrite(A3, map(midiMode_info[28].toInt(), 0, 255, 0, 1023));
        for (int i = 0; i < sizeof(midiMode_info); i++)
          EEPROM.write(i + 50, midiMode_info[i].toInt());
      }
    }
    if (strread == "ColorChange") {
      int k = micros();
      int flag = 0;
      while (!Serial.available()) {
        if (micros() - k > 2000)
          flag = 1;
        Serial.println("error");
        break;
      }
      if (flag == 0) {
        String* colors = Split(Serial.readString(), '_');
        analogWrite(A1, map(colors[0].toInt(), 0, 255, 0, 1023)); analogWrite(A1, map(colors[1].toInt(), 0, 255, 0, 1023)); analogWrite(A1, map(colors[2].toInt(), 0, 255, 0, 1023));
      }
    } if (strread == "I'm PMTM!") {
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
  for (byte i = 0b000; i <= 0b100 ; i++) {
    digitalWrite(2, bitRead(i, 0));
    digitalWrite(3, bitRead(i, 1));
    digitalWrite(4, bitRead(i, 2));
    for (byte j = 0b000; j <= 0b111 ; j++) {
      digitalWrite(5, bitRead(j, 0));
      digitalWrite(6, bitRead(j, 1));
      digitalWrite(7, bitRead(j, 2));
      if (i == 0) {
        int analRead = map(analogRead(A0), 0, 1023, 0, 127);
        if (j < 5) {
          if ((analRead > 125)&&(addpstatus[j]< 125)) {
            addpstatus[j]=analRead;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[29].toInt(), HEX), midiMode_info[j + 16], 127);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(127);
            }
          }else if((analRead < 125)&&(addpstatus[j]> 125)){
            addpstatus[j]=analRead;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[29].toInt(), HEX), midiMode_info[j + 16], 0);
            } else {
              Serial.print("A1 "); Serial.print("cc "); Serial.print(String(j + 1)+" "); Serial.println(0);
            }
          }
        } else {
          if ((addpstatus[j] - analRead >= 2) || (addpstatus[j] - analRead <= -2)) {
            addpstatus[j] = analRead;if(addpstatus[j]==126)addpstatus[j] = analRead = 127;
            if (midiMode) {
              midi_Send("0xB" + String(midiMode_info[29].toInt(), HEX), midiMode_info[j + 16], analRead);
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
            midi_Send("0xB" + String(midiMode_info[29].toInt(), HEX), midiMode_info[21 + i], analRead);
          } else {
            Serial.print("A1 "); Serial.print("cc "); Serial.print(String(6 + i)+" "); Serial.println(analRead);
          }
        }
      } else if (bstatus[j + (i - 1) * 8] == 1) {
        //analogWrite(A0, 1023);
      } else; //analogWrite(A0, 0);

    }
    delay(2);
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
