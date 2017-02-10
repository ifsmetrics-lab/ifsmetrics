using System;

namespace IFSComparer.Params
{
	public class Source
	{

		/// <summary>
		/// Source identifier.
		/// </summary>
		public String Code;

		/// <summary>
		/// File containing documents to be used for evaluation. 
		/// e.g.: ~/datasets/RCV1/lyrl2004_tokens_test_pt0.dat 
		/// </summary>
		public String DocumentsFile;

		/// <summary>
		/// The number of documents to be loaded for testing.
		/// </summary>
		public Int32 nDocuments;
	}
}

