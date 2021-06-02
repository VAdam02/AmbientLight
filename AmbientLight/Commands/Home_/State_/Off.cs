using AmbientLight.Strip;

namespace AmbientLight.Commands.Home_.State_
{
	class Off
	{
		public static void Run(Logger logger, ref string[] data)
		{
			logger = new Logger(logger);
			logger.AddLevel("Off");

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

				if (data[0] == "exit") { running = false; }
				else if (data[0] == "help") { Help(logger); }
				else if (data[0] == "all") { All(logger); }
				else if (1 <= int.Parse(data[0]) && int.Parse(data[0]) <= VirtualStrip.strips.Count) { One(logger, int.Parse(data[0]) - 1); }

				else { logger.Log("Use HELP for the commands"); }
			}
		}

		private static void All(Logger logger)
		{
			for (int i = 0; i < VirtualStrip.strips.Count; i++)
			{
				VirtualStrip.strips[i].effect.Off();
			}
			logger.Log("All configured effects are stopped");
		}
		private static void One(Logger logger, int ID)
		{
			VirtualStrip.strips[ID].effect.Off();
			logger.Log("Selected effect is stopped");
		}

		private static void Help(Logger logger)
		{
			logger.Log("-------------------------HELP-------------------------");
			logger.Log("Exit \t" + "Leave the current level");
			logger.Log("Help \t" + "Write out the commands");
			logger.Log("All \t" + "Stop all the configured effects");
			logger.Log("1-" + VirtualStrip.strips.Count + " \t" + "Stop the selected strip");
			logger.Log("-------------------------HELP-------------------------");
		}
	}
}
