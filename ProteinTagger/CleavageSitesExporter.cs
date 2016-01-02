using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biosek.Formats.UniProt;
using Biosek;
using System.IO;

namespace ProteinTagger
{
	public class CleavageSitesExporter
	{
		/// <summary>
		/// Export cleavage site information for each cleavage site detected.
		/// Information is written per cleavage site in a JSON file.
		/// </summary>
		/// <param name="vm">Main Application ViewModel</param>
		/// <param name="db">whole dataset</param>
		/// <param name="folderPath">Output folder path where JSON files will be written</param>
		public static void Export(MainViewModel vm, Dictionary<string, entry> db, string folderPath)
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
