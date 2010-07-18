using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFMessengerSeg.Core;
using System.Windows.Controls;

namespace WPFMessengerSeg.UI
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {

        private MSNUser selectedUser;

        public ConfigWindow()
        {
            InitializeComponent();

            this.selectedUser = null;
            this.LoadUsers();

            this.btAlterar.Visibility = Visibility.Hidden;
            this.ControlFields(false);
        }


        private void LoadUsers()
        {

            this.comboUsers.Items.Clear();

            //atualiza a lista
            IList<MSNUser> listUsers = (from msnUser in TCPConnection.GetListUsers()
                                        select msnUser).ToList();


            foreach (MSNUser user in listUsers)
            {
                this.comboUsers.Items.Add(user);
            }

            this.comboUsers.SelectedIndex = -1;
        }

        private void ControlFields(bool enabled)
        {
            this.RegUsers.IsEnabled = enabled;
            this.ChangeProp.IsEnabled = enabled;
            this.Auditor.IsEnabled = enabled;

            this.SendMsg.IsEnabled = enabled;
            this.RecMsg.IsEnabled = enabled;
            this.SendEmoticons.IsEnabled = enabled;
            this.RecEmoticons.IsEnabled = enabled;
        }

        private void comboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedUser = this.comboUsers.SelectedItem as MSNUser;

            if (selectedUser != null)
            {
                this.btAlterar.Visibility = Visibility.Visible;
                this.ControlFields(true);
            }
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
