using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSSharedObjects.Models;


namespace IFSComparer.Measures
{
	public abstract class IFSMeasure
    {
        public abstract Double GetSimilarity(IFS P, IFS Q);

		private String _code = "";
		public String Code
		{
			get{ return _code;}
			set{ _code = value; }
		}

		private String _citeKey = "";
		public String CiteKey
		{
			get{ return _citeKey;}
			set{ _citeKey = value; }
		}

		private String _cite = "";
		public String Cite
		{
			get { return _cite; }
			set { _cite = value; }
		}

		public IFSMeasure(){
			_code = "";
			_cite = "";
			_citeKey = "";
		}

		public IFSMeasure(String code){
			_code = code;
			_cite = "";
			_citeKey = "";
		}

		public IFSMeasure(String code, String citeKey, String cite){
			_code = code;
			_cite = cite;
			_citeKey = citeKey;
		}

    }
}
