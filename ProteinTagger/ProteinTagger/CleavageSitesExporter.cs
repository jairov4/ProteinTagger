using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biosek.Formats.UniProt;
using Biosek.Core;
using System.IO;

namespace ProteinTagger
{
	public class CleavageSitesExporter
	{
		public static void Export(MainViewModel vm, Dictionary<string, entry> db, string folderPath, Action<int> r)
		{
			var pairs0 = from c in vm.ProteinDB
									 from d in vm.ProteinDB
									 where
										 c.Accession == d.Accession &&
										 c.ChainIndex == d.ChainIndex - 1 &&
										 !string.IsNullOrWhiteSpace(c.Tag) &&
										 !string.IsNullOrWhiteSpace(d.Tag)
									 select new
									 {
										 c.Accession,
										 Tag1 = c.Tag,
										 Tag2 = d.Tag,
										 ChainIndex1 = c.ChainIndex,
										 ChainIndex2 = d.ChainIndex,
										 LocationBegin = c.LocationEnd,
										 LocationEnd = d.LocationBegin
									 };

			var p = from c in pairs0 group c by string.Concat(c.Tag1, "_", c.Tag2);
			foreach (var item in p)
			{
				var fn = string.Format("{0}.json", item.Key);
				fn = Path.Combine(folderPath, fn);
				item.ToArray().SaveJsonFile(fn);
			}
		}
	}
}
