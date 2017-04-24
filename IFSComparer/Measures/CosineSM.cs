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
	/// 	Szmidt, E., Kacprzyk, J.: Geometric similarity measures for the intuitionistic fuzzy sets. 
	/// 	In: 8th conference of the European Society for Fuzzy Logic and Technology (EUSFLAT-13), pp. 840–847. 
	///     Atlantis Press (2013)
	/// 
	/// </summary>
    public class CosineSM: IFSMeasure
    {
		const String _CITEKEY = "Szmidt2013";
		const String _CITE = "``Geometric similarity measures for the intuitionistic fuzzy sets,'' in \\emph{8th conference of the European Society for Fuzzy Logic and Technology  (EUSFLAT-13)}. Atlantis Press, 2013,  pp. 840--847.";

		public CosineSM()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		public override double GetSimilarity(IFS P, IFS Q)
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
                sum = sum + (p.Membership * q.Membership + p.Nonmembership * q.Nonmembership + p.Hesitation * p.Hesitation) /
                    (
                        Math.Sqrt(Math.Pow(p.Membership, 2) + Math.Pow(p.Nonmembership, 2) + Math.Pow(p.Hesitation,2)) *
                        Math.Sqrt(Math.Pow(q.Membership, 2) + Math.Pow(q.Nonmembership, 2) + Math.Pow(q.Hesitation, 2))
                    );
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + (p2.Membership * q2.Membership + p2.Nonmembership * q2.Nonmembership + p2.Hesitation * p2.Hesitation) /
                    (
                        Math.Sqrt(Math.Pow(p2.Membership, 2) + Math.Pow(p2.Nonmembership, 2) + Math.Pow(p2.Hesitation, 2)) *
                        Math.Sqrt(Math.Pow(q2.Membership, 2) + Math.Pow(q2.Nonmembership, 2) + Math.Pow(q2.Hesitation, 2))
                    );
                }
            }

            ret =  sum / n;

            return ret;
        }

       
    }
}
