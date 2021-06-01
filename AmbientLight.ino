#include <FastLED.h>

CRGB *leds[6];
CLEDController *controllers[6];
byte lastBrightness[6];

bool button;
void setup() 
{
  leds[0] = new CRGB[30];
  controllers[0] = &FastLED.addLeds<WS2811, 7, GRB>(leds[0], 30);
  for (int i = 0; i < 30; i++){leds[0][i] = CRGB(0, 0, 0);}
  
  leds[1] = new CRGB[27];
  controllers[1] = &FastLED.addLeds<WS2811, 6, GRB>(leds[1], 27);
  for (int i = 0; i < 27; i++){leds[1][i] = CRGB(0, 0, 0);}
  
  leds[2] = new CRGB[12];
  controllers[2] = &FastLED.addLeds<WS2811, 5, BRG>(leds[2], 12);
  for (int i = 0; i < 12; i++){leds[2][i] = CRGB(0, 0, 0);}
  
  leds[3] = new CRGB[38];
  controllers[3] = &FastLED.addLeds<WS2811, 4, BRG>(leds[3], 38);
  for (int i = 0; i < 38; i++){leds[3][i] = CRGB(0, 0, 0);}
  
  leds[4] = new CRGB[1];
  controllers[4] = &FastLED.addLeds<WS2811, 3, GRB>(leds[4], 1);
  for (int i = 0; i < 1; i++){leds[4][i] = CRGB(0, 0, 0);}
  
  leds[5] = new CRGB[1];
  controllers[5] = &FastLED.addLeds<WS2811, 2, BRG>(leds[5], 1);
  for (int i = 0; i < 1; i++){leds[5][i] = CRGB(0, 0, 0);}

  for (int i = 0; i < 6; i++)
  {
    byte data[5];
    data[0] = 253;
    data[1] = i;
    data[2] = 255;
    data[3] = 0;
    data[4] = 0;
    setBrightness(data);
    lastBrightness[i] = data[2];
  }
  Serial.begin(19200);
}

void testConnection(byte check[])
{
  //2.2
  check[2] = check[1] * 2 /*+ 2*/;
  Serial.write(check, 5);
}

void showLED(byte data[])
{
  controllers[data[1]]->showLeds(lastBrightness[data[1]]);
  //FastLED[data[1]].showLeds();
}

void setBrightness(byte data[])
{
  controllers[data[1]]->showLeds(data[2]);
  controllers[data[1]]->setDither(0);
  lastBrightness[data[1]] = data[2];
  //FastLED[data[1]].showLeds(255);
  //FastLED[data[1]].showLeds(data[2]);
}

int count = 0;
void loop() 
{
  if (!(Serial.available() < 5))
  {
    byte data[5];
    Serial.readBytes(data, 5);

    /*flush*/
    if (data[0] == 255) {showLED(data);}
    /*testConnection*/
    else if (data[0] == 254) {testConnection(data);}
    /*setBrightness*/
    else if (data[0] == 253) {setBrightness(data);}
    else
    {
      leds[data[0]][data[1]] = CRGB(data[2], data[3], data[4]);
    }
  }
}
