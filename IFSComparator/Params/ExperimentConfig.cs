using System;
using System.Xml;
using System.Xml.Serialization;

namespace IFSComparer.Params
{
	[XmlRoot("ExperimentConfig")]
	public class ExperimentConfig
	{
		public ComparisonProcessParams ComparisonProcessParams;


	}
}

