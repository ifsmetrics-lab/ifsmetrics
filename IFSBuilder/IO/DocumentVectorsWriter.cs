using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Collections;
using IFSBuilder.Entities;

namespace IFSBuilder.IO
{


	class DocumentVectorsWriter
	{
		private StreamWriter _writer = null;
		private Object _lock = new Object();

		public DocumentVectorsWriter(String path)
		{
			_writer = new StreamWriter(path);
		}
		public void Close()
		{
			if (_writer != null)
			{
				_writer.Close();
			}
		}


		private void Write(Document doc, Features tokens)
		{
			Double docMagnitude = 0;
			Double docWeight;

			Dictionary<String, Int32> docTokens = new Dictionary<string, int>();

			foreach (KeyValuePair<String, Int32> pair in doc.Tokens)
			{
				if (tokens.ContainsKey(pair.Key))
				{
					docWeight = tokens[pair.Key].GetWeight(pair.Value);
					docMagnitude+= docWeight * docWeight;

					docTokens.Add(pair.Key, tokens[pair.Key].NumericCode);
				}
			}
			//No document-token in dictionary!
			docMagnitude = Math.Sqrt(docMagnitude);

			if(docMagnitude<=0) return;

			lock (_lock)
			{
				_writer.Write("{0}", doc.Code);

				IOrderedEnumerable<KeyValuePair<String, Int32>> query = docTokens.OrderBy<KeyValuePair<String, Int32>, Int32>(se => se.Value);

				foreach (KeyValuePair<String, Int32> pair in query)
				{
					if(tokens.ContainsKey(pair.Key))
					{
						docWeight = tokens[pair.Key].GetWeight(doc.Tokens[pair.Key]) / docMagnitude;
						_writer.Write(" {0}:{1:F14}", pair.Value, docWeight);
					}
				}
				_writer.WriteLine(" #");
			}
		}

		private void Write(Document doc, Features tokens, bool inCategory)
		{
			Double docMagnitude = 0;
			Double docWeight;
			Dictionary<String, Int32> docTokens = new Dictionary<string, int>();

			foreach (KeyValuePair<String, Int32> pair in doc.Tokens)
			{
				if (tokens.ContainsKey(pair.Key))
				{
					docWeight = tokens[pair.Key].GetWeight(pair.Value);
					docMagnitude += docWeight * docWeight;

					docTokens.Add(pair.Key, tokens[pair.Key].NumericCode);
				}
			}
			//No document-token in dictionary!
			docMagnitude = Math.Sqrt(docMagnitude);

			lock (_lock)
			{
				_writer.Write("{0}", inCategory?"+1":"-1");

				IOrderedEnumerable<KeyValuePair<String, Int32>> query = docTokens.OrderBy<KeyValuePair<String, Int32>, Int32>(se => se.Value);

				foreach (KeyValuePair<String, Int32> pair in query)
				{
					if (tokens.ContainsKey(pair.Key))
					{
						docWeight = tokens[pair.Key].GetWeight(doc.Tokens[pair.Key]) / docMagnitude;
						_writer.Write(" {0}:{1:F14}", pair.Value, docWeight);
					}
				}
				_writer.WriteLine(" # {0}", doc.Code);
			}
		}

		private void WriteNoNormalized(Document doc, Features tokens, bool inCategory)
		{

			Dictionary<String, Int32> docTokens = new Dictionary<string, int>();

			lock (_lock)
			{
				_writer.Write("{0}", inCategory ? "+1" : "-1");

				IOrderedEnumerable<KeyValuePair<String, Int32>> query = docTokens.OrderBy<KeyValuePair<String, Int32>, Int32>(se => se.Value);

				foreach (KeyValuePair<String, Int32> pair in query)
				{
					if (tokens.ContainsKey(pair.Key))
					{
						_writer.Write(" {0}:{1:F14}", pair.Value, tokens[pair.Key].GetWeight(doc.Tokens[pair.Key]));
					}
				}
				_writer.WriteLine(" # {0}", doc.Code);
			}
		}

		public void Write(Documents docs, Features tokens)
		{            
			foreach (Document doc in docs.Values)
			{
				this.Write(doc, tokens);
			}
		}

		public void Write(Documents docs, Features tokens, DocumentCategoryRelations docCatRelations, String cat)
		{
			_writer.WriteLine("# Documents in category '{0}'", cat);
			foreach (Document doc in docs.Values)
			{
				bool inCategory = docCatRelations[doc.Code].Contains(cat);
				this.Write(doc, tokens, inCategory);
			}
		}

		public void Write(Documents docs, Features tokens, DocumentCategoryRelations docCatRelations, String cat, double oppositePct)
		{

			int nDocs = docs.Count;
			int nOppositeDocs = (int)Math.Round(oppositePct * nDocs);

			//Random assigment for opposite docs
			Dictionary<Int32, int> rndPos = new Dictionary<int, int>();

			Random rnd = new Random(nOppositeDocs);
			foreach (Document doc in docs.Values)
			{
				rndPos.Add(doc.Code, rnd.Next());
			}

			IOrderedEnumerable<KeyValuePair<int,int>> query = rndPos.OrderByDescending(kvp => kvp.Value);

			_writer.WriteLine("# Documents in category '{0}' ({1} docs ({2:F2}%)  with random opposite labels) ", cat, nOppositeDocs, oppositePct*100);

			int i = 0;
			foreach (KeyValuePair<int,int> se in query)
			{
				bool inCategory = docCatRelations[se.Key].Contains(cat);
				if (i < nOppositeDocs)
				{
					inCategory = !inCategory; 
				}
				i++;
				this.Write(docs[se.Key], tokens, inCategory);
			}

		}


	}
}
