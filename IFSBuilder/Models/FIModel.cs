using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSBuilder.Collections;

namespace IFSBuilder.Models
{
	class FIModel
	{
		private Double _b = 0; //threshold
		private Double _t = 0; //FI threshold
		private Dictionary<Int32, Double> w = null; //normal vector

		private bool isDirty = true;
		private Double _wMagnitude;

		private Features _ctxFeaturesDictio = null;
		private String _ctxCode = "";
		private String _category = "";

		public String ContextCode
		{
			get { return _ctxCode; }
		}

		public String ModelFor
		{
			get { return _category; }
		}

		public Features ContextTokensDictio
		{
			get { return _ctxFeaturesDictio; }
		}

		public Double b
		{
			get { return _b; }
			set { _b = value; }
		}

		public Double t
		{
			get
			{
				if (isDirty)
				{
					ComputeWMagnitude();
					_t = b / _wMagnitude;
				}
				return _t;
			}
		}

		public FIModel()
		{
			_b = 0;
			w = new Dictionary<int, double>();
			_wMagnitude = 1;
			_ctxFeaturesDictio = new Features();
			_ctxCode = "";
			_category = "";
		}

		public FIModel(String modelledCategory, String ctxCode, Features ctxTokensDictio)
		{
			_b = 0;
			w = new Dictionary<int, double>();
			_wMagnitude = 1;
			_ctxFeaturesDictio = ctxTokensDictio;
			_ctxCode = ctxCode;
			_category = modelledCategory;
		}


		public void AddFeature(int featureCode, double featureWeight)
		{
			if (w.ContainsKey(featureCode))
			{
				w[featureCode] = w[featureCode] + featureWeight;
			}
			else
			{
				w.Add(featureCode, featureWeight);
			}
			isDirty = true;
		}

		public Double GetNormalizedWeight(Int32 featureCode)
		{
			double ret = 0;
			if(w.ContainsKey(featureCode))
			{
				if(isDirty)
				{
					ComputeWMagnitude();
				}
				ret = w[featureCode] / _wMagnitude;
			}
			return ret;
		}


		private void ComputeWMagnitude()
		{
			_wMagnitude = 0;
			double sum = 0;
			foreach (double d in w.Values)
			{
				sum = sum + d * d;
			}
			_wMagnitude = Math.Sqrt(sum);
			isDirty = false;
		}

		private String ToString2()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0:F14} #b\n", b);
			sb.AppendFormat("{0:F14} #t\n", t);
			sb.AppendFormat("{0:F14} #|w|\n", _wMagnitude);
			foreach (KeyValuePair<Int32, Double> pair in w)
			{
				sb.AppendFormat("{0}:{1:F14} ", pair.Key, pair.Value/_wMagnitude);
			}
			sb.Append("# w/|w|");

			return sb.ToString();
		}

		public new String ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("b={0:F14}; ", b);
			sb.AppendFormat("t={0:F14}; ", t);
			sb.AppendFormat("|w|={0:F14} ", _wMagnitude);

			return sb.ToString();
		}

		public double GetAngleToInDegrees(FIModel other)
		{
			double ret = 0;
			double dotProduct = 0;
			foreach (KeyValuePair<Int32, Double> pair in this.w)
			{
				double otherValue = 0;

				//find the token
				string token = this.ContextTokensDictio.IdMapping[pair.Key];
				//find the other key
				int otherNumericCode = -1;
				//NB: the numericCode could be different!
				if (other.ContextTokensDictio.ContainsKey(token))
				{
					otherNumericCode = other.ContextTokensDictio[token].NumericCode;
					otherValue = other.w[otherNumericCode];
				}

				dotProduct += pair.Value * otherValue;
			}
			double cosAngle = dotProduct / (this._wMagnitude * other._wMagnitude);
			ret = Math.Acos(cosAngle) * 180 / Math.PI;
			return ret;
		}

		public void ReadFrom(System.IO.StreamReader reader)
		{
			String currentLine;
			char[] seps = { ';' };
			while (!reader.EndOfStream)
			{
				currentLine = reader.ReadLine();
				if (currentLine.StartsWith("#"))
				{
					string[] parts0 = currentLine.Split(seps);
					if (parts0[0] == "#b")
					{
						this.b = Double.Parse(parts0[1]);
					}
					continue;
				}
				string[] parts = currentLine.Split(seps);
				string token = parts[1];
				double featureWeight = Double.Parse(parts[2]);
				int featureCode = _ctxFeaturesDictio[token].NumericCode;

				w.Add(featureCode, featureWeight);
			}

			isDirty = true;

		}
		public void WriteTo(System.IO.StreamWriter writer)
		{
			writer.WriteLine("#Category: {0}", this.ModelFor);
			writer.WriteLine("#Evaluator: {0}", this.ContextCode);
			writer.WriteLine("#Threshold (t_A): {0:F14}", this.t);
			writer.WriteLine("#Directional vector (u_A): The components of this vector are given below");
			writer.WriteLine("#codeFeature;feature (f_j); coefficient (omega_j)");

			IOrderedEnumerable<String> query = this.ContextTokensDictio.Keys.OrderBy(se => se);
			foreach (string code in query)
			{
				Entities.Feature token = this.ContextTokensDictio[code];
				double specificWeight = 0;
				if (this.w.ContainsKey(token.NumericCode))
				{
					specificWeight = this.w[token.NumericCode];
				}
				writer.WriteLine("{0};{1};{2:F14}",
					token.NumericCode,
					token.Code,
					specificWeight
				);
			}
		}

	}
}
