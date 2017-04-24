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
	public class Xu17SM:IFSMeasure
	{

		const String _CITEKEY = "Xu2007";
		const String _CITE = "Z.~Xu, ``Some similarity measures of intuitionistic fuzzy sets and their applications to multiple attribute decision making,'' \\emph{Fuzzy Optimization and Decision Making}, vol.~6, no.~2, pp. 109--121, 2007.";

		double _alpha;
		public double Alpha{ 
			get { return _alpha;}
			set { _alpha = value; } 
		}

		public Xu17SM()
		{
			_alpha = 1;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}


		public Xu17SM(double alpha)
		{
			_alpha = alpha;
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
				sumNum = sumNum + Math.Pow(Math.Abs(p.Membership - q.Membership), _alpha) + Math.Pow(Math.Abs(p.Nonmembership - q.Nonmembership), _alpha) + Math.Pow(Math.Abs(p.Hesitation - q.Hesitation), _alpha);
				sumDen = sumDen + Math.Pow(Math.Abs(p.Membership + q.Membership), _alpha) + Math.Pow(Math.Abs(p.Nonmembership + q.Nonmembership), _alpha) + Math.Pow(Math.Abs(p.Hesitation + q.Hesitation), _alpha);

			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sumNum = sumNum + Math.Pow(Math.Abs(p2.Membership - q2.Membership), _alpha) + Math.Pow(Math.Abs(p2.Nonmembership - q2.Nonmembership), _alpha) + Math.Pow(Math.Abs(p2.Hesitation - q2.Hesitation), _alpha);
					sumDen = sumDen + Math.Pow(Math.Abs(p2.Membership + q2.Membership), _alpha) + Math.Pow(Math.Abs(p2.Nonmembership + q2.Nonmembership), _alpha) + Math.Pow(Math.Abs(p2.Hesitation + q2.Hesitation), _alpha);
				}
			}

			ret = 1 - Math.Pow( sumNum / sumDen, (1/_alpha));

			return ret;
		}



	}

}
