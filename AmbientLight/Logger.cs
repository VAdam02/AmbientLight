using System;
using System.Collections.Generic;

namespace AmbientLight
{
	public enum DebugCategory
	{
		Spammer,
		Common,
		Rare
	}

	class Logger
	{
		private List<string> currentLevel;

		public Logger(Logger original)
		{
			currentLevel = new List<string>();
			foreach (string cur in original.currentLevel)
			{
				currentLevel.Add(cur);
			}
		}
		public Logger()
		{
			currentLevel = new List<string>();
		}

		public string ReadLine(string message)
		{
			Console.Write(GetLevel() + message);
			return Console.ReadLine();
		}

		internal void AddLevel(object p)
		{
			throw new NotImplementedException();
		}

		public void Debug(DebugCategory category, string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine(GetLevel() + message);
			Console.ForegroundColor = ConsoleColor.White;
		}
		public void Debug(DebugCategory category, string message, string extrainfo)
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine(GetLevel() + message + "\t" + extrainfo);
			Console.ForegroundColor = ConsoleColor.White;
		}
		public void Error(string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(GetLevel() + message);
			Console.ForegroundColor = ConsoleColor.White;
		}
		public void Log(string message)
		{
			Console.WriteLine(GetLevel() + message);
		}

		public string GetLevel()
		{
			string level = "";
			foreach (string cur in currentLevel)
			{
				level += cur + "\\";
			}
			level = level.Substring(0, level.Length - 1);
			level += "> \t";
			return level;
		}
		public void AddLevel(string level)
		{
			currentLevel.Add(level);
		}
	}
}
