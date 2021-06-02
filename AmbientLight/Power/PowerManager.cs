using AmbientLight.API;
using AmbientLight.Strip;
using AmbientLight.Strip.LEDs;
using System;

namespace AmbientLight.Power
{
	class PowerManager
	{
		public static PowerManager instance;

		#region Config
		private int minDeltaTimeBetweenRefresh = 50;
		private double maxusage = 0.75;
		int[] powerlimits = new int[] { 750, 750, 750 };
		#endregion

		public int[] powerusages;
		double[] alphas;
		long[] lasttime;

		Logger logger = new Logger();

		public static PowerManager Instantiate()
		{
			instance = new PowerManager();
			return instance;
		}

		PowerManager()
		{
			logger.AddLevel("PowerManager");

			powerusages = new int[powerlimits.Length];
			for (int i = 0; i < powerusages.Length; i++)
			{
				powerusages[i] = 1;
			}

			alphas = new double[powerlimits.Length];
			for (int i = 0; i < alphas.Length; i++)
			{
				alphas[i] = 0;
			}

			lasttime = new long[powerlimits.Length];
			for (int i = 0; i < lasttime.Length; i++)
			{
				lasttime[i] = 0;
			}
		}

		#region CalculateUsage
		public int CalculateUsage(Color color, byte brightness, Voltage voltage, LEDType type)
		{
			return CalculateUsage(color, (double)brightness/255, voltage, type);
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
				return usage * 3;
			}
			else
			{
				return 0;
			}
		}
		#endregion

		public double GetAlpha(Voltage voltage)
		{
			#region Decrase recalculat count
			if (DateTimeOffset.Now.ToUnixTimeMilliseconds() < lasttime[(int)voltage] + minDeltaTimeBetweenRefresh)
			{
				return this.alphas[(int)voltage];
			}
			lasttime[(int)voltage] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			#endregion

			#region Local alpha
			double localAlphaDivider = (powerlimits[(int)voltage] * maxusage * 0.9) * (1000 / minDeltaTimeBetweenRefresh) * 10;
			double alpha1 = alphas[(int)voltage] + ((powerlimits[(int)voltage] * (maxusage*0.9)) - powerusages[(int)voltage]) / localAlphaDivider;
			if (alpha1 > 1) { alpha1 = 1; }
			else if (alpha1 < 0) { alpha1 = 0; }
			#endregion

			#region Global alha
			double globalAlphaDivider = (powerlimits[0] * maxusage * 0.9) * (1000 / minDeltaTimeBetweenRefresh) * 10;
			double alpha2;
			if (DateTimeOffset.Now.ToUnixTimeMilliseconds() < lasttime[0] + minDeltaTimeBetweenRefresh)
			{
				alpha2 = alphas[0];
			}
			else
			{
				alpha2 = alphas[0] + ((powerlimits[0] * (maxusage * 0.9)) - powerusages[0]) / globalAlphaDivider;
			}
			lasttime[0] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			if (alpha2 > 1) { alpha2 = 1; }
			else if (alpha2 < 0) { alpha2 = 0; }
			#endregion

			#region Selector
			double alpha;
			if (alpha1 <= alpha2)
			{
				alpha = alpha1;
				logger.Debug(DebugCategory.Common, "jej - " + voltage.ToString(), "Local alpha: " + Math.Round(alpha1 * 100, 0) + "% \tGlobal alpha: " + Math.Round(alpha2 * 100, 0) + "% \tNew alpha: " + Math.Round(alpha * 100, 0));
			}
			else
			{
				alpha = alpha2;
				logger.Debug(DebugCategory.Common, "Alpha overridden by global limit at " + voltage.ToString(), "Local alpha: " + Math.Round(alpha1 * 100, 0) + "% \tGlobal alpha: " + Math.Round(alpha2 * 100, 0) + "% \tNew alpha: " + Math.Round(alpha * 100, 0));
			}
			
			#pragma warning disable IDE0059 // Unnecessary assignment of a value
			if (alpha > 1) { alpha = 1; }
			else if (alpha < 0) { alpha = 0; }
			#pragma warning restore IDE0059 // Unnecessary assignment of a value
			#endregion

			alphas[(int)voltage] = Math.Round(alpha, 3);
			alphas[0] = Math.Round(alpha2, 3);

			#region Brightness controller
			if (StripPart.strips.Count > 0)
			{
				bool[][] voltages = new bool[LED.leds.Length][];
				for (int i = 0; i < voltages.Length; i++)
				{
					voltages[i] = new bool[LED.voltages.Length];
					for (int j = 0; j < voltages[i].Length; j++)
					{
						voltages[i][j] = false;
					}
				}

				for (int i = 0; i < StripPart.strips.Count; i++)
				{
					StripPart.strips[i].GetData(out byte pin, out _, out _, out Voltage voltage1, out _);
					voltages[pin][(int)voltage1] = true;
				}

				for (int i = 0; i < voltages.Length; i++)
				{
					int maxj = -1;
					for (int j = 0; j < voltages[i].Length; j++)
					{
						if (voltages[i][j] && (maxj == -1 || alphas[j] > alphas[maxj]))
						{
							maxj = j;
						}
					}

					if (maxj != -1)
					{
						//TODO debug...
						logger.Error("módosítom " + (byte)(255 * alphas[maxj]) + "\t" + ((Voltage)maxj).ToString() + " ");
						Arduino.instance.SetBrightness((byte)(255 * alphas[maxj]), (byte)i, (Voltage)maxj);
					}
				}
			}
			#endregion


			//TODO debug...
			LED.leds[Arduino.BrigthnessID_Alt][3].GetRGB(out byte[] data);
			logger.Error(powerusages[0] + "\t" + powerusages[1] + "\t" + powerusages[2] + "\t" + alphas[(int)voltage] + "\t" + data[2]);
			return alphas[(int)voltage];
		}
	}
}
