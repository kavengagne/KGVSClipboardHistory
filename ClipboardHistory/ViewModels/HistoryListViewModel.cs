using System;
using System.Windows.Input;
using ClipboardHistoryApp.Classes;
using ClipboardHistoryApp.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ClipboardHistoryApp.ViewModels
{
    public class HistoryListViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        private ClipboardService _clipboardService;
        #endregion Fields


        #region Properties
        public HistoryCollection HistoryCollection { get; }
        public HistoryConfiguration HistoryConfiguration { get; set; }
        
        public ICommand CopyToClipboardCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        #endregion Properties


        #region Constructors
        public HistoryListViewModel()
        {
            HistoryCollection = new HistoryCollection();
            HistoryConfiguration = new HistoryConfiguration(HistoryCollection);
            
            InitializeClipboardService();
            InitializeCommands();
        }
        #endregion Constructors


        #region Public Methods
        public void SetVisualStudioHandle(IntPtr mainWindowHandle)
        {
            _clipboardService.SetWindowHandle(mainWindowHandle);
        }
        #endregion Public Methods


        #region Commands
        private bool CanCopyToClipboard(ClipboardDataItem item)
        {
            return true;
        }

        private void CopyToClipboard(ClipboardDataItem item)
        {
            _clipboardService.SetClipboardText(item.Data, errorItem =>
            {
                HistoryCollection.AddItem(errorItem);
            });
        }

        private bool CanDeleteItem(ClipboardDataItem item)
        {
            return true;
        }

        private void DeleteItem(ClipboardDataItem item)
        {
            HistoryCollection.Remove(item);
        }
        #endregion Commands


        #region Private Methods
        private void InitializeClipboardService()
        {
            _clipboardService = new ClipboardService(OnClipboardUpdate);
            _clipboardService.EnableNotifications();
            AddStringToHistoryCollection(_clipboardService.GetClipboardDataItem());
        }

        private void InitializeCommands()
        {
            CopyToClipboardCommand = new RelayCommand<ClipboardDataItem>(CopyToClipboard, CanCopyToClipboard);
            DeleteItemCommand = new RelayCommand<ClipboardDataItem>(DeleteItem, CanDeleteItem);
        }

        private void OnClipboardUpdate(ClipboardDataItem item)
        {
            AddStringToHistoryCollection(item);
        }

        private void AddStringToHistoryCollection(ClipboardDataItem item)
        {
            if (!String.IsNullOrEmpty(item.Data))
            {
                HistoryCollection.AddItem(item);
            }
        }
        #endregion Private Methods


        #region Implement IDisposable Interface
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HistoryListViewModel()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_clipboardService != null)
                {
                    _clipboardService.Dispose();
                    _clipboardService = null;
                }
            }
        }
        #endregion
    }
}