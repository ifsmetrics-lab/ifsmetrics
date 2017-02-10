using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	A novel similarity measure between intuitionistic fuzzy sets based on the centroid points of transformed fuzzy numbers with applications to pattern recognition
	/// 	Shyi-Ming Chen, Shou-Hsiung Cheng and Tzu-Chun Lan
	/// 	Journal: Information Sciences, 2016, Volume 343-344, Page 15
	/// 
	/// 	DOI: 10.1016/j.ins.2016.01.040
	/// </summary>
	public class CCSM:IFSMeasure
	{
		const String _CITEKEY = "Chen2016";
		const String _CITE = "S.-M. Chen, S.-H. Cheng, and T.-C. Lan, ``A novel similarity measure between intuitionistic fuzzy sets based on the centroid points of transformed fuzzy numbers with applications to pattern recognition,'' \\emph{Information Sciences}, vol. 343--344, pp. 15 -- 40, 2016.";

		public CCSM ()
		{
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}


		public override double GetSimilarity (IFS P, IFS Q)
		{
			double ret = 0;
			double sum = 0;

			//Equally distributed weight
			double w = 1.0/ Math.Max (P.Count, Q.Count);

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
				double rs = 1 - (Math.Abs (p.Membership - q.Membership) * (1 - (p.Hesitation + q.Hesitation) / 2));

				double area_Px=0, area_Qx=0;

				if (p.Membership == (1 - p.Nonmembership)) {
					area_Px = 1;
				} else {
					area_Px = 1 * (1 - p.Nonmembership - p.Membership) / 2;  //p.Hesitation/2 ?
				}

				if (q.Membership == (1 - q.Nonmembership)) {
					area_Qx = 1;
				} else {
					area_Qx = 1 * (1 - q.Nonmembership - q.Membership) / 2;  //q.Hesitation/2 ?
				}

				double us = Math.Abs(area_Px- area_Qx) *((p.Hesitation + q.Hesitation) / 2);

				double s_Px_Qx = rs - us;

				sum = sum + w * s_Px_Qx;
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					double rs = 1 - (Math.Abs (p2.Membership - q2.Membership) * (1 - (p2.Hesitation + q2.Hesitation) / 2));

					double area_Px=0, area_Qx=0;

					if (p2.Membership == (1 - p2.Nonmembership)) {
						area_Px = 1;
					} else {
						area_Px = 1 * (1 - p2.Nonmembership - p2.Membership) / 2;  //p2.Hesitation/2 ?
					}

					if (q2.Membership == (1 - q2.Nonmembership)) {
						area_Qx = 1;
					} else {
						area_Qx = 1 * (1 - q2.Nonmembership - q2.Membership) / 2;  //q2.Hesitation/2 ?
					}

					double us = Math.Abs(area_Px- area_Qx) *((p2.Hesitation + q2.Hesitation) / 2);

					double s_Px_Qx = rs - us;

					sum = sum + w * s_Px_Qx;
				}
			}

			ret = sum;

			return ret;
		}
	}
}

