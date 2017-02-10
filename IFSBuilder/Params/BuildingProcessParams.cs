using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;

namespace IFSBuilder.Params
{
	[XmlRoot("BuildingProcessParams")]
	public class BuildingProcessParams
	{

		/// <summary>
		/// Evaluation process parameters.
		/// </summary>
		public EvaluationProcessParams EvaluationParams;

		/// <summary>
		/// Learning process parameters.
		/// </summary>
		public LearningProcessParams LearningParams;

		/// <summary>
		/// Scenarios to be used during the building process.
		/// </summary>
		public Scenarios Scenarios;

		/// <summary>
		/// Categories to be used during the building process.
		/// </summary>
		public Categories Categories;


		/// <summary>
		/// Directory to place the temporary files.
		/// </summary>
		public String WorkingDir;

	}
}

