using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	///  This measure was proposed in:
	/// 
	///  Some similarity measures of intuitionistic fuzzy sets 
	///  and their applications to multiple attribute decision making.
	/// 
	///  DOI: 10.1007/s10700-007-9004-z
	/// 
	/// </summary>
	public class Xu19SM:IFSMeasure
	{
		const String _CITEKEY = "Xu2007";
		const String _CITE = "Z.~Xu, ``Some similarity measures of intuitionistic fuzzy sets and their applications to multiple attribute decision making,'' \\emph{Fuzzy Optimization and Decision Making}, vol.~6, no.~2, pp. 109--121, 2007.";


		public Xu19SM()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}


		public override double GetSimilarity(IFS P, IFS Q)
		{
			double ret = 0;
			double sumNum = 0;
			double sumDen = 0;

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
				sumNum = sumNum + Math.Min(p.Membership,q.Membership) + Math.Min(p.Nonmembership, q.Nonmembership) + Math.Min(p.Hesitation, q.Hesitation);
				sumDen = sumDen + Math.Max(p.Membership,q.Membership) + Math.Max(p.Nonmembership, q.Nonmembership) + Math.Max(p.Hesitation, q.Hesitation);

			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sumNum = sumNum + Math.Min(p2.Membership,q2.Membership) + Math.Min(p2.Nonmembership, q2.Nonmembership) + Math.Min(p2.Hesitation, q2.Hesitation);
					sumDen = sumDen + Math.Max(p2.Membership,q2.Membership) + Math.Max(p2.Nonmembership, q2.Nonmembership) + Math.Max(p2.Hesitation, q2.Hesitation);
				}
			}

			ret = sumNum / sumDen;

			return ret;
		}



	}

}
