using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{
    

    class XVB1SM: IFSMeasure
    {

        Double _alpha;
        Double _delta;

        Double _middleMarkWeight = 1;
        Double _upMarkWeight = .6;
        Double _downMarkWeight = .4;

        int _k = 1;

        bool _isDirty = true;
        Result _result = null;

#if TEST
        int _w2_b;
        int _w2_c;
        int _w2_middleMarkWeight;
        int _w2_upMarkWeight;
        int _w2_downMarkWeight;
        int _w2_IFSMagnitude;
#endif

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

        public XVB1SM(Double alpha, Double delta)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
        }

        public XVB1SM(Double alpha, Double delta, int k)
        {
            _alpha = alpha;
            _delta = delta;
            _spotDiffs = new Dictionary<int, double>();
            _isDirty = true;
            _result = new Result();
            _k = k;
        }

#if TEST
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
#endif

        public void SetMarkWeights(Double middleMarkWeight, Double upMarkWeight, Double downMarkWeight)
        {
            _middleMarkWeight = middleMarkWeight;
            _upMarkWeight = upMarkWeight;
            _downMarkWeight = downMarkWeight;
        }

        public override double GetSimilarity(Models.IFS P, Models.IFS Q)
        {
            double ret =0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }
            double cdf = GetConnotationDifferentialFactor(_result);
            ret = _result.Level * cdf;
            return ret;
        }

#if TEST
        public  double GetSimilarity2(Models.IFS P, Models.IFS Q)
        {
            double ret = 0;

            if (_isDirty)
            {
                _result = GetSimilarityExtended(P, Q);
            }
            double cdf = GetConnotationDifferentialFactorByWeight2(_result, _w2_b, _w2_c, _w2_upMarkWeight, _w2_downMarkWeight);
            //double cdf = GetConnotationDifferentialFactorByWeight4(_result, _w2_b, _w2_c, _w2_upMarkWeight, _w2_downMarkWeight, _w2_IFSMagnitude);
            
            ret = _result.Level * cdf;
            return ret;
        }
#endif
        public Result GetSimilarityExtended(Models.IFS P, Models.IFS Q)
        {

            if (!_isDirty)
            {
                return _result;
            }

           

            GetSpotDiffs(P, Q);

            _result = new Result();

            IOrderedEnumerable<KeyValuePair<Int32, Double>> query = _spotDiffs.OrderBy<KeyValuePair<Int32, Double>, Double>(se => se.Value); 
           
            int nMarkers = 0;
            foreach (KeyValuePair<Int32, Double> pair in query)
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

            query = _spotDiffs.OrderByDescending<KeyValuePair<Int32, Double>, Double>(se => se.Value); 
            nMarkers = 0;
            foreach (KeyValuePair<Int32, Double> pair in query)
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

            

            //Compute the gross similarity level
            int n = _spotDiffs.Count;
            double sum = 0;
            foreach (Double val in _spotDiffs.Values)
            {
                sum += Math.Abs(val);
            }
            _result.Level = 1 - sum / n;


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


        double GetConnotationDifferentialFactor(Result result)
        {

            double sumWellFitted = 0;

            int k = result.CDPWellFitted.Count;

            for (int i = 0; i < k; i++ )
            {
                switch (result.CDPWellFitted[i])
                {
                    case Marker.UpMarker:
                        sumWellFitted += _upMarkWeight;
                        break;
                    case Marker.MiddleMarker:
                        sumWellFitted += _middleMarkWeight;
                        break;
                    case Marker.DownMarker:
                        sumWellFitted += _downMarkWeight;
                        break;
                }
            }
            double avgWellFitted = sumWellFitted / _k;

            double sumWellUnfitted = 0;

            k = result.CDPWellUnfitted.Count;
            for (int i = 0; i < k; i++)
            {
                switch (result.CDPWellUnfitted[i])
                {
                    case Marker.UpMarker:
                        sumWellUnfitted += _upMarkWeight;
                        break;
                    case Marker.MiddleMarker:
                        sumWellUnfitted += _middleMarkWeight;
                        break;
                    case Marker.DownMarker:
                        sumWellUnfitted += _downMarkWeight;
                        break;
                }
            }
            double avgWellUnfitted = sumWellUnfitted / _k;



            return Math.Max(avgWellFitted, avgWellUnfitted);
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
