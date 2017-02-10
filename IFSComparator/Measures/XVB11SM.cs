using System;
using IFSSharedObjects.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{


    class XVB11SM : IFSMeasure
    {

        Double _alpha;
        Double _delta;

		public Double Alpha
		{
			get { return _alpha;}
			set { _alpha = value; }
		}

		public Int32 K
		{
			get{ return _k;}
			set{ _k = value; }
		}

        int _k = 1;

        Result _result = null;
        Dictionary<Int32 /*object id*/, SpotRatio /*spot comparison*/> _spotRatios;


        public class SpotRatio
        {
            public int ElementId;
            public Double Ratio;
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
                set { _grossLevel = value; }
            }

            private Double _cdf;
            public Double CDF
            {
                get { return _cdf; }
                set { _cdf = value; }
            }


            public Result()
            {
                _level = 0;
            }

            public override string ToString()
            {

                return String.Format("sim: {0:F4}; grossLevel: {1:F4}; CDF: {2:F4}",
                    _level,
                    _grossLevel,
                    _cdf
                    );
            }
        }

        public XVB11SM(Double alpha, Double delta, int k)
        {
            _alpha = alpha;
            _k = k;
            
        }



        public override double GetSimilarity(IFS P, IFS Q)
        {
            double ret = 0;
			_result = new Result();
			_spotComparisons = new Dictionary<int, SpotComparison>();

            _result = GetSimilarityExtended(P, Q);
            

            ret = _result.Level;
            return ret;
        }

        private Dictionary<int, int> SortByDescending(IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());
            
            int nElements = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, nElements);
                    nElements++;

                }
            }
            return ret;
        }

        private Dictionary<int, int> SortByAscending(IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, IFSElement>> query = R.OrderBy<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());
            
            int nElements = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, nElements);
                    nElements++;

                }
            }
            return ret;
        }


        

        private Double GetCDFByOppositeVsSupportRatio(IFS P, IFS Q, int k, double delta, double alpha)
        {
            Double ret = 0;

            Dictionary<int, int> sortedP = SortByDescending(P);
            int n = sortedP.Count;
            int j = 0;
            double sumSpotResults = 0;

            if (n == 0)
            {
                return ret;
            }

            foreach (KeyValuePair<int, int> pair in sortedP)
            {
                //11b -> init 0.5, 11 -> init 0
                double oppositeRatio0 = 0.5;
                double oppositeRatio1 = 0.5;
                double oppositeRatio = 0.5;
                double supportRatio0 = 0.5;
                double supportRatio1 = 0.5;
                double supportRatio = 0.5;
                double spotResult = 0.5;

                double spotDiff = (P[pair.Key].Membership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);

                double spotDiffSwap = (P[pair.Key].Nonmembership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation); //this is different when compared to XVB8b
                

                if (Math.Abs(spotDiffSwap) < Math.Abs(spotDiff)) //More opposite
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) > delta)
                        oppositeRatio0 = (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation);

                    if ((P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation) > delta)
                        oppositeRatio1 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation); //XVB8b -> easy to explain


                    oppositeRatio = Math.Max(oppositeRatio0, oppositeRatio1);
                    if (oppositeRatio > 1)
                        oppositeRatio = 1;

                    spotResult = 1 - oppositeRatio;
                }
                else if (Math.Abs(spotDiffSwap) > Math.Abs(spotDiff)) //More supportive
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) > delta)
                        supportRatio0 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation);

                    if ((P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation) > delta)
                        supportRatio1 = (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) / (P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation);

                    supportRatio = Math.Max(supportRatio0, supportRatio1);

                    if (supportRatio > 1)
                        supportRatio = 1;

                    spotResult = supportRatio;
                }

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
        private void DoSpotComparisons(IFS P, IFS Q)
        {
            int n = Math.Max(P.Count, Q.Count);
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

                Marker marker;
                if (Math.Abs(spotComparison.Difference) <= _delta)
                    marker = Marker.MiddleMarker;
                else if (spotComparison.Difference > _delta)
                    marker = Marker.UpMarker;
                else
                    marker = Marker.DownMarker;

                spotComparison.CDM = marker;


                _spotComparisons.Add(p.ElementId, spotComparison);

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

                }

            }
        }



        public Result GetSimilarityExtended(IFS P, IFS Q)
        {

            DoSpotComparisons(P, Q);

            //---------Compute the "gross" similarity level
            double sum = 0;
            int n = P.Count;
            foreach (SpotComparison sc in _spotComparisons.Values)
            {
                sum += Math.Abs(sc.Difference);
            }
            double grossLevel = 1 - sum / n;
            double cdf = GetCDFByOppositeVsSupportRatio(P, Q, _k, _delta, _alpha);

            _result = new Result();
            _result.GrossLevel = grossLevel;
            _result.CDF = cdf;
            _result.Level = _result.CDF;

            return _result;
        }


	

        public static int GetHeuristicK(IFS ifsRef)
        {
            int ret = 1;

            int n = ifsRef.Count;
            int nPos = 0;
            int nNeg = 0;
            foreach (IFSElement el in ifsRef.Values)
            {
                if (el.Membership > el.Nonmembership)
                {
                    nPos++;
                }
                else if (el.Membership < el.Nonmembership)
                {
                    nNeg++;
                }
            }
            int nEquilibrium = Math.Min(nPos, nNeg);

            if (nEquilibrium > 0.1 * n)
            {
                ret = (int)(0.1 * n);
            }
            else
            {
                ret = nEquilibrium;
            }

            if (ret < 1)
                ret = 1;

            return ret;
        }

    }
}
