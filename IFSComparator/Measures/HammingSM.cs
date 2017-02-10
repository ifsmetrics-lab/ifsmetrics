using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class HammingSM:IFSMeasure
    {
		const String _CITEKEY = "Szmidt2000";
		const String _CITE = "E.~Szmidt and J.~Kacprzyk, ``{Distances between intuitionistic fuzzy sets},'' \\emph{Fuzzy Sets and Systems}, vol. 114, no.~3, pp. 505--518, Sep. 2000.";

        public enum Type
        {
            TwoDimensions,
            ThreeDimensions
        }

        Type _type = Type.ThreeDimensions;

		public Type HammingType{
			get{ return _type;}
			set { _type = value; }
		}

        public HammingSM()
        {
            _type = Type.ThreeDimensions;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }


        public HammingSM(Type type)
        {
            _type = type;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }

        public override double GetSimilarity(IFS P, IFS Q)
        {
            if (_type == Type.ThreeDimensions)
                return this.GetSimilarity3D(P, Q);
            else
                return this.GetSimilarity2D(P, Q);
        }

        double GetSimilarity3D(IFS P, IFS Q)
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
                    //throw new ArgumentException("Objects in IFS P are different to objects in IFS Q");
                    q = new IFSElement();
                }
                sum = sum + Math.Abs((p.Membership - q.Membership)) + Math.Abs((p.Nonmembership - q.Nonmembership)) + Math.Abs(p.Hesitation - q.Hesitation);
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Abs((p2.Membership - q2.Membership)) + Math.Abs((p2.Nonmembership - q2.Nonmembership)) + Math.Abs(p2.Hesitation - q2.Hesitation);
                }
            }

            ret = 1 - 0.5 * sum / n;

            return ret;
        }

        double GetSimilarity2D(IFS P, IFS Q)
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
                    //throw new ArgumentException("Objects in IFS P are different to objects in IFS Q");
                    q = new IFSElement();
                }
                sum = sum + Math.Abs((p.Membership - q.Membership)) + Math.Abs((p.Nonmembership - q.Nonmembership)) ;
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Abs((p2.Membership - q2.Membership)) + Math.Abs((p2.Nonmembership - q2.Nonmembership)) + Math.Abs(p2.Hesitation - q2.Hesitation);
                }
            }

            ret = 1 - 0.5 * sum / n;

            return ret;
        }
    }
   
}
