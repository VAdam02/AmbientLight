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
			Color forecolor = foreColorManager.GetColor();
			int j = 0;
			while (j < 10)
			{
				LED.leds[3][j].SetRGB(forecolor.r, forecolor.g, forecolor.b);
				j++;
			}
			Color backcolor = backColorManager.GetColor();
			while (j < 37)
			{
				LED.leds[3][j].SetRGB(backcolor.r, backcolor.g, backcolor.b);
				j++;
			}

		}
	}
}
