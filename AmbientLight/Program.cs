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

			ColorManager roundForeColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			ColorManager roundBackColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			roundBackColor.CloneDeltaTime(roundForeColor, 10000);
			VirtualStrip round = new VirtualStrip(logger, Effects.TesterEffect, 20, roundForeColor, roundBackColor, new StripPart[]
			{
				new StripPart(2, 11, 0, Voltage.V12, LEDType.WS2811),

				new StripPart(0, 12, 29, Voltage.V5, LEDType.WS2812),
				new StripPart(0, 0, 11, Voltage.V5, LEDType.WS2812),

				new StripPart(2, 0, 11, Voltage.V12, LEDType.WS2811)
			});
			//round.effect.On();

			ColorManager fanForeColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			ColorManager fanBackColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			fanBackColor.CloneDeltaTime(fanForeColor, 10000);
			VirtualStrip fan = new VirtualStrip(logger, Effects.TesterEffect, 20, fanForeColor, fanBackColor, new StripPart[]
			{
				new StripPart(1, 0, 26, Voltage.V5, LEDType.WS2812),
			});
			//fan.effect.On();

			ColorManager monitorForeColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			ColorManager monitorBackColor = ColorManager.GetColorManagerByID(ColorManagers.TesterColorManager);
			monitorBackColor.CloneDeltaTime(monitorForeColor, 10000);
			VirtualStrip monitor = new VirtualStrip(logger, Effects.TesterEffect, 20, monitorForeColor, monitorBackColor, new StripPart[]
			{
				new StripPart(3, 0, 4, Voltage.V12, LEDType.WS2811),
				new StripPart(3, 5, 19, Voltage.V12, LEDType.WS2811),
				new StripPart(3, 20, 37, Voltage.V12, LEDType.WS2811)
			});
			//monitor.effect.On();

			//TODO do the things

			Home.Run(logger);

			StopProgram();
		}

		public static void StopProgram()
		{
			for (int i = 0; i < VirtualStrip.strips.Count; i++)
			{
				VirtualStrip.strips[i].effect.Off();
			}

			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}
