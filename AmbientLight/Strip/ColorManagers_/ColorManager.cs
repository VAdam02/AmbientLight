using AmbientLight.Strip.ColorManagers_;
using System;

namespace AmbientLight.Strip
{
	enum ColorManagers
	{
		Empty,
		TesterColorManager
	}

	class ColorManager
	{
		private long deltatime;
		private long lasttime;
		private bool paused = false;
		private bool running = false;

		public static ColorManager GetColorManagerByID(ColorManagers colorManager)
		{
			switch (colorManager)
			{
				case ColorManagers.Empty:
					return new ColorManager();
				case ColorManagers.TesterColorManager:
					return new TesterColorManager();
			}

			return new ColorManager();
		}

		#region Cloning
		private bool cloning = false;
		private ColorManager clone;
		private int cloneDifference;
		public void CloneDeltaTime(ColorManager original, int difference)
		{
			cloning = true;
			clone = original;
			cloneDifference = difference;
		}
		#endregion

		#region On/Pause/Off/Update
		//Please note that if cloning is active than these functions are not effective
		public void On()
		{
			if (paused)
			{
				paused = false;
				lasttime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}
			else if (running)
			{

			}
			else
			{
				running = true;
				deltatime = 0;
				lasttime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}

			Update();
		}

		public void Pause()
		{
			Update();
			paused = true;
		}

		public void Off()
		{
			running = false;
			deltatime = 0;
		}

		private void Update()
		{
			if (cloning)
			{
				clone.Update();
				deltatime = clone.deltatime + cloneDifference;
			}
			else if (!paused)
			{
				deltatime += (DateTimeOffset.Now.ToUnixTimeMilliseconds() - lasttime);
				lasttime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}
		}
		#endregion

		public Color GetColor()
		{
			Update();
			return GetColor(deltatime);
		}

		#region Overridables
		protected virtual Color GetColor(long deltatime) { return new Color() { r = 0, b = 0, g = 0 }; }
		#endregion
	}
}
