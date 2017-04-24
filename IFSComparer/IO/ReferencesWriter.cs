
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSComparer.Measures;
using IFSComparer.Params;
using System.Xml;
using System.Xml.Serialization;

namespace IFSComparer.IO
{
	class ReferencesWriter
	{
		private StreamWriter _writer = null;



		public ReferencesWriter(String path)
		{
			_writer = new StreamWriter(path);
		}
		public void Close()
		{
			if (_writer != null)
			{
				_writer.Close();
			}
		}


		public void Write(SimilarityMeasures measures)
		{

			References refs = new References ();
			foreach (IFSMeasure measure in measures.SMConfigs) {
				Reference r = new Reference ();
				r.Cite = measure.Cite;
				r.CiteKey = measure.CiteKey;
				r.Code = measure.Code;
				refs.Add (r);
			}

			XmlSerializer serializer = new XmlSerializer(typeof(References));
			serializer.Serialize (_writer, refs);


		}


	}
}
