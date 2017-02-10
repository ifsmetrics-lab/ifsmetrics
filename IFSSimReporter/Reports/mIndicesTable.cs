using System;

namespace IFSSimReporter.Reports
{
	public abstract class mIndicesTable
	{


		protected abstract void WriteHeader();
	 	protected abstract void WriteLine (String measure, double slope, double intercept, double R, double mIndex);
		protected abstract void WriteFooter();

		protected double GetMIndex(double refSlope, double refIntercept, double refR,
			double slope, double intercept, double R)
		{
			double index = (slope*intercept*R*R)/(refSlope*refIntercept*refR*refR);

			return index;
		}

	}
}

