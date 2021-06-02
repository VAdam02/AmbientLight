using AmbientLight.Strip.LEDs;
using System.Collections.Generic;
using System.Linq;

namespace AmbientLight.Strip
{
	class StripPart
	{
		public static List<StripPart> strips = new List<StripPart>();

		private List<LED> leds = new List<LED>();

		private byte pin;
		private byte from;
		private byte to;
		private Voltage voltage;
		private LEDType type;

		public StripPart(byte pin, byte from, byte to, Voltage voltage, LEDType type)
		{
			this.pin = pin;
			this.from = from;
			this.to = to;
			this.voltage = voltage;
			this.type = type;

			leds = LED.GetLEDArray(pin, from, to).ToList();

			foreach (LED cur in leds)
			{
				cur.SetTypeAndVoltage(voltage, type);
			}

			strips.Add(this);
		}

		public LED[] GetLEDs()
		{
			return LED.GetLEDArray(pin, from, to);
		}

		public void GetData(out byte pin, out byte from, out byte to, out Voltage voltage, out LEDType type)
		{
			pin = this.pin;
			from = this.from;
			to = this.to;
			voltage = this.voltage;
			type = this.type;
		}
	}
}
