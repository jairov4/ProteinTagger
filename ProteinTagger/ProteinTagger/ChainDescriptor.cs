using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProteinTagger
{
	public class ChainDescriptor : ObservableObject
	{
		public string ChainId
		{
			get { return _ChainId; }
			set { if (_ChainId != value) { _ChainId = value; RaisePropertyChanged("ChainId"); } }
		}
		string _ChainId;

		public string Accession
		{
			get { return _Accession; }
			set { if (_Accession != value) { _Accession = value; RaisePropertyChanged("Accession"); } }
		}
		string _Accession;

		public int ChainIndex
		{
			get { return _ChainIndex; }
			set { if (_ChainIndex != value) { _ChainIndex = value; RaisePropertyChanged("ChainIndex"); } }
		}
		int _ChainIndex;

		public string Description
		{
			get { return _Description; }
			set { if (_Description != value) { _Description = value; RaisePropertyChanged("Description"); } }
		}
		string _Description;

		public string Tag
		{
			get { return _Tag; }
			set { if (_Tag != value) { _Tag = value; RaisePropertyChanged("Tag"); } }
		}
		string _Tag;

		public int LocationBegin
		{
			get { return _LocationBegin; }
			set { if (_LocationBegin != value) { _LocationBegin = value; RaisePropertyChanged("LocationBegin"); } }
		}
		int _LocationBegin;

		public int LocationEnd
		{
			get { return _LocationEnd; }
			set { if (_LocationEnd != value) { _LocationEnd = value; RaisePropertyChanged("LocationEnd"); } }
		}
		int _LocationEnd;

		public int Length
		{
			get { return _Length; }
			set { if (_Length != value) { _Length = value; RaisePropertyChanged("Length"); } }
		}
		int _Length;

		public string Lineage
		{
			get { return _Lineage; }
			set { if (_Lineage != value) { _Lineage = value; RaisePropertyChanged("Lineage"); } }
		}
		string _Lineage;
	}
}
