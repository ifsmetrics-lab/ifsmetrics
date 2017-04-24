using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{

	/// <summary>
	/// This measure was proposed in:
	/// 
	/// 	Loor, M., De Tré, G.: In a Quest for Suitable Similarity Measures to Compare Experience-Based Evaluations. 
	/// 	In: Computational Intelligence: International Joint Conference, IJCCI 2015 Lisbon, Portugal, November 12-14, 2015, Revised Selected Papers 
	/// 	pp. 291-314 (2017)
	/// 
	/// 	DOI: 10.1007/978-3-319-48506-5_15
	/// 
	/// </summary>
	public class XVBrSM : IFSMeasure
    {
		const String _CITEKEY = "Loor2017";
		const String _CITE = "M.~Loor and G.~De~Tr{\\'e}, In a Quest for Suitable Similarity Measures to Compare Experience-Based Evaluations. In: Computational Intelligence: International Joint Conference, IJCCI 2015 Lisbon, Portugal, November 12-14, 2015, Revised Selected Papers pp. 291-314 (2017)";


        Double _alpha;
		public Double Alpha
		{
			get { return _alpha;}
			set { _alpha = value; }
		}
        
        int _k = 1;
		public Int32 K
		{
			get{ return _k;}
			set{ _k = value; }
		}

        Result _result = null;
		public Result ExtendedSimilarity{
			get { return _result;}
		}
        
	
        public class SpotComparison
        {
            public int ElementId;
            public Double Difference;
        }
			
        public class Result
        {

            private Double _level;
            public Double Level
            {
                get { return _level; }
				set { _level = value; }
            }

     
			private Double _grossLevel;
			public Double GrossLevel
			{
				get { return _grossLevel; }
				set { _grossLevel = value;}
			}

            private Double _cdf;
            public Double CDF
            {
                get { return _cdf; }
				set { _cdf = value; }
            }

            private Double _cdfMembership;
            public Double CDFMembership
            {
                get { return _cdfMembership; }
                set { _cdfMembership = value; }
            }

            private Double _cdfNonMembership;
            public Double CDFNonMembership
            {
                get { return _cdfNonMembership; }
                set { _cdfNonMembership = value; }
            }

            public Result()
            {
                _level = 0;
				_grossLevel = 0;
            }
				
        }

		public XVBrSM()
		{
			_alpha = 0.5;
			_result = new Result();
			_k = -1;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;

		}

        public XVBrSM(Double alpha, int k)
        {
            _alpha = alpha;
            _result = new Result();
            _k = k;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }



        public override double GetSimilarity(IFS P, IFS Q)
        {
            double ret = 0;
			if (_k < 0) {
				_k = P.Count;
			}
			_result = GetSimilarityExtended(P, Q);
            ret = _result.Level;
            return ret;
        }

		private Dictionary<Int32, SpotComparison> GetSpotComparisons(IFS P, IFS Q)
		{
			Dictionary<Int32 /*object id*/, SpotComparison> spotComparisons = new Dictionary<int, SpotComparison>();
			IFSElement q;

			//First, consider the IFSElements in P & Q and P - Q
			foreach (IFSElement p in P.Values)
			{
				if (Q.ContainsKey(p.ElementId)) // P & Q
				{
					q = Q[p.ElementId];
				}
				else //P - Q
				{
					q = new IFSElement();
					q.ElementId = p.ElementId;
				}

				SpotComparison spotComparison = new SpotComparison();
				spotComparison.ElementId = p.ElementId;
				spotComparison.Difference = (p.Membership - q.Membership) + _alpha * (p.Hesitation - q.Hesitation);


				spotComparisons.Add(p.ElementId, spotComparison);

			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					p2.ElementId = q2.ElementId;

					SpotComparison spotComparison2 = new SpotComparison();
					spotComparison2.ElementId = q2.ElementId;
					spotComparison2.Difference = (p2.Membership - q2.Membership) + _alpha * (p2.Hesitation - q2.Hesitation);

					spotComparisons.Add(q2.ElementId, spotComparison2);
				}

			}
			return spotComparisons;
		}

		private double GetSpotRatio(IFSElement a, IFSElement b, double alpha)
		{
			double r = 0.5;  //default value


			double a_mu = a.Membership + alpha * a.Hesitation;
			double a_nu = a.Nonmembership + (1 - alpha) * a.Hesitation;

			double b_mu = b.Membership + alpha * b.Hesitation;
			double b_nu = b.Nonmembership + (1 - alpha) * b.Hesitation;

			//<o_mu, o_nu> is the vector representation of the complement of <a_mu, a_nu>
			double o_mu = a.Nonmembership + alpha * a.Hesitation; 
			double o_nu = a.Membership +  (1 - alpha) * a.Hesitation;

			//Area of the paralellogram formed by <a_mu, a_nu> and <o_mu, o_nu>
			double Aao = a_mu * o_nu - a_nu * o_mu;

			//Area of the paralellogram formed by <b_mu, b_nu> and <o_mu, o_nu>
			double Abo = b_mu * o_nu - b_nu * o_mu;

			if (Math.Abs (Aao) > 0) {
				r = Abo / Aao;
				if (r > 0) {
					r = Math.Min (1, r);
				} else {
					r = 0;
				}
			}
			return r;
		}


        private Dictionary<int, int> SortByDescendingMembership(IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
			IOrderedEnumerable<KeyValuePair<Int32, IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, IFSElement>, IFSElement>(se => se.Value, new IFSElementMembershipComparer());
			int n = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, n);
                    n++;

                }
            }
            return ret;
        }

        private Dictionary<int, int> SortByAscendingNonMembership(IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, IFSElement>> query = R.OrderBy<KeyValuePair<Int32, IFSElement>, IFSElement>(se => se.Value, new IFSElementNonMembershipComparer());

			int n = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, n);
                    n++;

                }
            }
            return ret;
        }

		private Double ApproximateMembershipCDF(IFS P, IFS Q, int k,  double alpha)
		{
			Double ret = 0;

			int n = P.Count;
			int j = 0;
			double sumSpotResults = 0;

			if (n == 0)
			{
				return ret;
			}

			//P is the referent
			Dictionary<int, int> sortedP = SortByDescendingMembership(P);

			foreach (KeyValuePair<int, int> pair in sortedP)
			{
				double spotResult = GetSpotRatio (P [pair.Key], Q [pair.Key], alpha);

				sumSpotResults += spotResult;
				j++;
				if (j >= k)
				{
					break;
				}

			}

			ret = sumSpotResults / k;
			return ret;
		}

		private Double ApproximateNonMembershipCDF(IFS P, IFS Q, int k,  double alpha)
		{
			Double ret = 0;

			int n = P.Count;
			int j = 0;
			double sumSpotResults = 0;

			if (n == 0)
			{
				return ret;
			}

			//P is the referent
			Dictionary<int, int> sortedP = SortByAscendingNonMembership(P);

			foreach (KeyValuePair<int, int> pair in sortedP)
			{
				double spotResult = GetSpotRatio (P [pair.Key], Q [pair.Key], alpha);

				sumSpotResults += spotResult;
				j++;
				if (j >= k)
				{
					break;
				}

			}

			ret = sumSpotResults / k;
			return ret;
		}


        public Result GetSimilarityExtended(IFS P, IFS Q)
        {
			
			Dictionary<Int32 /*object id*/, SpotComparison> spotComparisons = GetSpotComparisons(P, Q);

            //---------Compute the "gross" similarity level
            double sumAbs = 0;
			int n = spotComparisons.Count;
            foreach (SpotComparison sc in spotComparisons.Values)
            {
                sumAbs += Math.Abs(sc.Difference);
             }
            double grossLevel = 1 - sumAbs / n;

            double cdfMembership = ApproximateMembershipCDF(P, Q, _k, _alpha);
			double cdfNonMembership = ApproximateNonMembershipCDF(P, Q, _k, _alpha);
           
            _result = new Result();
            _result.CDFMembership = cdfMembership;
            _result.CDFNonMembership = cdfNonMembership;
            _result.CDF = (cdfMembership + cdfNonMembership)/2;
			_result.GrossLevel = grossLevel;
			_result.Level = grossLevel * _result.CDF;

            return _result;
        }
			
    }
}

