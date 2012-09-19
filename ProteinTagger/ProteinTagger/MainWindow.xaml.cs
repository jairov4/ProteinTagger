using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Biosek.Formats.UniProt;
using System.Diagnostics;

namespace ProteinTagger
{
	/// <summary>
	/// Lógica de interacción para MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ViewModel.LogFn = x => { txtLog.AppendText(x); txtLog.AppendText("\n"); };
			foreach (var i in new[] { "P1", "HC", "P3", "6K1", "CI", "6K2", "VPg", "NIa", "NIb", "CP" })
			{
				ViewModel.Tags.Add(i);
			}
			DataContext = ViewModel;
		}

		MainViewModel ViewModel = new MainViewModel();
		Dictionary<string, entry> uniprotDB = new Dictionary<string, entry>();

		private void dgChainNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ViewModel.SelectedChainNames = dgChainNames.SelectedItems.Cast<ChainNameGroup>();
		}

		private void btnLoadUniProt_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Title = "Load UniProt XML dataset";
			dialog.Filter = "UniProt XML File (*.xml)|*.xml|All files (*.*)|*.*";
			if (dialog.ShowDialog() == true)
			{
				var db = uniprot.LoadFromFile(dialog.FileName);
				ViewModel.SetupFromUniProt(db);
				MakeUniprotDictionary(db);
			}
		}

		private void MakeUniprotDictionary(uniprot db)
		{
			uniprotDB.Clear();
			foreach (var item in db.entry)
			{
				foreach (var accession in item.accession)
				{
					uniprotDB.Add(accession, item);
				}
			}
		}

		private void btnLoadChangeOrders_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Title = "Load Change Orders";
			dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
			if (dialog.ShowDialog() == true)
			{
				ViewModel.LoadChangeOrdersFile(dialog.FileName);
			}
		}

		private void btnSaveChangeOrders_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Save Change Orders";
			dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
			if (dialog.ShowDialog() == true)
			{
				ViewModel.SaveChangeOrdersFile(dialog.FileName);
			}
		}

		private void btnExportTagged_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Title = "Export tags file";
			dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
			if (dialog.ShowDialog() == true)
			{
				ViewModel.ExportTags(dialog.FileName);
			}
		}

		private void lblPresetItem_Click(object sender, MouseButtonEventArgs e)
		{
			var str = sender as ContentControl;
			var tag = str.Content as string;
			ViewModel.RecordTagByDescription(tag, true);
		}

		private void btnExportCleavageSites_Click(object sender, RoutedEventArgs e)
		{
			if (uniprotDB.Count == 0)
			{
				MessageBox.Show("UniProt Dataset has not been loaded");
				return;
			}
			var dlg = new WPFFolderBrowser.WPFFolderBrowserDialog();
			dlg.Title = "Export cleavage sites to folder";
			dlg.ShowDialog();
			CleavageSitesExporter.Export(ViewModel, uniprotDB, dlg.FileName, v => pbrExportCleavageSites.Value = v);
			pbrExportCleavageSites.Value = 0;
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}

		private void btnRemoveTag_Click(object sender, RoutedEventArgs e)
		{
			var mi = e.OriginalSource as MenuItem;
			if (mi == null) return;
			ViewModel.Tags.Remove(mi.Header as string);
		}

		private void btnAddTag_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtNewTag.Text))
			{
				MessageBox.Show("New tag can not be empty");
			}
			else
			{
				ViewModel.Tags.Add(txtNewTag.Text);
				txtNewTag.Text = string.Empty;
			}
		}

		private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.Tags.Clear();
		}
	}
}
