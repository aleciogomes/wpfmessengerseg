using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFMessengerSeg.Core;
using System.Windows.Controls;
using MessengerLib;

namespace WPFMessengerSeg.UI
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {

        private MSNUser selectedUser;

        private IList<CheckBox> listCheckbox;

        public ConfigWindow()
        {
            InitializeComponent();

            this.selectedUser = null;
            this.LoadUsers();

            this.btAlterar.Visibility = Visibility.Hidden;

            listCheckbox = new List<CheckBox>();

            listCheckbox.Add(this.RegUsers);
            listCheckbox.Add(this.ChangeProp);
            listCheckbox.Add(this.Auditor);

            listCheckbox.Add(this.SendMsg);
            listCheckbox.Add(this.SendMsgOffUser);
            listCheckbox.Add(this.RecMsg);
            listCheckbox.Add(this.SendEmoticons);
            listCheckbox.Add(this.RecEmoticons);

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
            foreach (CheckBox check in this.listCheckbox)
            {
                check.IsEnabled = enabled;
            }
        }

        private void CheckFields()
        {
            foreach (CheckBox check in this.listCheckbox)
            {
                check.IsChecked = MSNUser.HasFeature(selectedUser, OperationHandler.GetOperation(check.Name));
            }
        }

        private void comboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedUser = this.comboUsers.SelectedItem as MSNUser;

            if (selectedUser != null)
            {
                this.btAlterar.Visibility = Visibility.Visible;
                this.ControlFields(true);

                TCPConnection.LoadFeatures(selectedUser);

                this.CheckFields();
            }
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btAlterar_Click(object sender, RoutedEventArgs e)
        {

            IList<Operation> list = new List<Operation>();

            foreach (CheckBox check in this.listCheckbox)
            {
                if ((bool) check.IsChecked)
                {
                    list.Add(OperationHandler.GetOperation(check.Name));
                }
            }

            //resseta a lista do usuário selecionado
            selectedUser.ListFeature = null;

            UDPConnection.UpdatePermissions(list, selectedUser);
            MessageBox.Show("Dados alterados com sucesso", "Alteração de permissões", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

    }
}
