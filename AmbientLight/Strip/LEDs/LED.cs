using AmbientLight.Power;
using System;

namespace AmbientLight.Strip.LEDs
{
	enum LEDType
	{
		None,
		WS2811,
		WS2812
	}

	enum Voltage
	{
		None,
		V5,
		V12
	}

	class LED : BrightnessChangeEventHandler
	{
		public static Voltage[] voltages = new Voltage[] { Voltage.None, Voltage.V5, Voltage.V12 };

		private static bool ready = false;
		public static LED[][] leds = new LED[8][];

		public static void SetLEDStripLength(byte pin, byte count)
		{
			leds[pin] = new LED[count];
			for (int i = 0; i < leds[pin].Length; i++)
			{
				leds[pin][i] = new LED(pin, (byte)i, Voltage.None, LEDType.None);
			}

			Arduino.instance.Flush(pin);
		}
		
		private byte pin;
		private byte id;
		private Color color;
		private bool changed;
		private Voltage voltage;
		private LEDType type;
		private int powerusage;

		private LED(byte pin, byte id, Voltage voltage, LEDType type)
		{
			Setup(pin);
			if (!ready && !(pin == Arduino.BrigthnessID_Alt || pin == Arduino.FlushID_Alt))
			{
				SetLEDStripLength(Arduino.FlushID_Alt, (byte)leds.Length);
				SetLEDStripLength(Arduino.BrigthnessID_Alt, (byte)leds.Length);
				ready = true;
			}

			this.pin = pin;
			this.id = id;
			color.r = 0;
			color.g = 0;
			color.b = 0;
			changed = true;
			this.voltage = voltage;
			this.type = type;
			powerusage = 0;

			Arduino.instance.SendRGB(this);
		}

		#region Get/Set RGB
		public bool SetRGB(Color color) { return SetRGB(color.r, color.g, color.b); }
		public bool SetRGB(byte r, byte g, byte b)
		{
			if (!(color.r == r && color.g == g && color.b == b))
			{
				color.r = r;
				color.g = g;
				color.b = b;
				changed = true;
				Arduino.instance.SendRGB(this);
				return true;
			}
			return false; //not changed
		}

		public bool GetData(out byte[] data)
		{
			data = new byte[5];
			data[0] = pin;
			data[1] = id;
			data[2] = color.r;
			data[3] = color.g;
			data[4] = color.b;
			return changed;
		}

		public Voltage GetVoltage()
		{
			return voltage;
		}

		public static LED GetLED(byte pin, byte ID)
		{
			return leds[pin][ID];
		}

		public static LED[] GetLEDArray(byte pin, byte from, byte to)
		{
			LED[] ledarray = new LED[Math.Abs(to - from) + 1];
			
			if (ledarray.Length > 1)
			{
				int delta = (to - from) / Math.Abs(to - from);
				for (int i = 0; i < (ledarray.Length - 1); i++)
				{
					ledarray[i] = GetLED(pin, (byte)(from + (i * delta)));
				}
			}
			ledarray[ledarray.Length - 1] = leds[pin][to];
			return ledarray;
		}
		#endregion

		public void SetTypeAndVoltage(Voltage voltage, LEDType type)
		{
			SetRGB(0, 0, 0);
			RefreshPower();
			this.voltage = voltage;
			this.type = type;
		}

		public void Written()
		{
			changed = false;
			leds[Arduino.FlushID_Alt][pin].changed = true;

			RefreshPower();
		}

		public void RefreshPower()
		{
			int last = powerusage;
			powerusage = PowerManager.instance.CalculateUsage(color , PowerManager.instance.GetAlpha(voltage), voltage, type);
			PowerManager.instance.powerusages[(int)voltage] += powerusage - last;
			PowerManager.instance.powerusages[0] += powerusage - last;
		}

		public override void Activate(BrightnessChangeEvent brightnessChangeEvent)
		{
			changed = true;
			Arduino.instance.SendRGB(this);
			//powermanager -> send new brightness -> event -> normalized do nothing, all the others run Activate -> send out led with the correction -> refresh power usage
		}
	}
}
