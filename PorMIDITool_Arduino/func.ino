String* Split(String mass, char sep)
{
  int masscounter = 0;
  for (int i = 0; i < mass.length(); i++) {
    if (mass[i] == sep)masscounter++;
  }
  String *Mass=new String[masscounter+1]; masscounter = 0;
  String line;
  for (int i = 0; i < mass.length(); i++) {
    if (mass[i] != sep) {
      line = line + mass.charAt(i);
    } else {
      Mass[masscounter++] = line; line = "";
    }
  } Mass[masscounter++] = line;
  return Mass; //здесь не знаю как вернуть
}
