using System;
using IFSComparer.Measures;
using IFSComparer.Params;
using IFSComparer.IO;
using System.Xml.Serialization;
using IFSSharedObjects.Diagnostics;
using System.Threading;
using IFSSharedObjects.Models;
using IFSSharedObjects.IO;


namespace IFSComparer
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
			Console.WriteLine ("Usage: 'ifscomparer CONFIG'");
		}
		static void PrintHelp(){
			PrintVersion ();
			Console.WriteLine ("Compares collections of IFSs according to the parameters in CONFIG file.");
			PrintUsage ();

		}

		static ComparisonProcessParams LoadParameters(string configFile){
			ExperimentConfig cfg = new ExperimentConfig ();
			ComparisonProcessParams ret = new ComparisonProcessParams ();

			XmlSerializer reader = new XmlSerializer(typeof(ExperimentConfig));
			System.IO.StreamReader file = new System.IO.StreamReader(configFile);
			cfg =  (ExperimentConfig)reader.Deserialize(file);
			file.Close();
			ret = cfg.ComparisonProcessParams;

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
				else if (prm == "--single" || prm == "-s") {
					string strFile1, strFile2;

					strFile1 = args [1];
					strFile2 = args [2];

					return SingleComparison(strFile1, strFile2);
				}
				else {
					return MainProcess (prm);
				}
			}
			return 0;
		}

		static int SingleComparison(string strFile1, string strFile2){
			// -s  file1 file1

			IFS ifs1 = new IFS(strFile1.Substring(strFile1.LastIndexOf("/") + 1));
			IFS ifs2 = new IFS(strFile2.Substring(strFile2.LastIndexOf("/") + 1));


			IFSReader reader1 = new IFSReader(strFile1);
			IFSReader reader2 = new IFSReader(strFile2);

			reader1.ReadTo(ifs1);
			reader2.ReadTo(ifs2);

			ifs1.EnsureIFSDefinition();
			ifs2.EnsureIFSDefinition();

			IFSMeasure measure = new XVBrSM (0.5, 2);
			double sim = measure.GetSimilarity (ifs1, ifs2);

			Console.WriteLine(String.Format("sim({0},{1}) = {2:F4}", ifs1.Name, ifs2.Name, sim));

			return 0;
		}


		static int MainProcess(string configFile)
		{
			// ../../../input/config/experiment-tutorial.xml

			Log log;
			int ret = 0;

			ComparisonProcessParams prms;
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
				log = new Log(prms.WorkingDir, true, "ifscomparer");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				PrintUsage ();
				return -1;
			}

			long ticks0 = System.DateTime.Now.Ticks;


			log.WriteLine("Starting comparison process ... ", Log.Level.Medium);
			try{
				PerformSimilarityComparisons(prms,log);
			}
			catch(Exception ex) {
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}


			log.WriteLine("Writing references file ... ", Log.Level.Medium);
			try{
				WriteReferencesFile(prms);
			}
			catch(Exception ex) {
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}
			log.WriteLine (String.Format("Find refereces file in {0}", prms.ReferencesFile), Log.Level.Medium);


			log.WriteLine("IFSs comparison process is done!", Log.Level.Medium);
			log.WriteLine (String.Format("Find the results in {0}", prms.SimilaritiesFile), Log.Level.Medium);


			EXIT_WITHLOG:
			long ticks1 = System.DateTime.Now.Ticks;
			DateTime dt = new DateTime(ticks1 - ticks0);
			log.WriteLine(String.Format("Done in {0}!", dt.ToString("HH:mm:ss")), Log.Level.High);

			log.Close();

			return ret;

		}

		static void PerformSimilarityComparisons(
			ComparisonProcessParams prms,
			Log log)
		{

			IO.SimilaritiesWriter writer = new SimilaritiesWriter(prms.SimilaritiesFile);
			writer.WriteHeader();

			Int32 nMaxRunningThreads = 50;
			 
			ManualResetEvent[] doneEvents = new ManualResetEvent[nMaxRunningThreads];
			for (int j = 0; j < nMaxRunningThreads; j++)
			{
				doneEvents[j] = new ManualResetEvent(true);
			}
			int runningThreads = 0;


			foreach(Source source in prms.Sources) {
				int nBatches = source.nDocuments / prms.nDocumentsInEvaluationBatch;
				foreach (Category cat in prms.Categories) {
					foreach (Scenario scenario in prms.Scenarios) {
						int nCurrentBatch = 1;
						while (nCurrentBatch <= nBatches) {

							IFSComparerThreadParams threadParams = new IFSComparerThreadParams ();
							threadParams.category = cat;
							threadParams.refScenario = prms.ReferentScenario;
							threadParams.otherScenario = scenario;
							threadParams.evaluationBatch = nCurrentBatch;
							threadParams.IFSsDir = prms.IFSsDir;
							threadParams.measures = prms.Measures;
							threadParams.log = log;
							threadParams.simWriter = writer;
							threadParams.evaluationSource = source.Code;

							doneEvents [runningThreads].Reset ();
							IFSComparatorThread worker = new IFSComparatorThread (doneEvents [runningThreads]);

							ThreadPool.QueueUserWorkItem (worker.Run, threadParams);
							runningThreads++;

							if (runningThreads >= nMaxRunningThreads) {
								WaitHandle.WaitAll (doneEvents);
								runningThreads = 0;
							}
							nCurrentBatch++;
						}
					}
				}
			}


			WaitHandle.WaitAll(doneEvents);

			writer.Close();

		}


		private static void WriteReferencesFile(ComparisonProcessParams prms){
			ReferencesWriter writer = new ReferencesWriter (prms.ReferencesFile);
			writer.Write (prms.Measures);
			writer.Close ();
		}

		/*
		public static void Main00 (string[] args)
		{
			
			ExperimentConfig xCfg = new ExperimentConfig ();

			ComparisonProcessParams cmpPrms = new ComparisonProcessParams ();
			xCfg.ComparisonProcessParams = cmpPrms;

			cmpPrms.IFSsDir = "/home/marc/Output/IFSApps/output/IFSs/";

			cmpPrms.SimilaritiesFile = "/home/marc/Output/IFSApps/similarities.dat";
			//cmpPrms.nEvaluatedDocuments = 200;
			cmpPrms.nDocumentsInEvaluationBatch = 20;

			cmpPrms.Categories = new Categories ();
			Category cat = new Category ();
			cat.Code = "E11";
			cat.Description = "E11 Desc";
			cmpPrms.Categories.Add (cat);


			Scenarios scenarios = new Scenarios ();

			Scenario scn = new Scenario ();
			scn.Description = "R0";
			scn.Code = "R0";
			scn.OppositesPercentage = 0;

			scenarios.Add (scn);

			cmpPrms.ReferentScenario = scn;


			scn = new Scenario ();
			scn.Code = "R20";
			scn.Description = "R20";
			scn.OppositesPercentage = 0.2F;
			scenarios.Add (scn);

			cmpPrms.Scenarios = scenarios;

			SimilarityMeasures SMs = new SimilarityMeasures ();
			SMs.SMConfigs = new System.Collections.ArrayList ();

			HammingSM sm1 = new HammingSM (HammingSM.Type.TwoDimensions);
			sm1.Code = "H2D";
			SMs.SMConfigs.Add (sm1);

			HammingSM sm2 = new HammingSM (HammingSM.Type.ThreeDimensions);
			sm2.Code = "H3D";
			SMs.SMConfigs.Add (sm2);

			CosineSM sm3 = new CosineSM ();
			sm3.Code = "COS";
			SMs.SMConfigs.Add (sm3);



			VBSM sm4 = new VBSM (0.5);
			sm4.Code = "VB-0.5";
			SMs.SMConfigs.Add (sm4);


			VBSM sm5 = new VBSM (0);
			sm5.Code = "VB-0";
			SMs.SMConfigs.Add (sm5);


			IFSMeasure sm = new AgreementOnDecisionSM ();
			sm.Code = "AoD";
			SMs.SMConfigs.Add (sm);


			sm = new EuclideanSM (EuclideanSM.EuclideanSMType.ThreeDimensions);
			sm.Code = "E-3D";
			SMs.SMConfigs.Add (sm);


			sm = new EuclideanSM (EuclideanSM.EuclideanSMType.TwoDimensions);
			sm.Code = "E-2D";
			SMs.SMConfigs.Add (sm);

			sm = new XVBSM (0.5, 0.1, 5, 0.8, 0.1, 0.1);
			sm.Code = "XVB";
			SMs.SMConfigs.Add (sm);

			sm = new SK1SM (SK1SM.SK1SMType.ThreeDimensions);
			sm.Code = "SK1-3D";
			SMs.SMConfigs.Add (sm);

			sm = new SK1SM (SK1SM.SK1SMType.TwoDimensions);
			sm.Code = "SK1-2D";
			SMs.SMConfigs.Add (sm);


			sm = new SK2SM (SK2SM.SK2SMType.ThreeDimensions);
			sm.Code = "SK2-3D";
			SMs.SMConfigs.Add (sm);

			sm = new SK2SM (SK2SM.SK2SMType.TwoDimensions);
			sm.Code = "SK2-2D";
			SMs.SMConfigs.Add (sm);

			sm = new SK3SM (SK3SM.SK3SMType.ThreeDimensions);
			sm.Code = "SK3-3D";
			SMs.SMConfigs.Add (sm);

			sm = new SK3SM (SK3SM.SK3SMType.TwoDimensions);
			sm.Code = "SK3-2D";
			SMs.SMConfigs.Add (sm);


			sm = new SK4SM (SK4SM.SK4SMType.ThreeDimensions);
			sm.Code = "SK4-3D";
			SMs.SMConfigs.Add (sm);

			sm = new SK4SM (SK4SM.SK4SMType.TwoDimensions);
			sm.Code = "SK4-2D";
			SMs.SMConfigs.Add (sm);




			cmpPrms.Measures = SMs;
			XmlSerializer writer = new XmlSerializer(typeof(ExperimentConfig));

			System.IO.StreamWriter file = new System.IO.StreamWriter (@"/home/marc/Development/experiment2.xml");
			writer.Serialize (file, xCfg);
		}
		*/
	}
}
