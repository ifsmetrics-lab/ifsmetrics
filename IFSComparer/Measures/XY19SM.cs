using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	Z.S. Xu, R.R. Yager, "Intuitionistic and interval-valued intuitionistic fuzzy 
	/// 		preference relations and their measures of similarity for the evaluation of 
	/// 		agreement within a group", Fuzzy Optim. Decis. Making 8 (2) (2009) 123–139.
	/// 
	/// 	DOI: 10.1007/s10700-009-9056-3
	/// </summary>
	public class XY19SM:IFSMeasure
	{
		const String _CITEKEY = "Xu2009";
		const String _CITE = "Z.~Xu and R.~R. Yager, ``Intuitionistic and interval-valued intutionistic fuzzy preference relations and their measures of similarity for the evaluation of agreement within a group,'' \\emph{Fuzzy Optimization and Decision Making},  vol.~8, no.~2, pp. 123--139, 2009.";

		public XY19SM ()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		/// <summary>
		/// 	Normalized Hamming distance between IFSElements p and q.
		/// </summary>
		/// <param name="p">IFSElement p</param>
		/// <param name="q">IFSElement q</param>
		private double d(IFSElement p, IFSElement q)
		{
			return (Math.Abs(p.Membership - q.Membership) + 
				Math.Abs(p.Nonmembership - q.Nonmembership) +
				Math.Abs(p.Hesitation - q.Hesitation) 
			)/2;
		}

		/// <summary>
		/// 	Degree of similarity between IFSElements p and q. (see Eq. (18))
		/// </summary>
		/// <param name="p">P.</param>
		/// <param name="q">Q.</param>
		private double s(IFSElement p, IFSElement q)
		{
			double ret;
			if(p.Membership == q.Membership &&
				p.Nonmembership == q.Nonmembership){
				ret = 0.5;  //According to Eq. (18)
				//ret = 1;  //p and q are equal! ... this should be the highest similarity value.
			}
			else{
				ret = d(p,complement(q))/(d(p,q) + d(p, complement(q)));
			}

			return ret;
		}

		private IFSElement complement(IFSElement p)
		{
			IFSElement ret = new IFSElement ();
			ret.ElementId = p.ElementId;
			ret.Membership = p.Nonmembership;
			ret.Nonmembership = p.Membership;

			return ret;
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

				sum = sum + s(p,q);
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey (q2.ElementId)) {
					p2 = new IFSElement ();
					sum = sum + s(p2,q2);
				}
			}

			ret = sum/n;

			return ret;
		}
	}
}

