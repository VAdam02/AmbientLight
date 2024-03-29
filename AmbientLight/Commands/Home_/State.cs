﻿using AmbientLight.Commands.Home_.State_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLight.Commands.Home_
{
	class State
	{
		public static void Run(Logger logger, ref string[] data)
		{
			logger = new Logger(logger);
			logger.AddLevel("State");

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
				else if (data[0] == "on") { On.Run(logger, ref data); }
				else if (data[0] == "pause") { Pause.Run(logger, ref data); }
				else if (data[0] == "off") { Off.Run(logger, ref data); }

				else { logger.Log("Use HELP for the commands"); }
			}
		}

		private static void Help(Logger logger)
		{
			logger.Log("-------------------------HELP-------------------------");
			logger.Log("Exit \t" + "Leave the current level");
			logger.Log("Help \t" + "Write out the commands");
			logger.Log("On \t" + "Enable the selected lighting effects");
			logger.Log("Pause \t" + "Pause the selected lighting effects");
			logger.Log("Off \t" + "Disable the selected lighting effects");
			logger.Log("-------------------------HELP-------------------------");
		}
	}
}
