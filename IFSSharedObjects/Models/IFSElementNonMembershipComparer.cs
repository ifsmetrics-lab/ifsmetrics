using System;
using System.Collections.Generic;

namespace IFSSharedObjects.Models
{
	public class IFSElementNonMembershipComparer: IComparer<IFSElement>
	{

		public int Compare(IFSElement s1, IFSElement s2)
		{
			int ret = 0;

			if (s1.Nonmembership > s2.Nonmembership)
			{
				ret = 1;
			}
			else if (s1.Nonmembership == s2.Nonmembership)
			{
				if (s1.Hesitation < s2.Nonmembership)
				{
					ret = 1;
				}
				else if (s1.Hesitation == s2.Hesitation)
				{
					ret = 0;
				}
				else
				{
					ret = -1;
				}
			}
			else
			{
				ret = -1;
			}

			return ret;
		}
	}
}

