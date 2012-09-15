using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProteinTagger
{
	public class ProteinEntry : ObservableObject
	{
		public string Accession
		{
			get { return _Accession; }
			set { if (_Accession != value) { _Accession = value; RaisePropertyChanged("Accession"); } }
		}
		string _Accession;

		public string ChainId
		{
			get { return _ChainId; }
			set { if (_ChainId != value) { _ChainId = value; RaisePropertyChanged("ChainId"); } }
		}
		string _ChainId;

		public int ChainIndex
		{
			get { return _ChainIndex; }
			set { if (_ChainIndex != value) { _ChainIndex = value; RaisePropertyChanged("ChainIndex"); } }
		}
		int _ChainIndex;

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
	}
}
