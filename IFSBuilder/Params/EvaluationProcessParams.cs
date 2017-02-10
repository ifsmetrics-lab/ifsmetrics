using System;

namespace IFSBuilder.Params
{
	public class EvaluationProcessParams
	{
		
		/// <summary>
		/// The sources of documents to be used for testing. 
		/// </summary>
		public Sources Sources;


		/// <summary>
		/// Preferred number of documents in an evaluation batch.
		/// Since an IFS-element characterizes the evaluation of a document,
		/// this number represents the number of IFS-elements in a resulting IFS.
		/// </summary>
		public Int32 nDocumentsInBatch = 50;


		/// <summary>
		/// Directory to put the documents represented as SVM document-vectors.
		/// </summary>
		public String SVMDocumentVectorsDir = "";




		/// <summary>
		/// Directory to put the resulting IFSs.
		/// </summary>
		public String IFSsDir = "";




		public EvaluationProcessParams ()
		{
		}
	}
}

