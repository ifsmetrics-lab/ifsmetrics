using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	///		Y. Li, D.L. Olson, Z. Qin, "Similarity measures between intuitionistic 
	/// 	fuzzy (vague) sets: a comparative analysis, Pattern Recognit. Lett. 28 (2) (2007) 278–285.
	/// 
	/// 	DOI: 10.1016/j.patrec.2006.07.009
	/// 
	/// N.B. The implementations of this measure and EuclideanSM-TwoDimensions are the same.
	/// 
	/// </summary>
	public class LOQSM:IFSMeasure
	{
		const String _CITEKEY = "Li2007";
		const String _CITE = "Y.~Li, D.~L. Olson, and Z.~Qin, ``Similarity measures between intuitionistic fuzzy (vague) sets: A comparative analysis,'' \\emph{Pattern Recognition Letters}, vol.~28, no.~2, pp. 278 -- 285, 2007.";

		public LOQSM ()
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

				sum = sum + Math.Pow(p.Membership-q.Membership,2) + Math.Pow(p.Nonmembership - q.Nonmembership,2);
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum +  Math.Pow(p2.Membership-q2.Membership,2) + Math.Pow(p2.Nonmembership - q2.Nonmembership,2);
				}
			}

			ret = 1 - Math.Sqrt(sum/(2*n));  //Euclidean 2D?

			return ret;
		}
	}
}

