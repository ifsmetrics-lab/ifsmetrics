using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSComparator.Measures
{
    class FeaturesRatioPSM: AAIFSMeasure
    {
        double _proMembership=0;
        double _proNonMembership = 0;

        public FeaturesRatioPSM()
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

            int nPandQ = 0;
            int nPnotQ = 0;
            int nQnotP = 0;

            foreach (int code in P.ProMemembershipFeatures)
            {
                if (Q.ProMemembershipFeatures.Contains(code))
                {
                    nPandQ++;
                }
                else
                {
                    nPnotQ++;
                }
            }

            nQnotP = Q.ProMemembershipFeatures.Count - nPandQ;

            //ret = (double)nPandQ / (double)(nPandQ + nPnotQ + nQnotP);
            ret = (double)nPandQ / (double)(nPandQ + nPnotQ); //ProM3

            _proMembership = ret;
            return ret;
        }

        public double GetSimilarityProNonMembership(Models.AAIFS P, Models.AAIFS Q)
        {
            double ret = 0;

            int nPandQ = 0;
            int nPnotQ = 0;
            int nQnotP = 0;

            foreach (int code in P.ProNonMemembershipFeatures)
            {
                if (Q.ProNonMemembershipFeatures.Contains(code))
                {
                    nPandQ++;
                }
                else
                {
                    nPnotQ++;
                }
            }

            nQnotP = Q.ProNonMemembershipFeatures.Count - nPandQ;

            //ret = (double)nPandQ / (double)(nPandQ + nPnotQ + nQnotP); //ProM
            ret = (double)nPandQ / (double)(nPandQ + nPnotQ); //ProM3
            _proNonMembership = ret;
            return ret;
        }

    }
}
