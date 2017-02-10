
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{
    class FeaturesR2PSM: AAIFSMeasure
    {
        double _proMembership=0;
        double _proNonMembership = 0;

        public FeaturesR2PSM()
        {
            _proMembership = 0;
            _proNonMembership = 0;
        }

        public override double GetSimilarity(Models.AAIFS P, Models.AAIFS Q)
        {
            double ret = 0;
            ret = Math.Min(_proMembership, _proNonMembership);
            
            return ret;
        }

        public double GetSimilarityProMembership(Models.AAIFS P, Models.AAIFS Q)
        {
            double ret = 0;
 
            foreach (int elem in P.Keys)
            {
                if(Q.ContainsKey(elem))
                {
                    int nPandQ = 0;
                    int nPnotQ = 0;
                    int nQnotP = 0;

                    if (P[elem].FeaturesProMembership.Count > 0)
                    {
                        foreach (int code in P[elem].FeaturesProMembership.Keys)
                        {
                            if (Q[elem].FeaturesProMembership.ContainsKey(code))
                            {
                                nPandQ++;
                            }
                            else
                            {
                                nPnotQ++;
                            }
                        }

                        nQnotP = Q[elem].FeaturesProMembership.Count - nPandQ;

                        ret = ret + (double)nPandQ / (double)(nPandQ + nPnotQ + nQnotP);
                    }
                    else
                    {
                        if (Q[elem].FeaturesProMembership.Count > 0)
                        {
                            ret = ret + 0;
                        }
                        else
                        {
                            ret = ret + 1;
                        }
                    }
                }
            }

            _proMembership = ret/P.Count;

            System.Diagnostics.Debug.Assert(!Double.IsNaN(_proMembership));
            return _proMembership;
        }

        public double GetSimilarityProNonMembership(Models.AAIFS P, Models.AAIFS Q)
        {
            double ret = 0;

            foreach (int elem in P.Keys)
            {
                if (Q.ContainsKey(elem))
                {
                    int nPandQ = 0;
                    int nPnotQ = 0;
                    int nQnotP = 0;
                    if (P[elem].FeaturesProNonMembership.Count > 0)
                    {

                        foreach (int code in P[elem].FeaturesProNonMembership.Keys)
                        {
                            if (Q[elem].FeaturesProNonMembership.ContainsKey(code))
                            {
                                nPandQ++;
                            }
                            else
                            {
                                nPnotQ++;
                            }
                        }

                        nQnotP = Q[elem].FeaturesProNonMembership.Count - nPandQ;

                        ret = ret + (double)nPandQ / (double)(nPandQ + nPnotQ + nQnotP);
                    }
                    else
                    {
                        if (Q[elem].FeaturesProNonMembership.Count > 0)
                        {
                            ret = ret + 0;
                        }
                        else
                        {
                            ret = ret + 1;
                        }
                    }
                }
            }

            _proNonMembership = ret / P.Count;
            System.Diagnostics.Debug.Assert(!Double.IsNaN(_proNonMembership));
            return _proNonMembership;
        }

    }
}
