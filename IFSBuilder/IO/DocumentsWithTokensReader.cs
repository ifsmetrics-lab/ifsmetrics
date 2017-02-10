using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IFSBuilder.Collections;
using IFSBuilder.Entities;

namespace IFSBuilder.IO
{
	class DocumentsWithTokensReader
	{
		private String _path;
		private StreamReader _reader;
		private bool _isOpen = false;
		private int _lineOffset = 0;

		public DocumentsWithTokensReader(String path)
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

		public void ReadNext(int countOfDocumentsToLoad, Documents docs)
		{
			if (!_isOpen)
				Open();

			int nDocs;
			String currentLine;
			char[] seps = { ' ' };
			Document currentDocument = new Document(0);
			Boolean flag;

			flag = false;
			nDocs = 0;

			//_reader.BaseStream.Seek(_offset, SeekOrigin.Begin);

			while (!_reader.EndOfStream && !flag)
			{
				currentLine = _reader.ReadLine();
				_lineOffset++;
				//_offset = _reader.BaseStream.Position;

				String[] tokens = currentLine.Split(seps);
				if (tokens.Length > 0)
				{
					if (tokens[0].CompareTo(".I") == 0)
					{

						//new document
						int key = Int32.Parse(tokens[1]);
						currentDocument = new Document(key);
						docs.Add(key, currentDocument);
						nDocs++;

					}
					else if (tokens[0].CompareTo(".W") == 0)
					{
						//next line has tokens!
					}
					else if (tokens[0].TrimEnd().Length > 0)
					{
						foreach (String token in tokens)
						{
							currentDocument.AddCountToken(token);
						}

					}
					else
					{
						//end of document
						flag = nDocs >= countOfDocumentsToLoad;
					}

				}
				else
				{
					flag = nDocs >= countOfDocumentsToLoad;
				}
			}

			//Keep the file open to next readings

		}

	}
}
