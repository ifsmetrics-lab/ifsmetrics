

using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	///		F.E. Boran, D. Akay, A biparametric similarity measure on intuitionistic 
	/// 	fuzzy sets with applications to pattern recognition, Inform. Sci. 255 (10) (2014) 45–57.
	/// 
	/// 	DOI: 10.1016/j.ins.2013.08.013
	/// 
	/// 
	/// </summary>
	public class BASM:IFSMeasure
	{
		const String _CITEKEY = "Boran2014";
		const String _CITE = "F.~E. Boran and D.~Akay, ``A biparametric similarity measure on intuitionistic fuzzy sets with applications to pattern recognition,'' \\emph{Information Sciences}, vol. 255, pp. 45 -- 57, 2014.";

		double _p = 1; // _p>=1

		public double P
		{
			get{ return _p; }
			set{ _p = value;}
		}

		double _s = 2; // _s>=2

		public double S{
			get { return _s;}
			set { _s = value;}
		}

		public BASM ()
		{
			_s = 2;
			_p = 1;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		public BASM (double p, double s)
		{
			_s = s;
			_p = p;
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

				sum = sum + Math.Pow (Math.Abs (_s * (p.Membership - q.Membership) - (p.Nonmembership - q.Nonmembership)), _p) +
							Math.Pow (Math.Abs (_s * (p.Nonmembership - q.Nonmembership) - (p.Membership - q.Membership)), _p);
					
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + Math.Pow (Math.Abs (_s * (p2.Membership - q2.Membership) - (p2.Nonmembership - q2.Nonmembership)), _p) +
						Math.Pow (Math.Abs (_s * (p2.Nonmembership - q2.Nonmembership) - (p2.Membership - q2.Membership)), _p);
					}
			}

			ret = 1 - Math.Pow(sum/(2*n*Math.Pow(_s+1, _p)), 1/_p); 

			return ret;
		}
	}
}

