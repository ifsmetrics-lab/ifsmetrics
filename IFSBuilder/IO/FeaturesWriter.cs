using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Collections;
using IFSBuilder.Entities;

namespace IFSBuilder.IO
{


	class FeaturesWriter
	{
		private StreamWriter _writer = null;
		private Object _lock = new Object();

		public FeaturesWriter(String path)
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

		public void WriteHeader(bool byCats=true)
		{
			lock (_lock)
			{                                
				_writer.Write("Feature;");
				_writer.Write("Id;");
				_writer.Write("IDF;");
				_writer.Write("DF\n");
			}
		}

		public void Write(Feature feature)
		{

			lock (_lock)
			{
				_writer.WriteLine("{0};{1};{2:F14};{3}",
					feature.Code,
					feature.NumericCode,
					feature.IDF(),
					feature.nDocumentsHavingMe                
				);
			}

		}

		public void Write(Features features)
		{
			IOrderedEnumerable<String> query = features.Keys.OrderBy(se => se);

			foreach (String token in query)
			{
				this.Write(features[token]);
			}
		}


	}
}
