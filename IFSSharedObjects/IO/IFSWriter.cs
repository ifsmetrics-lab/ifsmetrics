using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IFSSharedObjects.IO
{
	public class IFSWriter
	{
		private StreamWriter _writer = null;
		private Object _lock = new Object();
		private Models.IFS _ifs;

		public IFSWriter(String path, Models.IFS ifs)
		{
			_writer = new StreamWriter(path);
			_ifs = ifs;
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
				_writer.Write("#x;");
				_writer.Write("mu(x);");
				_writer.Write("nu(x);");
				_writer.Write("h(x)\n");
			}
		}

		public void Write(Models.IFSElement el)
		{

			lock (_lock)
			{
				_writer.WriteLine("{0};{1:F14};{2:F14};{3:F14}",
					el.ElementId,
					el.Membership,
					el.Nonmembership,
					el.Hesitation                
				);
			}

		}

		public void Write()
		{
			foreach (Models.IFSElement el in _ifs.Values)
			{
				this.Write(el);
			}
		}


	}
}
