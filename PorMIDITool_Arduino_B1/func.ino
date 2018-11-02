String* Split(String mass, char sep)
{
  int masscounter = 0;
  for (int i = 0; i < mass.length(); i++) {
    if (mass[i] == sep)masscounter++;
  }
  String *Mass=new String[masscounter+1]; masscounter = 0;
  String line="";
  for (int i = 0; i < mass.length(); i++) {
    if (mass[i] != sep) {
      line = line + mass[i];
    } else {
      Mass[masscounter++] = line; line = "";
    }
  } Mass[masscounter++] = line;
  return Mass;
}

void midi_Send(String str, int note, int velocity) {
  int x;char status[4];str.toCharArray(status,4);
  sscanf(status,"%x",&x);
  Serial.write(x);
  Serial.write(note);
  Serial.write(velocity);
}
