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
  Serial.begin(38400);
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
  mode = EEPROM.read(4);
  for (int i = 0; i < 4; i++)midiMode[i] = EEPROM.read(i);
  while (!Serial.available()) {
    delay(1000);
    Serial.print("I'm PMTM!");
  }
  if (Serial.available()) {
    midiMode = Serial.readString().toInt();
    if (midiMode > 10) {
      channel = String(midiMode).substring(1, String(midiMode).length());
      midiMode = 1;
    }
  }
}
void loop() {
  if ((Serial.available()) && (midiMode == false)) {
    String strread = Serial.readString();
    if (strread == "write_midiinfoB1FS") {
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
        for (int i = 0; i < sizeof(midiMode_info); i++) {
          int Name = Split(String(midiMode_info[i]), '&')[0].toInt();
          if (Name > 0 && Name < 5) {
            midiMode_info[Name] = Split(midiMode_info[i], '&')[0].toInt()[1];
            (if midiMode_info[Name] > 127)midiMode_info[Name] = 127; if (midiMode_info[Name] < 0)midiMode_info[Name] = 0;
            EEPROM.write(Name, midiMode_info[Name]);
          }
        }
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
    mode = 1 - mode;
    EEPROM.write(4, mode);
    if (mode == 0) {
      for (int i = 0; i < 5; i++) {
        but_on[i] = 0;
      }
    }
  }
  for (int j = 0; j < 3; j++) {
    //////////////////////////////////////////////////2////////////////////////
    if (digitalRead(j + 3) == false) {
      if (but_on[j + 1] == false) {
        //Serial.print("2 on, ");Serial.println(mode);
        but_on[j + 1] = true;
        if (mode == 0) {
          digitalWrite(j + 7, HIGH);
          if (midiMode == 0)
            Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 127");
          else
            midi_Send("0xB" + String(channel, HEX), midiMode_info[j], 127);
        } else {
          if (mode_active != j) {
            digitalWrite(7, LOW);
            digitalWrite(8, LOW);
            digitalWrite(9, LOW);
            digitalWrite(j + 7, HIGH);
            mode_active = j;
            if (midiMode == 0) {
              Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 127");
              Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 0");
            } else {
              midi_Send("0xB" + String(channel, HEX), midiMode_info[j], 127);
              midi_Send("0xB" + String(channel, HEX), midiMode_info[j], 0);
            }
          }
        }
      }
    } else if (but_on[j + 1] == true) {
      but_on[j + 1] = false;
      //Serial.print("2 off, ");Serial.println(mode);
      if (mode == 0) {
        digitalWrite(j + 7, LOW);
        Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 0");
      } else {
        if (mode_active != j) {
          digitalWrite(8, LOW);
          digitalWrite(9, LOW);
          digitalWrite(10, LOW);
          digitalWrite(j + 7, HIGH);
          mode_active = j;
          if (midiMode == 0) {
            Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 127");
            Serial.print("B1FS "); Serial.print("cc "); Serial.println(String(j + 1) + " 0");
          } else {
            midi_Send("0xB" + String(channel, HEX), midiMode_info[j], 127);
            midi_Send("0xB" + String(channel, HEX), midiMode_info[j], 0);
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
      if (midiMode == 0)
        Serial.print("B1FS "); Serial.println("cc 4 0");
      else
        midi_Send("0xB" + String(channel, HEX), midiMode_info[3], 0);
    }
  } else if (but_on[4] == true) {
    but_on[4] = false;
    //Serial.print("5 off, ");Serial.println(mode);
    digitalWrite(10, LOW);
    if (midiMode == 0)
      Serial.print("B1FS "); Serial.println("cc 4 0");
    else
      midi_Send("0xB" + String(channel, HEX), midiMode_info[3], 127);
  }
}
