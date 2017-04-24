using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace IFSComparer.Params
{
	[XmlRootAttribute("Scenarios", 
		IsNullable = false)]
	public class Scenarios:ICollection{
		private ArrayList _scenarios = new ArrayList(); 

		public Scenario this[int index]{
			get{return (Scenario) _scenarios[index];}
		}

		public void CopyTo(Array a, int index){
			_scenarios.CopyTo(a, index);
		}
		public int Count{
			get{return _scenarios.Count;}
		}
		public object SyncRoot{
			get{return this;}
		}
		public bool IsSynchronized{
			get{return false;}
		}
		public IEnumerator GetEnumerator(){
			return _scenarios.GetEnumerator();
		}

		public void Add(Scenario newScenario){
			_scenarios.Add(newScenario);
		}
	}
}

