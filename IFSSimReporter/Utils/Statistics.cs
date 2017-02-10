using System;
using System.Collections.Generic;

namespace IFSSimReporter.Utils
{
	public static class Statistics
	{
		public static double Pearson (IEnumerable<double> Xs, IEnumerable<double> Ys)
		{
			double ret = 0;
			double sumXY = 0;
			double sumX2 = 0;
			double sumY2 = 0;
			double sumX = 0;
			double sumY = 0;
			int n = 0;

			using (IEnumerator<double> pX = Xs.GetEnumerator ())
			using (IEnumerator<double> pY = Ys.GetEnumerator ()) {
				while (pX.MoveNext ()) {
					if (!pY.MoveNext ()) {
						throw new ArgumentOutOfRangeException ();
					}
					double x = pX.Current;
					double y = pY.Current;

					sumX += x;
					sumY += y;
					sumX2 += x * x;
					sumY2 += y * y;
					sumXY += x * y;
					n++;
				}
			}


			ret = (n*sumXY-sumX*sumY)/Math.Sqrt((n*sumX2-sumX*sumX)*(n*sumY2-sumY*sumY));

			return ret;
		}

		public static double Intercept (IEnumerable<double> Xs, IEnumerable<double> Ys)
		{
			double ret = 0;
			double sumXY = 0;
			double sumX2 = 0;
			double sumY2 = 0;
			double sumX = 0;
			double sumY = 0;
			int n = 0;

			using (IEnumerator<double> pX = Xs.GetEnumerator ())
			using (IEnumerator<double> pY = Ys.GetEnumerator ()) {
				while (pX.MoveNext ()) {
					if (!pY.MoveNext ()) {
						throw new ArgumentOutOfRangeException ();
					}
					double x = pX.Current;
					double y = pY.Current;

					sumX += x;
					sumY += y;
					sumX2 += x * x;
					sumY2 += y * y;
					sumXY += x * y;
					n++;
				}
			}

			ret = (sumY*sumX2-sumX*sumXY)/(n*sumX2-sumX*sumX);

			return ret;
		}

		public static double Slope (IEnumerable<double> Xs, IEnumerable<double>Ys)
		{
			double ret = 0;
			double sumXY = 0;
			double sumX2 = 0;
			double sumY2 = 0;
			double sumX = 0;
			double sumY = 0;
			int n = 0;

			using (IEnumerator<double> pX = Xs.GetEnumerator ())
			using (IEnumerator<double> pY = Ys.GetEnumerator ()) {
				while (pX.MoveNext ()) {
					if (!pY.MoveNext ()) {
						throw new ArgumentOutOfRangeException ();
					}
					double x = pX.Current;
					double y = pY.Current;

					sumX += x;
					sumY += y;
					sumX2 += x * x;
					sumY2 += y * y;
					sumXY += x * y;
					n++;
				}
			}

			ret = (n*sumXY-sumX*sumY)/(n*sumX2-sumX*sumX);

			return ret;
		}
	}
}

