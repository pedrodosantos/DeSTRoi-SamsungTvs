// DeSTRoi.Views.SortAdorner
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System;
namespace DeSTRoi.Views
{
	public class SortAdorner : Adorner
	{
		private static readonly Geometry _AscGeometry = Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

		private static readonly Geometry _DescGeometry = Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

		public ListSortDirection Direction
		{
			get;
			private set;
		}

		public SortAdorner(UIElement element, ListSortDirection dir)
			: base(element)
		{
			Direction = dir;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (!(base.AdornedElement.RenderSize.Width < 20.0))
			{
				drawingContext.PushTransform(new TranslateTransform(base.AdornedElement.RenderSize.Width - 15.0, (base.AdornedElement.RenderSize.Height - 5.0) / 2.0));
				drawingContext.DrawGeometry(Brushes.Black, null, (Direction == ListSortDirection.Ascending) ? _AscGeometry : _DescGeometry);
				drawingContext.Pop();
			}
		}
	}
}