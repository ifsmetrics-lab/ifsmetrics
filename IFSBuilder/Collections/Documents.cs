using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSBuilder.Entities;

namespace IFSBuilder.Collections
{
	class Documents: System.Collections.Generic.Dictionary<int, Document>
	{

		public void Load(IO.DocumentsWithTokensReader reader, int nDocs)
		{
			reader.ReadNext(nDocs, this);
		}

		public void LoadTaggedCategories(Collections.DocumentCategoryRelations relations)
		{

			foreach(Document doc in this.Values){
				doc.AddTaggedCategories(relations);
			}
		}
			 

	}
}
