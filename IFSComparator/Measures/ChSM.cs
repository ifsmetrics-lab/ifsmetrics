using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	Chen, Shyi-Ming. "Similarity measures between vague sets and between 
	/// 	elements." IEEE TRANSACTIONS ON SYSTEMS MAN AND CYBERNETICS 
	/// 	PART B-CYBERNETICS 27.1 (1997): 153-158.
	/// 
	/// 	DOI: 10.1109/3477.552198
	/// </summary>
	public class ChSM:IFSMeasure
	{
		const String _CITEKEY = "Chen1997";
		const String _CITE = "S.-M. Chen \\emph{et~al.}, ``Similarity measures between vague sets and between elements,'' \\emph{IEEE TRANSACTIONS ON SYSTEMS MAN AND CYBERNETICS PART B-CYBERNETICS}, vol.~27, no.~1, pp. 153--158, 1997.";

		public ChSM ()
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

				sum = sum + Math.Abs(p.Membership-p.Nonmembership - (q.Membership - q.Nonmembership));
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + Math.Abs(p2.Membership-p2.Nonmembership - (q2.Membership - q2.Nonmembership));
				}
			}

			ret = 1 - sum/(2*n);

			return ret;
		}
	}
}

