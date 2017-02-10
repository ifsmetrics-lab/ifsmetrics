using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSBuilder.Entities;
using IFSBuilder.IO;

namespace IFSBuilder.Collections
{
	class DocumentCategoryRelations : System.Collections.Generic.Dictionary<int /*docCode*/, System.Collections.Generic.List<String> /*tagged topics*/>
	{
		public void LoadTaggedCategories(String path)
		{
			DocumentCategoryRelationsReader reader = new DocumentCategoryRelationsReader(path);
			reader.Read(this);
		}
	}
}

