
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSBuilder.Entities;
using IFSBuilder.Collections;
using System.IO;

namespace IFSBuilder.Collections
{
	class Features : Dictionary<string, Feature>
	{
		private Dictionary<int, string> _idMapping = new Dictionary<int, string>();

		//public String TrainingSet = "";
		public void Load(Documents trainingDocs, Int32 nTrainingCategories)
		{
			//N: number of documents in (training) collection
			Int32 nDocs = trainingDocs.Count();

			foreach (int idoc in trainingDocs.Keys)
			{
				foreach (string token in trainingDocs[idoc].Tokens.Keys)
				{
					if (!this.ContainsKey(token))
					{
						this.Add(token, new Feature(token));
						this[token].nTrainingDocuments = nDocs;
					}

					//df: document frequency of token
					this[token].nDocumentsHavingMe++;

					//Nc: number of categories in (training) collection
					this[token].nTrainingCategories = nTrainingCategories;

					foreach (string topic in trainingDocs[idoc].Categories)
					{
						this[token].AddAndOrCount(topic);
					}
						
				}
			}
		}

		public void AssignNumericCodes()
		{
			int pos = 0;
			IOrderedEnumerable<String> query = this.Keys.OrderBy(se => se);
			_idMapping = new Dictionary<int, string>();

			foreach (String token in query)
			{
				pos++;
				this[token].NumericCode = pos;
				_idMapping.Add(pos, token);
			}
		}

		public Dictionary<int, string> IdMapping
		{
			get { return _idMapping; }
		}

		public void WriteTo(StreamWriter writer)
		{
			writer.WriteLine("#token;nTrainingDocuments;FocusingLevel;nDocumentsHavingMe;nTrainingCategories");
			foreach (Feature feature in this.Values)
			{
				writer.WriteLine("{0};{1};{2};{3};{4}",
					feature.Code,
					feature.nTrainingDocuments,
					feature.FocusingLevel,
					feature.nDocumentsHavingMe,
					feature.nTrainingCategories

				);
			}
		}

		public void ReadFrom(StreamReader reader)
		{
			String currentLine;
			char[] seps = { ';' };
			this.Clear();
			while (!reader.EndOfStream)
			{
				currentLine = reader.ReadLine();
				if (currentLine.StartsWith("#"))
				{
					continue;
				}

				String[] parts = currentLine.Split(seps);

				string token = parts[0];
				int nDocs = Int32.Parse(parts[1]);
				double focusingLevel = Double.Parse(parts[2]);
				int nDocumentsHavingMe = Int32.Parse(parts[3]);
				int nCategories = Int32.Parse(parts[4]);

				this.Add(token, new Feature(token));
				this[token].nTrainingDocuments = nDocs;

				this[token].FocusingLevel = focusingLevel;
				//df: document frequency of token
				this[token].nDocumentsHavingMe = nDocumentsHavingMe;

				//Nc: number of categories in (training) collection
				this[token].nTrainingCategories = nCategories;

			}
			AssignNumericCodes();
		}
	}
}
