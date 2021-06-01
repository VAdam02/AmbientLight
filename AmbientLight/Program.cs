using System;
using Microsoft.Win32;
using System.Windows.Forms;
using AmbientLight.Strip.LEDs;
using AmbientLight.Commands;
using AmbientLight.Power;
using AmbientLight.Strip;
using System.Threading;
using AmbientLight.Strip.Effects_;

namespace AmbientLight
{
	class Program
	{
		static void Main(string[] args)
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
			key.SetValue("AmibentLight v2.2", Application.ExecutablePath);
			//key.DeleteValue("AmibentLight v2.2", false);

			Logger logger = new Logger();
			logger.AddLevel("Home");

			try { Arduino.Instantiate(); } catch (ArduinoNotFoundException)
			{
				StopProgram();
			}

			PowerManager.Instantiate();

			//Thread.Sleep(100);

			LED.SetLEDStripLength(0, 30);
			LED.SetLEDStripLength(1, 27);
			LED.SetLEDStripLength(2, 12);
			LED.SetLEDStripLength(3, 38);
			LED.SetLEDStripLength(4, 1);
			LED.SetLEDStripLength(5, 1);

			/*
			for (int i = 0; i < LED.leds.Length; i++)
			{
				Arduino.instance.SetBrightness(255, (byte)i, Voltage.None);
			}
			*/

			/*
			VirtualStrip round = new VirtualStrip(new StripPart[]
			{
				new StripPart(0, 12, 29, Voltage.V5, LEDType.WS2812),
				new StripPart(0, 0, 11, Voltage.V5, LEDType.WS2812)
			});
			*/

			VirtualStrip monitor = new VirtualStrip(logger, Effects.Rainbow, 10, new ColorManager(), new StripPart[]
			{
				new StripPart(3, 0, 18, Voltage.V12, LEDType.WS2811),
				new StripPart(3, 19, 19, Voltage.V12, LEDType.WS2811),
				new StripPart(3, 20, 37, Voltage.V12, LEDType.WS2811)
			});
			Thread.Sleep(5000);
			logger.Log("----------------------------------------");
			monitor.effect.On();
			
			/*
			Thread.Sleep(5000);
			for (int k = 0; k < 25; k++)
			{
				logger.Log("----------------------------------------");
				
				int j = 0;
				while (j < 10)
				{
					LED.leds[3][j].SetRGB((byte)(k * 10), (byte)(k * 10), (byte)(k * 10));
					j++;
				}
				while (j < 37)
				{
					LED.leds[3][j].SetRGB(255, 255, 255);
					j++;
				}
				

				Arduino.instance.Flush(3);

				
				Thread.Sleep(1000);
			}
			*/

			//TODO do the things

			Home.Run(logger);

			StopProgram();
		}

		public static void StopProgram()
		{
			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}
