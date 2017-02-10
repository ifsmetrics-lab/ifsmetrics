
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSSimReporter.Entities;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using IFSSimReporter.Params;

namespace IFSSimReporter.IO
{
	public class ReferencesReader
	{
		private String _path;

		public ReferencesReader(String path)
		{
			_path = path;

		}

		public void ReadTo(Dictionary<string ,Reference> references)
		{

			StreamReader reader = new StreamReader(_path);
			References refs = new References ();
			XmlSerializer serializer = new XmlSerializer(typeof(References));
			try{
				refs = (References)serializer.Deserialize (reader);

				foreach(Reference r in refs)
				{
					if(!references.ContainsKey(r.Code))
					{
						references.Add(r.Code, r);

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
