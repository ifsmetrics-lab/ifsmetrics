using System;

namespace IFSSharedObjects.Models
{
	public class IFSElement
	{
		Int32 _elementId;
		Double _membership;
		Double _nonmembership;


		public Int32 ElementId
		{
			get { return _elementId; }
			set { _elementId = value; } 
		}

		public Double Membership
		{
			get { return _membership; }
			set { _membership = value; }
		}
		public Double Nonmembership
		{
			get { return _nonmembership; }
			set { _nonmembership = value; }
		}
		public Double Hesitation
		{
			get { return 1 - _membership - _nonmembership; }
		}

		public IFSElement()
		{
			_membership = 0;
			_nonmembership = 0;
			_elementId = -1;
		}

		public override string ToString()
		{

			return String.Format("<{3}, {0:F6}, {1:F6}, {2:F6}>", _membership, _nonmembership, Hesitation, _elementId);
		}
	}
}

