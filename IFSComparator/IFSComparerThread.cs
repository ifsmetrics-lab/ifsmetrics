using System;
using System.Threading;
using IFSComparer.IO;
using IFSComparer.Params;
using IFSSharedObjects.Diagnostics;
using IFSSharedObjects.Models;
using IFSSharedObjects.IO;
using IFSComparer.Measures;

namespace IFSComparer
{
	struct IFSComparerThreadParams
	{

		public Scenario refScenario;
		public Scenario otherScenario;
		public Category category;
		public int evaluationBatch;
		public String evaluationSource;
		public String IFSsDir;

		public SimilarityMeasures measures; 

		public SimilaritiesWriter simWriter;
		public Log log;
	}

	class IFSComparatorThread
	{
		private ManualResetEvent _doneEvent;

		public IFSComparatorThread(ManualResetEvent doneEvent)
		{
			_doneEvent = doneEvent;
		}


		public void Run(Object threadContext)
		{
			IFSComparerThreadParams prms = (IFSComparerThreadParams)threadContext;
			try
			{
				Compare(prms);

			}
			catch(Exception e) {
				prms.log.WriteLine (e.Message, Log.Level.High);
			}
			finally
			{            
				_doneEvent.Set();
			}
		}

		private void Compare(IFSComparerThreadParams prms)
		{

			String strFile1 = String.Format("{0}{1}-{2}-{4}.{3:0000}.ifs", prms.IFSsDir, prms.refScenario.Code, prms.category.Code, prms.evaluationBatch, prms.evaluationSource);
			String strFile2 = String.Format("{0}{1}-{2}-{4}.{3:0000}.ifs", prms.IFSsDir, prms.otherScenario.Code, prms.category.Code, prms.evaluationBatch, prms.evaluationSource);

			Log log = prms.log;

			IFS ifs1 = new IFS(strFile1.Substring(strFile1.LastIndexOf("/") + 1));
			IFS ifs2 = new IFS(strFile2.Substring(strFile2.LastIndexOf("/") + 1));


			IFSReader reader1 = new IFSReader(strFile1);
			IFSReader reader2 = new IFSReader(strFile2);

			reader1.ReadTo(ifs1);
			reader2.ReadTo(ifs2);

			ifs1.EnsureIFSDefinition();
			ifs2.EnsureIFSDefinition();

			log.WriteLine(String.Format("{0} vs. {1}", ifs1.Name, ifs2.Name));

			foreach (IFSMeasure measure in prms.measures.SMConfigs) {
				//log.WriteLine(String.Format("{0} vs. {1}: {2} ", ifs1.Name, ifs2.Name, measure.Code), Log.Level.Normal);
				double sim = measure.GetSimilarity (ifs1, ifs2);
				//log.WriteLine(String.Format("{0} vs. {1}: {3} => {2:F3}", ifs1.Name, ifs2.Name,sim, measure.Code), Log.Level.Medium);
				prms.simWriter.Write(
					prms.category.Code, 
					prms.refScenario.Code,
					prms.refScenario.OppositesPercentage,
					prms.otherScenario.Code, 
					prms.otherScenario.OppositesPercentage, 
					prms.evaluationBatch,
					measure.Code,
					sim);
			}
		}
	}

}

