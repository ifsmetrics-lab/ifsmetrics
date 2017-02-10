using System;
using System.Collections.Generic;
using IFSSharedObjects.Models;
using System.Text;

namespace IFSBuilder.Models
{
	class FIObject
	{
		Int32 _code; 
		Int32 _label; //-1 or 1
		Dictionary<Int32, Double> _features; //features   <featureId, featureWeight>
		private Double _magnitude;
		private Boolean _isDirty;


		public FIObject()
		{
			_code = 0;
			_label = 0;
			_features = new Dictionary<int, double>();
			_magnitude = 0;
			_isDirty = true;
		}

		public Int32 Code
		{
			get { return _code; }
			set { _code = value; }
		}

		public Int32 Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public Double Magnitude
		{
			get { 
				if (_isDirty) { ComputeMagnitude(); }
				return _magnitude;
			}
		}

		public void AddFeature(Int32 key, Double weight)
		{
			_features.Add(key, weight);
			_isDirty = true;
		}

		public Double GetFeatureOverallInfluence(Int32 key)
		{
			Double ret = 0;
			if(_features.ContainsKey(key))
			{
				ret = _features[key];
			}
			return ret;
		}

		public Double GetFeatureSpecificInfluence(Int32 key, FIModel model)
		{
			Double ret = 0;
			Double weight;
			if (_features.ContainsKey(key))
			{                
				weight = model.GetNormalizedWeight(key);
				ret = _features[key] * weight;
			}

			return ret;
		}

		public Dictionary<Int32, Double> GetFeatures()
		{
			return _features;
		}

		public IFSElement GetSoftEvaluation(FIModel model)
		{
			IFSElement ret = new IFSElement();            

			foreach (KeyValuePair<Int32, Double> p in _features)
			{
				Double sinfluence = this.GetFeatureSpecificInfluence(p.Key, model);
				if (sinfluence > 0)
				{
					ret.Membership += sinfluence;
				}
				else if (sinfluence < 0)
				{
					ret.Nonmembership += Math.Abs(sinfluence);
				}
			}

			if (model.t < 0)
			{
				ret.Membership += Math.Abs(model.t);
			}
			else if (model.t > 0)
			{
				ret.Nonmembership += Math.Abs(model.t);
			}

			if (_isDirty)
			{
				this.ComputeMagnitude();
			}

			ret.Membership = ret.Membership / _magnitude;
			ret.Nonmembership = ret.Nonmembership / _magnitude;
			ret.ElementId = this.Code;

			return ret;
		}


		private void ComputeMagnitude()
		{
			_magnitude = 0;
			double sum = 0;
			foreach (double d in _features.Values)
			{
				sum = sum + d * d;
			}
			_magnitude = Math.Sqrt(sum);
			_isDirty = false;
		}
	}
}
