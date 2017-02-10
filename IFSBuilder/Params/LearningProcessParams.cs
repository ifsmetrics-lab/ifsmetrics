using System;

namespace IFSBuilder
{
	public class LearningProcessParams
	{
		/// <summary>
		/// The number of documents to be used for training. 
		///  e.g.: 'lyrl2004_tokens_train.dat' has 23149 docs.
		/// </summary>
		public Int32 nDocuments = 0;

		/// <summary>
		/// Path of the collection to be used for training. 
		/// e.g.: ~/datasets/RCV1/lyrl2004_tokens_train.dat
		/// </summary>
		public String DocumentsFile = "";


		/// <summary>
		/// Path of the document-category relations.
		/// </summary>
		public String DocumentCategoryRelationsFile = "";


		/// <summary>
		/// Path of the SVM learning module.
		/// e.g.: ~/SVMLight/svm_learn
		/// </summary>
		public String SVMLearningExecFile = "";


		/// <summary>
		/// Directory to put the resulting SVM-models.
		/// </summary>
		public String SVMModelsDir = "";

		/// <summary>
		/// Directory to put the documents characterized as SVM document-vectors.
		/// </summary>
		public String SVMDocumentVectorsDir = "";

		public LearningProcessParams ()
		{
		}
	}
}

