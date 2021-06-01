using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbientLight.Strip
{
	class VirtualStrip
	{
		public static List<VirtualStrip> strips = new List<VirtualStrip>();

		public List<StripPart> parts = new List<StripPart>();
		
		public VirtualStrip(StripPart[] stripParts)
		{
			parts = stripParts.ToList();
			strips.Add(this);
		}
	}
}
