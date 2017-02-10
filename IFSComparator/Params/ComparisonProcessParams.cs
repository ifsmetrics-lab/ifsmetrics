using System;
using System.Xml;
using System.Xml.Serialization;

namespace IFSComparer.Params
{
	[XmlRoot("ComparisonProcessParams")]
	public class ComparisonProcessParams
	{
		
		/// <summary>
		/// The number of documents in an evaluation batch 
		/// i.e., BuildingProcessParams.EvaluationProcessParams.nDocumentsInBatch
		/// </summary>
		public Int32 nDocumentsInEvaluationBatch;


		/// <summary>
		/// Scenarios to be used during the comparison process.
		/// </summary>
		public Scenarios Scenarios = null;

		/// <summary>
		/// Categories to be used during the comparison process.
		/// </summary>
		public Categories Categories = null;


		/// <summary>
		/// Sources to be used during the comparison process.
		/// </summary>
		public Sources Sources = null;


		/// <summary>
		/// Path of the file containing the similarity results.
		/// </summary>
		public String SimilaritiesFile;


		/// <summary>
		/// Path of the file containing the references to the
		/// similarity measures used during the comparison process.
		/// </summary>
		public String ReferencesFile;

		/// <summary>
		/// The referent scenario.
		/// </summary>
		public Scenario ReferentScenario;

		/// <summary>
		/// Directory to place the temporary files.
		/// </summary>
		public String WorkingDir = "";


		/// <summary>
		/// Directory where the IFSs are located.
		/// </summary>
		public String IFSsDir = "";

		/// <summary>
		/// Similarity measures to be used during the comparison process.
		/// </summary>
		public IFSComparer.Params.SimilarityMeasures Measures;



	}
}

