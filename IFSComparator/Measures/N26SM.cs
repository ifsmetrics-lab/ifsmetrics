using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	H. Nguyen, "A novel similarity/dissimilarity measure for intuitionistic 
	/// 		fuzzy sets and its application in pattern recognition", 
	/// 	Expert Systems with Applications, Volume 45, 1 March 2016, Pages 97-107
	///  
	/// 	DOI: 10.1016/j.eswa.2015.09.045
	/// </summary>
	public class N26SM:IFSMeasure
	{
		const String _CITEKEY = "Nguyen2016";
		const String _CITE = "H.~Nguyen, ``A novel similarity/dissimilarity measure for intuitionistic fuzzy sets and its application in pattern recognition,'' \\emph{Expert Systems with Applications}, vol.~45, pp. 97 -- 107, 2016.";

		public N26SM ()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		public override double GetSimilarity (IFS P, IFS Q)
		{
			double ret = 0;
			double Kf_P = 0;
			double Kf_Q = 0;
			double sum_P = 0;
			double sum_Q = 0;

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

				sum_P = sum_P + Math.Sqrt (Math.Pow(p.Membership, 2) + Math.Pow(p.Nonmembership, 2) + Math.Pow(1 - p.Hesitation, 2));
				sum_Q = sum_Q + Math.Sqrt (Math.Pow(q.Membership, 2) + Math.Pow(q.Nonmembership, 2) + Math.Pow(1 - q.Hesitation, 2));

			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum_P = sum_P + Math.Sqrt (Math.Pow(p2.Membership, 2) + Math.Pow(p2.Nonmembership, 2) + Math.Pow(1 - p2.Hesitation, 2));
					sum_Q = sum_Q + Math.Sqrt (Math.Pow(q2.Membership, 2) + Math.Pow(q2.Nonmembership, 2) + Math.Pow(1 - q2.Hesitation, 2));

				}
			}

			Kf_P = sum_P / (n * Math.Sqrt (2));
			Kf_Q = sum_Q / (n * Math.Sqrt (2));

			if (Kf_P * Kf_Q >= 0) {
				ret = 1 - Math.Abs(Kf_P - Kf_Q);
			} else {
				ret = Math.Abs(Kf_P - Kf_Q) - 1;
			}



			return ret;
		}
	}
}

