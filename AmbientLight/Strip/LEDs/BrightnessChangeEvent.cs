using System.Collections.Generic;

namespace AmbientLight.Strip.LEDs
{
	class BrightnessChangeEvent
	{
		#region Handlers
		private static List<BrightnessChangeEventHandler>[] handlers;
		private static bool ready = false;

		private static void Setup()
		{
			if (ready) { return; }

			handlers = new List<BrightnessChangeEventHandler>[LED.leds.Length];
			for (int i = 0; i < handlers.Length; i++)
			{
				handlers[i] = new List<BrightnessChangeEventHandler>();
			}
			ready = true;
		}

		public static void AddHandler(BrightnessChangeEventHandler handler, byte pin)
		{
			Setup();
			handlers[pin].Add(handler);
		}
		public static void RemoveHandler(BrightnessChangeEventHandler handler, byte pin)
		{
			Setup();
			handlers[pin].Remove(handler);
		}
		#endregion

		public byte pin;
		public byte newBrightness;
		public Voltage normalizedVoltage;

		public BrightnessChangeEvent(byte pin, byte brightness, Voltage normalizedVoltage)
		{
			Setup();

			this.pin = pin;
			newBrightness = brightness;
			this.normalizedVoltage = normalizedVoltage;
		}

		public void Activate()
		{
			for (int i = 0; i < handlers[pin].Count; i++)
			{
				if (LED.leds[pin][i].GetVoltage() != normalizedVoltage)
				{
					handlers[pin][i].Activate(this);
				}
				else
				{
					LED.leds[pin][i].RefreshPower();
				}
			}
			
		}
	}

	class BrightnessChangeEventHandler
	{
		public void Setup(byte pin)
		{
			BrightnessChangeEvent.AddHandler(this, pin);
		}

		public virtual void Activate(BrightnessChangeEvent brightnessChangeEvent) { }
	}
}
