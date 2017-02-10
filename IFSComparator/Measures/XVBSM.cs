using System;
using IFSSharedObjects.Models;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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
    public class XVBSM: IFSMeasure
    {

		const String _CITEKEY = "Loor2017";
		const String _CITE = "M.~Loor and G.~De~Tr{\\'e}, In a Quest for Suitable Similarity Measures to Compare Experience-Based Evaluations. In: Computational Intelligence: International Joint Conference, IJCCI 2015 Lisbon, Portugal, November 12-14, 2015, Revised Selected Papers pp. 291-314 (2017)";


        Double _alpha;
		Double _wide;


		public Double Alpha
		{
			get { return _alpha;}
			set { _alpha = value; }
		}

		public Double Wide
		{
			get { return _wide;}
			set { _wide = value;}
		}

        Double _middleMarkWeight = 1;
        Double _upMarkWeight = .01;
        Double _downMarkWeight = .01;

		public Double MiddleMarkWeight
		{
			get{ return _middleMarkWeight; }
			set{ _middleMarkWeight = value;}
		}

		public Double UpMarkWeight
		{
			get{ return _upMarkWeight; }
			set{ _upMarkWeight = value;}
		}

		public Double DownMarkWeight
		{
			get{ return _downMarkWeight; }
			set{ _downMarkWeight = value;}
		}

        int _k = 1;

		public Int32 K
		{
			get{ return _k;}
			set{ _k = value; }
		}

      
        XVBSMResult _result = null;

        

        public enum Marker
        {
            MiddleMarker,
            UpMarker,
            DownMarker
        }

        public class XVBSMResult
        {
            private Double _level;
            public Double Level
            {
                get { return _level; }
                set { _level = value; }
            }

            private Dictionary<Int32, Marker> _cdpWellFitted;
            private Dictionary<Int32, Marker> _cdpWellUnfitted;

            public Dictionary<Int32, Marker> CDPWellFitted
            {
                get { return _cdpWellFitted; }
            }
            public Dictionary<Int32, Marker> CDPWellUnfitted
            {
                get { return _cdpWellUnfitted; }
            }

            public XVBSMResult()
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



        public XVBSM()
        {
			_alpha = 0.5;
			_wide = 0.1;
			_k = 1;
			_middleMarkWeight = 1;
			_upMarkWeight = .6;
			_downMarkWeight = .4;

			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }

        public XVBSM(Double alpha, Double wide, int k,
			Double middleMarkWeight, Double upMarkWeight, Double downMarkWeight)
        {
            _alpha = alpha;
            _wide = wide;
  
            _k = k;
			_middleMarkWeight = middleMarkWeight;
			_upMarkWeight = upMarkWeight;
			_downMarkWeight = downMarkWeight;

			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }
			

        public override double GetSimilarity(IFS P, IFS Q)
        {
            double ret =0;
			_result = new XVBSMResult();
            _result = GetSimilarityExtended(P, Q);
            double cdf = GetConnotationDifferentialFactor(_result);
            ret = _result.Level * cdf;
            return ret;
        }

        public XVBSMResult GetSimilarityExtended(IFS P, IFS Q)
        {

            double sum = 0;

			Dictionary<Int32, Double> spotDiffs = GetSpotDiffs(P, Q);
            

            int n = spotDiffs.Count;

			System.Diagnostics.Debug.Assert (n == P.Count);

            foreach (Double val in spotDiffs.Values)
            {
                sum += Math.Abs(val);
            }
            double level = 1 - sum / n;

            XVBSMResult extRet = new XVBSMResult();
            extRet.Level = level;

            IOrderedEnumerable<KeyValuePair<Int32, IFSElement>> query = P.OrderBy<KeyValuePair<Int32, IFSElement>, IFSElement>(se => se.Value, new IFSElementMembershipComparer());
			Double delta = GetHeuristicDelta(P ,_wide);
            int nMarkers = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                Marker marker;
                if (Math.Abs(spotDiffs[pair.Key]) <= delta)
                    marker = Marker.MiddleMarker;
                else if (spotDiffs[pair.Key] > delta)
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



            query = P.OrderByDescending<KeyValuePair<Int32, IFSElement>, IFSElement>(se => se.Value, new IFSElementMembershipComparer());
            nMarkers = 0;
            foreach (KeyValuePair<Int32, IFSElement> pair in query)
            {
                Marker marker;
                if (Math.Abs(spotDiffs[pair.Key]) <= delta)
                    marker = Marker.MiddleMarker;
                else if (spotDiffs[pair.Key] > delta)
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


            return extRet;
        }

		private Dictionary<Int32, Double> GetSpotDiffs(IFS P, IFS Q)
        {

			Dictionary<Int32, Double> spotDiffs = new Dictionary<int, double> ();
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

				spotDiffs.Add(p.ElementId, (p.Membership - q.Membership) + _alpha * (p.Hesitation - q.Hesitation));
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
					p2.ElementId = q2.ElementId;
                    spotDiffs.Add(q2.ElementId, (p2.Membership - q2.Membership) + _alpha * (p2.Hesitation - q2.Hesitation));
                }

            } 
			return spotDiffs;
        }

        double GetConnotationDifferentialFactor(XVBSMResult result)
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


        private double GetHeuristicDelta(IFS ifsRef, double wide)
        {
            double delta = 0;

            Double sumDistanceMuNu = 0;
            int n = ifsRef.Count;
            foreach (IFSElement el in ifsRef.Values)
            {
                sumDistanceMuNu += (el.Membership + el.Nonmembership);
            }
           
            delta = wide * sumDistanceMuNu / n;

            return delta;
        }
        
      
    }
}
