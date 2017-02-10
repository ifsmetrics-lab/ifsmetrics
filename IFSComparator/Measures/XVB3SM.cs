using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{
    

    class XVB3SM: IFSMeasure
    {

        Double _alpha;
        Double _delta;

        Int32 _middleMarkWeight = 2;
        Int32 _upMarkWeight = 1;
        Int32 _downMarkWeight = 1;
        Int32 _b = 3; //_middleMarkWeight + 1;
        Int32 _c = 1;


        bool _isDirty = true;
        Result _result = null;


        System.Collections.Generic.Dictionary<Int32, Double> _spotDiffs;

        public enum Marker
        {
            MiddleMarker,
            UpMarker,
            DownMarker
        }

        public class Result
        {
            private Double _level;
            public Double Level
            {
                get { return _level; }
                set { _level = value; }
            }

            private System.Collections.Generic.Dictionary<Int32, Marker> _cdp;

            public System.Collections.Generic.Dictionary<Int32, Marker> CDP
            {
                get { return _cdp; }
            }
            

            public Result()
            {
                _level = 0;
                _cdp = new Dictionary<int, Marker>();
            }

            public void AddMarker(Int32 key, Marker marker)
            {
                _cdp.Add(key, marker);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                String c = "";

                int k = this._cdp.Count;
                //foreach (Marker m in this._cdpWellFitted.Values)
                for(int i=0;i<k;i++)
                {
                    switch (this._cdp[i])
                    {
                        case Marker.MiddleMarker:
                            c = "o";
                            break;
                        case Marker.UpMarker:
                            c = "+";
                            break;
                        case Marker.DownMarker:
                            c = "-";
                            break;
                    }
                    sb.Append(c);
                }

               
                return String.Format("<{0}, '{1}', '{2}'>", _level, sb.ToString(), sb2.ToString());
            }
        }

        public XVB3SM(Double alpha, Double delta)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
        }

   

        /// <summary>
        /// Set (int) weights for each CDM
        /// </summary>
        /// <param name="middleMarkWeight">middleMarkWeight = base - 1</param>
        /// <param name="upMarkWeight">upMarkWeight &lt middleMarkWeight</param>
        /// <param name="downMarkWeight">downMarkWeighte &lt middleMarkWeight</param>
        public void SetMarkWeights(Int32 b,  Int32 c, Int32 upMarkWeight, Int32 downMarkWeight)
        {
            _middleMarkWeight = b-1;
            _b = b;
            _c = c;
            _upMarkWeight = upMarkWeight;
            _downMarkWeight = downMarkWeight;
        }
        
        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }
            double cdf = GetConnotationDifferentialFactor(_result, _b, _c, _upMarkWeight, _downMarkWeight);
            
            ret = _result.Level * cdf;
            return ret;
        }

        public Result GetSimilarityExtended(Models.IFS P, Models.IFS Q)
        {

            if (!_isDirty)
            {
                return _result;
            }

            double sum = 0;

            GetSpotDiffs(P, Q);
            int n = _spotDiffs.Count;
            foreach (Double val in _spotDiffs.Values)
            {
                sum += Math.Abs(val);
            }
            double level = 1 - sum / n;

            Result extRet = new Result();
            extRet.Level = level;

          
            int nMarkers = 0;
            foreach (int key in _spotDiffs.Keys)
            {
                Marker marker;
                if (Math.Abs(_spotDiffs[key]) <= _delta)
                    marker = Marker.MiddleMarker;
                else if (_spotDiffs[key] > _delta)
                    marker = Marker.UpMarker;
                else
                    marker = Marker.DownMarker;

                extRet.AddMarker(nMarkers, marker);
                nMarkers++;                
            }

                  
            _result = extRet;
            _isDirty = false;

            return extRet;
        }

        private void GetSpotDiffs(Models.IFS P, Models.IFS Q)
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
                }

                _spotDiffs.Add(p.ElementId, (p.Membership - q.Membership) + _alpha * (p.Hesitation - q.Hesitation));
            }

            Models.IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (Models.IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new Models.IFSElement();
                    _spotDiffs.Add(q2.ElementId, (p2.Membership - q2.Membership) + _alpha * (p2.Hesitation - q2.Hesitation));
                }

            }           
        }

        /// <summary>
        /// Get the CDF by means of the weighting schema proposed in paper 3
        /// </summary>
        /// <param name="result">Resulting Connotation-Differential Print(s)</param>
        /// <param name="b">base</param>
        /// <returns></returns>
        public double GetConnotationDifferentialFactor(Result result, int b, int c, Int32 upMarkWeight, Int32 downMarkWeight)
        {           

            int n = result.CDP.Count;
            int middleMarkWeight = b - 1;
            int nUp = 0;
            int nDown = 0;
            int nMiddle = 0;

            for (int i = 0; i<n; i++)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDP[i])
                {
                    case Marker.UpMarker:
                        nUp ++;
                        break;
                    case Marker.MiddleMarker:
                        nMiddle++;
                        break;
                    case Marker.DownMarker:
                        nDown++;
                        break;
                }
            }

            double weight = Math.Pow(b, -Math.Pow(_delta, c))
                * (n * middleMarkWeight - nUp * (middleMarkWeight - upMarkWeight) - nDown * (middleMarkWeight - downMarkWeight))
                / (n * (b - 1));

            //double weight = Math.Pow(b, -Math.Pow(_delta, c)) * nMiddle / n;

            return weight;
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
      
    }
}
