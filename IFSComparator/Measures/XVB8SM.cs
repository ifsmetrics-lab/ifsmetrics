using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{


    class XVB8SM : IFSMeasure
    {

        Double _alpha;
        Double _delta;

        int _k = 1;

        bool _isDirty = true;
        Result _result = null;
        System.Collections.Generic.Dictionary<Int32 /*object id*/, SpotComparison /*spot comparison*/> _spotComparisons;


        public enum Marker
        {

            DownMarker = -1,
            MiddleMarker = 0,
            UpMarker = +1
        }


        public class SpotComparison
        {
            public int ElementId;
            public Double Difference;
            public Marker CDM;
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

        public XVB8SM(Double alpha, Double delta, int k)
        {
            _alpha = alpha;
            _delta = delta;
            _isDirty = true;
            _result = new Result();
            _k = k;
            _spotComparisons = new Dictionary<int, SpotComparison>();
        }



        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }


            ret = _result.Level ;
            return ret;
        }

        private Dictionary<int, int> SortByDescending(Models.IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());
            //IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer(Models.IFSElementComparer.FocusedCriterion.Membership));            

            int nElements = 0;
            foreach (KeyValuePair<Int32, Models.IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, nElements);
                    nElements++;

                }
            }
            return ret;
        }

        private Dictionary<int, int> SortByAscending(Models.IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = R.OrderBy<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());
            //IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer(Models.IFSElementComparer.FocusedCriterion.NonMembership));

            int nElements = 0;
            foreach (KeyValuePair<Int32, Models.IFSElement> pair in query)
            {
                if (pair.Value.ElementId > 0)
                {
                    ret.Add(pair.Value.ElementId, nElements);
                    nElements++;

                }
            }
            return ret;
        }


        private Double GetOppositeVsSupportRatiov0(Models.IFS P, Models.IFS Q, int k, double delta, double alpha)
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
                double oppositeRatio0 = 0;
                double oppositeRatio1 = 0;
                double oppositeRatio = 0;
                double supportRatio0 = 0;
                double supportRatio1 = 0;
                double supportRatio = 0;
                double spotResult = 0;

                double spotDiffSwap = (P[pair.Key].Membership - Q[pair.Key].Nonmembership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);
                double spotDiff = (P[pair.Key].Membership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);

                if (Math.Abs(spotDiffSwap) < Math.Abs(spotDiff)) //More opposite
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) > delta)
                        oppositeRatio0 = (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation);

                    if ((Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) > delta)
                        oppositeRatio1 = (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) / (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation);

                    oppositeRatio = Math.Max(oppositeRatio0, oppositeRatio1);
                    if (oppositeRatio > 1)
                        oppositeRatio = 1;

                    spotResult = oppositeRatio;
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

                    spotResult = 1 - supportRatio;
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

        private new Double GetOppositeVsSupportRatio(Models.IFS P, Models.IFS Q, int k, double delta, double alpha)
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
                double oppositeRatio0 = 0;
                double oppositeRatio1 = 0;
                double oppositeRatio = 0;
                double supportRatio0 = 0;
                double supportRatio1 = 0;
                double supportRatio = 0;
                double spotResult = 0;

                double spotDiffSwap = (P[pair.Key].Membership - Q[pair.Key].Nonmembership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation); //best
                //double spotDiffSwap = (P[pair.Key].Membership - Q[pair.Key].Nonmembership) + (alpha * P[pair.Key].Hesitation - (1-alpha) * Q[pair.Key].Hesitation); //XVB8a => worst
                double spotDiff = (P[pair.Key].Membership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);

                if (Math.Abs(spotDiffSwap) < Math.Abs(spotDiff)) //More opposite
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) > delta)
                        oppositeRatio0 = (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation);

                    //if ((Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation) > delta)
                    //    oppositeRatio1 = (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) / (Q[pair.Key].Nonmembership + (1 - alpha) * Q[pair.Key].Hesitation);

                    if ((P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation) > delta)
                        oppositeRatio1 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Nonmembership + (1 - alpha) * P[pair.Key].Hesitation); //XVB8b -> easy to explain


                    oppositeRatio = Math.Max(oppositeRatio0, oppositeRatio1);
                    if (oppositeRatio > 1)
                        oppositeRatio = 1;

                    spotResult = oppositeRatio;
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

                    spotResult = 1 - supportRatio;
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
        private void DoSpotComparisons(Models.IFS P, Models.IFS Q)
        {
            int n = Math.Max(P.Count, Q.Count);
            Models.IFSElement q;

            //First, consider the IFSElements in P & Q and P - Q
            foreach (Models.IFSElement p in P.Values)
            {
                if (Q.ContainsKey(p.ElementId)) // P & Q
                {
                    q = Q[p.ElementId];
                }
                else //P - Q
                {
                    q = new Models.IFSElement();
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

            Models.IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (Models.IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new Models.IFSElement();
                    p2.ElementId = q2.ElementId;

                    SpotComparison spotComparison2 = new SpotComparison();
                    spotComparison2.ElementId = q2.ElementId;
                    spotComparison2.Difference = (p2.Membership - q2.Membership) + _alpha * (p2.Hesitation - q2.Hesitation);

                }

            }
        }



        public Result GetSimilarityExtended(Models.IFS P, Models.IFS Q)
        {

            if (!_isDirty)
            {
                return _result;
            }
            DoSpotComparisons(P, Q);

            //---------Compute the "gross" similarity level
            double sum = 0;
            int n = P.Count;
            foreach (SpotComparison sc in _spotComparisons.Values)
            {
                sum += Math.Abs(sc.Difference);
            }
            double grossLevel = 1 - sum / n;
            double oppositeLevel = GetOppositeVsSupportRatio(P, Q, _k, _delta, _alpha);

            _result = new Result();
            _result.GrossLevel = grossLevel;
            _result.CDF = 1 - oppositeLevel;
            _result.Level = _result.GrossLevel * _result.CDF;
                        
            _isDirty = false;

            return _result;
        }

      

        public static double GetHeuristicDelta(Models.IFS ifsRef, double pct)
        {
            double delta = 0;

            Double sumDistanceMuNu = 0;
            int n = ifsRef.Count;
            foreach (Models.IFSElement el in ifsRef.Values)
            {
                sumDistanceMuNu += (el.Membership + el.Nonmembership);
            }

            delta = pct * sumDistanceMuNu / n;

            return delta;
        }

        public static int GetHeuristicK(Models.IFS ifsRef)
        {
            int ret = 1;

            int n = ifsRef.Count;
            int nPos = 0;
            int nNeg = 0;
            foreach (Models.IFSElement el in ifsRef.Values)
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
