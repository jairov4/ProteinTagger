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
			ViewModel = new MainViewModel();
			ViewModel.LogFn = x => { txtLog.AppendText(x); txtLog.AppendText("\n"); };
			ViewModel.Tags.AddRange(new[] { "P1", "HC", "P3", "6K1", "CI", "6K2", "VPg", "NIa", "NIb", "CP" });
			DataContext = ViewModel;
		}

		MainViewModel ViewModel;
		uniprot db;

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
				db = uniprot.LoadFromFile(dialog.FileName);
				ViewModel.SetupFromUniProt(db);
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
			if (db == null)
			{
				MessageBox.Show("UniProt Dataset has not been loaded");
				return;
			}
			var dlg = new WPFFolderBrowser.WPFFolderBrowserDialog();
			dlg.Title = "Export cleavage sites to folder";
			dlg.ShowDialog();
			CleavageSitesExporter.Export(ViewModel, db, dlg.FileName, v => pbrExportCleavageSites.Value = v);
			pbrExportCleavageSites.Value = 0;
		}
	}
}
