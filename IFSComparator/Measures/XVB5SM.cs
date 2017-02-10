using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{


    class XVB5SM : IFSMeasure
    {

        Double _alpha;
        Double _delta;

        int _k = 1;

        Double _wellFittedInfluence = .5;
        Double _wellUnfittedInfluence = 0.5;

        Int32 _b = 3; 
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

            private Double _cdf;
            public Double CDF
            {
                get { return _cdf; }
                set { _cdf = value; }
            }

            private Double _uniformnessWellFitted;
            private Double _uniformnessWellUnfitted;
            public Double UniformnessWellFitted
            {
                get { return _uniformnessWellFitted; }
                set { _uniformnessWellFitted = value; }
            }

            public Double UniformnessWellUnfitted
            {
                get { return _uniformnessWellUnfitted; }
                set { _uniformnessWellUnfitted = value; }
            }

            private Double _thresholdInfluence;
            public Double ThresholdInfluence
            {
                get { return _thresholdInfluence; }
                set { _thresholdInfluence = value; }
            }

            
            private System.Collections.Generic.Dictionary<Int32, Marker> _cdpWellFitted;
            private System.Collections.Generic.Dictionary<Int32, Marker> _cdpWellUnfitted;

            public System.Collections.Generic.Dictionary<Int32, Marker> CDPWellFitted
            {
                get { return _cdpWellFitted; }
            }

            public System.Collections.Generic.Dictionary<Int32, Marker> CDPWellUnfitted
            {
                get { return _cdpWellUnfitted; }
            }


            public Result()
            {
                _level = 0;
                _cdpWellFitted = new Dictionary<int, Marker>();
                _cdpWellUnfitted = new Dictionary<int, Marker>();
            }

            public void AddWellFittedMarker(Int32 key, Marker marker)
            {
                _cdpWellFitted.Add(key, marker);
            }
            public void AddWellUnfittedMarker(Int32 key, Marker marker)
            {
                _cdpWellUnfitted.Add(key, marker);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                String c = "";

                int k = this._cdpWellFitted.Count;
                //foreach (Marker m in this._cdpWellFitted.Values)
                for (int i = 0; i < k; i++)
                {
                    switch (this._cdpWellFitted[i])
                    {
                        case Marker.MiddleMarker:
                            c = ".";
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

                k = this._cdpWellUnfitted.Count;
                for (int i = 0; i < k; i++)
                {
                    switch (this._cdpWellUnfitted[i])
                    {
                        case Marker.MiddleMarker:
                            c = ".";
                            break;
                        case Marker.UpMarker:
                            c = "+";
                            break;
                        case Marker.DownMarker:
                            c = "-";
                            break;
                    }
                    sb2.Append(c);
                }

                return String.Format("sim: <{0:F4}, '{1}', '{2}'>; uniformness: [{3:F4}, {4:F4}]; thresholdInfluence: {5:F4}; CDF: {6:F4} ", 
                    _level, sb.ToString(), 
                    sb2.ToString(),
                    _uniformnessWellFitted,
                    _uniformnessWellUnfitted,
                    _thresholdInfluence,
                    _cdf);
            }
        }

        public XVB5SM(Double alpha, Double delta, int k)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
            _k = k;
        }

        
        public void SetWeights(Int32 b, Int32 c, Double wellFittedInfluence)
        {            
            _b = b;
            _c = c;
            _wellFittedInfluence = wellFittedInfluence;
            _wellUnfittedInfluence = 1 - _wellFittedInfluence;

            if (_wellFittedInfluence < 0 || _wellFittedInfluence > 1)
            {
                throw new ArgumentException("wellFittedInfluence does not belong to the unit interval [0, 1]");
            }
        }

        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }


            ret = _result.Level * _result.CDF;
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

            _result = new Result();
            _result.Level = level;


            IOrderedEnumerable<KeyValuePair<Int32, Models.IFSElement>> query = P.OrderBy<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());

            int nMarkers = 0;
            foreach (KeyValuePair<Int32, Models.IFSElement> pair in query)
            {
                Marker marker;
                if (Math.Abs(_spotDiffs[pair.Key]) <= _delta)
                    marker = Marker.MiddleMarker;
                else if (_spotDiffs[pair.Key] > _delta)
                    marker = Marker.UpMarker;
                else
                    marker = Marker.DownMarker;

                
                _result.AddWellUnfittedMarker(nMarkers, marker);
                nMarkers++;
                if (nMarkers >= _k)
                {
                    break;
                }              
                
            }


            query = P.OrderByDescending<KeyValuePair<Int32, Models.IFSElement>, Models.IFSElement>(se => se.Value, new Models.IFSElementComparer());
            nMarkers = 0;
            foreach (KeyValuePair<Int32, Models.IFSElement> pair in query)
            {
                Marker marker;
                if (Math.Abs(_spotDiffs[pair.Key]) <= _delta)
                    marker = Marker.MiddleMarker;
                else if (_spotDiffs[pair.Key] > _delta)
                    marker = Marker.UpMarker;
                else
                    marker = Marker.DownMarker;

                
                _result.AddWellFittedMarker(nMarkers, marker);
                nMarkers++;
                if (nMarkers >= _k)
                {
                    break;
                }
               
            }

            GetResultIndices(_b, _c);

            _isDirty = false;

            return _result;
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
        /// Get the result indices
        /// </summary>
        /// <param name="result">Partial Result with  Connotation-Differential Print(s)</param>
        /// <returns></returns>
        private void GetResultIndices_b(int b, int c)
        {

            int n = _result.CDPWellFitted.Count;
            int nUp = 0;
            int nDown = 0;
            int nMiddle = 0;

            for (int i = 0; i < n; i++)
            {
                switch (_result.CDPWellFitted[i])
                {
                    case Marker.UpMarker:
                        nUp++;
                        break;
                    case Marker.MiddleMarker:
                        nMiddle++;
                        break;
                    case Marker.DownMarker:
                        nDown++;
                        break;
                }
            }

            int nMaxInvariants = Math.Max(nUp, Math.Max(nDown, nMiddle));
            /*
             * nMaxInvariants approaching to n/3 => high variability (weight ~> 0) 
             * nMaxInvariants approaching to 2n/3 => medium variability (weight ~> 0.5) 
             * nMaxInvariants approaching to n => low variability  (weight > 1)*/

            _result.UniformnessWellFitted = (3.0 * nMaxInvariants) / (2.0 * n) - .5;      
     
            ///------ WellUnfitted
            n = _result.CDPWellUnfitted.Count;
            nUp = 0;
            nDown = 0;
            nMiddle = 0;

            for (int i = 0; i < n; i++)
            {
                switch (_result.CDPWellUnfitted[i])
                {
                    case Marker.UpMarker:
                        nUp++;
                        break;
                    case Marker.MiddleMarker:
                        nMiddle++;
                        break;
                    case Marker.DownMarker:
                        nDown++;
                        break;
                }
            }

            nMaxInvariants = Math.Max(nUp, Math.Max(nDown, nMiddle));
            /*
             * nMaxInvariants approaching to n/3 => high variability (weight ~> 0) 
             * nMaxInvariants approaching to 2n/3 => medium variability (weight ~> 0.5) 
             * nMaxInvariants approaching to n => low variability  (weight > 1)*/

            _result.UniformnessWellUnfitted = (3.0 * nMaxInvariants) / (2.0 * n) - .5;   


         

            double thresholdInfluence = Math.Pow(b, -Math.Pow(_delta, c));

            double cdf = thresholdInfluence * (_result.UniformnessWellFitted * _wellFittedInfluence + _result.UniformnessWellUnfitted*_wellUnfittedInfluence  ) ;
            _result.ThresholdInfluence = thresholdInfluence;
            _result.CDF = cdf;

        }
        private void GetResultIndices(int b, int c) //c
        {

            int n = _result.CDPWellFitted.Count;
            int nVariations = 0;


            for (int i = 0; i + 1 < n; i++)
            {
                nVariations = nVariations + Math.Abs((int)_result.CDPWellFitted[i + 1] - (int)_result.CDPWellFitted[i]);
            }

            if (n > 1)
            {
                _result.UniformnessWellFitted = 1 - nVariations / (2.0 * (n - 1));
            }
            else
            {
                _result.UniformnessWellFitted = 1;
            }

            ///------ WellUnfitted
            n = _result.CDPWellUnfitted.Count;
            nVariations = 0;

            for (int i = 0; i + 1 < n; i++)
            {
                nVariations = nVariations + Math.Abs((int)_result.CDPWellUnfitted[i + 1] - (int)_result.CDPWellUnfitted[i]);
            }

            if (n > 1)
            {
                _result.UniformnessWellUnfitted = 1 - nVariations / (2.0 * (n - 1));
            }
            else
            {
                _result.UniformnessWellUnfitted = 1;
            }

            //System.Diagnostics.Debug.Assert(_result.UniformnessWellUnfitted >= 0 && _result.UniformnessWellUnfitted <= 1);
            //System.Diagnostics.Debug.Assert(_result.UniformnessWellFitted >= 0 && _result.UniformnessWellFitted <= 1);

            double thresholdInfluence = Math.Pow(b, -Math.Pow(_delta, c));

            double cdf = thresholdInfluence * (_result.UniformnessWellFitted * _wellFittedInfluence + _result.UniformnessWellUnfitted * _wellUnfittedInfluence);
            _result.ThresholdInfluence = thresholdInfluence;
            _result.CDF = cdf;

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

            if (2 * nEquilibrium > 0.1 * n)
            {
                ret = (int)(0.1 * n);
            }
            else
            {
                ret = 2 * nEquilibrium;
            }

            if (ret < 2)
                ret = 2;

            return ret;
        }
    }
}
