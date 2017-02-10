using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	W.L. Hung, M.S. Yang, Similarity measures of intuitionistic fuzzy sets 
	/// 	based on Hausdorff distance, Pattern Recognit. Lett. 25 (14) (2004) 1603–1611.
	/// 
	/// 	DOI: 10.1016/j.patrec.2004.06.006
	/// 
	/// </summary>
	public class HY17SM:IFSMeasure
	{
		const String _CITEKEY = "Hung2004";
		const String _CITE = "W.-L. Hung and M.-S. Yang, ``Similarity measures of intuitionistic fuzzy sets based on Hausdorff distance,'' \\emph{Pattern Recognition Letters}, vol.~25, no.~14, pp. 1603 -- 1611, 2004.";

		public HY17SM ()
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

				sum = sum + Math.Max( Math.Abs(p.Membership-q.Membership),Math.Abs(p.Nonmembership - q.Nonmembership));
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey (q2.ElementId)) {
					p2 = new IFSElement ();
					sum = sum + Math.Max (Math.Abs (p2.Membership - q2.Membership), Math.Abs (p2.Nonmembership - q2.Nonmembership));
				}
			}

			double dHausdorff = sum / n;
			ret = (1- dHausdorff) / (1 + dHausdorff);

			return ret;
		}
	}
}

