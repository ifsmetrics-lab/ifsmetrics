using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSSharedObjects.Models;


namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in
	/// 
	/// 	Loor, M., De Tré, G.: Vector based similarity measure for intuitionistic fuzzy sets. 
	/// 	In: Atanassov, K.T., Baczyn ́ski, M., Drewniak, J., Kacprzyk,J., Krawczak, M., Szmidt, E., Wygralak, M., Zadrożny, S. (eds.) 
	/// 	Modern Approaches in Fuzzy Sets, Intuitionistic Fuzzy Sets, Generalized Nets and Related Topics: Volume I: Foundations, 
	/// 	SRI-PAS, pp. 105–127 (2014)
	/// 
	/// </summary>
    public class VBSM: IFSMeasure
    {
		const String _CITEKEY = "Loor2013";
		const String _CITE = "M.~Loor and G.~De~Tr{\\'e}, ``{{Vector Based Similarity Measure for Intuitionistic Fuzzy Sets}},'' in \\emph{{{Modern Approaches in Fuzzy Sets, Intuitionistic Fuzzy Sets, Generalized Nets and Related Topics : Volume I: Foundations}}}, K.~T. Atanassov, M.~Baczy{\\'n}ski, J.~Drewniak, J.~Kacprzyk, M.~Krawczak, E.~Szmidt, M.~Wygralak, and S.~Zadro{\\.z}ny, Eds. SRI-PAS, 2014, pp. 105--127.";

        double _alpha = 0.5;

		public double Alpha{
			get { return _alpha;}
			set { _alpha = value; }
		}

		public VBSM()
		{
			_alpha = 0.5;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
		}

        public VBSM(double alpha)
        {
            _alpha = alpha;
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;
        }

        public override double GetSimilarity(IFS P, IFS Q)
        {
            double ret =0;            
            double sum = 0;

            int n = Math.Max(P.Count, Q.Count);
            IFSElement q;

            //First, consider the IFSElements in P & Q and P - Q
            foreach (IFSElement p in P.Values)
            {
                if (Q.ContainsKey(p.ElementId)) // P & Q
                {
                    q = Q[p.ElementId];
                }
                else //P - Q
                {
                    q = new IFSElement();
                }
                sum = sum + Math.Abs((p.Membership - q.Membership) + _alpha * (p.Hesitation - q.Hesitation));
            }

            IFSElement p2;
            //Then, the IFSElements in Q - P
            foreach (IFSElement q2 in Q.Values)
            {
                if (!P.ContainsKey(q2.ElementId))
                {
                    p2 = new IFSElement();
                    sum = sum + Math.Abs((p2.Membership - q2.Membership) + _alpha * (p2.Hesitation - q2.Hesitation));
                }
               
            }

            ret = 1 - sum / n;

            return ret;
        }
    }
}
