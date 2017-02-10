using System;

namespace IFSSimReporter.Entities
{
	public class CAT_SM_SCN1_SCN2_Tuple
	{
		
		public String Category{ get; set;}

		public String SimilarityMeasure{ get; set;}

		public String ReferentScenario{ get; set;}

		public Double OppositePercentageReferentScenario{ get; set;}

		public String OtherScenario{ get; set;}

		public Double OppositePercentageOtherScenario{ get; set;}

		public Double Sum{ get; set;}

		public Int32 Count{ get; set;}

		public Double Average{ get{ if(Count>0) return Sum/Count; else return Double.NaN;} }


		public CAT_SM_SCN1_SCN2_Tuple ()
		{
			Category ="";
			SimilarityMeasure="";
			ReferentScenario="";
			OtherScenario="";
			Sum = 0;
			Count = 0;
			OppositePercentageReferentScenario = 0;
			OppositePercentageOtherScenario = 0;
		}

		public string GetKey ()
		{
			return string.Format ("{0}_{1}_{2}_{3}", Category, SimilarityMeasure, ReferentScenario, OtherScenario);
		}

		public override string ToString ()
		{
			return string.Format ("[CAT_SM_SCN1_SCN2_Tuple: Category={0}, SimilarityMeasure={1}, ReferentScenario={2}, OppositePercentageReferentScenario={3}, OtherScenario={4}, OppositePercentageOtherScenario={5}, Sum={6}, Count={7}, Average={8}]", Category, SimilarityMeasure, ReferentScenario, OppositePercentageReferentScenario, OtherScenario, OppositePercentageOtherScenario, Sum, Count, Average);
		}
	}
}

