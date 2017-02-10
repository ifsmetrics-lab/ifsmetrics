using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFSSharedObjects.Models
{
	public class IFSElementMembershipComparer: IComparer<IFSElement>
	{
		public int Compare(IFSElement s1, IFSElement s2)
		{
			int ret = 0;

			if (s1.Membership > s2.Membership)
			{
				ret = 1;
			}
			else if (s1.Membership == s2.Membership)
			{
				if (s1.Hesitation > s2.Hesitation)
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
