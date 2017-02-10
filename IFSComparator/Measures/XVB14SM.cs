
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{


    class XVB14SM : IFSMeasure
    {

        Double _alpha;
        Double _beta;
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

            private Double _membershipCDF;
            public Double MembershipCDF
            {
                get { return _membershipCDF; }
                set { _membershipCDF = value; }
            }

            private Double _nonMembershipCDF;
            public Double NonMembershipCDF
            {
                get { return _nonMembershipCDF; }
                set { _nonMembershipCDF = value; }
            }

            public Result()
            {
                _level = 0;
            }

            public override string ToString()
            {

                return String.Format("sim: {0:F4}; grossLevel: {1:F4}; M-CDF: {2:F4}; NM-CDF: {2:F4}",
                    _level,
                    _grossLevel,
                    _membershipCDF,
                    _nonMembershipCDF
                    );
            }
        }

        public XVB14SM(Double alpha, Double beta, Double delta, int k)
        {
            _alpha = alpha;
            _delta = delta;
            _isDirty = true;
            _result = new Result();
            _k = k;
            _beta = beta;
            _spotComparisons = new Dictionary<int, SpotComparison>();
        }



        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }


            ret = _result.Level;
            return ret;
        }

        private Dictionary<int, int> SortByDescendingMembership(Models.IFS R)
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

        private Dictionary<int, int> SortByDescendingNonMembership(Models.IFS R)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = R.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementNonMembershipComparer());
           
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

   
        private Double GetMembershipCDFByOppositeVsSupportRatio(Models.IFS P, Models.IFS Q, int k, double alpha, double beta)
        {
            Double ret = 0;

            Dictionary<int, int> sortedP = SortByDescendingMembership(P);
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
                double supportRatio0 = 0.5;
                double supportRatio1 = 0.5;
                double spotResult = 0.5;

                //double spotDiff = (P[pair.Key].Membership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);
                double spotDiff = (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) * (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) -
                                  (P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) * (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation);

                //double spotDiffSwap = (P[pair.Key].Nonmembership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation); //this is different when compared to XVB8b
                double spotDiffSwap = (P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) * (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) -
                                  (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) * (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation);


                if (Math.Abs(spotDiffSwap) < Math.Abs(spotDiff)) //More opposite
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) > 0)
                    {
                        oppositeRatio0 = (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                        oppositeRatio1 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                    }

                    spotResult = 1 - Math.Min(1, Math.Max(oppositeRatio0, oppositeRatio1));
                }
                else if (Math.Abs(spotDiffSwap) > Math.Abs(spotDiff)) //More supportive
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) > 0)
                    {
                        supportRatio0 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                        supportRatio1 = (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                    }                        

                    spotResult = Math.Min(1, Math.Max(supportRatio0, supportRatio1));
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

        private Double GetNonMembershipCDFByOppositeVsSupportRatio(Models.IFS P, Models.IFS Q, int k,  double alpha, double beta)
        {
            Double ret = 0;

            Dictionary<int, int> sortedP = SortByDescendingNonMembership(P);
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
                double supportRatio0 = 0.5;
                double supportRatio1 = 0.5;
                double spotResult = 0.5;

                //double spotDiff = (P[pair.Key].Membership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation);
                double spotDiff = (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) * (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) -
                                  (P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) * (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation);

                //double spotDiffSwap = (P[pair.Key].Nonmembership - Q[pair.Key].Membership) + alpha * (P[pair.Key].Hesitation - Q[pair.Key].Hesitation); //this is different when compared to XVB8b
                double spotDiffSwap = (P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) * (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) -
                                  (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation) * (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation);


                if (Math.Abs(spotDiffSwap) < Math.Abs(spotDiff)) //More opposite
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) > 0)
                    {
                        oppositeRatio0 = (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                        oppositeRatio1 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                    }

                    spotResult = 1 - Math.Min(1, Math.Max(oppositeRatio0, oppositeRatio1));
                }
                else if (Math.Abs(spotDiffSwap) > Math.Abs(spotDiff)) //More supportive
                {
                    if ((P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation) > 0)
                    {
                        supportRatio0 = (Q[pair.Key].Membership + alpha * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                        supportRatio1 = (Q[pair.Key].Nonmembership + beta * Q[pair.Key].Hesitation) / (P[pair.Key].Membership + alpha * P[pair.Key].Hesitation + P[pair.Key].Nonmembership + beta * P[pair.Key].Hesitation);
                    }

                    spotResult = Math.Min(1, Math.Max(supportRatio0, supportRatio1));
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
            double cdf = GetMembershipCDFByOppositeVsSupportRatio(P, Q, _k,  _alpha, _beta);
            double nonMcdf = GetNonMembershipCDFByOppositeVsSupportRatio(P, Q, _k, _alpha, _beta);

            _result = new Result();
            _result.GrossLevel = grossLevel;
            _result.MembershipCDF = cdf;
            _result.NonMembershipCDF = nonMcdf;
            _result.Level = _result.GrossLevel * (_result.MembershipCDF + _result.NonMembershipCDF) / 2;  //ver c
            
            _isDirty = false;

            return _result;
        }


        public double GetNonMembershipCDF(Models.IFS P, Models.IFS Q)
        {          
            return GetNonMembershipCDFByOppositeVsSupportRatio(P, Q, _k, _alpha, _beta); 
        }

        public double GetMembershipCDF(Models.IFS P, Models.IFS Q)
        {
            return GetMembershipCDFByOppositeVsSupportRatio(P, Q, _k, _alpha, _beta);
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
