using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ProteinTagger
{
	public class ObservableObject : INotifyPropertyChanged
	{
		#region Miembros de INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChanged(string prop)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		#endregion
	}
}
