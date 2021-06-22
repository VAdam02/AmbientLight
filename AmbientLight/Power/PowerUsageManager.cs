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

			StartQueue();
		}

		public int GetPowerUsage(Voltage voltage)
		{
			return powerusages[(int)voltage];
		}

		#region Queue
		private List<PowerUsageChange> queue;
		public int GetQueueSize()
		{
			return queue.Count;
		}

		public void RegisterChange(PowerUsageChange change)
		{
			queue.Add(change);
		}

		private Thread queueManager;
		private bool running;
		public static void QueueManager(PowerUsageManager usageManager)
		{
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
		}

		private void StartQueue()
		{
			running = true;
			queueManager.Start();
		}

		private void StopQueue()
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

	struct PowerUsageChange
	{
		public Voltage voltage;
		public int deltaUsage;
	}
}
