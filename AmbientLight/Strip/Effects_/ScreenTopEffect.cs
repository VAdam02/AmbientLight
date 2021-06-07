using AmbientLight.Strip.LEDs;
using System.Drawing;

namespace AmbientLight.Strip.Effects_
{
	class ScreenTopEffect : Effect
	{
		private Logger logger;

		private int monitorID = 0;
		private int parcelHeight = 0;
		private int ledCount = 0;
		private int checkcount = 5;

		public ScreenTopEffect(Logger logger, VirtualStrip strip, int maxFPS, ColorManager foreColorManager, ColorManager backColorManager)
		{
			this.logger = new Logger(logger);
			this.logger.AddLevel("ScreenTopEffect");
			Setup(this.logger, strip, maxFPS, foreColorManager, backColorManager);
		}

		protected override void Once()
		{
			ledCount = 0;
			for (int i = 0; i < strip.parts.Count; i++)
			{
				LED[] leds = strip.parts[i].GetLEDs();
				ledCount += leds.Length;
			}

			screen = new Bitmap(System.Windows.Forms.Screen.AllScreens[monitorID].Bounds.Width, System.Windows.Forms.Screen.AllScreens[monitorID].Bounds.Height);
			RefreshScreen();
		}

		protected override void Loop(int deltatime)
		{
			RefreshScreen();
			double parcelWidth = screen.Width / ledCount;
			parcelHeight = screen.Height / 3;
			int LEDindex = 0;

			for (int i = 0; i < strip.parts.Count; i++)
			{
				LED[] leds = strip.parts[i].GetLEDs();
				for (int j = 0; j < leds.Length; j++)
				{
					int x1 = (int)(parcelWidth * LEDindex);
					LEDindex++;
					int x2 = (int)(parcelWidth * LEDindex);

					if (x1 < 0) { x1 = 0; }
					if (x2 > screen.Width) { x2 = screen.Width; }

					//x1 counted, x2 over
					leds[j].SetRGB(ParcelColor(x1, x2, 0, parcelHeight));
				}
			}
		}

		private Color ParcelColor(int x1, int x2, int y1, int y2)
		{
			int r = 0;
			int g = 0;
			int b = 0;
			int checks = 0;

			int i = x1;
			while (i < x2)
			{
				int j = y1;
				while (j < y2)
				{
					r += screen.GetPixel(i, j).R;
					g += screen.GetPixel(i, j).G;
					b += screen.GetPixel(i, j).B;
					checks++;

					j += checkcount;
				}

				i += checkcount;
			}

			return new Color()
			{
				r = (byte)(r / checks),
				g = (byte)(g / checks),
				b = (byte)(b / checks)
			};
		}

		private Graphics gr;
		private Bitmap screen;
		private void RefreshScreen()
		{
			try
			{
				gr = Graphics.FromImage(screen);
				gr.CopyFromScreen(System.Windows.Forms.Screen.AllScreens[monitorID].Bounds.Location.X, System.Windows.Forms.Screen.AllScreens[monitorID].Bounds.Location.Y, 0, 0, screen.Size);
			}
			catch
			{
				logger.Error("Error at refreshing screen data");
			}
		}
	}
}
