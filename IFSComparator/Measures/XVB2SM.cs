using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{
    

    class XVB2SM: IFSMeasure
    {

        Double _alpha;
        Double _delta;

        //Double _middleMarkWeight = 1;
        //Double _upMarkWeight = .6;
        //Double _downMarkWeight = .4;

        int _k = 1;

        bool _isDirty = true;
        Result _result = null;


        int _w2_b;
        int _w2_c;
        int _w2_middleMarkWeight;
        int _w2_upMarkWeight;
        int _w2_downMarkWeight;
        int _w2_IFSMagnitude;


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
                for(int i=0;i<k;i++)
                {
                    switch (this._cdpWellFitted[i])
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

                k = this._cdpWellUnfitted.Count;
                for (int i = 0; i < k; i++)
                {
                    switch (this._cdpWellUnfitted[i])
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
                    sb2.Append(c);
                }

                return String.Format("<{0}, '{1}', '{2}'>", _level, sb.ToString(), sb2.ToString());
            }
        }

        public XVB2SM(Double alpha, Double delta)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
        }

        public XVB2SM(Double alpha, Double delta, int k)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
            _k = k;
        }

        public void SetWeight2Parameters(int w2_b,
            int w2_c,
            int w2_upMarkWeight,
            int w2_downMarkWeight,
            int w2_IFSMagnitude=50)
        {
            _w2_b = w2_b;
            _w2_c = w2_c;
            _w2_middleMarkWeight = w2_b - 1;
            _w2_IFSMagnitude = w2_IFSMagnitude;

            if (w2_upMarkWeight > _w2_middleMarkWeight)
                throw new ArgumentException("w2_upMarkWeight>b-1");

            if (w2_downMarkWeight > _w2_middleMarkWeight)
                throw new ArgumentException("w2_downMarkWeight>b-1");

            _w2_upMarkWeight = w2_upMarkWeight;
            _w2_downMarkWeight = w2_downMarkWeight;
        }


        
        
        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }
            double cdf = GetConnotationDifferentialFactor(_result, _w2_b, _w2_c, _w2_upMarkWeight, _w2_downMarkWeight);
            
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

                extRet.AddWellUnfittedMarker(nMarkers, marker);
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

                extRet.AddWellFittedMarker(nMarkers, marker);
                nMarkers++;
                if (nMarkers >= _k)
                {
                    break;
                }
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
        /// Get the CDF by means of the weighting schema proposed in paper 2
        /// </summary>
        /// <param name="result"></param>
        /// <param name="b">base (b>1)</param>
        /// <param name="c">delta-controller (c>=1)</param>
        /// <returns></returns>
        public double GetConnotationDifferentialFactor(Result result, int b, int c, int upMarkWeight, int downMarkWeight)
        {

            double sumWellFitted = 0;
            double sumDenominator = 0;
            int middleMarkWeight = b - 1; //according to the definition of the weight
            

            int k = result.CDPWellFitted.Count;


            for (int i = k; i >= 1; i--)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellFitted[k-i])
                {
                    case Marker.UpMarker:
                        sumWellFitted += (upMarkWeight * bi_1);
                        break;
                    case Marker.MiddleMarker:
                        sumWellFitted += (middleMarkWeight * bi_1);
                        break;
                    case Marker.DownMarker:
                        sumWellFitted += (downMarkWeight * bi_1);
                        break;
                }
                sumDenominator += ((b - 1) * bi_1);
            }

            double weightWellFitted = Math.Pow(b,-Math.Pow(_delta, c))*sumWellFitted / sumDenominator;

            double sumWellUnfitted = 0;
            sumDenominator = 0;
            k = result.CDPWellUnfitted.Count;
            for (int i = k; i >= 1; i--)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellUnfitted[k-i])
                {
                    case Marker.UpMarker:
                        sumWellUnfitted += (upMarkWeight * bi_1);
                        break;
                    case Marker.MiddleMarker:
                        sumWellUnfitted += (middleMarkWeight * bi_1);
                        break;
                    case Marker.DownMarker:
                        sumWellUnfitted += (downMarkWeight * bi_1);
                        break;
                }
                sumDenominator += ((b - 1) * bi_1);
            }

            
            double weightWellUnfitted = Math.Pow(b, -Math.Pow(_delta, c)) * sumWellUnfitted / sumDenominator;


           // System.Diagnostics.Debug.Print("delta: {0:F3};  weightWellFitted: {1:F3};  weightWellUnfitted: {2:F3};", _delta, weightWellFitted, weightWellUnfitted);
            return Math.Min(weightWellFitted, weightWellUnfitted);
            //return  (weightWellFitted + weightWellUnfitted)/2;
        }

#if TEST
        private double GetConnotationDifferentialFactorByWeight3(Result result, int b, int c, int upMarkWeight, int downMarkWeight)
        {

            double sumWellFitted = 0;
            double sumDenominator = 0;
            int middleMarkWeight = b - 1; //according to the definition of the weight


            int k = result.CDPWellFitted.Count;


            for (int i = k; i >= 1; i--)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellFitted[i - 1])
                {
                    case Marker.UpMarker:
                        sumWellFitted += (upMarkWeight );
                        break;
                    case Marker.MiddleMarker:
                        sumWellFitted += (middleMarkWeight );
                        break;
                    case Marker.DownMarker:
                        sumWellFitted += (downMarkWeight );
                        break;
                }
                sumDenominator += ((b - 1));
            }

            double weightWellFitted = Math.Pow(b, -Math.Pow(_delta, c)) * sumWellFitted / sumDenominator;

            double sumWellUnfitted = 0;
            sumDenominator = 0;
            k = result.CDPWellUnfitted.Count;
            for (int i = k; i >= 1; i--)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellUnfitted[i - 1])
                {
                    case Marker.UpMarker:
                        sumWellUnfitted += (upMarkWeight );
                        break;
                    case Marker.MiddleMarker:
                        sumWellUnfitted += (middleMarkWeight );
                        break;
                    case Marker.DownMarker:
                        sumWellUnfitted += (downMarkWeight );
                        break;
                }
                sumDenominator += ((b - 1));
            }


            double weightWellUnfitted = Math.Pow(b, -Math.Pow(_delta, c)) * sumWellUnfitted / sumDenominator;


            // System.Diagnostics.Debug.Print("delta: {0:F3};  weightWellFitted: {1:F3};  weightWellUnfitted: {2:F3};", _delta, weightWellFitted, weightWellUnfitted);
            //return Math.Min(weightWellFitted, weightWellUnfitted);
            return (weightWellFitted + weightWellUnfitted) / 2;
        }


        private double GetConnotationDifferentialFactorByWeight4(Result result, int b, int c, int upMarkWeight, int downMarkWeight, int nElementsInIFS)
        {

            double sumWellFitted = 0;
            double sumDenominator = 0;
            int middleMarkWeight = b - 1; //according to the definition of the weight
            int groupSize = nElementsInIFS / 10;


            int k = result.CDPWellFitted.Count;


            for (int i = k, j=0; i >= 1; i--,j++)
            {
                int mult = j / groupSize;

                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellFitted[i - 1])
                {
                    case Marker.UpMarker:
                        sumWellFitted += (upMarkWeight * bi_1);
                        break;
                    case Marker.MiddleMarker:
                        sumWellFitted += (middleMarkWeight * bi_1);
                        break;
                    case Marker.DownMarker:
                        sumWellFitted += (downMarkWeight * bi_1);
                        break;
                }
                sumDenominator += ((b - 1) * bi_1);
            }

            double weightWellFitted = Math.Pow(b, -Math.Pow(_delta, c)) * sumWellFitted / sumDenominator;

            double sumWellUnfitted = 0;
            sumDenominator = 0;
            k = result.CDPWellUnfitted.Count;
            for (int i = k; i >= 1; i--)
            {
                double bi_1 = Math.Pow(b, i - 1);
                switch (result.CDPWellUnfitted[i - 1])
                {
                    case Marker.UpMarker:
                        sumWellUnfitted += (upMarkWeight * bi_1);
                        break;
                    case Marker.MiddleMarker:
                        sumWellUnfitted += (middleMarkWeight * bi_1);
                        break;
                    case Marker.DownMarker:
                        sumWellUnfitted += (downMarkWeight * bi_1);
                        break;
                }
                sumDenominator += ((b - 1) * bi_1);
            }


            double weightWellUnfitted = Math.Pow(b, -Math.Pow(_delta, c)) * sumWellUnfitted / sumDenominator;


            // System.Diagnostics.Debug.Print("delta: {0:F3};  weightWellFitted: {1:F3};  weightWellUnfitted: {2:F3};", _delta, weightWellFitted, weightWellUnfitted);
            return Math.Min(weightWellFitted, weightWellUnfitted);
            //return  (weightWellFitted + weightWellUnfitted)/2;
        }

#endif        
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
