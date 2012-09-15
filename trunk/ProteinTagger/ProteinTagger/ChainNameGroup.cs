using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProteinTagger
{
	public class ChainNameGroup
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public int LengthMax { get; set; }
		public int LengthMin { get; set; }
		public double LengthAvg { get; set; }
		public IEnumerable<ChainDescriptor> Chains { get; set; }
	}
}
