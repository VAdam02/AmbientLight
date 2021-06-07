using AmbientLight.Strip.Effects_;
using AmbientLight.Strip.LEDs;
using System;
using System.Threading;

namespace AmbientLight.Strip
{
	enum Effects
	{
		SceenTopEffect,
		SceenLeftEffect,
		SceenBottomEffect,
		SceenRightEffect,
		TesterEffect
	}

	class Effect
	{
		protected ColorManager foreColorManager;
		protected ColorManager backColorManager;
		protected VirtualStrip strip;
		
		private int maxFPS = 1;
		private int minDeltaTime;
		private Logger logger;

		public static Effect GetEffectByID(Logger logger, Effects effect, VirtualStrip strip, int maxFPS, ColorManager forecolorManager, ColorManager backcolorManager)
		{
			switch (effect)
			{
				case Effects.SceenTopEffect:
					return new ScreenTopEffect(logger, strip, maxFPS, forecolorManager, backcolorManager);
				case Effects.SceenLeftEffect:
					return new ScreenLeftEffect(logger, strip, maxFPS, forecolorManager, backcolorManager);
				case Effects.SceenBottomEffect:
					return new ScreenBottomEffect(logger, strip, maxFPS, forecolorManager, backcolorManager);
				case Effects.SceenRightEffect:
					return new ScreenRightEffect(logger, strip, maxFPS, forecolorManager, backcolorManager);

				case Effects.TesterEffect:
					return new TesterEffect(logger, strip, maxFPS, forecolorManager, backcolorManager);
			}

			return new Effect();
		}

		protected void Setup(Logger logger, VirtualStrip strip, int maxFPS, ColorManager foreColorManager, ColorManager backColorManager)
		{
			this.logger = logger;
			logger.Debug(DebugCategory.Rare, "Effect setuped");

			this.foreColorManager = foreColorManager;
			this.backColorManager = backColorManager;
			this.strip = strip;
			this.maxFPS = maxFPS;
			minDeltaTime = 1000 / this.maxFPS;
		}

		#region On/Pause/Off
		Thread t;
		private bool running = false;
		private bool paused = false;
		public void On()
		{
			logger.Debug(DebugCategory.Rare, "Effect started");
			Start();
			foreColorManager.On();
			backColorManager.On();
		}

		public void Pause()
		{
			logger.Debug(DebugCategory.Rare, "Effect paused");
			running = false;
			paused = true;
			try { t.Join(); } catch { }
			foreColorManager.Pause();
			backColorManager.Pause();
		}

		public void Off()
		{
			Pause();
			paused = false;

			logger.Debug(DebugCategory.Rare, "Effect stopped");

			foreColorManager.Off();
			backColorManager.Off();

			Thread.Sleep(2 * minDeltaTime);

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

		private static double avg = 0;
		private static void LoopManager(Effect effect)
		{
			if (effect.paused)
			{
				//tasks if paused and unpaused
				//return to normal state
				effect.paused = false;
			}
			else
			{
				//tasks if started in normal the normal way
				effect.Once();
				effect.Show();
			}

			long lasttime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

			while (effect.running)
			{
				long current = DateTimeOffset.Now.ToUnixTimeMilliseconds();

				if (Arduino.instance.GetQueueSize() > 200)
				{
					effect.minDeltaTime = (int)(1000 / Math.Max((double)effect.maxFPS / ((double)Arduino.instance.GetQueueSize() / 200), 2));
				}
				else
				{
					effect.minDeltaTime = 1000 / effect.maxFPS;
				}

				//TODO debug...
				avg = avg * 0.9 + effect.minDeltaTime * 0.1;
				effect.logger.Log("FPS: " + Math.Round(1000 / avg, 0));

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
