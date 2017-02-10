using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using IFSSimReporter.Entities;
using IFSSimReporter.Params;
using IFSSimReporter.Utils;
using IFSSharedObjects.Diagnostics;

namespace IFSSimReporter.Reports
{
	public class mIndices_AllCategoriesTable:mIndicesTable
	{
		private StreamWriter _writer = null;
		private Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> _tuples = null;
		private ArrayList _measures;
		private ReportingProcessParams _prms;
		private Log _log;

		public mIndices_AllCategoriesTable (
			ArrayList measures,
			Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples,
			ReportingProcessParams prms, 
			Log log
		)
		{
			_tuples = tuples;
			_measures = measures;
			_log = log;
			_prms = prms;


		}


		protected override void WriteFooter ()
		{
			string footer = ""; 
			string versionInfo = String.Format("{0} {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			if (_prms.TablesFileFormat != null) {
				switch (_prms.TablesFileFormat.ToLower ()) {
				case "csv":
					footer = "";
					footer = footer + "\n#" + versionInfo;
					break;
				case "latex":
					footer = "\\end{longtable}\n";
					footer = footer + "\n%" + versionInfo;
					break;
				default:
					footer = "---------------------------------------------------------------------------";
					footer = footer + "\n#" + versionInfo;
					break;
				}

			}


			_writer.WriteLine (footer);
		}

		protected override void WriteHeader ()
		{
			string header = "SM,slope (a),intercept (b),R^2,m-index"; 
			if (_prms.TablesFileFormat != null) {
				switch (_prms.TablesFileFormat.ToLower ()) {
				case "csv":
					header = "SM,slope (a),intercept (b),R^2,m-index";
					break;
				
				case "latex":
					header = "%\\usepackage{booktabs}\n" +
						"%\\usepackage{longtable}\n" +
						"\\begin{longtable}[ht]{lrcrr}\n" + 
						"\\caption{Linear models and $m$-indices for each \\emph{SM-vs.-OP} representing the relationship between the averages levels that result from the (configuration of) similarity measure \\emph{SM} and the percentage of opposites \\emph{OP}.}\n" +
						"\\label{tbl:SMs-results}\\\\\n" +
						"\\toprule\n" +
						"& \\multicolumn{3}{c}{ \\emph{SM-vs.-OP} (linear model: $y=ax+b$)}& \\\\\n" +
						"\\cmidrule(r){2-4}\n" +
						"\\multicolumn{1}{c}{Similarity Measure (SM)} & \\multicolumn{1}{c}{slope ($a$)} & \\multicolumn{1}{c}{intercept ($b$)} & \\multicolumn{1}{c}{$R^2$} & \\multicolumn{1}{c}{m-index} \\\\\n" + 
						"\\midrule\n" +
						"\\endhead\n" + 
						"\\bottomrule\n" +
						"\\endfoot\n";
					break;
				default: // "txt":
					header = String.Format ("{0,15}\t{1,10}\t{2,10}\t{3,10}\t{4,10}\n", "SM", "slope (a)", "intercept (b)", "R^2", "m-index");
					header = header + "---------------------------------------------------------------------------";
					break;
				}
			}


			_writer.WriteLine (header);

		}

		protected override void WriteLine (string measure, double slope, double intercept, double r2, double mIndex)
		{
			string lineFormat = "";
			if (_prms.TablesFileFormat != null) {
				switch (_prms.TablesFileFormat.ToLower ()) {
				case "csv":
					lineFormat = "{0},{1:F4},{2:F4},{3:F4},{4:F4}";
					break;
				case "latex":
					lineFormat = "{0}&${1:F4}$&${2:F4}$&${3:F4}$&${4:F4}$\\\\";
					break;
				default: // "txt":
					lineFormat = "{0,15}\t{1,10:F4}\t{2,10:F4}\t{3,10:F4}\t{4,10:F4}";
					break;
				}
			}

			string line = String.Format(lineFormat,
				measure,
				slope,
				intercept,
				r2,
				mIndex
			);

			_writer.WriteLine (line);
			
		}

		private void ComputeStatistics(String measure, out double slope, out double intercept, out double R)
		{
			IEnumerable<CAT_SM_SCN1_SCN2_Tuple> query = _tuples.Values.Where (t => t.SimilarityMeasure == measure);

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

			R = Statistics.Pearson (Xs, Ys);
			slope = Statistics.Slope (Xs, Ys);
			intercept = Statistics.Intercept (Xs, Ys);
		}

		public string Build()
		{
			String ext = "";
			if (_prms.TablesFileFormat != null) {
				switch (_prms.TablesFileFormat.ToLower ()) {
				case "csv":
					ext = ".csv";
					break;
				case "latex":
					ext = ".tex";
					break;
				default: // "txt":
					ext = ".txt";
					break;
				}
			}

			String reportPath = String.Format ("{0}allcat_mIndices{1}", _prms.ReportsDir, ext);
			_log.WriteLine (String.Format("Building mIndices_AllCategories table '{0}'...", reportPath));

			_writer = new StreamWriter (reportPath);
		

			double refSlope;
			double refIntercept;
			double refR;

			ComputeStatistics (_prms.mIndexReferentMeasure, out refSlope, out refIntercept, out refR);

			WriteHeader ();

			foreach (String measure in _measures) {
				if (measure != _prms.mIndexReferentMeasure) {
					double slope;
					double intercept;
					double R;
					ComputeStatistics (measure, out slope, out intercept, out R);

					double mIndex = GetMIndex (refSlope, refIntercept, refR, slope, intercept, R);

					WriteLine (measure, slope, intercept, R * R, mIndex);
				}
			}

			double refMIndex = GetMIndex (refSlope, refIntercept, refR, refSlope, refIntercept, refR);
			WriteLine (_prms.mIndexReferentMeasure, refSlope, refIntercept, refR *refR, refMIndex);

			WriteFooter ();


			_writer.Close ();

			_log.WriteLine (String.Format("mIndices_AllCategories table '{0}' is done!", reportPath));
			return reportPath;
		}
	}
}

