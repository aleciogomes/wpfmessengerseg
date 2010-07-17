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

namespace WPFMessengerSeg.UI
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            this.btIncluir.Visibility = Visibility.Hidden;
            this.btAlterar.Visibility = Visibility.Hidden;
            this.btExcluir.Visibility = Visibility.Hidden;
            this.userLastDesblock.Text = DateTime.Now.ToString() ;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btCadastrar_Click(object sender, RoutedEventArgs e)
        {
            this.btIncluir.Visibility = Visibility.Visible;
            this.btAlterar.Visibility = Visibility.Hidden;
            this.btExcluir.Visibility = Visibility.Hidden;

            this.ControlFields(true);
            this.ResetFieldsValues();

        }

        private void ResetFieldsValues()
        {
            this.userID.Text = String.Empty;
            this.userPassword.Password = String.Empty;
            this.userPassword2.Password = String.Empty;
            this.userName.Text = String.Empty;

            this.userExpiration.Text = String.Empty;
            this.userExpirationWarning.Text = String.Empty;
            this.userEnabled.IsChecked = true;
        }

        private void ControlFields(bool enabled)
        {
            this.userID.IsEnabled = enabled;
            this.userPassword.IsEnabled = enabled;
            this.userPassword2.IsEnabled = enabled;
            this.userName.IsEnabled = enabled;

            this.userExpiration.IsEnabled = enabled;
            this.userExpirationWarning.IsEnabled = enabled;
            this.userEnabled.IsEnabled = enabled;
        }

        private void btIncluir_Click(object sender, RoutedEventArgs e)
        {
            string newName = this.userName.Text.ToString(),
                    newUser = this.userID.Text.ToString(),
                    newPassword = this.userPassword.Password.ToString();

            string result = MSNUser.ValidateChanges(newName, newUser, true, newPassword, this.userPassword2.Password.ToString());

            if (!String.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                newPassword = MessengerLib.Encoder.GenerateMD5(newPassword);

                string expiration = String.Empty;

                try
                {
                    expiration = String.Format(MessengerLib.Config.DateFormat, DateTime.Parse(this.userExpiration.Text));
                }
                catch
                {
                }

               UDPConnection.CreateAccount(newName, newUser, newPassword, expiration, int.Parse(this.userExpirationWarning.Text), !this.userEnabled.IsChecked);
               MessageBox.Show("Usuário cadastrado com sucesso", "Novo usuário", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

    }
}
