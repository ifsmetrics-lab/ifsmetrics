using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Collections;

namespace IFSBuilder.IO
{
	class DocumentCategoryRelationsReader
	{
		private String _path;
		private StreamReader _reader;
		private bool _isOpen = false;

		public DocumentCategoryRelationsReader(String path)
		{
			_path = path;

		}

		public void Open()
		{
			if(!_isOpen)
				_reader = new StreamReader(_path);
			_isOpen = true;
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

		public void Read(DocumentCategoryRelations relations)
		{
			String currentLine;
			char[] seps = { ' ' };

			StreamReader reader = new StreamReader(_path);
			try
			{
				while (!reader.EndOfStream)
				{
					currentLine = reader.ReadLine();
					String[] tokens = currentLine.Split(seps);
					if (tokens.Length == 3)
					{
						int doc = Int32.Parse(tokens[1]);
						String topic = tokens[0];

						if (relations.ContainsKey(doc))
						{
							relations[doc].Add(topic);
						}
						else
						{
							relations.Add(doc, new List<string>());
							if (!relations[doc].Contains(topic))
							{
								relations[doc].Add(topic);
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
