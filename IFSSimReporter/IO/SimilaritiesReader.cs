using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSSimReporter.Entities;
using System.Collections;

namespace IFSSimReporter.IO
{
	public class SimilaritiesReader
	{
		private String _path;

		public SimilaritiesReader(String path)
		{
			_path = path;

		}

		public void ReadTo(Dictionary<string ,CAT_SM_SCN1_SCN2_Tuple> tuples, 
			ArrayList measures, ArrayList categories)
		{
			String currentLine;
			char[] seps = { ',' };

			/*
             * #Category,RefSource,RefSourceOppositePct,OtherSource,OtherSourceOppositePct,EvaluationBatch,Measure,Value
				E11,R0,0,R0,0,1,H2D,1.00000000000000
				E11,R0,0,R0,0,1,H3D,1.00000000000000
             * */

			StreamReader reader = new StreamReader(_path);
			try
			{
				while (!reader.EndOfStream)
				{
					currentLine = reader.ReadLine();
					if (!currentLine.StartsWith("#"))
					{
						String[] parts = currentLine.Split(seps);

						CAT_SM_SCN1_SCN2_Tuple el = new CAT_SM_SCN1_SCN2_Tuple();
					

						el.Category = parts[0];
						el.ReferentScenario = parts[1];
						el.OppositePercentageReferentScenario = Double.Parse(parts[2]);
						el.OtherScenario = parts[3];
						el.OppositePercentageOtherScenario = Double.Parse(parts[4]);
						el.SimilarityMeasure = parts[6];
						double value  = Double.Parse(parts[7]);

						string key = el.GetKey(); 
						if(tuples.ContainsKey(key))
						{
							tuples[key].Sum = tuples[key].Sum + value;
							tuples[key].Count = tuples[key].Count + 1;
						}
						else
						{
							tuples.Add(key, el);
							tuples[key].Sum = value;
							tuples[key].Count = 1;

						}

						if(!categories.Contains(el.Category)){
							categories.Add(el.Category);
						}

						if(!measures.Contains(el.SimilarityMeasure)){
							measures.Add(el.SimilarityMeasure);
						}
					}                  
				}
			}
			finally
			{
				reader.Close();
			}

		}
	}
}
