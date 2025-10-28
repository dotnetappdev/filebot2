using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RenameIt.Core;
using System.Collections.Generic;

namespace RenameIt
{
    public sealed partial class TemplatesDialog : ContentDialog
    {
        private readonly TemplateRepository _repository;

        public TemplatesDialog(TemplateRepository repository)
        {
            this.InitializeComponent();
            _repository = repository;
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            var templates = _repository.GetAll();
            TemplatesListView.ItemsSource = templates;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TemplateEditDialog(null);
            dialog.XamlRoot = this.XamlRoot;
            
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.Template != null)
            {
                _repository.Add(dialog.Template);
                LoadTemplates();
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TemplatesListView.SelectedItem is RenameTemplate template)
            {
                var dialog = new TemplateEditDialog(template);
                dialog.XamlRoot = this.XamlRoot;
                
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && dialog.Template != null)
                {
                    _repository.Update(dialog.Template);
                    LoadTemplates();
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TemplatesListView.SelectedItem is RenameTemplate template)
            {
                var confirmDialog = new ContentDialog
                {
                    Title = "Delete Template",
                    Content = $"Are you sure you want to delete the template '{template.Name}'?",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };

                var result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    _repository.Delete(template.Id);
                    LoadTemplates();
                }
            }
        }

        private void TemplatesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var hasSelection = TemplatesListView.SelectedItem != null;
            EditButton.IsEnabled = hasSelection;
            DeleteButton.IsEnabled = hasSelection;
        }
    }
}
