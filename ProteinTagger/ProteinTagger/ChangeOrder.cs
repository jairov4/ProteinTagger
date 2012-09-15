using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProteinTagger
{
	public enum ChangeOrderType
	{
		TagByDescription, TagByChainId, TagByTag
	}

	public class ChangeOrder
	{
		public ChangeOrderType Type { get; set; }
		public bool ExcludeAlreadyTagged { get; set; }
		public string[] Parameters { get; set; }
		public string Tag { get; set; }
	}
}
