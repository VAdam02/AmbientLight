using AmbientLight.Strip;
using System;

namespace AmbientLight.Commands.Home_.State_
{
	class Pause
	{
		public static void Run(Logger logger, ref string[] data)
		{
			logger = new Logger(logger);
			logger.AddLevel("Pause");

			/*
			string[] data = new string[args.Length - 1];
			for (int i = 1; i < args.Length; i++)
			{
				data[i - 1] = args[i];
			}
			*/

			bool running = true;
			while (running)
			{
				if (data.Length > 0)
				{
					string[] cache = new string[data.Length - 1];
					for (int i = 0; i < cache.Length; i++)
					{
						cache[i] = data[i + 1];
					}
					data = cache;
				}

				if (data.Length == 0)
				{
					data = logger.ReadLine("").ToLower().Split(' ');
				}

				try
				{
					if (data[0] == "exit") { running = false; }
					else if (data[0] == "help") { Help(logger); }
					else if (data[0] == "all") { All(logger); }
					else if (0 <= int.Parse(data[0]) && int.Parse(data[0]) < VirtualStrip.strips.Count) { One(logger, int.Parse(data[0])); }

					else { logger.Log("Use HELP for the commands"); }
				}
				catch (FormatException)
				{
					logger.Log("Use HELP for the commands");
				}
			}
		}

		private static void All(Logger logger)
		{
			for (int i = 0; i < VirtualStrip.strips.Count; i++)
			{
				VirtualStrip.strips[i].effect.Pause();
			}
			logger.Log("All configured effects are paused");
		}
		private static void One(Logger logger, int ID)
		{
			VirtualStrip.strips[ID].effect.Pause();
			logger.Log("Selected effect is paused");
		}

		private static void Help(Logger logger)
		{
			logger.Log("-------------------------HELP-------------------------");
			logger.Log("Exit \t" + "Leave the current level");
			logger.Log("Help \t" + "Write out the commands");
			logger.Log("All \t" + "Pause all the configured effects");
			logger.Log("0-" + (VirtualStrip.strips.Count-1) + " \t" + "Pause the selected strip");
			logger.Log("-------------------------HELP-------------------------");
		}
	}
}
