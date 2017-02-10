using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace IFSSharedObjects.Diagnostics
{

	public class Log
	{
		private Object _lock = new Object();
		private StreamWriter _writer;
		private bool _withConsoleOutput = false;

		public enum Level
		{
			High,
			Medium,
			Normal
		}

		public Log(String outputDir, bool withConsoleOutput = false, String label="Log")
		{
			String path = String.Format("{0}{2}_{1}.log", outputDir, DateTime.Now.ToString("yyyyMMddHHmm"), label);
			_writer = new StreamWriter(path);
			_withConsoleOutput = withConsoleOutput;
		}

		public void WriteLine(String msg, Level level = Level.Normal)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} ", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
			sb.Append(msg);

			this.WriteLine(sb.ToString());

			if (_withConsoleOutput)
			{
				ConsoleColor previosColor = Console.ForegroundColor;
				ConsoleColor currentColor;
				switch (level)
				{
				case Level.High:
					currentColor = ConsoleColor.Yellow;
					break;
				case Level.Medium:
					currentColor = ConsoleColor.White;
					break;
				default:
					currentColor = ConsoleColor.Gray;
					break;
				}

				Console.ForegroundColor = currentColor;
				Console.WriteLine(msg);
				Console.ForegroundColor = previosColor;
			}

		}

		private void WriteLine(String msg)
		{

			lock (_lock)
			{
				_writer.WriteLine(msg);
			}
		}



		public void Close()
		{
			_writer.Close();
		}
	}
}
