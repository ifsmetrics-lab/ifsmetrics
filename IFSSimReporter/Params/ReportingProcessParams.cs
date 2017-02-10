using System;
using System.Xml;
using System.Xml.Serialization;

namespace IFSSimReporter.Params
{
	public enum ReportType{
		SimVsOpp_AllCategoriesChart,
		SimVsOpp_SpecificCategoryChart,
		SimVsOppLinearModel_AllCategoriesChart,
		SimVsOppLinearModel_SpecificCategoryChart,
		mIndices_AllCategoriesTable,
		ConsolidatedReport
	}

	[XmlRoot("ReportingProcessParams")]
	public class ReportingProcessParams
	{
		/// <summary>
		/// Path of the file containing the similarity results.
		/// </summary>
		public String SimilaritiesFile="";


		/// <summary>
		/// Path of the file containing the references to the
		/// similarity measures used during the comparison process.
		/// </summary>
		public String ReferencesFile;

		/// <summary>
		/// Similarity measure to be used as referent.
		/// </summary>
		public String ReferentSimilarityMeasure = "";

		/// <summary>
		/// Path of the gnuplot module.
		/// e.g.: /usr/bin/gnuplot
		/// </summary>
		public String GNUPlotExecFile = "";


		/// <summary>
		/// Path of the pdflatex module.
		/// e.g.: /usr/bin/pdflatex
		/// </summary>
		public String PDFLatexExecFile = "";



		/// <summary>
		/// Directory to place the temporary files.
		/// </summary>
		public String WorkingDir;


		/// <summary>
		/// Directory to place the resulting reports.
		/// </summary>
		public String ReportsDir;

		/// <summary>
		/// Chart file output format.
		/// e.g.: PDF, LATEX
		/// </summary>
		public String ChartsFileFormat;


		/// <summary>
		/// Table file output format.
		/// e.g.: TXT, CSV, LATEX
		/// </summary>
		public String TablesFileFormat;

		/// <summary>
		/// Report to build.
		/// </summary>
		public ReportType Report = ReportType.SimVsOppLinearModel_AllCategoriesChart;

		/// <summary>
		/// The referent measure to compute a m-Index.
		/// </summary>
		public String mIndexReferentMeasure = "AoD"; 
	}
}

