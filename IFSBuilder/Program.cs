using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml.Serialization;
using IFSBuilder.Params;
using System.IO;
using System.Collections;
using IFSSharedObjects.Diagnostics;
using IFSBuilder.IO;
using IFSBuilder.Collections;


namespace IFSBuilder
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
			Console.WriteLine ("Usage: 'ifsbuilder CONFIG'");
		}
		static void PrintHelp(){
			PrintVersion ();
			Console.WriteLine ("Builds a collection of IFSs according to the parameters in CONFIG file.");
			PrintUsage ();

		}

		static BuildingProcessParams LoadParameters(string configFile){
			ExperimentConfig cfg = new ExperimentConfig ();
			BuildingProcessParams ret = new BuildingProcessParams ();

			XmlSerializer reader = new XmlSerializer(typeof(ExperimentConfig));
			System.IO.StreamReader file = new System.IO.StreamReader(configFile);
			cfg =  (ExperimentConfig)reader.Deserialize(file);
			file.Close();
			ret = cfg.BuildingProcessParams;

			return ret;
		}


		static int Main(string[] args)	
		{
			if (args.Length < 1) {
				Console.WriteLine("Missing configuration file.");
				PrintUsage ();
				return -1;
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

		static int MainProcess(String configFile)
		{
			Log log;
			int ret = 0;

			BuildingProcessParams prms;
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
				log = new Log(prms.WorkingDir, true, "ifsbuilder");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				PrintUsage ();
				return -1;
			}

			long ticks0 = System.DateTime.Now.Ticks;


			log.WriteLine("Starting building process ... ", Log.Level.Medium);

			DocumentCategoryRelations documentTags;
			Documents trainingDocs;
			Features features;

			log.WriteLine("Loading training data ... ", Log.Level.Medium);
			try{
				documentTags = LoadDocumentsTags (prms, log);
				trainingDocs = LoadTrainingDocumens(prms, log, documentTags);
				features = ExtractFeaturesFromTrainingDocs (prms, log,trainingDocs);

				FeaturesWriter tokensWriter = new FeaturesWriter(String.Format("{0}featuresDictionary.dat", prms.WorkingDir));
				tokensWriter.WriteHeader(true);
				tokensWriter.Write(features);
				tokensWriter.Close();

			}
			catch(Exception ex)
			{
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}
			log.WriteLine("Training data is loaded ", Log.Level.Medium);


			log.WriteLine("Starting learning process ... ", Log.Level.Medium);
			Dictionary<String, String> kModels;
			try{
				kModels = PerformTrainingProcess (prms, log, documentTags, trainingDocs, features);
			}
			catch(Exception ex)
			{
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}
			log.WriteLine("Learning process is done!", Log.Level.Medium);


			log.WriteLine("Starting IFSs building process ... ", Log.Level.Medium);
			try{
				BuildIFSs (prms, log, documentTags, trainingDocs, features, kModels);
			}
			catch (Exception ex)
			{
				log.WriteLine(ex.Message, Log.Level.High);
				log.WriteLine("Process halted!", Log.Level.High);
				ret = -1;
				goto EXIT_WITHLOG;
			}

			log.WriteLine("IFSs building process is done!", Log.Level.Medium);
			log.WriteLine (String.Format("Find the resulting IFSs in {0}", prms.EvaluationParams.IFSsDir), Log.Level.Medium);


EXIT_WITHLOG:
			long ticks1 = System.DateTime.Now.Ticks;
			DateTime dt = new DateTime(ticks1 - ticks0);
			log.WriteLine(String.Format("Done in {0}!", dt.ToString("HH:mm:ss")), Log.Level.High);

			log.Close();

			return ret;
		}


		static Documents LoadTrainingDocumens(
			BuildingProcessParams prms,
			Log log,
			DocumentCategoryRelations documentTags
		)
		{
			DocumentsWithTokensReader trainingDocsReader = new DocumentsWithTokensReader(prms.LearningParams.DocumentsFile);

			Documents trainingDocs = new Documents();

			int nLoadedTrainingDocs = 0;
			int nDocumentsToLoad;
			int nDocumentsForTraining = prms.LearningParams.nDocuments;        
			int nDocumentsInBatch = 50; // it must be less or equal to 64 because of the handles!        

			do
			{
				nDocumentsToLoad = nDocumentsForTraining - nLoadedTrainingDocs > nDocumentsInBatch ? nDocumentsInBatch : nDocumentsForTraining - nLoadedTrainingDocs;
				trainingDocs.Load(trainingDocsReader, nDocumentsToLoad);
				nLoadedTrainingDocs += nDocumentsInBatch;


			} while ((nLoadedTrainingDocs < nDocumentsForTraining) && !trainingDocsReader.EndOfStream);
			trainingDocsReader.Close();

			nLoadedTrainingDocs = trainingDocs.Count;
			log.WriteLine(String.Format("{0} training documents loaded!", nLoadedTrainingDocs));

			log.WriteLine("Loading tagged categories to training documents ...");
			trainingDocs.LoadTaggedCategories(documentTags);

			return trainingDocs;
		}

		static DocumentCategoryRelations LoadDocumentsTags(BuildingProcessParams prms,
			Log log
		)
		{
			DocumentCategoryRelations documentTags = new DocumentCategoryRelations();

			log.WriteLine("Loading document-category relations ...");
			documentTags.LoadTaggedCategories(prms.LearningParams.DocumentCategoryRelationsFile);
			log.WriteLine(String.Format("{0} documents loaded with their corresponding category relations!", documentTags.Count));

			return documentTags;
		}


		static Features ExtractFeaturesFromTrainingDocs(BuildingProcessParams prms,
			Log log,
			Documents trainingDocs
		)
		{
			Features features = new Features();
			log.WriteLine("Extracting features  ...");

			int nTrainingCategories = prms.Categories.Count;
			features.Load(trainingDocs, nTrainingCategories);
			features.AssignNumericCodes();

			return features;
		}

		static Dictionary<String, String> PerformTrainingProcess(BuildingProcessParams prms,
			Log log,
			DocumentCategoryRelations documentsTags,
			Documents trainingDocs,
			Features features

		){
			DocumentVectorsWriter vectorsWriter;

			Dictionary<String, String> svmModels = new Dictionary<string, string>();
			Process svmProcess;
			String svmTrainingFile;
			String svmModelFile;
			Int32 nDocumentsForTraining = trainingDocs.Count;

			foreach (Scenario scenario in prms.Scenarios) {
				foreach (Category category in prms.Categories) {
					
 					log.WriteLine(String.Format("Writing {0} training document-vectors for {1}-{2} ...", nDocumentsForTraining, scenario.Code, category.Code));
					svmTrainingFile = String.Format("{0}{1}-{2}-training.dvs", prms.LearningParams.SVMDocumentVectorsDir, scenario.Code, category.Code);
					vectorsWriter = new DocumentVectorsWriter(svmTrainingFile); 
					vectorsWriter.Write(trainingDocs, features, documentsTags, category.Code, scenario.OppositesPercentage);
					vectorsWriter.Close();


					log.WriteLine(String.Format("SVM-learning for {0}-{1} ...", scenario.Code, category.Code), Log.Level.Medium);
					svmProcess = new Process();
					svmModelFile = String.Format("{0}{1}-{2}-svm-model.dat", prms.LearningParams.SVMModelsDir,scenario.Code, category.Code);

					svmModels.Add(scenario.Code + "-" + category.Code, svmModelFile);

					svmProcess.StartInfo.UseShellExecute = false;
					svmProcess.StartInfo.Arguments = String.Format("-c 1 {0} {1}", svmTrainingFile, svmModelFile);
					svmProcess.StartInfo.FileName = prms.LearningParams.SVMLearningExecFile;
					svmProcess.StartInfo.CreateNoWindow = true;
					svmProcess.StartInfo.RedirectStandardOutput = true;                        

					svmProcess.Start();
					log.WriteLine(svmProcess.StandardOutput.ReadToEnd(), Log.Level.Normal);
					svmProcess.WaitForExit();


				}
			}
			return svmModels;
		}

		public static void BuildIFSs(BuildingProcessParams prms,
			Log log,
			DocumentCategoryRelations documentTags,
			Documents trainingDocs,
			Features features,
			Dictionary<String, String> svmModels
		)
		{
			Dictionary<String, Models.FIModel> fiModels = new Dictionary<string, Models.FIModel>();
			foreach (KeyValuePair<String, String> pair in svmModels)
			{
				Models.FIModel model = new Models.FIModel();
				SVMModelReader reader = new SVMModelReader(pair.Value);
				reader.ReadTo(model);
				fiModels.Add(pair.Key, model);
			}

			IO.IFSsStatisticsWriter writer2 = new IFSsStatisticsWriter(String.Format("{0}IFSsStatistics.dat", prms.WorkingDir));
			writer2.WriteHeader();

			int nDocumentsInBatch = prms.EvaluationParams.nDocumentsInBatch;

			ManualResetEvent[] doneEvents = new ManualResetEvent[nDocumentsInBatch];
			for (int j = 0; j < nDocumentsInBatch; j++)
			{
				doneEvents[j] = new ManualResetEvent(true);
			}
			int runningThreads = 0;
			int nDocumentsToLoad = 0;

			foreach (Source source in prms.EvaluationParams.Sources) {

				if (source.nDocuments > 0) {

					DocumentsWithTokensReader reader = new DocumentsWithTokensReader (source.DocumentsFile);
					log.WriteLine (String.Format ("Loading testing {0} documents from {1} ...", source.nDocuments, source.DocumentsFile));

					Int32 nLoadedDocs = 0;
					int nBatch = 0;

					do {
						nDocumentsToLoad = source.nDocuments - nLoadedDocs > nDocumentsInBatch ? nDocumentsInBatch : source.nDocuments - nLoadedDocs;

						Documents docs = new Documents ();
						docs.Load (reader, nDocumentsToLoad);
						nBatch++;

						doneEvents [runningThreads].Reset ();


						IFSBuilderThread thread = new IFSBuilderThread (doneEvents [runningThreads]);
						IFSBuilderThreadParams threadPrms = new IFSBuilderThreadParams ();
						threadPrms.buildingProcessParams = prms;
						threadPrms.documentsToEvaluate = docs;
						threadPrms.documentTags = documentTags;
						threadPrms.knowledgeModels = fiModels;
						threadPrms.log = log;
						threadPrms.sourceCode = source.Code;
						threadPrms.nBatch = nBatch;
						threadPrms.statisticsWriter = writer2;
						threadPrms.features = features;                                

						ThreadPool.QueueUserWorkItem (thread.Run, threadPrms);
						runningThreads++;

						if (runningThreads >= nDocumentsInBatch) {
							// Wait for all threads in pool to calculate.
							WaitHandle.WaitAll (doneEvents);
							runningThreads = 0;
						}
						
						nLoadedDocs += nDocumentsInBatch;

					} while ((nLoadedDocs < source.nDocuments) && !reader.EndOfStream);
					reader.Close ();
				}
			}

			WaitHandle.WaitAll(doneEvents);
			writer2.Close();

			//log.WriteLine(String.Format("The production of IFSs for the scenario {0} is done!", currentScenario), Log.Level.High);

		}


		public static void Main00 (string[] args)
		{
			XmlSerializer writer = new XmlSerializer(typeof(BuildingProcessParams));
			System.IO.StreamWriter file = new StreamWriter (@"/home/marc/Development/experiment.xml");

			BuildingProcessParams prms = new BuildingProcessParams ();

			Scenarios scenarios = new Scenarios ();

			Scenario scn = new Scenario ();
			scn.Description = "R0";
			scn.OppositesPercentage = 0;
		
			scenarios.Add (scn);

			scn = new Scenario ();
			scn.Description = "R20";
			scn.OppositesPercentage = 0.2F;
			scenarios.Add (scn);

			prms.Scenarios = scenarios;


			LearningProcessParams learningPrms = new LearningProcessParams ();
			learningPrms.DocumentsFile = "/home/marc/share/datasets/RCV1/lyrl2004_tokens_train.dat";
			learningPrms.nDocuments = 100;
			learningPrms.SVMLearningExecFile = "/home/marc/Development/SVMLight/svm_learn";
			learningPrms.SVMDocumentVectorsDir = "/home/marc/Output/IFSApps/output/SVMVectors/";
			learningPrms.SVMModelsDir = "/home/marc/Output/IFSApps/output/SVMModels/";
			learningPrms.DocumentCategoryRelationsFile = "/home/marc/share/datasets/RCV1/rcv1-v2.topics.qrels";


			prms.LearningParams = learningPrms;
			prms.WorkingDir = "/home/marc/Output/IFSApps/output/";

			prms.Categories = new Categories ();
			Category cat = new Category ();
			cat.Code = "E11";
			cat.Description = "E11 Category";
			prms.Categories.Add (cat);
			//prms.Categories.Add ("E11");


			writer.Serialize (file, prms);
		}
	}
}
