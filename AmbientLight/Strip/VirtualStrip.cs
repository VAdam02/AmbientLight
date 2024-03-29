﻿using System.Collections.Generic;
using System.Linq;

namespace AmbientLight.Strip
{
	class VirtualStrip
	{
		public static List<VirtualStrip> strips = new List<VirtualStrip>();

		public List<StripPart> parts = new List<StripPart>();
		public Effect effect;

		private Logger logger;

		public VirtualStrip(Logger logger, Effects effect, int maxFPS, ColorManager forecolorManager, ColorManager backcolorManager, StripPart[] stripParts)
		{
			parts = stripParts.ToList();
			strips.Add(this);

			this.logger = new Logger(logger);
			this.logger.AddLevel("VirtualStrip-" + strips.IndexOf(this));

			SetEffect(Effect.GetEffectByID(this.logger, effect, this, maxFPS, forecolorManager, backcolorManager));
		}

		public void SetEffect(Effect effect)
		{
			logger.Log("Effect changed");
			if (this.effect != null)
			{
				this.effect.Off();
			}
			this.effect = effect;
		}
	}
}
