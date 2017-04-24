using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IFSSharedObjects.Models;

namespace IFSComparer.Measures
{
	/// <summary>
	/// This measure was proposed in:
	/// 
	/// 	Loor, M., De Tré, G.: In a Quest for Suitable Similarity Measures to Compare Experience-Based Evaluations. 
	/// 	In: Computational Intelligence: International Joint Conference, IJCCI 2015 Lisbon, Portugal, November 12-14, 2015, Revised Selected Papers 
	/// 	pp. 291-314 (2017)
	/// 
	/// 	DOI: 10.1007/978-3-319-48506-5_15
	/// 
	/// </summary>
    public class AgreementOnDecisionSM: IFSMeasure
    {
		const String _CITEKEY = "Loor2017";
		const String _CITE = "Loor, M., De Tr{\\'e}, G.: In a Quest for Suitable Similarity Measures to Compare Experience-Based Evaluations. In: Computational Intelligence: International Joint Conference, IJCCI 2015 Lisbon, Portugal, November 12-14, 2015, Revised Selected Papers pp. 291-314 (2017)";


		public AgreementOnDecisionSM(){
			base.CiteKey = _CITEKEY;
			base.Cite = _CITE;	
		}
        

        public override double GetSimilarity(IFS P, IFS Q)
        {
            
            double ret = 0;
            double sum = 0;

            int n = Math.Max(P.Count, Q.Count);
            IFSElement q;

            //First, consider the IFSElements in P & Q and P - Q
            foreach (IFSElement p in P.Values)
            {
                if (Q.ContainsKey(p.ElementId)) // P & Q
                {
                    q = Q[p.ElementId];
                    if ((p.Membership > p.Nonmembership && q.Membership > q.Nonmembership) ||
                        (p.Membership <= p.Nonmembership && q.Membership <= q.Nonmembership))
                    {
                        sum = sum + 1;
                    }
                }
               
                
            }

			IFSElement p2;
			//Then, the IFSElements in Q - P
			foreach (IFSElement q2 in Q.Values)
			{
				if (!P.ContainsKey(q2.ElementId))
				{
					p2 = new IFSElement();
					if ((p2.Membership > p2.Nonmembership && q2.Membership > q2.Nonmembership) ||
						(p2.Membership <= p2.Nonmembership && q2.Membership <= q2.Nonmembership))
					{
						sum = sum + 1;
					}
				}
			}


            ret =  ((double)sum)/ n;

            return ret;
        }

       
    }
    
}
