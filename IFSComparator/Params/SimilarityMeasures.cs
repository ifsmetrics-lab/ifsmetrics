using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using IFSComparer.Measures;

namespace IFSComparer.Params
{

	[XmlRoot("SimilarityMeasures")]
	public class SimilarityMeasures
	{
		
		[XmlElement("CosineSM", typeof(CosineSM)),
			XmlElement("HammingSM", typeof(HammingSM)),
			XmlElement("VBSM", typeof(VBSM)),
			XmlElement("AgreementOnDecisionSM", typeof(AgreementOnDecisionSM)),
			XmlElement("EuclideanSM", typeof(EuclideanSM)),
			XmlElement("XVBSM", typeof(XVBSM)),
			XmlElement("SK1SM", typeof(SK1SM)),
			XmlElement("SK2SM", typeof(SK2SM)),
			XmlElement("SK3SM", typeof(SK3SM)),
			XmlElement("SK4SM", typeof(SK4SM)),
			XmlElement("XVBrSM", typeof(XVBrSM)),
			XmlElement("GGDMSM", typeof(GGDMSM)),
			XmlElement("Xu17SM", typeof(Xu17SM)),
			XmlElement("Xu19SM", typeof(Xu19SM)),
			XmlElement("Xu21SM", typeof(Xu21SM)),
			XmlElement("LOQSM", typeof(LOQSM)),
			XmlElement("CCSM", typeof(CCSM)),
			XmlElement("ChSM", typeof(ChSM)),
			XmlElement("HKSM", typeof(HKSM)),
			XmlElement("BASM", typeof(BASM)),
			XmlElement("HY15SM", typeof(HY15SM)),
			XmlElement("HY16SM", typeof(HY16SM)),
			XmlElement("HY17SM", typeof(HY17SM)),
			XmlElement("N26SM", typeof(N26SM)),
			XmlElement("XY19SM", typeof(XY19SM))

			]
		public ArrayList SMConfigs;


	}
}

