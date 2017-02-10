using System;
using System.IO;

namespace IFSSimReporter.Templates
{
	public class PDFTemplate
	{
		private string _path;
		public PDFTemplate (String path)
		{
			_path = path;
		}

		public void Write(String reportPath,
			String category,
			String referentMeasureDataPath,
			String otherMeasureDataPath
		){
			StreamWriter w = new StreamWriter (_path);

			w.WriteLine("set terminal pdf");
			w.WriteLine("set output '{0}'",reportPath);
			w.WriteLine ("set title 'Category: {0}'", category);
			w.WriteLine ("set xlabel '% opposites'");
			w.WriteLine ("set ylabel 'Average similarity' offset -1");
			w.WriteLine ("set format x '%.0f%%'");
			w.WriteLine ("set format y '%g'");
			w.WriteLine ("set key top left");
			w.WriteLine ("plot '{0}' using 2:3 title 'SM 1' with lines, " +
				"'{1}' using 2:3 title 'SM 2' with linespoints ls 5 pt 4 pi 0.2",
				referentMeasureDataPath, otherMeasureDataPath);
			w.Close ();

		}
	}
}

