using Microsoft.UI.Xaml.Controls;
using RenameIt.Core;
using System;

namespace RenameIt
{
    public sealed partial class TemplateEditDialog : ContentDialog
    {
        public RenameTemplate? Template { get; private set; }
        private readonly bool _isEdit;

        public TemplateEditDialog(RenameTemplate? template)
        {
            this.InitializeComponent();
            
            _isEdit = template != null;
            if (_isEdit && template != null)
            {
                NameTextBox.Text = template.Name;
                PatternTextBox.Text = template.Pattern;
                DescriptionTextBox.Text = template.Description;
                Template = template;
                Title = "Edit Template";
            }
            else
            {
                Template = new RenameTemplate();
                Title = "Add Template";
            }

            ValidateInput(null, null);
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (Template != null)
            {
                Template.Name = NameTextBox.Text.Trim();
                Template.Pattern = PatternTextBox.Text.Trim();
                Template.Description = DescriptionTextBox.Text.Trim();
            }
        }

        private void ValidateInput(object? sender, object? e)
        {
            var isValid = !string.IsNullOrWhiteSpace(NameTextBox.Text) && 
                          !string.IsNullOrWhiteSpace(PatternTextBox.Text);
            IsPrimaryButtonEnabled = isValid;
        }
    }
}
