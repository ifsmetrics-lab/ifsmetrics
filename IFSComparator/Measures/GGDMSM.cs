

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
	public class GGDMSM:IFSMeasure
	{
		const String _CITEKEY = "Xu2007";
		const String _CITE = "Z.~Xu, ``Some similarity measures of intuitionistic fuzzy sets and their applications to multiple attribute decision making,'' \\emph{Fuzzy Optimization and Decision Making}, vol.~6, no.~2, pp. 109--121, 2007.";

		public enum GGDMSMType
		{
			TwoDimensions,
			ThreeDimensions
		}

		GGDMSMType _type = GGDMSMType.ThreeDimensions;
		public GGDMSMType  GGDMType{
			get{ return _type;}
			set { _type = value; }
		}

		double _alpha;
		public double Alpha{ 
			get { return _alpha;}
			set { _alpha = value; } 
		}

		public GGDMSM()
		{
			_alpha = 1;
			_type = GGDMSMType.ThreeDimensions;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}


		public GGDMSM(GGDMSMType type, double alpha)
		{
			_type = type;
			_alpha = alpha;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		public override double GetSimilarity(IFS P, IFS Q)
		{
			if (_type == GGDMSMType.ThreeDimensions)
				return this.GetSimilarity3D(P, Q);
			else
				return this.GetSimilarity2D(P, Q);
		}

		/// <summary>
		/// See Eq. 15 in Xu2007
		/// </summary>
		private double GetSimilarity3D(IFS P, IFS Q)
		{
			double ret = 0;
			double sum = 0;

			int n = Math.Max(P.Count, Q.Count);
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
				sum = sum + Math.Pow(Math.Abs(p.Membership - q.Membership), _alpha) + Math.Pow(Math.Abs(p.Nonmembership - q.Nonmembership), _alpha) + Math.Pow(Math.Abs(p.Hesitation - q.Hesitation), _alpha);
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + Math.Pow(Math.Abs(p2.Membership - q2.Membership), _alpha) + Math.Pow(Math.Abs(p2.Nonmembership - q2.Nonmembership), _alpha) + Math.Pow(Math.Abs(p2.Hesitation - q2.Hesitation), _alpha);
				}
			}

			ret = 1 - Math.Pow( 0.5 * sum / n, (1/_alpha));

			return ret;
		}

		private double GetSimilarity2D(IFS P, IFS Q)
		{
			double ret = 0;
			double sum = 0;

			int n = Math.Max(P.Count, Q.Count);
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
				sum = sum + Math.Pow(Math.Abs(p.Membership - q.Membership), _alpha) + Math.Pow(Math.Abs(p.Nonmembership - q.Nonmembership), _alpha);
			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + Math.Pow(Math.Abs(p2.Membership - q2.Membership), _alpha) + Math.Pow(Math.Abs(p2.Nonmembership - q2.Nonmembership), _alpha);
				}
			}

			ret = 1 - Math.Pow(0.5 * sum / n, 1/_alpha);

			return ret;
		}

	}

}
