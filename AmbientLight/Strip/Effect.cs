using AmbientLight.Strip.Effects_;
using AmbientLight.Strip.LEDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmbientLight.Strip
{
	enum Effects
	{
		Rainbow
	}

	class Effect
	{
		protected ColorManager colorManager;
		protected VirtualStrip strip;
		
		private int maxFPS = 1;
		private int minDeltaTime;
		private Logger logger;

		public static Effect GetEffectByID(Logger logger, Effects effect, VirtualStrip strip, int maxFPS, ColorManager colorManager)
		{
			switch (effect)
			{
				case Effects.Rainbow:
					return new Rainbow(logger, strip, maxFPS, colorManager);
			}

			return new Effect();
		}

		protected void Setup(Logger logger, VirtualStrip strip, int maxFPS, ColorManager colorManager)
		{
			this.logger = logger;
			logger.Debug(DebugCategory.Rare, "Effect setuped");

			this.colorManager = colorManager;
			this.strip = strip;
			this.maxFPS = maxFPS;
			minDeltaTime = 1000 / this.maxFPS;
		}

		#region On/Pause/Off
		Thread t;
		private bool running = false;
		public void On()
		{
			logger.Debug(DebugCategory.Rare, "Effect started");
			Start();
			colorManager.On();
		}

		public void Pause()
		{
			logger.Debug(DebugCategory.Rare, "Effect paused");
			running = false;
			try { t.Join(); } catch { }
			colorManager.Pause();
		}

		public void Off()
		{
			Pause();

			logger.Debug(DebugCategory.Rare, "Effect stopped");

			colorManager.Off();
			
			Thread.Sleep(maxFPS * 20);

			for (int i = 0; i < strip.parts.Count; i++)
			{
				LED[] leds = strip.parts[i].GetLEDs();
				for (int j = 0; j < leds.Length; j++)
				{
					leds[j].SetRGB(0, 0, 0);
				}
			}

			Show();
		}
		#endregion

		public void Show()
		{
			for (int i = 0; i < strip.parts.Count; i++)
			{
				strip.parts[i].GetData(out byte pin, out _, out _, out _, out _);
				Arduino.instance.Flush(pin);
			}
		}

		#region LoopManager
		private void Start()
		{
			if (running)
			{
				running = false;
				t.Join();
			}
			running = true;
			t = new Thread(() => LoopManager(this));
			t.Start();
		}

		private static void LoopManager(Effect effect)
		{
			effect.Once();

			long lasttime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

			while (effect.running)
			{
				long current = DateTimeOffset.Now.ToUnixTimeMilliseconds();

				if (Arduino.instance.GetQueueSize() > 100)
				{
					effect.minDeltaTime = (int)(1000 / Math.Max((double)effect.maxFPS / ((double)Arduino.instance.GetQueueSize() / 100), 2));
				}
				else
				{
					effect.minDeltaTime = 1000 / effect.maxFPS;
				}

				if ((current - lasttime) < effect.minDeltaTime)
				{
					Thread.Sleep(effect.minDeltaTime - (int)(current - lasttime));
					current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				}

				effect.Loop((int)(current - lasttime));
				effect.Show();

				lasttime = current;
			}
		}
		#endregion

		#region Overridables
		protected virtual void Once() { }

		protected virtual void Loop(int deltatime) { }
		#endregion
	}
}
