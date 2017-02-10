using System;
using IFSSimReporter.Params;
using IFSSimReporter.IO;
using IFSSimReporter.Entities;
using IFSSimReporter.Templates;
using IFSSimReporter.Reports;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using IFSSharedObjects.Diagnostics;
using System.Collections;
using System.Text;

namespace IFSSimReporter
{
	class MainClass
	{
		static void PrintVersion()
		{
			string versionInfo = String.Format("{0} {1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
				System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

			Console.WriteLine (versionInfo);
		}
		static void PrintUsage(){
			Console.WriteLine ("Usage: 'ifssimreporter CONFIG'");
		}
		static void PrintHelp(){
			PrintVersion ();
			Console.WriteLine ("Reports the similarity results according to the parameters in CONFIG file.");
			PrintUsage ();

		}

		static ReportingProcessParams LoadParameters(string configFile){
			ExperimentConfig cfg = new ExperimentConfig ();
			ReportingProcessParams ret = new ReportingProcessParams ();
		

			XmlSerializer reader = new XmlSerializer(typeof(ExperimentConfig));
			System.IO.StreamReader file = new System.IO.StreamReader(configFile);
			cfg =  (ExperimentConfig)reader.Deserialize(file);
			file.Close();
			ret = cfg.ReportingProcessParams;

			return ret;
		}

		static int Main(string[] args)	
		{
			if (args.Length < 1) {
				Console.WriteLine("Missing configuration file.");
				PrintUsage ();
				return 0;
			}
			foreach (String prm in args) {
				if (prm == "--help" || prm == "-h" ) {
					PrintHelp ();
					return 0;
				}
				else if (prm == "--version" || prm == "-v") {
					PrintVersion ();
					return 0;
				}
				else {
					return MainProcess (prm);
				}
			}
			return 0;
		}

		public static int MainProcess (string configFile)
		{
			Log log;
			int ret = 0;

			ReportingProcessParams prms;
			try
			{
				prms = LoadParameters(configFile);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				PrintUsage ();
				return -1;
			}

			try
			{
				log = new Log(prms.WorkingDir, true, "ifssimreporter");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				PrintUsage ();
				return -1;
			}

			long ticks0 = System.DateTime.Now.Ticks;


			log.WriteLine("Starting IFS similarities reporting process ... ", Log.Level.Medium);
			try{
				BuildReports(prms,log);
			}
			catch(Exception ex) {
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}

			log.WriteLine("IFS similarities reporting process is done!", Log.Level.Medium);
			log.WriteLine (String.Format("Find the reports in {0}", prms.ReportsDir), Log.Level.Medium);


			EXIT_WITHLOG:
			long ticks1 = System.DateTime.Now.Ticks;
			DateTime dt = new DateTime(ticks1 - ticks0);
			log.WriteLine(String.Format("Done in {0}!", dt.ToString("HH:mm:ss")), Log.Level.High);

			log.Close();


			return ret;

		}



		static void BuildReports(ReportingProcessParams prms, Log log){

			Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples = new Dictionary<string, CAT_SM_SCN1_SCN2_Tuple> ();
			ArrayList measures = new ArrayList ();
			ArrayList categories = new ArrayList ();


			SimilaritiesReader reader = new SimilaritiesReader (prms.SimilaritiesFile);
			reader.ReadTo (tuples, measures, categories);

			StringBuilder sb = new StringBuilder ("Measures: ");
			foreach (string measure in measures) {
				sb.AppendFormat ("{0} ", measure);
			}
			log.WriteLine (sb.ToString (), Log.Level.Medium);


			sb = new StringBuilder ("Categories: ");
			foreach (string category in categories) {
				sb.AppendFormat ("{0} ", category);
			}
			log.WriteLine (sb.ToString (), Log.Level.Medium);


			switch(prms.Report){
			case ReportType.SimVsOppLinearModel_AllCategoriesChart:
				foreach (String measure in measures) {
					SimVsOppLinearModel_AllCategoriesChart writer = new SimVsOppLinearModel_AllCategoriesChart(
						measure, tuples, prms, log);
					writer.Build ();

					System.GC.Collect ();
				}
				break;
			case ReportType.SimVsOppLinearModel_SpecificCategoryChart:
				foreach (String category in categories) {
					foreach (String measure in measures) {
						SimVsOppLinearModel_SpecificCategoryChart writer = new SimVsOppLinearModel_SpecificCategoryChart(
							category, measure, tuples, prms, log);
						writer.Build ();

						System.GC.Collect ();
					}
				}
				break;
			case ReportType.SimVsOpp_AllCategoriesChart:
				foreach (String measure in measures) {
					SimVsOpp_AllCategoriesChart writer = new SimVsOpp_AllCategoriesChart(
					measure, tuples, prms, log);
					writer.Build ();

					System.GC.Collect ();
				}
				break;
			case ReportType.SimVsOpp_SpecificCategoryChart:
				foreach (String category in categories) {
					foreach (String measure in measures) {
						SimVsOpp_SpecificCategoryChart writer = new SimVsOpp_SpecificCategoryChart(
							category, measure, tuples, prms, log);
						writer.Build ();

						System.GC.Collect ();
					}
				}
				break;
			case ReportType.mIndices_AllCategoriesTable:
				mIndices_AllCategoriesTable writer2 = new mIndices_AllCategoriesTable(measures,
					tuples,  prms, log);
				writer2.Build();
				break;
			case ReportType.ConsolidatedReport:
				ConsolidatedReport report = new ConsolidatedReport (
					                            measures,
					                            categories,
					                            tuples,
					                            prms,
					                            log);
				report.Build ();
				break;
			}

				
		}


	
	}
}
