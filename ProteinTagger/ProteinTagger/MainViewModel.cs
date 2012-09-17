using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Biosek.Core;
using Biosek.Formats.UniProt;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace ProteinTagger
{
	public class MainViewModel : ObservableObject
	{
		/// <summary>
		/// Setup all for data loaded from UniProt
		/// </summary>
		/// <param name="db"></param>
		public void SetupFromUniProt(uniprot db)
		{
			Log("Building chain names from UniProt dataset");
			var proteinDB = from entry in db.entry
											let chains = entry.GetChains().ToArray()
											from chain in chains
											let chainIndex = Array.IndexOf(chains, chain)
											select new ChainDescriptor
											{
												ChainId = string.Format("{0}-{1}", entry.accession[0], chainIndex),
												Accession = entry.accession[0],
												ChainIndex = chainIndex,
												Description = chain.description,
												LocationBegin = chain.GetFirstLocation(),
												LocationEnd = chain.GetLastLocation(),
												Length = chain.GetFeatureLength(),
												Lineage = entry.organism.lineage.LastOrDefault()
											};
			ProteinDB = proteinDB.ToList();
		}

		/// <summary>
		/// Introduces a new change order and execute it
		/// </summary>		
		public void RecordTagByDescription(string tag, bool exclude)
		{
			var changeOrder = new ChangeOrder();
			changeOrder.Parameters = SelectedChainNames.Select(x => x.Name).ToArray();
			changeOrder.Tag = tag;
			changeOrder.Type = ChangeOrderType.TagByDescription;
			changeOrder.ExcludeAlreadyTagged = exclude;
			_ChangeOrders.Add(changeOrder);
			ExecuteChangeOrder(changeOrder);
		}

		public void LoadChangeOrdersFile(string filename)
		{
			try
			{
				JToken dataLoaded = (JToken)JsonSerializerHelper.LoadJsonFile(filename);
				if (dataLoaded.Type != JTokenType.Array)
				{
					throw new InvalidOperationException("Not has format of array of Change Orders");
				}
				var data = dataLoaded.Select(x => x.ToObject<ChangeOrder>());
				foreach (var item in data)
				{
					ExecuteChangeOrder(item);
					_ChangeOrders.Add(item);
				}
			}
			catch (Exception ex)
			{
				Log("Error loading Change Orders file: \"{0}\" | {1}", filename, ex.Message);
				throw;
			}
			Log("Change Orders file loaded: {0}", filename);
		}

		public void SaveChangeOrdersFile(string filename)
		{
			try
			{
				_ChangeOrders.SaveJsonFile(filename);
			}
			catch (Exception ex)
			{
				Log("Error saving Change Orders file: \"{0}\" | {1}", filename, ex.Message);
				throw;
			}
			Log("Change Orders file saved: {0}", filename);
		}

		public void ExportTags(string filename)
		{
			try
			{
				ProteinDB.SaveJsonFile(filename);
			}
			catch (Exception ex)
			{
				Log("Error exporting tags file: \"{0}\" | {1}", filename, ex.Message);
				throw;
			}
			Log("Tags file exported: {0}", filename);
		}

		public Action<string> LogFn;

		/// <summary>
		/// Extract and group chain names from all chains
		/// </summary>
		private void BuildChainNames()
		{
			ChainNames = ProteinDB
				.Where(x => string.IsNullOrWhiteSpace(x.Tag))
				.GroupBy(x => x.Description ?? string.Empty)
				.Select(x => new ChainNameGroup
				{
					Name = x.Key,
					Count = x.Count(),
					LengthMax = x.Max(y => y.Length),
					LengthMin = x.Min(y => y.Length),
					LengthAvg = x.Average(y => y.Length),
					Chains = x.AsEnumerable()
				});
			Log("{0} chain names found in {1} chains", ChainNames.Count(), ProteinDB.Count());
			Stats = ProteinDB
				.GroupBy(x => x.Tag)
				.Select(x => new 
				{ 
					Tag = x.Key, 
					Count = x.Count() 
				});
			OnSelectedChainNamesChanged();
		}

		/// <summary>
		/// Recompute chain name groups
		/// </summary>
		private void OnProteinDBChanged()
		{
			BuildChainNames();
		}

		/// <summary>
		/// Compute filtered change order from selected chain name group
		/// </summary>
		private void OnSelectedChainNamesChanged()
		{
			if (SelectedChainNames != null && SelectedChainNames.Count() > 0)
			{
				FilteredChainCollection = from c in SelectedChainNames from d in c.Chains select d;
				IsTagAllowed = true;
			}
			else
			{
				FilteredChainCollection = ProteinDB;
				IsTagAllowed = false;
			}
		}

		/// <summary>
		/// Execute the change order, assigning tags as required
		/// </summary>		
		void ExecuteChangeOrder(ChangeOrder order)
		{
			int affectedItems = 0;
			Log("Executing {0} | Excl:{1} Params:{2} Tag:{3}", order.Type, order.ExcludeAlreadyTagged, string.Join(", ", order.Parameters), order.Tag);
			if (order.Type == ChangeOrderType.TagByChainId)
			{
				foreach (var item in ProteinDB.Where(x => order.Parameters.Contains(x.ChainId)))
				{
					if (!string.IsNullOrWhiteSpace(order.Tag) || !order.ExcludeAlreadyTagged)
					{
						item.Tag = order.Tag;
						affectedItems++;
					}
				}
			}
			else if (order.Type == ChangeOrderType.TagByDescription)
			{
				foreach (var item in ProteinDB.Where(x => order.Parameters.Contains(x.Description)))
				{
					if (!string.IsNullOrWhiteSpace(order.Tag) || !order.ExcludeAlreadyTagged)
					{
						item.Tag = order.Tag;
						affectedItems++;
					}
				}
			}
			else if (order.Type == ChangeOrderType.TagByTag)
			{
				if (order.ExcludeAlreadyTagged) throw new InvalidOperationException("Change Order TagByTag can not exclude already tagged chains");
				foreach (var item in ProteinDB.Where(x => order.Parameters.Contains(x.Description)))
				{
					item.Tag = order.Tag;
					affectedItems++;
				}
			}
			else
			{
				throw new NotImplementedException("Change Order not implemented");
			}
			Log("Afected {0} chains", affectedItems);
			BuildChainNames();
		}

		/// <summary>
		/// Append text to log
		/// </summary>		
		void Log(string s, params object[] p)
		{
			if (LogFn != null)
			{
				var str = string.Format(s, p);
				LogFn(str);
			}
		}

		/// <summary>
		/// Change Orders
		/// </summary>
		public IEnumerable<ChangeOrder> ChangeOrders
		{
			get { return _ChangeOrders; }
		}
		ObservableCollection<ChangeOrder> _ChangeOrders = new ObservableCollection<ChangeOrder>();

		/// <summary>
		/// Protein database
		/// </summary>
		public IEnumerable<ChainDescriptor> ProteinDB
		{
			get { return _ProteinDB ?? Enumerable.Empty<ChainDescriptor>(); }
			private set { if (_ProteinDB != value) { _ProteinDB = value; OnProteinDBChanged(); RaisePropertyChanged("ProteinDB"); } }
		}
		IEnumerable<ChainDescriptor> _ProteinDB;

		/// <summary>
		/// Filtered View of proteins
		/// </summary>
		public IEnumerable<ChainDescriptor> FilteredChainCollection
		{
			get { return _FilteredChainCollection; }
			set { if (_FilteredChainCollection != value) { _FilteredChainCollection = value; RaisePropertyChanged("FilteredChainCollection"); } }
		}
		IEnumerable<ChainDescriptor> _FilteredChainCollection;

		/// <summary>
		/// All protein names except tagged
		/// </summary>
		public IEnumerable<ChainNameGroup> ChainNames
		{
			get { return _ChainNames; }
			set { if (_ChainNames != value) { _ChainNames = value; RaisePropertyChanged("ChainNames"); } }
		}
		IEnumerable<ChainNameGroup> _ChainNames;

		/// <summary>
		/// Selected protein names to unify by tag
		/// </summary>
		public IEnumerable<ChainNameGroup> SelectedChainNames
		{
			get { return _SelectedChainNames; }
			set { if (_SelectedChainNames != value) { _SelectedChainNames = value; RaisePropertyChanged("SelectedChainNames"); OnSelectedChainNamesChanged(); } }
		}
		IEnumerable<ChainNameGroup> _SelectedChainNames;

		/// <summary>
		/// Indicates if there are chain names selected
		/// </summary>
		public bool IsTagAllowed
		{
			get { return _IsTagAllowed; }
			private set { if (_IsTagAllowed != value) { _IsTagAllowed = value; RaisePropertyChanged("IsTagAllowed"); } }
		}
		bool _IsTagAllowed;

		/// <summary>
		/// Tags available
		/// </summary>
		public List<string> Tags
		{
			get { return _Tags ?? (Tags = new List<string>()); }
			set { if (_Tags != value) { _Tags = value; RaisePropertyChanged("Tags"); } }
		}
		List<string> _Tags;
				
		public object Stats
		{
			get { return _Stats; }
			set { if (_Stats != value) { _Stats = value; RaisePropertyChanged("Stats"); } }
		}
		object _Stats;
	}
}
