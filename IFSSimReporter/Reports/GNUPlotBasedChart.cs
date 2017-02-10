using System;
using System.Diagnostics;

namespace IFSSimReporter.Reports
{
	public abstract class GNUPlotBasedChart
	{
		public GNUPlotBasedChart ()
		{
		}

		protected void Plot(String reportTemplatePath, String GNUPlotExecFile){

			Process gnuplotProcess;
			gnuplotProcess = new Process();
			gnuplotProcess.StartInfo.UseShellExecute = false;
			gnuplotProcess.StartInfo.Arguments = reportTemplatePath;
			gnuplotProcess.StartInfo.FileName = GNUPlotExecFile;
			gnuplotProcess.StartInfo.CreateNoWindow = true;
			gnuplotProcess.StartInfo.RedirectStandardOutput = true;                        

			gnuplotProcess.Start();
			gnuplotProcess.WaitForExit();

		}
	}
}

