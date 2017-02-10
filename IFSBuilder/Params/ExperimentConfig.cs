using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace IFSBuilder.Params
{
	[XmlRoot("ExperimentConfig")]
	public class ExperimentConfig
	{
		public BuildingProcessParams BuildingProcessParams;

	}
}

