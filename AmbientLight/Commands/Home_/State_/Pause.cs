using AmbientLight.Strip;

namespace AmbientLight.Commands.Home_.State_
{
	class Pause
	{
		public static void Run(Logger logger, string[] args)
		{
			logger = new Logger(logger);
			logger.AddLevel("Pause");

			string[] data = new string[args.Length - 1];
			for (int i = 1; i < args.Length; i++)
			{
				data[i - 1] = args[i];
			}

			bool running = true;
			while (running)
			{
				if (data.Length == 0)
				{
					data = logger.ReadLine("").ToLower().Split(' ');
				}

				if (data[0] == "exit") { running = false; }
				else if (data[0] == "help") { Help(logger); }
				else if (data[0] == "all") { All(logger); }
				else if (1 <= int.Parse(data[0]) && int.Parse(data[0]) <= VirtualStrip.strips.Count) { One(logger, int.Parse(data[0]) - 1); }

				else { logger.Log("Use HELP for the commands"); }
				data = new string[0];
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
			logger.Log("All configured effects are pause");
		}

		private static void Help(Logger logger)
		{
			logger.Log("-------------------------HELP-------------------------");
			logger.Log("Exit \t" + "Leave the current level");
			logger.Log("Help \t" + "Write out the commands");
			logger.Log("All \t" + "Pause all the configured effects");
			logger.Log("1-" + VirtualStrip.strips.Count + " \t" + "Pause the selected strip");
			logger.Log("-------------------------HELP-------------------------");
		}
	}
}
