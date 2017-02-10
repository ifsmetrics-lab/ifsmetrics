using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace IFSSimReporter.Entities
{
	[XmlRootAttribute("References", 
		IsNullable = false)]
	public class References:ICollection{
		private ArrayList _references = new ArrayList(); 

		public Reference this[int index]{
			get{return (Reference) _references[index];}
		}

		public void CopyTo(Array a, int index){
			_references.CopyTo(a, index);
		}
		public int Count{
			get{return _references.Count;}
		}
		public object SyncRoot{
			get{return this;}
		}
		public bool IsSynchronized{
			get{return false;}
		}
		public IEnumerator GetEnumerator(){
			return _references.GetEnumerator();
		}

		public void Add(Reference newScenario){
			_references.Add(newScenario);
		}
	}
}

