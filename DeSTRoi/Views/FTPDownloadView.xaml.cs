// DeSTRoi.Views.FTPDownloadView
using DeSTRoi.Models;
using DeSTRoi.ViewModels;
using DeSTRoi.Views;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using WPFLocalizeExtension.Extensions;

namespace DeSTRoi.Views
{

  public partial class FTPDownloadView : Window, IComponentConnector
  {
    private GridViewColumnHeader _CurSortCol;

    private SortAdorner _CurAdorner;


    public FTPDownloadView()
    {

      InitializeComponent();
      ((FTPDownloadViewModel)base.DataContext).RequestClose += FTPDownloadView_RequestClose;
    }

    private void FTPDownloadView_RequestClose(object sender, EventArgs e)
    {
      Close();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      ((FTPDownloadViewModel)base.DataContext).Dispose();
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      FTPDownloadViewModel fTPDownloadViewModel;
      fTPDownloadViewModel = (base.DataContext as FTPDownloadViewModel);
      foreach (FTPFileModel addedItem in e.AddedItems)
      {
        fTPDownloadViewModel.SelectedFTPFiles.Add(addedItem);
      }
      foreach (FTPFileModel removedItem in e.RemovedItems)
      {
        fTPDownloadViewModel.SelectedFTPFiles.Remove(removedItem);
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      FTPDownloadViewModel fTPDownloadViewModel;
      fTPDownloadViewModel = (base.DataContext as FTPDownloadViewModel);
      fTPDownloadViewModel.Owner = this;
    }

    private void SortClick(object sender, RoutedEventArgs e)
    {
      GridViewColumnHeader gridViewColumnHeader;
      gridViewColumnHeader = (sender as GridViewColumnHeader);
      string propertyName;
      propertyName = (gridViewColumnHeader.Tag as string);
      if (_CurSortCol != null)
      {
        AdornerLayer.GetAdornerLayer(_CurSortCol).Remove(_CurAdorner);
        lvDlFiles.Items.SortDescriptions.Clear();
      }
      ListSortDirection listSortDirection;
      listSortDirection = ListSortDirection.Ascending;
      if (_CurSortCol == gridViewColumnHeader && _CurAdorner.Direction == listSortDirection)
      {
        listSortDirection = ListSortDirection.Descending;
      }
      _CurSortCol = gridViewColumnHeader;
      _CurAdorner = new SortAdorner(_CurSortCol, listSortDirection);
      AdornerLayer.GetAdornerLayer(_CurSortCol).Add(_CurAdorner);
      lvDlFiles.Items.SortDescriptions.Add(new SortDescription(propertyName, listSortDirection));
    }


  }
}
