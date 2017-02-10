using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IFSSharedObjects.Diagnostics;
using IFSSharedObjects.Models;
using IFSSharedObjects.IO;
using IFSBuilder.Collections;
using IFSBuilder.Entities;
using IFSBuilder.Params;
using IFSBuilder.Models;
using IFSBuilder.IO;

namespace IFSBuilder
{
	struct IFSBuilderThreadParams
	{
		public BuildingProcessParams buildingProcessParams;
		public Log log;

		public Documents documentsToEvaluate;
		public Dictionary<String, FIModel> knowledgeModels;
		public String sourceCode;
		public int nBatch;
		public DocumentCategoryRelations documentTags;
		public Features features;
		public IFSsStatisticsWriter statisticsWriter;
	}

	class IFSBuilderThread
	{
		private ManualResetEvent _doneEvent;

		public IFSBuilderThread(ManualResetEvent doneEvent)
		{
			_doneEvent = doneEvent;
		}


		public void Run(Object threadContext)
		{
			IFSBuilderThreadParams prms = (IFSBuilderThreadParams)threadContext;
			try
			{
				EvaluateDocsAndBuildIFS(prms);               
			}
			catch(Exception e) {
				prms.log.WriteLine (e.Message, Log.Level.High);
			}
			finally
			{            
				_doneEvent.Set();
			}
		}

		private void EvaluateDocsAndBuildIFS(
			IFSBuilderThreadParams prms
		)
		{

			Int32 nLoadedDocs = 0;
			Documents docs = prms.documentsToEvaluate;
			DocumentCategoryRelations documentTags = prms.documentTags;
			Features features = prms.features;
			Int32 nBatch = prms.nBatch;
			IFSsStatisticsWriter statisticsWriter = prms.statisticsWriter;
			Scenarios scenarios = prms.buildingProcessParams.Scenarios;
			Categories categories = prms.buildingProcessParams.Categories;
			Dictionary<String,FIModel> fiModels = prms.knowledgeModels;
			String source = prms.sourceCode;

			nLoadedDocs = docs.Count;

			docs.LoadTaggedCategories (documentTags);

			DocumentVectorsWriter vectorsWriter;
			String svmTestFile;

			prms.log.WriteLine(String.Format("Evaluating {0} documents in batch {1}... ", nLoadedDocs, nBatch));

			foreach (Scenario scenario in scenarios) {
				foreach (Category category in categories) {
					String currentScenario = scenario.Code;
					String cat = category.Code;

					svmTestFile = String.Format ("{0}{1}-{2}-{4}.{3:0000}.dvs", prms.buildingProcessParams.LearningParams.SVMDocumentVectorsDir, currentScenario, cat, nBatch,source);
					vectorsWriter = new IO.DocumentVectorsWriter (svmTestFile);
					vectorsWriter.Write (docs, features, documentTags, cat);
					vectorsWriter.Close ();

					//prms.log.WriteLine(String.Format("FI-evaluation for {0}-{1} ...", currentScenario, cat));
					//prms.log.WriteLine(String.Format("Using model {0}  ...", fiModels[currentScenario + "-" + cat].ToString()));

					IO.SVMVectorsReader reader2 = new IO.SVMVectorsReader (svmTestFile);
					List<Models.FIObject> vectors = new List<Models.FIObject> ();
					reader2.ReadNext (nLoadedDocs, vectors);
					reader2.Close ();


					IFS ifsP = new IFS (currentScenario);

					foreach (Models.FIObject o in vectors) {
						ifsP.Add (o.Code, o.GetSoftEvaluation (fiModels [currentScenario + "-" + cat]));
					}
					ifsP.EnsureIFSDefinition ();

					statisticsWriter.Write (cat, currentScenario, String.Format ("{0}", nBatch), ifsP.AvgDistanceMuNu, ifsP.AvgMu, ifsP.AvgNu, ifsP.AvgH);

					IFSWriter ifsWriter = new IFSWriter (String.Format ("{0}{1}-{2}-{4}.{3:0000}.ifs", prms.buildingProcessParams.EvaluationParams.IFSsDir, currentScenario, cat, nBatch, source), ifsP);
					ifsWriter.WriteHeader ();
					ifsWriter.Write ();
					ifsWriter.Close ();
		
						
				}

			}
		}

	}
}
