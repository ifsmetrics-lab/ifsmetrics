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
	public class Xu21SM:IFSMeasure
	{

		const String _CITEKEY = "Xu2007";
		const String _CITE = "Z.~Xu, ``Some similarity measures of intuitionistic fuzzy sets and their applications to multiple attribute decision making,'' \\emph{Fuzzy Optimization and Decision Making}, vol.~6, no.~2, pp. 109--121, 2007.";

		public Xu21SM()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}


		public override double GetSimilarity(IFS P, IFS Q)
		{
			double ret = 0;
			double sumNum = 0;
			double sumDenP = 0;
			double sumDenQ = 0;

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
				sumNum = sumNum + p.Membership * q.Membership + p.Nonmembership * q.Nonmembership + p.Hesitation * q.Hesitation;
				sumDenP = sumDenP + p.Membership * p.Membership + p.Nonmembership * p.Nonmembership + p.Hesitation * p.Hesitation;
				sumDenQ = sumDenQ + q.Membership * q.Membership + q.Nonmembership * q.Nonmembership + q.Hesitation * q.Hesitation;
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sumNum = sumNum + p2.Membership * q2.Membership + p2.Nonmembership * q2.Nonmembership + p2.Hesitation * q2.Hesitation;
					sumDenP = sumDenP + p2.Membership * p2.Membership + p2.Nonmembership * p2.Nonmembership + p2.Hesitation * p2.Hesitation;
					sumDenQ = sumDenQ + q2.Membership * q2.Membership + q2.Nonmembership * q2.Nonmembership + q2.Hesitation * q2.Hesitation;
				}
			}

			ret = sumNum / Math.Max(sumDenP,sumDenQ);

			return ret;
		}



	}

}
