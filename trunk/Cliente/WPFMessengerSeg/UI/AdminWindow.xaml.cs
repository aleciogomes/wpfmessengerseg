using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFMessengerSeg.Core;
using System.Collections.ObjectModel;
using WPFMessengerSeg.Core.events;

namespace WPFMessengerSeg.UI
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {

        public event DeleteUserHandler UserDeleted;

        private bool passChanged;
        private MSNUser selectedUser;

        public AdminWindow()
        {
            InitializeComponent();

            this.selectedUser = null;

            this.InitializeLayout();
            this.LoadUsers();

        }

        private void InitializeLayout()
        {
            this.btIncluir.Visibility = Visibility.Hidden;
            this.btAlterar.Visibility = Visibility.Hidden;
            this.btExcluir.Visibility = Visibility.Hidden;
            this.userUnblockDate.Text = DateTime.Now.ToString(MessengerLib.Util.Config.DateFormat);
        }

        private void LoadUsers()
        {

            this.comboUsers.Items.Clear();

            //atualiza a lista
            IList<MSNUser> listUsers = TCPConnection.GetListUsers();
            
            foreach(MSNUser user in listUsers)
            {
                this.comboUsers.Items.Add(user);
            }

             this.comboUsers.SelectedIndex = -1;

        }

        private void ResetFieldsValues()
        {
            this.userID.Text = String.Empty;
            this.userPassword.Password = String.Empty;
            this.userPassword2.Password = String.Empty;
            this.userName.Text = String.Empty;

            this.userExpiration.Text = String.Empty;
            this.userTimeAlert.Text = String.Empty;
            this.userEnabled.IsChecked = true;
        }

        private void ControlFields(bool enabled)
        {
            this.userID.IsEnabled = enabled;
            this.userPassword.IsEnabled = enabled;
            this.userPassword2.IsEnabled = enabled;
            this.userName.IsEnabled = enabled;

            this.userExpiration.IsEnabled = enabled;
            this.userTimeAlert.IsEnabled = enabled;
            this.userEnabled.IsEnabled = enabled;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btCadastrar_Click(object sender, RoutedEventArgs e)
        {
            this.comboUsers.SelectedIndex = -1;

            this.btIncluir.Visibility = Visibility.Visible;
            this.btAlterar.Visibility = Visibility.Hidden;
            this.btExcluir.Visibility = Visibility.Hidden;

            this.ControlFields(true);
        }

        private void btIncluir_Click(object sender, RoutedEventArgs e)
        {
            this.InsertUpdate(true);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Deseja realmente excluir esse usuário?", "Exclusão de usuário", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                UDPConnection.DeleteAccount(selectedUser.Login);
                MessageBox.Show("Usuário excluído com sucesso", "Exclusão de usuário", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //remove o usuário da listagem na tela principal
                if (UserDeleted != null)
                {
                    UserDeleted(this, new Arguments(MessengerLib.Util.Config.FormatUserDisplay(selectedUser.Name, selectedUser.Login)));
                }

                //recarrega para garantir que vai trazer todos os usuários (inclusive os adicionados e removidos por outros admins)
                this.LoadUsers();

            }
        }

        private void comboUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedUser = this.comboUsers.SelectedItem as MSNUser;

            if (selectedUser != null)
            {
                this.btIncluir.Visibility = Visibility.Hidden;
                this.btAlterar.Visibility = Visibility.Visible;
                this.btExcluir.Visibility = Visibility.Visible;

                this.ControlFields(true);

                selectedUser = TCPConnection.GetUserInfo(selectedUser.Login);

                this.userID.Text = selectedUser.Login;
                this.userPassword.Password = new StringBuilder().Append('X', selectedUser.Login.Length).ToString();
                this.userPassword2.Clear();
                this.userName.Text = selectedUser.Name;

                this.userExpiration.Text = selectedUser.ExpirationString;
                this.userTimeAlert.Text = selectedUser.TimeAlert.ToString();
                this.userEnabled.IsChecked = !selectedUser.Blocked;
                this.userUnblockDate.Text = selectedUser.UnblockDateString;

                this.passChanged = false;
            }
            else
            {
                this.InitializeLayout();
                this.ControlFields(false);
                this.ResetFieldsValues();
            }

        }

        private void btAlterar_Click(object sender, RoutedEventArgs e)
        {
            this.InsertUpdate(false);
        }

        private void userPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            this.userPassword.Clear();
            this.passChanged = true;
        }

        private void InsertUpdate(bool insert)
        {

            if (insert)
            {
                this.passChanged = true;
            }

            string newName = this.userName.Text.ToString(),
                     newUser = this.userID.Text.ToString(),
                     newPassword = this.userPassword.Password.ToString();

            string user = (insert ? String.Empty :  selectedUser.Login);

            string result = MSNUser.ValidateChanges( user, newName, newUser, passChanged, newPassword, this.userPassword2.Password.ToString());

            if (!String.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (this.passChanged)
                {
                    newPassword = MessengerLib.Util.Encoder.GenerateMD5(newPassword);
                }
                else
                {
                    //mantém a senha
                    newPassword = selectedUser.Password;
                }

                string expiration = String.Empty;

                try
                {
                    expiration = DateTime.Parse(this.userExpiration.Text).ToString(MessengerLib.Util.Config.DateFormat);
                }
                catch
                {
                }

                string timeAlert = String.Empty;

                try
                {
                    timeAlert = int.Parse(this.userTimeAlert.Text).ToString();
                }
                catch
                {
                }

                if (insert)
                {
                    UDPConnection.CreateAccount(newName, newUser, newPassword, expiration, timeAlert, !this.userEnabled.IsChecked);
                    MessageBox.Show("Usuário cadastrado com sucesso", "Novo usuário", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    //recarrega para garantir que vai trazer todos os usuários (inclusive os adicionados e removidos por outros admins)
                    this.LoadUsers();
                }
                else
                {
                    string unblockDate = String.Empty;

                    //está reabilitando o usuário
                    if (selectedUser.Blocked && (bool) this.userEnabled.IsChecked)
                    {
                        unblockDate = DateTime.Now.ToString(MessengerLib.Util.Config.DateFormat);
                        this.userUnblockDate.Text = unblockDate;
                    }

                    selectedUser.Blocked = !(bool)this.userEnabled.IsChecked;

                    UDPConnection.UpdateAccount(selectedUser.Login, newName, newUser, newPassword);
                    UDPConnection.UpdateAccountOtherInfo(newUser, expiration, timeAlert, !this.userEnabled.IsChecked, unblockDate);
                    MessageBox.Show("Dados alterados com sucesso", "Alteração de usuário", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
        }

    }
}
