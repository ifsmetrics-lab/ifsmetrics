using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSSimReporter.Entities;
using IFSSharedObjects.Diagnostics;
using IFSSimReporter.Params;
using System.Diagnostics;

namespace IFSSimReporter.Reports
{
	public class SimVsOpp_SpecificCategoryChart:GNUPlotBasedChart
	{
		private StreamWriter _writer = null;
		private Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> _tuples = null;
		private String _category;
		private String _measure;
		private ReportingProcessParams _prms;
		private Log _log;


		public SimVsOpp_SpecificCategoryChart(
			String category,
			String measure,
			Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples,
			ReportingProcessParams prms, 
			Log log
			)
		{
			_tuples = tuples;
			_category = category;
			_measure = measure;
			_log = log;
			_prms = prms;
		
		}


		public string Build()
		{
			String ext = ".pdf";
			if (_prms.ChartsFileFormat != null) {
				switch (_prms.ChartsFileFormat.ToLower ()) {
				case "pdf":
					ext = ".pdf";
					break;
				case "svg":
					ext = ".svg";
					break;
				case "latex":
					ext = ".tex";
					break;
				}
			}

			String reportPath = String.Format ("{0}{1}_{2}{3}", _prms.ReportsDir, _category, _measure, ext);

			_log.WriteLine (String.Format("Building SimVsOpp_SpecificCategory chart '{0}'...", reportPath));

			String dataFilePath = BuildDataFile ();

			String templateFilePath = BuildTemplateFile (dataFilePath, reportPath);
			Plot (templateFilePath, _prms.GNUPlotExecFile);

			_log.WriteLine (String.Format("SimVsOpp_SpecificCategory chart'{0}' is done!", reportPath));
			return reportPath;
		}

		private String BuildDataFile()
		{

			String path = string.Format ("{0}rp.{1}.{2}.dat", _prms.WorkingDir, _category, _measure);
		
			_log.WriteLine (String.Format("Building data file '{0}'...", path));
			IEnumerable<CAT_SM_SCN1_SCN2_Tuple> query = _tuples.Values.Where(t => t.Category==_category && t.SimilarityMeasure ==_measure ).OrderByDescending ( t => t.OppositePercentageOtherScenario);
			_writer = new StreamWriter (path);

			try{
				foreach(CAT_SM_SCN1_SCN2_Tuple tuple in query){
					_writer.WriteLine ("{0}-{1}\t{2}\t{3}", tuple.ReferentScenario, tuple.OtherScenario, tuple.OppositePercentageOtherScenario*100, tuple.Average);
				}
			}
			finally{
				
				_writer.Close ();
			}
			_log.WriteLine (String.Format("Data file '{0}' is done!", path));
			return path;
		}

		private String BuildTemplateFile(String dataFilePath, String reportPath)
		{
			String terminal = "pdf";
			String xLabel = "'% opposites'";
			String yLabel = "'Average similarity' offset -1";
			String xFormat = "%.0f%%";
			if (_prms.ChartsFileFormat != null) {
				switch (_prms.ChartsFileFormat.ToLower ()) {
				case "pdf":
					terminal = "pdf";
					xLabel = "'% opposites'";
					xFormat = "%.0f%%";
					break;
				case "svg":
					terminal = "svg";
					xLabel = "'% opposites'";
					xFormat = "%.0f%%";
					break;
				case "latex":
					terminal = "latex";
					xLabel = "'\\% opposites'";
					yLabel = "'\\rotatebox{90}{Average similarity}' offset -1";
					xFormat = "%.0f\\%%";
					break;
				}
			}

			String reportTemplatePath = String.Format ("{0}{1}_{2}.template", _prms.WorkingDir, _category, _measure);

			_log.WriteLine (String.Format("Building template file '{0}'...", reportTemplatePath));
			StreamWriter w = new StreamWriter (reportTemplatePath);
			try{
				w.WriteLine("set terminal {0}", terminal);
				w.WriteLine("set output '{0}'",reportPath);
				w.WriteLine ("set title 'Category: {0}'", _category);
				w.WriteLine ("set xlabel {0}", xLabel);
				w.WriteLine ("set ylabel {0}", yLabel);
				w.WriteLine ("set format x '{0}'",xFormat);
				w.WriteLine ("set format y '%g'");
				w.WriteLine ("set key top right");
				w.WriteLine ("set size square");
				w.WriteLine ("set yrange [0:1]");
				w.WriteLine ("plot '{0}' using 2:3 title '{1}' with linespoints ls 3 pt 18 pi 0.2", dataFilePath,  _measure);
			}
			finally{
				w.Close ();
			}
			_log.WriteLine (String.Format("Template file '{0}' is done", reportTemplatePath));
			return reportTemplatePath;
		}



	}
}

