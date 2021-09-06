using AmbientLight.API;
using AmbientLight.Strip;
using AmbientLight.Strip.LEDs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AmbientLight.Power
{
	class PowerUsageManager
	{
		public static PowerUsageManager instance;

		private int[] powerusages;
		private Logger logger;

		public static PowerUsageManager Instantiate(Logger logger)
		{
			instance = new PowerUsageManager(logger);
			return instance;
		}
		public PowerUsageManager(Logger logger)
		{
			this.logger = new Logger(logger);
			this.logger.AddLevel("PowerUsageManager");

			powerusages = new int[LED.voltages.Length];
			for (int i = 0; i < powerusages.Length; i++)
			{
				powerusages[i] = 1;
			}

			readManager = new Thread(() => ReadManager(this));
			StartRead();
		}

		public int GetPowerUsage(Voltage voltage)
		{
			return powerusages[(int)voltage];
		}

		#region Reading
		private Thread readManager;
		private bool running = false;
		public static void ReadManager(PowerUsageManager usageManager)
		{
			while (usageManager.running)
			{
				int[] new_powerusages = new int[LED.voltages.Length];
				for (int i = 0; i < new_powerusages.Length; i++)
				{
					new_powerusages[i] = 0;
				}

				for (int i = 0; i < LED.leds.Length; i++)
				{
					for (int j = 0; j < LED.leds[i].Length; j++)
					{
						try
						{
							new_powerusages[(int)LED.leds[i][j].GetVoltage()] += LED.leds[i][j].powerusage;
						}
						catch { }
					}
				}

				for (int i = 1; i < new_powerusages.Length; i++)
				{
					new_powerusages[0] += new_powerusages[i];
				}

				usageManager.powerusages = new_powerusages;
			}
			/*
			int error = 0;
			Logger logger = new Logger(usageManager.logger);
			logger.AddLevel("Queue");

			logger.Debug(DebugCategory.Rare, "Queue started");

			while (usageManager.running)
			{
				while (usageManager.running && usageManager.queue.Count < 1) { Thread.Sleep(1); }

				try
				{
					if (usageManager.queue.Count > 0)
					{
						usageManager.powerusages[(int)usageManager.queue[0].voltage] += usageManager.queue[0].deltaUsage;
						if (usageManager.queue[0].voltage != Voltage.None)
						{
							usageManager.powerusages[(int)Voltage.None] += usageManager.queue[0].deltaUsage;
						}
					}
					usageManager.queue.RemoveAt(0);
					error = 0;
				}
				catch
				{
					error++;
					if (error >= 3)
					{
						usageManager.queue.RemoveAt(0);
						error = 0;
					}
				}
			}

			logger.Debug(DebugCategory.Rare, "Queue stopped");
			*/
		}

		private void StartRead()
		{
			running = true;
			readManager.Start();
		}

		private void StopRead()
		{
			running = false;
		}
		#endregion

		#region CalculateUsage
		public int CalculateUsage(Color color, byte brightness, Voltage voltage, LEDType type)
		{
			return CalculateUsage(color, (double)brightness / 255, voltage, type);
		}
		public int CalculateUsage(Color color, double brightness, Voltage voltage, LEDType type)
		{
			return CalculateUsage(new Color() { r = (byte)(color.r * brightness), g = (byte)(color.g * brightness), b = (byte)(color.b * brightness) }, voltage, type);
		}
		private int CalculateUsage(Color color, Voltage voltage, LEDType type)
		{
			if (voltage == Voltage.V12 && type == LEDType.WS2811)
			{
				double[] time = new double[] { 0, 255.1 };
				double[][] values = new double[][] { new double[] { 0.2 }, new double[] { 4.5 } };

				int usage = 0;
				usage += (int)Math.Round(Curve.Linear(color.r, time, values)[0], 0);
				usage += (int)Math.Round(Curve.Linear(color.g, time, values)[0], 0);
				usage += (int)Math.Round(Curve.Linear(color.b, time, values)[0], 0);
				return usage * 3;
			}
			else if (voltage == Voltage.V5 && type == LEDType.WS2812)
			{
				double[] time = new double[] { 0, 255.1 };
				double[][] values = new double[][] { new double[] { 0.2 }, new double[] { 4.5 } };

				int usage = 0;
				usage += (int)Math.Round(Curve.Linear(color.r, time, values)[0], 0);
				usage += (int)Math.Round(Curve.Linear(color.g, time, values)[0], 0);
				usage += (int)Math.Round(Curve.Linear(color.b, time, values)[0], 0);
				return usage;
			}
			else
			{
				return 0;
			}
		}
		#endregion
	}
}
