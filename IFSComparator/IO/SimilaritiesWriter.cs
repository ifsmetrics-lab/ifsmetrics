using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IFSComparer.IO
{
	class SimilaritiesWriter
	{
		private StreamWriter _writer = null;
		private Object _lock = new Object();

		public SimilaritiesWriter(String path)
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

		public void WriteHeader()
		{
			lock (_lock)
			{                                
				_writer.Write("#Category,");
				_writer.Write("RefSource,");
				_writer.Write("RefSourceOppositePct,");
				_writer.Write("OtherSource,");
				_writer.Write("OtherSourceOppositePct,");
				_writer.Write("EvaluationBatch,");
				_writer.Write("Measure,");
				_writer.WriteLine("Value");
			}
		}

		public void Write(String cat, String refSource, double refSourceOppositePct, String otherSource, double otherSourceOppositePct, Int32 group, String measure, Double value)
		{

			lock (_lock)
			{
				_writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7:F14}",
					cat,
					refSource,
					refSourceOppositePct,
					otherSource,
					otherSourceOppositePct,
					group,
					measure,
					value               
				);
			}

		}


	}
}
