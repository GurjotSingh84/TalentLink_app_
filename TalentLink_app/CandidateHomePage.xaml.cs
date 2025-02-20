using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace TalentLink_app
{
    public partial class CandidateHomePage : ContentPage
    {
        public ObservableCollection<DashboardItem> DashboardItems { get; set; }

        public CandidateHomePage()
        {
            InitializeComponent();

            DashboardItems = new ObservableCollection<DashboardItem>
            {
                new DashboardItem { Title = "Search Jobs", Icon = "https://th.bing.com/th/id/OIP.t1NpyhaiL4gouIOOsF6z0AHaHa?w=195&h=195&c=7&r=0&o=5&dpr=1.3&pid=1.7", PageType = typeof(SearchJobsPage) },
                new DashboardItem { Title = "Jobs I've Applied To", Icon = "https://th.bing.com/th/id/R.2f2ac23ea27080a280428bd5b80c3f5f?rik=pKpfKm%2fUjd4JiQ&riu=http%3a%2f%2fwww.newdesignfile.com%2fpostpic%2f2011%2f05%2fjob-application-icon_2006.png&ehk=SUJuTN0TfgKWYijQWlstZs4CH4rKnmqPW7sUlKiQ0VI%3d&risl=&pid=ImgRaw&r=0", PageType = typeof(AppliedJobsPage) },
                new DashboardItem { Title = "Messaging", Icon = "https://th.bing.com/th/id/R.22587d7e7ffaf7376c4fe4bfaff3e7b6?rik=PUt4CTBCVO5Wcw&pid=ImgRaw&r=0", PageType = typeof(MessagingPage) },
                new DashboardItem { Title = "Notifications", Icon = "https://static.vecteezy.com/system/resources/previews/000/442/359/original/notification-vector-icon.jpg", PageType = typeof(NotificationPage) },
                new DashboardItem { Title = "Profile & Settings", Icon = "https://static.vecteezy.com/system/resources/previews/012/791/178/non_2x/profile-settings-icon-free-vector.jpg", PageType = typeof(ProfileSettingsPage) },
                new DashboardItem { Title = "Application Status", Icon = "https://static.vecteezy.com/system/resources/previews/014/384/963/non_2x/tracking-icon-style-free-vector.jpg", PageType = typeof(ApplicationStatusPage) },
                new DashboardItem { Title = "Saved Jobs", Icon = "https://cdn4.iconfinder.com/data/icons/social-media-2658/512/11_Saved_Bookmark_Badge_Interface_Jobs_Ui-512.png", PageType = typeof(SavedJobsPage) },
               
            };

            BindingContext = this;
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;
            var selected = (DashboardItem)e.CurrentSelection[0];

            if (selected.PageType != null)
            {
                Page page = (Page)Activator.CreateInstance(selected.PageType);
                await Navigation.PushAsync(page);
            }
            else
            {
                await DisplayAlert("Coming Soon", "Feature under development!", "OK");
            }

            ((CollectionView)sender).SelectedItem = null; // Deselect item
        }
    }

    public class DashboardItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type PageType { get; set; }
    }
}

