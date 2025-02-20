using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace TalentLink_app
{
    public partial class ResumeParsingPage : ContentPage
    {
        public ResumeParsingPage()
        {
            InitializeComponent();
        }

        private async void OnSelectResumeClicked(object sender, EventArgs e)
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf
            });

            if (fileResult != null)
            {
                ResumeText.Text = "Parsing Resume: " + fileResult.FileName;
                await Task.Delay(2000); // Simulating parsing process
                ResumeText.Text = "Resume Parsed Successfully!";
            }
        }
    }
}
