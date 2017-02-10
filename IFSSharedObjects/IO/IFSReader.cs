using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IFSSharedObjects.IO
{
	public class IFSReader
	{
		private String _path;

		public IFSReader(String path)
		{
			_path = path;

		}

		public void ReadTo(Models.IFS ifs)
		{
			String currentLine;
			char[] seps = { ';' };

			/*
             * #x;mu(x);nu(x);h(x)
                26151;0.08291756880010;0.01714005514577;0.89994237605413
                26152;0.07160154821757;0.02935421630686;0.89904423547558
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
						Models.IFSElement el = new Models.IFSElement();
						el.ElementId = Int32.Parse(parts[0]);
						el.Membership = Double.Parse(parts[1]);
						el.Nonmembership = Double.Parse(parts[2]);

						ifs.Add(el.ElementId, el);
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
