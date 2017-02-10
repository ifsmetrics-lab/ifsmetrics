using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Models;

namespace IFSBuilder.IO
{

	class SVMVectorsReader
	{
		private String _path;
		private StreamReader _reader;
		private bool _isOpen = false;
		private int _lineOffset = 0;

		public SVMVectorsReader(String path)
		{
			_path = path;

		}

		public void Open()
		{
			if (!_isOpen)
			{
				_reader = new StreamReader(_path);
				_isOpen = true;
				_lineOffset = 0;
			}

		}

		public void Close()
		{
			if (_isOpen)
			{
				_reader.Close();
				_isOpen = false;
			}
		}

		public Boolean EndOfStream
		{
			get { return _reader.EndOfStream; }
		}

		public void ReadNext(int countOfDocumentsToLoad, List<FIObject> vectors)
		{
			if (!_isOpen)
				Open();

			int nDocs;
			String currentLine;
			char[] sepSpace = { ' ' };
			char[] sepColon = { ':' };
			Boolean flag;

			flag = false;
			nDocs = 0;

			//_reader.BaseStream.Seek(_offset, SeekOrigin.Begin);

			while (!_reader.EndOfStream && !flag)
			{
				currentLine = _reader.ReadLine();
				_lineOffset++;
				if (!currentLine.StartsWith("#"))
				{
					Int32 featureCode = 0;
					Double featureWeight = 0;
					String[] parts = currentLine.Split(sepSpace);
					Int32 pos = 0;

					FIObject vector = new FIObject();

					foreach (string part in parts)
					{
						pos++;
						if (pos == 1)
						{
							vector.Label = Int32.Parse(parts[pos - 1]);
						}
						else if (pos == parts.Length)
						{
							if (parts[pos - 1] != "#")
							{
								vector.Code = Int32.Parse(parts[pos - 1]);
							}
						}
						else
						{
							String[] parts2 = part.Split(sepColon);
							if (parts2.Length == 2)
							{
								featureCode = Int32.Parse(parts2[0]);
								featureWeight = Double.Parse(parts2[1]);
								vector.AddFeature(featureCode, featureWeight);
							}
						}
					}
					vectors.Add(vector);
					nDocs++;
					flag = nDocs >= countOfDocumentsToLoad;
				}

				//Keep the file open to next readings
			}
		}
	}   
}
