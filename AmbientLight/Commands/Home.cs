﻿using AmbientLight.Commands.Home_;

namespace AmbientLight.Commands
{
	class Home
	{
		public static void Run(Logger logger)
		{
			logger = new Logger(logger);

			bool running = true;
			string[] data = new string[0];
			while (running)
			{
				if (data.Length == 0)
				{
					data = logger.ReadLine("").ToLower().Split(' ');
				}

				if (data[0] == "exit") { running = false; }
				else if (data[0] == "help") { Help(logger); }
				else if (data[0] == "state") { State.Run(logger, data); }

				else { logger.Log("Use HELP for the commands"); }

				data = new string[0];
			}
		}

		private static void Help(Logger logger)
		{
			logger.Log("-------------------------HELP-------------------------");
			logger.Log("Exit \t" + "Leave the current level");
			logger.Log("Help \t" + "Write out the commands");
			logger.Log("State \t" + "Set the lighting system's state");
			logger.Log("-------------------------HELP-------------------------");
		}
	}
}
