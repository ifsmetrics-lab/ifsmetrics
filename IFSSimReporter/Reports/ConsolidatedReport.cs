using System;
using System.Collections;
using System.Collections.Generic;
using IFSSimReporter.Params;
using IFSSharedObjects.Diagnostics;
using IFSSimReporter.Entities;
using System.IO;
using System.Text;
using IFSSimReporter.IO;
using System.Diagnostics;

namespace IFSSimReporter.Reports
{
	public class ConsolidatedReport
	{
		ArrayList _measures;
		ArrayList _categories;

		private Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> _tuples = null;
		private ReportingProcessParams _prms;
		private Log _log;

		public ConsolidatedReport (
			ArrayList measures, 
			ArrayList categories,
			Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples,
			ReportingProcessParams prms, 
			Log log
		)
		{
			_measures = measures;
			_categories = categories;
			_tuples = tuples;
			_prms = prms;
			_log = log;

		}

		private void WritePart1(StreamWriter writer){
			string versionInfo = String.Format("{0} {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
				
			writer.WriteLine ("\\documentclass[a4paper,10pt]{article}");
			writer.WriteLine ("\\usepackage{graphicx}");
			writer.WriteLine ("\\usepackage{amssymb}");
			writer.WriteLine ("\\usepackage{booktabs}");
			writer.WriteLine ("\\usepackage{subcaption}");
			writer.WriteLine ("\\usepackage{alphalph}");
			writer.WriteLine ("\\usepackage{longtable}");
			writer.WriteLine ("\\renewcommand{\\thesubfigure}{\\alphalph{\\value{subfigure}}}");
			writer.WriteLine ("\\title{Comparison of Similarity Measures for Intuitionistic Fuzzy Sets - Report}");

			writer.WriteLine ("\\author{{{0}}}", versionInfo );
			writer.WriteLine (" \\begin{document}");
			writer.WriteLine (" \\maketitle");
		}

	
		private void WritePartN(StreamWriter writer){
			writer.WriteLine (" \\end{document}");

		}

		private void WriteBiblio(StreamWriter writer, Dictionary<string, Reference> references){
			Dictionary<String, String> citeKeys = new Dictionary<String, String> ();
			foreach (Reference r in references.Values) {
				if (r.CiteKey != "") {
					if (!citeKeys.ContainsKey (r.CiteKey)) {
						citeKeys.Add (r.CiteKey, r.Cite);
					}
				}
			}

			writer.WriteLine ("\\begin{thebibliography}{00}");

			foreach (KeyValuePair<String,String> r in citeKeys) {
				writer.WriteLine ("\\bibitem{{{0}}} {1}", r.Key, r.Value);
			}

			writer.WriteLine ("\\end{thebibliography}");

		}

		public string Build()
		{
			String reportPath = String.Format ("{0}IFSReport.tex", _prms.ReportsDir);
			_log.WriteLine (String.Format("Building consolidated report '{0}'...", reportPath));

			//References
			Dictionary<string, Reference> references = new  Dictionary<string, Reference>();
			ReferencesReader refReader = new ReferencesReader(_prms.ReferencesFile);
			refReader.ReadTo (references);

			StreamWriter writer = new StreamWriter (reportPath);
			 WritePart1 (writer);

			//------------------------Settings---------------------------
			writer.WriteLine ("\\section{Experiment Settings}");
			writer.WriteLine ("\\subsection{Similarity Measures}");
			writer.WriteLine ("The following similarity measures for intuitionistic fuzzy sets were used through the experiment: ");

			StringBuilder sb = new StringBuilder ();
			foreach (String measure in _measures) {
				if (references.ContainsKey (measure)) {
					if (references [measure].CiteKey != "") {
						if (sb.Length > 0)
							sb.AppendFormat (", {0} (\\cite{{{1}}})", measure, references [measure].CiteKey);
						else
							sb.AppendFormat ("{0} (\\cite{{{1}}})", measure, references [measure].CiteKey);
					} else {
						if (sb.Length > 0)
							sb.AppendFormat (", {0}", measure);
						else
							sb.Append (measure);
					}
					
				} else {
					if (sb.Length > 0)
						sb.AppendFormat (", {0}", measure);
					else
						sb.Append (measure);
				}
			}
			sb.Append (".");
			writer.WriteLine (sb.ToString());

			writer.WriteLine ("\\subsection{Testing Data}");
			writer.WriteLine ("The following categories from the RCV1 data set were used in the experiment: ");

			sb = new StringBuilder ();
			foreach (String category in _categories) {
				if (sb.Length > 0)
					sb.AppendFormat (", {0}", category);
				else
					sb.Append (category);
			}
			sb.Append (".");
			writer.WriteLine (sb.ToString());
			writer.WriteLine ("\\pagebreak");

			//------------------------Results---------------------------
			writer.WriteLine ("\\section{Results}");

			//writer.WriteLine ("\\subsection{m-Indices}");
			String tmpFormat = _prms.TablesFileFormat;
			_prms.TablesFileFormat =  "latex";
			mIndices_AllCategoriesTable writer0 = new mIndices_AllCategoriesTable(_measures,
				_tuples,  _prms, _log);
			String tablePath = writer0.Build ();
			writer.WriteLine ("\\input{{{0}}}", tablePath.Substring(tablePath.LastIndexOf("/") + 1));
			_prms.TablesFileFormat = tmpFormat;
			writer.WriteLine ("\\pagebreak");

			writer.WriteLine ("\\subsection{Charts}");
			writer.WriteLine ("\\begin{figure}[ht]{\\centering");
			int n = 0;
			int nn = 0;
			foreach (String measure in _measures) {
				SimVsOpp_AllCategoriesChart writer2 = new SimVsOpp_AllCategoriesChart(
					measure, _tuples, _prms, _log);
				String chartPath = writer2.Build ();

				if (n % 20 == 0 && n>0) {
					nn++;
					writer.WriteLine ("\\caption{{Averages of the similarity levels per scenario versus the percentage of opposites included in each scenario (Part {0}).}}", nn);
					writer.WriteLine ("}\\end{figure}");
					writer.WriteLine ("\\begin{figure}[ht]{\\ContinuedFloat\\centering");
				}

				writer.WriteLine ("\\begin{subfigure}[b]{2.5cm}\n\\centering");
				if (_prms.ChartsFileFormat.ToLower () == "latex") {
					writer.WriteLine ("\\resizebox{2cm}{!} {");
					writer.WriteLine ("\\input{{{0}}}", chartPath.Substring (chartPath.LastIndexOf ("/") + 1));
					writer.WriteLine ("}");
				} else {


					writer.WriteLine ("\\includegraphics[height=2cm]{{{0}}}", chartPath.Substring (chartPath.LastIndexOf ("/") + 1));
				}
				writer.WriteLine ("\\caption{{{0}}}", measure);
				writer.WriteLine ("\\end{subfigure} ");
				n++;


				System.GC.Collect ();

			}
			writer.WriteLine ("\\caption{{Averages of the similarity levels per scenario versus the percentage of opposites included in each scenario (Part {0}).}}", nn + 1);
			writer.WriteLine ("}\\end{figure}");

			writer.WriteLine ("\\pagebreak");

			writer.WriteLine ("\\subsection{Linear models}");
			foreach (String measure in _measures) {
				SimVsOppLinearModel_AllCategoriesChart writer2 = new SimVsOppLinearModel_AllCategoriesChart(
					measure, _tuples, _prms, _log);
				String chartPath = writer2.Build ();

				if (_prms.ChartsFileFormat.ToLower () == "latex") {
					writer.WriteLine ("\\begin{figure}[ht]{\\centering");
					writer.WriteLine ("\\resizebox{5cm}{!} {");
					writer.WriteLine ("\\input{{{0}}}", chartPath.Substring (chartPath.LastIndexOf ("/") + 1));
					writer.WriteLine ("}");
					writer.WriteLine ("\\caption{{{0}}}", measure);
					writer.WriteLine ("}\\end{figure}");
				} else {
					writer.WriteLine ("\\begin{figure}[ht]{\\centering");
					writer.WriteLine ("\\includegraphics[height=5cm]{{{0}}}", chartPath.Substring (chartPath.LastIndexOf ("/") + 1));
					writer.WriteLine ("\\caption{{{0} - Linear model}}", measure);
					writer.WriteLine ("}\\end{figure}");
				}
				System.GC.Collect ();
			}
			WriteBiblio (writer, references);
			WritePartN (writer);
			writer.Close ();

			try{
				BuildPDF(reportPath, _prms.PDFLatexExecFile);
			}
			catch(Exception e){
				_log.WriteLine (e.Message, Log.Level.Medium);
			}

			return reportPath;
		}

		void BuildPDF(String latexFile, String pdfLatexPath){

			Process process;
			process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.Arguments = String.Format("-interaction=nonstopmode {0}",latexFile);
			process.StartInfo.FileName = pdfLatexPath;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;                        

			process.Start();
			process.WaitForExit();


		}
	}
}

