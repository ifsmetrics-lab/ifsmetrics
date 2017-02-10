using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in 
	/// 
	/// 	Szmidt, E., Kacprzyk, J.: Distances between intuitionistic fuzzy sets. 
	/// 	Fuzzy Sets Syst. 114(3), 505–518 (2000)
	/// 
	/// 	DOI: 10.1016/S0165-0114(98)00244-9
	/// </summary>
    public class EuclideanSM:IFSMeasure
    {
		const String _CITEKEY = "Szmidt2000";
		const String _CITE = "E.~Szmidt and J.~Kacprzyk, ``{Distances between intuitionistic fuzzy sets},'' \\emph{Fuzzy Sets and Systems}, vol. 114, no.~3, pp. 505--518, Sep. 2000.";

		public enum EuclideanSMType
        {
            TwoDimensions,
            ThreeDimensions
        }

		EuclideanSMType _type = EuclideanSMType.ThreeDimensions;

		public EuclideanSMType  EuclideanType{
			get{ return _type;}
			set { _type = value; }
		}

        public EuclideanSM()
        {
			_type = EuclideanSMType.ThreeDimensions;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }


		public EuclideanSM(EuclideanSMType type)
        {
            _type = type;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }

        public override double GetSimilarity(IFS P, IFS Q)
        {
			if (_type == EuclideanSMType.ThreeDimensions)
				return this.GetSimilarity3D(P, Q);
            else
				return this.GetSimilarity2D(P, Q);
        }

        private double GetSimilarity3D(IFS P, IFS Q)
        {
            double ret = 0;
            double sum = 0;

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
                }
                sum = sum + Math.Pow((p.Membership - q.Membership), 2) + Math.Pow((p.Nonmembership - q.Nonmembership), 2) + Math.Pow(p.Hesitation - q.Hesitation, 2);
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Pow((p2.Membership - q2.Membership), 2) + Math.Pow((p2.Nonmembership - q2.Nonmembership),2) + Math.Pow(p2.Hesitation - q2.Hesitation,2);
                }
            }

            ret = 1 - Math.Sqrt( 0.5 * sum / n);

            return ret;
        }

        private double GetSimilarity2D(IFS P, IFS Q)
        {
            double ret = 0;
            double sum = 0;

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
                }
                sum = sum + Math.Pow((p.Membership - q.Membership), 2) + Math.Pow((p.Nonmembership - q.Nonmembership), 2);
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Pow((p2.Membership - q2.Membership), 2) + Math.Pow((p2.Nonmembership - q2.Nonmembership), 2);
                }
            }

            ret = 1 - Math.Sqrt(0.5 * sum / n);

            return ret;
        }
        
    }
   
}
