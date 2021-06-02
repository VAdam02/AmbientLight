using AmbientLight.Strip.LEDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLight.Strip.Effects_
{
	class TesterEffect : Effect
	{
		public TesterEffect(Logger logger, VirtualStrip strip, int maxFPS, ColorManager foreColorManager, ColorManager backColorManager)
		{
			logger = new Logger(logger);
			logger.AddLevel("TesterEffect");
			Setup(logger, strip, maxFPS, foreColorManager, backColorManager);

			
		}

		protected override void Once()
		{

		}

		protected override void Loop(int deltatime)
		{
			int x = 0;

			Color forecolor = foreColorManager.GetColor();
			Color backcolor = backColorManager.GetColor();

			for (int i = 0; i < strip.parts.Count; i++)
			{
				LED[] leds = strip.parts[i].GetLEDs();
				for (int j = 0; j < leds.Length; j++)
				{
					if (x < 10)
					{
						leds[j].SetRGB(forecolor.r, forecolor.g, forecolor.b);
					}
					else
					{
						leds[j].SetRGB(backcolor.r, backcolor.g, backcolor.b);
					}
					x++;
				}
			}
		}
	}
}
