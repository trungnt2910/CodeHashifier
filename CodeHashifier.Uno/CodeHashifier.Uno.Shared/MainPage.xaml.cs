using CodeHashifier.Uno.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CodeHashifier.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HashItButton_Click(object sender, RoutedEventArgs e)
        {
            CodeOutput.Text = CodeHashifierEngine.Default.Hash(CodeInput.Text);
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            var package = new DataPackage();
            package.RequestedOperation = DataPackageOperation.Copy;
            package.SetText(CodeOutput.Text.Trim());
            Clipboard.SetContent(package);
        }

        private async void FileMenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".cpp");
            picker.FileTypeFilter.Add(".c");
            picker.FileTypeFilter.Add(".cc");
            picker.FileTypeFilter.Add(".cxx");
            picker.FileTypeFilter.Add(".c++");
            picker.FileTypeFilter.Add(".cp");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var inputStream = await file.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    CodeInput.Text = streamReader.ReadToEnd();
                }
            }
        }

        private async void FileMenuSave_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("C/C++ source files", new List<string>() { ".cpp", ".c", ".cc", ".cxx", ".c++", ".cp" });
            savePicker.SuggestedFileName = "Hacked";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                await Windows.Storage.FileIO.WriteTextAsync(file, CodeOutput.Text);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            await aboutDialog.ShowAsync();
        }
    }
}
