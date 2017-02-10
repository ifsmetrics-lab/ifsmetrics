using System;
using System.Xml;
using System.Xml.Serialization;

namespace IFSSimReporter.Params
{
	[XmlRoot("ExperimentConfig")]
	public class ExperimentConfig
	{
		public ReportingProcessParams ReportingProcessParams;


	}
}

