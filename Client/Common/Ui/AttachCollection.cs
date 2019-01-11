using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;


namespace GazRouter.Common.Ui
{
	[ContentProperty("Children")]
	public class AttachCollection : FrameworkElement, IEnumerable<Attach>
	{
		private readonly ObservableCollection<Attach> _children = new ObservableCollection<Attach>();

		public ObservableCollection<Attach> Children
		{
			get { return _children; }
		}

		#region IEnumerable<Attach> Members

		public IEnumerator<Attach> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		#endregion
	}
}