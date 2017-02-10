using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	D.H. Hong, C. Kim, "A note on similarity measures between vague 
	/// 	sets and between elements.", Inform. Sci. 115 (1–4) (1999) 83–96.
	/// 
	/// 	DOI: 10.1016/S0020-0255(98)10083-X
	/// </summary>
	public class HKSM:IFSMeasure
	{
		const String _CITEKEY = "Hong1999";
		const String _CITE = "D.~H. Hong and C.~Kim, ``A note on similarity measures between vague sets and between elements,'' \\emph{Information Sciences}, vol. 115, no.~1, pp. 83 --\n  96, 1999.";

		public HKSM ()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		public override double GetSimilarity (IFS P, IFS Q)
		{
			double ret = 0;
			double sum = 0;

			//Equally distribuited weight
			double n = Math.Max (P.Count, Q.Count);

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

				sum = sum + Math.Abs(p.Membership-q.Membership) + Math.Abs(p.Nonmembership - q.Nonmembership);
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + Math.Abs(p2.Membership-q2.Membership) + Math.Abs(p2.Nonmembership - q2.Nonmembership);
				}
			}

			ret = 1 - sum/(2*n);

			return ret;
		}
	}
}

