using System;
using System.Collections.Generic;


namespace IFSSharedObjects.Models
{
	public class IFS : Dictionary<Int32, IFSElement>
	{
		private String _name;
		private Double _avgMu;
		private Double _avgNu;
		private Double _avgH;
		private Double _avgDistanceMuNu;
		private bool _isDirty;

		public Double AvgMu
		{
			get { if (_isDirty) { _ComputeMetrics(); } return _avgMu; }
		}
		public Double AvgNu
		{
			get { if (_isDirty) { _ComputeMetrics(); } return _avgNu; }
		}
		public Double AvgH
		{
			get { if (_isDirty) { _ComputeMetrics(); } return _avgH; }
		}
		public Double AvgDistanceMuNu
		{
			get { if (_isDirty) { _ComputeMetrics(); } return _avgDistanceMuNu; }
		}


		public String Name
		{
			get { return _name; }
		}

		public IFS(String name)
		{
			_name = name;
			_isDirty = true;
		}

		private void _ComputeMetrics()
		{
			Double sumMu=0;
			Double sumNu = 0;
			Double sumH = 0;
			Double sumDistanceMuNu = 0;
			int n = this.Count;
			foreach (IFSElement el in this.Values)
			{
				sumMu += el.Membership;
				sumNu += el.Nonmembership;
				sumH += el.Hesitation;
				sumDistanceMuNu += (el.Membership + el.Nonmembership);
			}

			_avgMu = sumMu/n;
			_avgNu = sumNu / n;
			_avgH = sumH / n;
			_avgDistanceMuNu = sumDistanceMuNu / n;

			_isDirty = false;
		}


		public void EnsureIFSDefinition()
		{
			double sigma = 1;

			foreach (IFSElement elem in this.Values)
			{
				sigma = Math.Max(sigma, elem.Membership + elem.Nonmembership);
			}

			foreach (IFSElement elem in this.Values)
			{
				elem.Membership = elem.Membership / sigma;
				elem.Nonmembership = elem.Nonmembership / sigma;

				System.Diagnostics.Debug.Assert(elem.Hesitation >= 0);
			}
		}
	}
}
