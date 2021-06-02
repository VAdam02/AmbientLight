﻿using AmbientLight.Strip.LEDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLight.Strip.Effects_
{
	class Rainbow : Effect
	{
		public Rainbow(Logger logger, VirtualStrip strip, int maxFPS, ColorManager colorManager)
		{
			logger = new Logger(logger);
			logger.AddLevel("Rainbow");
			Setup(logger, strip, maxFPS, colorManager);

			
		}

		protected override void Once()
		{
			int j = 10;
			while (j < 37)
			{
				LED.leds[3][j].SetRGB(255, 255, 255);
				j++;
			}
		}

		int k = 0;
		protected override void Loop(int deltatime)
		{
			
			if (k < 250)
			{
				k++;
			}
			
			int j = 0;
			while (j < 10)
			{
				LED.leds[3][j].SetRGB((byte)(k), (byte)(k), (byte)(k));
				j++;
			}
			
		}
	}
}