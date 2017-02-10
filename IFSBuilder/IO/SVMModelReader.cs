using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Models;
namespace IFSBuilder.IO
{
	class SVMModelReader
	{
		private String _path;

		public SVMModelReader(String path)
		{
			_path = path;

		}

		public void ReadTo(FIModel model)
		{
			String currentLine;
			char[] sepSpace = { ' ' };
			char[] sepsSharp = { '#' };
			char[] sepsColon = { ':' };
			int nLine =0;

			/*
            SVM-light Version V6.02
            0 # kernel type
            3 # kernel parameter -d 
            1 # kernel parameter -g 
            1 # kernel parameter -s 
            1 # kernel parameter -r 
            empty# kernel parameter -u 
            7 # highest feature index 
            5 # number of training documents 
            4 # number of support vectors plus 1 - line 10
            -0.16666104 # threshold b, each following line is a SV (starting with alpha*y) - line 11
            -0.2083335694748942 1:1 3:2 5:2 # 3
            0.16666822864545586 2:1 3:2 7:2 # 2
*/
			//int nSupportVectors = 0;
			StreamReader reader = new StreamReader(_path);
			Double tmp;
			try
			{
				while (!reader.EndOfStream)
				{
					currentLine = reader.ReadLine();
					nLine++;

					if(nLine==10)
					{
						//nSupportVectors = Int32.Parse(currentLine.Split(sepsSharp)[0]) - 1;
					}
					else if(nLine==11)
					{
						tmp = Double.Parse(currentLine.Split(sepsSharp)[0]);
						model.b = tmp;
					} 
					else if(nLine>11)
					{
						Double featureMult =0;
						Int32 featureCode =0;
						Double featureWeight =0;
						String[] parts = currentLine.Split(sepSpace); 
						Int32 pos = 0;

						foreach(string part in parts)
						{
							if(pos==0)
							{
								featureMult = Double.Parse(parts[pos]);
								pos ++;
							}
							else{
								String[] parts2 = part.Split(sepsColon);
								if(parts2.Length ==2)
								{
									featureCode = Int32.Parse(parts2[0]);
									featureWeight = Double.Parse(parts2[1]);
									model.AddFeature(featureCode, featureMult * featureWeight);
								}

							}
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
