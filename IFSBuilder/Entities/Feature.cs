using System;
using System.Collections.Generic;
using System.Text;


namespace IFSBuilder.Entities
{
	class Feature
	{
		private String _code="";
		private Int32 _numericCode = 0;

		public Int32 NumericCode
		{
			get { return _numericCode; }
			set { _numericCode = value; }
		}

		//The focusing level that this feature should have
		private Double _focusingLevel = 1;
		public Double FocusingLevel
		{
			get { return _focusingLevel; }
			set { _focusingLevel = value; }
		}

		//How many documents of this category contains a given feature
		private Dictionary<String /*categoryCode*/, Int32 /*count*/> _categoryCounts;

		//How many documents contains the feature
		private Int32 _nDocumentsHavingMe;

		/// <summary>
		/// Document frequency: number of documents containing the feature
		/// </summary>
		public Int32 nDocumentsHavingMe
		{
			get { return _nDocumentsHavingMe; }
			set { _nDocumentsHavingMe = value; }
		}

		private Int32 _nTrainingDocuments;
		/// <summary>
		/// |D|: Number of documents in training collection
		/// </summary>
		public Int32 nTrainingDocuments
		{
			get { return _nTrainingDocuments; }
			set { _nTrainingDocuments = value; }
		}

		public Int32 nDocumentsNotHavingMe
		{
			get { return _nTrainingDocuments - _nDocumentsHavingMe; }
		}

		public Feature(string code)
		{
			_code = code;
			_categoryCounts = new Dictionary<string, int>();
			_nDocumentsHavingMe = 0;
			_nTrainingDocuments = 0;
			_nTrainingCategories = 0;
		}

		public String Code
		{
			get { return _code; }
		}

		//How many categories contains the feature
		private Int32 _nTrainingCategories;

		/// <summary>
		/// |C|: Number of categories in category collection C
		/// </summary>
		/// <returns></returns>
		public Int32 nTrainingCategories{
			get{
				return _nTrainingCategories;
			}
			set { _nTrainingCategories = value;}
		}


		/// <summary>
		/// Get a collection of categories that contain the feature. Each element
		/// includes the count of documents having the feature in the category.
		/// </summary>
		/// <returns>collection of categories that contain the feature</returns>
		public System.Collections.Generic.Dictionary<String, Int32> CategoriesHavingMe
		{
			get{
				return _categoryCounts;
			}
		}

		/// <summary>
		/// Get the number of categories having the feature within
		/// the (main) category collection.
		/// </summary>
		/// <param name="type">Main category: Topic, Region or Industry</param>
		/// <returns></returns>
		public Int32 nCategoriesHavingMe
		{
			get{
				return _categoryCounts.Count;
			}
		}

		/// <summary>
		/// Get the number of categories NOT having the feature within
		/// the (main) category collection.
		/// </summary>
		/// <param name="type">Main category: Topic, Region or Industry</param>
		/// <returns></returns>
		public Int32 nCategoriesNoHavingMe
		{
			get{
				return _nTrainingCategories - _categoryCounts.Count;
			}
		}

		/// <summary>
		/// Get the number of documents having the feature in the given category/
		/// </summary>
		/// <param name="categoryCode">Category code</param>
		/// <returns></returns>
		public Int32 GetCategoryFrequency(String categoryCode)
		{
			Int32 ret = 0;
			if(_categoryCounts.ContainsKey(categoryCode))
				ret = _categoryCounts[categoryCode];
		
			return ret;
		}

		/// <summary>
		/// Get the inverse category frequency of the feature in the given
		/// category
		/// </summary>
		/// <param name="categoryCode">The given category</param>
		/// <returns></returns>
		public Double ICF(String categoryCode)
		{
			Double ret = 0;
			if(_categoryCounts.ContainsKey(categoryCode))
			{
				ret = (Double)Math.Log(1 + (Double)nTrainingCategories / _categoryCounts[categoryCode]);
			}

			return ret;
		}

		/// <summary>
		/// Get the inverse document frequency of the feature.
		/// </summary>
		/// <returns>The idf of the feature</returns>
		public Double IDF()
		{
			return Math.Log((Double)_nTrainingDocuments / _nDocumentsHavingMe);
		}

		/// <summary>
		///   Get the inverse category frequency combined with the inverse document
		///   frequency of the feature in the given category
		/// </summary>
		/// <param name="categoryCode">The given category</param>
		/// <returns>The icd.idf of the feature</returns>
		public Double ICFIDF(String categoryCode)
		{
			return ICF(categoryCode) * IDF();
		}


		/// <summary>
		/// Get the weight of the feature using Cornell ltc term weighting (Buckley, Salton, and Allan, 1994)
		/// </summary>
		/// <param name="freqfeature">(logarithmic) Frequency of token in document</param>
		/// <returns></returns>

		public Double GetWeight(Int32 freqFeature)
		{
			Double ret = 0;
			ret = ((1 + Math.Log(freqFeature)) * Math.Log((Double)_nTrainingDocuments / _nDocumentsHavingMe));
			return ret;
		}


		/// <summary>
		/// Get the weight of the token using class indexing (TF.IDF.ICF schema)
		/// </summary>
		/// <param name="freqToken">(Logarithmic) Frequency of token in document</param>
		/// <param name="categoryCode">The given category</param>
		/// <param name="type">Main category: Topic, Region or Industry<</param>
		/// <returns></returns>
		public Single GetWeight(Single freqToken, String categoryCode)
		{
			Single ret = 0;
			Double icf = 0;
			Double idf = 0;

			if (_categoryCounts.ContainsKey(categoryCode))
			{
				icf = Math.Log(1 + (Single)nTrainingCategories/ _categoryCounts[categoryCode]);
			}
				
			idf = Math.Log(1 + (Single)_nTrainingDocuments / _nDocumentsHavingMe);

			ret = (Single)(freqToken * icf * idf);
			return ret;
		}


		/// <summary>
		/// Get X_max score of the token with respect to the category set (see
		/// Yang and Pedersen, 1997; Rogati and Yang, 2002)
		/// </summary>
		/// <param name="categoryCode">The given category</param>
		/// <param name="nDocumentsInCategory">Obtained using document-category relations</param>
		/// <returns>The score of the token</returns>
		public Single GetScore(String categoryCode,  Int32 nDocumentsInCategory )
		{
			Double ret = 0;
			Double a = 0; //a is the number of examples with both the token and the category (aka n11)
			Double b = 0; //b is the number of examples with the token and not the category (aka n10)
			Double c = 0; //c is the number of examples with the category and not the token (aka n01)
			Double d = 0; //d is the number of examples with neither the token nor the category (aka n00)
			Double n = 0; //n is the total number of examples used in calculating the statistic (aka n = n11 + n10 + n01 + n00)

			//ret = n * (a * d - b * c) / ((a + b) * (a + c) * (b + d) * (c + d));

			n = _nTrainingDocuments;
			if (_categoryCounts.ContainsKey(categoryCode))
			{
				a = _categoryCounts[categoryCode];
			}

			b = _nDocumentsHavingMe - a; 
			c = nDocumentsInCategory - a;
			d = _nTrainingDocuments - a - b - c;
			//d = _nTrainingDocuments  - _nDocumentsHavingMe - nDocumentsInCategory + a;

			if (a + b == 0) //no document has the token!
			{
				//System.Diagnostics.Debug.Print("No document has the '{0}'!", this._code);
				return 0;
			}

			if (a + c == 0) //the token has no category!
			{
				//System.Diagnostics.Debug.Print("Token '{0}' is not in any {1}-category!", this._code, type);
				return 0;
			}

			System.Diagnostics.Debug.Assert(a >= 0 || b >= 0 || c >= 0 || d >= 0);
			System.Diagnostics.Debug.Assert((a + c) * (a + b) * (b + d) * (c + d) > 0);

			//By Mutual Information (MI)
			#if FEATURESELECTION_MI
			ret = (a / n) * Math.Log((n * a) / ((a + b) * (a + c)), 2) + (c / n) * Math.Log((n * c) / ((c + d) * (a + c)), 2) + (b / n) * Math.Log((n * b) / ((a + b) * (b + d)), 2) + (d / n) * Math.Log((d * a) / ((c + d) * (b + d)), 2);
			#else
			//By X^2 (in IR)
			ret = n * (a * d - b * c) * (a * d - b * c) / ((a + c) * (a + b) * (b + d) * (c + d));
			#endif


			//By X^2 (in Lewis)
			//ret = n * (a * d - b * c) / ((a + b) * (a + c) * (b + d) * (c + d));




			return (Single)ret;
		}


		/// <summary>
		/// Add a category and/or count an ocurrence of token in the category.
		/// </summary>
		/// <param name="categoryCode">Category code</param>
		public void AddAndOrCount(String categoryCode)
		{
			if (_categoryCounts.ContainsKey(categoryCode))
			{
				_categoryCounts[categoryCode]++;
			}
			else
			{
				_categoryCounts.Add(categoryCode, 1);
			}

		}

		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("Token: {0} (idf: {1:F6}) [", _code, this.IDF());
			sb.AppendFormat("\n\tTopics (d.1/n: {0}/{1};  d.0/n: {2}/{1};) [", _nDocumentsHavingMe, _nTrainingDocuments, _nTrainingDocuments - _nDocumentsHavingMe);
			foreach (string category in _categoryCounts.Keys)
			{
				sb.AppendFormat("\n\t\t{0}: {1}; d11/d.1: {1}/{2}; d01/d.1: {3}/{2}", category, _categoryCounts[category], _nDocumentsHavingMe, _nDocumentsHavingMe - _categoryCounts[category]);
			}
			sb.Append("\n\t]\n");
			sb.Append("]\n");
			return sb.ToString();
		}
	}
}
