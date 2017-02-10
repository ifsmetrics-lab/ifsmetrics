
using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace IFSBuilder.Params
{
	[XmlRootAttribute("Sources", 
		IsNullable = false)]
	public class Sources:ICollection{
		private ArrayList _sources = new ArrayList(); 

		public Source this[int index]{
			get{return (Source) _sources[index];}
		}

		public void CopyTo(Array a, int index){
			_sources.CopyTo(a, index);
		}
		public int Count{
			get{return _sources.Count;}
		}
		public object SyncRoot{
			get{return this;}
		}
		public bool IsSynchronized{
			get{return false;}
		}
		public IEnumerator GetEnumerator(){
			return _sources.GetEnumerator();
		}

		public void Add(Source newScenario){
			_sources.Add(newScenario);
		}
	}
}

