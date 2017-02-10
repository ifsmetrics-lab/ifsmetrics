using System;
using System.IO;

namespace IFSBuilder.IO
{
	class IFSsStatisticsWriter
	{
		private StreamWriter _writer = null;
		private Object _lock = new Object();

		public IFSsStatisticsWriter(String path)
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
				_writer.Write("#Scenario;");
				_writer.Write("Category;");
				_writer.Write("Batch;");
				_writer.Write("AvgDistance(Mu,Nu);");
				_writer.Write("AvgMu;");
				_writer.Write("AvgNu;");
				_writer.Write("AvgH\n");
			}
		}

		public void Write(String cat, String scenario, String group, Double avgDistance, Double avgMu, Double avgNu, Double avgH)
		{

			lock (_lock)
			{
				_writer.WriteLine("{0};{1};{2};{3:F14};{4:F14};{5:F14};{6:F14}",
					scenario,
					cat,
					group,
					avgDistance,
					avgMu,
					avgNu,
					avgH               
				);
			}

		}
			
	}
}
