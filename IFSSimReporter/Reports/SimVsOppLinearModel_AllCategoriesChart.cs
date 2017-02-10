
using System;
using IFSSimReporter.Utils;
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
	public class SimVsOppLinearModel_AllCategoriesChart:GNUPlotBasedChart
	{

		private StreamWriter _writer = null;
		private Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> _tuples = null;
		private String _measure;
		private ReportingProcessParams _prms;
		private Log _log;


		public SimVsOppLinearModel_AllCategoriesChart(
			String measure,
			Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples,
			ReportingProcessParams prms, 
			Log log
		)
		{
			_tuples = tuples;
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

			String reportPath = String.Format ("{0}allcat_{1}_LM{2}", _prms.ReportsDir,  _measure.Replace('.','_'), ext);
			_log.WriteLine (String.Format("Building SimVsOppLinearModel_AllCategories chart '{0}'...", reportPath));



			IEnumerable<CAT_SM_SCN1_SCN2_Tuple> query = _tuples.Values.Where (t => t.SimilarityMeasure == _measure);

			Dictionary<String /*otherScenario*/, CAT_SM_SCN1_SCN2_Tuple> aggTuples = new Dictionary<string, CAT_SM_SCN1_SCN2_Tuple> ();
			foreach(CAT_SM_SCN1_SCN2_Tuple tuple in query){
				if (aggTuples.ContainsKey (tuple.OtherScenario)) {
					aggTuples[tuple.OtherScenario].Count = aggTuples[tuple.OtherScenario].Count + 1;
					aggTuples[tuple.OtherScenario].Sum = aggTuples[tuple.OtherScenario].Sum + tuple.Average;
				} else {
					CAT_SM_SCN1_SCN2_Tuple newTuple = new CAT_SM_SCN1_SCN2_Tuple ();
					newTuple.Count = 1;
					newTuple.Category = "All of them";
					newTuple.Sum = tuple.Average;
					newTuple.OppositePercentageOtherScenario = tuple.OppositePercentageOtherScenario;
					newTuple.OppositePercentageReferentScenario = tuple.OppositePercentageReferentScenario;
					newTuple.OtherScenario = tuple.OtherScenario;
					newTuple.ReferentScenario = tuple.ReferentScenario;
					newTuple.SimilarityMeasure = tuple.SimilarityMeasure;

					aggTuples.Add (tuple.OtherScenario, newTuple);
				}
			}

			query = aggTuples.Values.OrderBy(t => t.OppositePercentageOtherScenario);

			IEnumerable<double> Xs= query.Select (x => x.OppositePercentageOtherScenario);
			IEnumerable<double> Ys= query.Select (x => x.Average);

			double R = Statistics.Pearson (Xs, Ys);
			double slope = Statistics.Slope (Xs, Ys);
			double intercept = Statistics.Intercept (Xs, Ys);

			String dataFilePath = BuilLinearModelDataFile (query, R, slope, intercept);

			String templateFilePath = BuildTemplateFile (dataFilePath, reportPath, R, slope, intercept);
			Plot (templateFilePath,_prms.GNUPlotExecFile);

			_log.WriteLine (String.Format("SimVsOppLinearModel_AllCategories chart '{0}' is done!", reportPath));
			return reportPath;
		}

		private String BuilLinearModelDataFile(IEnumerable<CAT_SM_SCN1_SCN2_Tuple> query, double R, double slope, double intercept)
		{

			String path = string.Format ("{0}rp.allcat.{1}.LM.dat", _prms.WorkingDir, _measure);

			_log.WriteLine (String.Format("Building linear model data file '{0}'...", path));
			_writer = new StreamWriter (path);

			try{
				foreach(CAT_SM_SCN1_SCN2_Tuple tuple in query){
					_writer.WriteLine ("{0}-{1}\t{2}\t{3}\t{4}", 
						tuple.ReferentScenario, 
						tuple.OtherScenario, 
						tuple.OppositePercentageOtherScenario*100, 
						tuple.Average,
						slope*tuple.OppositePercentageOtherScenario + intercept
					);
				}
			}
			finally{

				_writer.Close ();
			}
			_log.WriteLine (String.Format("Data file '{0}' is done!", path));
			return path;
		}

		private String BuildDataFile()
		{

			String path = string.Format ("{0}rp.allcat.{1}.LM.dat", _prms.WorkingDir, _measure);

			_log.WriteLine (String.Format("Building data file '{0}'...", path));
			IEnumerable<CAT_SM_SCN1_SCN2_Tuple> query = _tuples.Values.Where (t => t.SimilarityMeasure == _measure);

			Dictionary<String /*otherScenario*/, CAT_SM_SCN1_SCN2_Tuple> aggTuples = new Dictionary<string, CAT_SM_SCN1_SCN2_Tuple> ();
			foreach(CAT_SM_SCN1_SCN2_Tuple tuple in query){
				if (aggTuples.ContainsKey (tuple.OtherScenario)) {
					aggTuples[tuple.OtherScenario].Count = aggTuples[tuple.OtherScenario].Count + 1;
					aggTuples[tuple.OtherScenario].Sum = aggTuples[tuple.OtherScenario].Sum + tuple.Average;
				} else {
					CAT_SM_SCN1_SCN2_Tuple newTuple = new CAT_SM_SCN1_SCN2_Tuple ();
					newTuple.Count = 1;
					newTuple.Category = "All of them";
					newTuple.Sum = tuple.Average;
					newTuple.OppositePercentageOtherScenario = tuple.OppositePercentageOtherScenario;
					newTuple.OppositePercentageReferentScenario = tuple.OppositePercentageReferentScenario;
					newTuple.OtherScenario = tuple.OtherScenario;
					newTuple.ReferentScenario = tuple.ReferentScenario;
					newTuple.SimilarityMeasure = tuple.SimilarityMeasure;

					aggTuples.Add (tuple.OtherScenario, newTuple);
				}
			}

			query = aggTuples.Values.OrderBy(t => t.OppositePercentageOtherScenario);
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

		private String BuildTemplateFile(String dataFilePath, String reportPath, double R, double slope, double intercept)
		{
			String reportTemplatePath = String.Format ("{0}allcat_{1}.LM.template", _prms.WorkingDir, _measure);
			String linearModel = String.Format ("y = {0:F3}x {1} {2:F3}",
				slope, intercept<0?'-':'+', Math.Abs(intercept)
			);
			String Rlabel = String.Format ("R = {0:F3}", R);

			String terminal = "pdf";
			String xLabel = "'% opposites'";
			String yLabel = "'Average similarity' offset -1";
			String xFormat = "%.0f%%";
			if (_prms.ChartsFileFormat != null) {
				switch (_prms.ChartsFileFormat.ToLower ()) {
				case "pdf":
					terminal = "pdf size 4,4";
					xLabel = "'% opposites'";
					xFormat = "%.0f%%";
					break;
				case "svg":
					terminal = "svg";
					xLabel = "'% opposites'";
					xFormat = "%.0f%%";
					break;
				case "latex":
					terminal = "latex  size 4,4";
					xLabel = "'\\% opposites'";
					yLabel = "'\\rotatebox{90}{Average similarity}' offset -1";
					xFormat = "%.0f\\%%";
					linearModel = "$\\tiny{" + linearModel + "}$";
					Rlabel = "$\\tiny{" + Rlabel + "}$";
					break;
				}
			}

			_log.WriteLine (String.Format("Building template file '{0}'...", reportTemplatePath));
			StreamWriter w = new StreamWriter (reportTemplatePath);
			try{
				w.WriteLine("set terminal {0}", terminal);
				w.WriteLine("set output '{0}'",reportPath);
				w.WriteLine ("set title 'Category: All of them'" );
				w.WriteLine ("set xlabel {0}", xLabel);
				w.WriteLine ("set ylabel {0}", yLabel);
				w.WriteLine ("set format x '{0}'",xFormat);
				w.WriteLine ("set format y '%g'");
				w.WriteLine ("set key top right");
				w.WriteLine ("set size square");
				w.WriteLine ("set yrange [0:1]");
				w.WriteLine ("set label '{0}' at graph 0.95, 0.80 right font 'Simbol,10'", linearModel);
				w.WriteLine ("set label '{0}' at graph 0.95, 0.75 right font 'Simbol,10'", Rlabel);
				w.WriteLine ("plot '{0}' using 2:3 title '{1}' with linespoints ls 3 pt 18 pi 0.2," +
					"'{0}' using 2:4 title 'LM {1}' with lines ", dataFilePath,  _measure);
			}
			finally{
				w.Close ();
			}
			_log.WriteLine (String.Format("Template file '{0}' is done", reportTemplatePath));
			return reportTemplatePath;
		}

	}
}

