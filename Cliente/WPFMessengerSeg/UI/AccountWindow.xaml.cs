using System;
using System.Windows;
using WPFMessengerSeg.Core;
using System.Text;

namespace WPFMessengerSeg.UI
{
    /// <summary>
    /// Interaction logic for Account.xaml
    /// </summary>
    public partial class AccountWindow : Window
    {
        private bool passChanged;

        public AccountWindow()
        {
            InitializeComponent();

            passChanged = false;

            this.userID.Text = MSNSession.User.UserLogin;

            //coloca a senha como 'X'
            StringBuilder sb = new StringBuilder();
            this.userPassword.Password = sb.Append('X', MSNSession.User.UserLogin.Length).ToString();

            this.userName.Text = MSNSession.User.UserName;

            if (MSNSession.User.Expiration != null)
            {
                this.userExpiration.Text = MSNSession.User.Expiration.ToString();
            }
            else
            {
                this.userExpiration.Text = "Sem limite";
            }
            
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btAlterar_Click(object sender, RoutedEventArgs e)
        {

            string newName = this.userName.Text.ToString(),
                   newUser = this.userID.Text.ToString(),
                   newPassword = this.userPassword.Password.ToString();

            string result = MSNUser.ValidateChanges(newName, newUser, passChanged, newPassword, this.userPassword2.Password.ToString());

            if (!String.IsNullOrEmpty(result))
            {
                MessageBox.Show(result, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (passChanged)
                {
                    newPassword = MessengerLib.Encoder.GenerateMD5(newPassword);
                }
                else
                {
                    //mantém a senha
                    newPassword = MSNSession.User.UserPassword;
                }

                UDPConnection.UpdateAccount(newName, newUser, newPassword);
                MessageBox.Show("Dados alterados com sucesso", "Minha conta", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                this.Close();
            }

        }

        private void userPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            this.userPassword.Clear();
            passChanged = true;
        }
    }
}
