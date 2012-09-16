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
		public static void Export(MainViewModel vm, uniprot db, string folderPath)
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
										 Tags = string.Format("{0}|{1}", c.Tag, d.Tag),
										 c.ChainIndex
									 };

			var pairs1 = from i in pairs0
									 from j in db.entry
									 where j.accession.Contains(i.Accession)
									 select new
									 {
										 Site = i.Tags,
										 LengthChain1 = j.GetChain(i.ChainIndex).GetFeatureLength(),
										 Chain1 = j.GetChainSequence(i.ChainIndex),
										 Chain2 = j.GetChainSequence(i.ChainIndex + 1)
									 };
			var p = from c in pairs1 group c by c.Site;
			foreach (var item in p)
			{
				var fn = string.Format("{0}.json", item.Key);
				fn = Path.Combine(folderPath, fn);
				item.ToArray().SaveJsonFile(fn);
			}
		}
	}
}
