using System;
using System.Collections.Generic;
using System.Text;
using IFSBuilder.Collections;

namespace IFSBuilder.Entities
{

	class Document
	{
		private Int32 _code=0;

		//Key: String Token; Value: Int32 Frequency of the token in the document
		private System.Collections.Generic.Dictionary<String, Int32> _tokenCounts;

		//Topics (categories) tagged to the document by one or more editors
		private System.Collections.Generic.List<String> _categories;


		public Document(int code)
		{
			_code = code;
			_tokenCounts = new Dictionary<string, int>();

			_categories = new List<string>();

		}

		public Int32 Code
		{
			get { return _code; }
		}

		public System.Collections.Generic.Dictionary<String, Int32> Tokens
		{
			get { return _tokenCounts;  }
		}



		public System.Collections.Generic.List<String> Categories
		{
			get{ return _categories; }
		}



		public Int32 GetTokensCount(String token)
		{
			if (_tokenCounts.ContainsKey(token))
			{
				return _tokenCounts[token];
			}
			else
			{
				return 0;
			}
		}

		public void AddCountToken(String token)
		{
			if (_tokenCounts.ContainsKey(token))
			{
				_tokenCounts[token]++;
			}
			else
			{
				_tokenCounts.Add(token, 1);
			}
		}

		public void AddTaggedCategories(DocumentCategoryRelations relations)
		{
			if (relations.ContainsKey(this._code))
			{
				List<String> relatedTopics = relations[this._code];
				foreach (String topic in relatedTopics)
				{
					this.AddCategory(topic);
				}
			}
		}


		public void AddCategory(String categoryCode)
		{
			//Avoid duplicates!
			if (!_categories.Contains(categoryCode))
			{
				_categories.Add(categoryCode);
			}

		}

		public String ToString(bool full=false)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("Document: {0} [", _code);
			sb.Append("\n\tTokens [\n\t\t");
			foreach (string token in _tokenCounts.Keys)
			{
				sb.AppendFormat("{0}:{1};", token, _tokenCounts[token]);
			}
			sb.Append("\n\t]");


			if (full)
			{

				sb.Append("\n\tCategories [\n\t\t");
				foreach (string topic in _categories)
				{
					sb.AppendFormat("{0};", topic);
				}
				sb.Append("\n\t]\n");



			}

			sb.Append("]\n");
			return sb.ToString();
		}
	}
}
