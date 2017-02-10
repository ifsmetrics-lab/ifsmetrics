using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace IFSBuilder.Params
{
	[XmlRootAttribute("Categories", 
		IsNullable = false)]
	public class Categories:ICollection{
		private ArrayList _categories = new ArrayList(); 

		public Category this[int index]{
			get{return (Category) _categories[index];}
		}

		public void CopyTo(Array a, int index){
			_categories.CopyTo(a, index);
		}
		public int Count{
			get{return _categories.Count;}
		}
		public object SyncRoot{
			get{return this;}
		}
		public bool IsSynchronized{
			get{return false;}
		}
		public IEnumerator GetEnumerator(){
			return _categories.GetEnumerator();
		}

		public void Add(Category newCat){
			_categories.Add(newCat);
		}
	}
}

