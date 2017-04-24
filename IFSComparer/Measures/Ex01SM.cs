using System;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	///  This measure was proposed in:
	/// 
	///  IFSMetrics Tutorial: How to code a new similarity measure
	/// 
	/// </summary>

	/// Ex01SM is derived from IFSMeasure
	public class Ex01SM:IFSMeasure
	{
		/// <summary>
		/// This is the citation key for this measure.
		/// </summary>
		const String _CITEKEY = "IFSMetricsTut2017";

		/// <summary>
		/// This is the citation for this measure.
		/// </summary>
		const String _CITE = "IFSMetrics, ``A new similarity measure'', IFSMetrics Tutorial";


		/// <summary>
		/// Alpha is a parameter of Ex01SM
		/// </summary>
		double _alpha;
		public double Alpha{ 
			get { return _alpha;}
			set { _alpha = value; } 
		}

		/// <summary>
		/// Beta is a parameter of Ex01SM
		/// </summary>
		double _beta;
		public double Beta{ 
			get { return _beta;}
			set { _beta = value; } 
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IFSComparer.Measures.Ex01SM"/> class.
		/// </summary>
		public Ex01SM()
		{
			_alpha = 0.5;
			_beta = 0.5;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IFSComparer.Measures.Ex01SM"/> class.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		/// <param name="beta">Beta.</param>
		public Ex01SM(double alpha, double beta)
		{
			_alpha = alpha;
			_beta = beta;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

		/// <summary>
		/// Computes the similarity between IFSs P and Q.
		/// </summary>
		/// <returns>The computed similarity value.</returns>
		/// <param name="P">IFS P.</param>
		/// <param name="Q">IFS Q.</param>
		public override double GetSimilarity(IFS P, IFS Q)
		{
			double ret =0;            
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
				sum = sum + _alpha * ((1 + Math.Min (p.Membership, q.Membership)) / (1 + Math.Max (p.Membership, q.Membership)))
					+ _beta * ((1 + Math.Min (p.Nonmembership, q.Nonmembership)) / (1 + Math.Max (p.Nonmembership, q.Nonmembership)));

			}

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					sum = sum + _alpha * ((1 + Math.Min (p2.Membership, q2.Membership)) / (1 + Math.Max (p2.Membership, q2.Membership)))
						+ _beta * ((1 + Math.Min (p2.Nonmembership, q2.Nonmembership)) / (1 + Math.Max (p2.Nonmembership, q2.Nonmembership)));

				}

			}

			ret = sum / n;

			return ret;
		}
			

	}

}