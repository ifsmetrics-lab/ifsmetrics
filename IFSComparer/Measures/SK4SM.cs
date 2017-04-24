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
	/// 	Szmidt, E., Kacprzyk, J.: A concept of similarity for intuitionistic fuzzy sets 
	/// 	and its use in group decision making. 
	/// 	In: IEEE International Conference on Fuzzy Systems, pp. 1129–1134 (2004)
	/// 
	/// </summary>
    public class SK4SM : IFSMeasure
    {
		const String _CITEKEY = "Szmidt2004";
		const String _CITE = "E.~Szmidt and J.~Kacprzyk, ``A concept of similarity for intuitionistic fuzzy sets and its use in group decision making,'' in \\emph{IEEE International Conference on Fuzzy Systems}, 2004, pp. 1129--1134.";

        public enum SK4SMType
        {
            TwoDimensions,
            ThreeDimensions
        }

        SK4SMType _type = SK4SMType.ThreeDimensions;

		public SK4SMType SK4Type{
			get{return _type; }
			set{ _type = value; }
		}

        public SK4SM()
        {
            _type = SK4SMType.ThreeDimensions;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }


        public SK4SM(SK4SMType type)
        {
            _type = type;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }

        public override double GetSimilarity(IFS P, IFS Q)
        {
            IFS QC = new IFS("QC");
            foreach (IFSElement el in Q.Values)
            {
                IFSElement nel = new IFSElement();
                nel.ElementId = el.ElementId;
                nel.Membership = el.Nonmembership;
                nel.Nonmembership = el.Membership;
                QC.Add(nel.ElementId, nel);
            }

            double ret = 0.5;
            double d1, d2;

            if (_type == SK4SMType.ThreeDimensions)
            {
                d1 = GetDistance3D(P, Q);
                d2 = GetDistance3D(P, QC);
            }
            else
            {
                d1 = GetDistance2D(P, Q);
                d2 = GetDistance2D(P, QC);
            }

            if (d1 + d2 > 0)
            {
                ret = (Math.Exp(-f(d1, d2)) - Math.Exp(-1)) / (1 - Math.Exp(-1));
            }
            return ret;
        }


        double GetDistance3D(IFS P, IFS Q)
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

            ret = 0.5 * sum / n;

            return ret;
        }

        double GetDistance2D(IFS P, IFS Q)
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
                sum = sum + Math.Abs((p.Membership - q.Membership)) + Math.Abs((p.Nonmembership - q.Nonmembership));
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Abs((p2.Membership - q2.Membership)) + Math.Abs((p2.Nonmembership - q2.Nonmembership));
                }
            }

            ret = 0.5 * sum / n;

            return ret;
        }

        double f(double a, double b)
        {
            double ret = 0;
            ret = a / (a + b);
            return ret;
        }

    }

}
