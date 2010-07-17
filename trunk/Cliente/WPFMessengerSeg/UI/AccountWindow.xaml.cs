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

            if (passChanged)
            {
                string newPassword2 = this.userPassword2.Password.ToString();

                if (String.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Informe e confirme a senha.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!newPassword.Equals(newPassword2))
                {
                    MessageBox.Show("As senhas não coincidem.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    newPassword = MessengerLib.Encoder.GenerateMD5(newPassword);
                }
            }
            else
            {
                //mantém a senha
                newPassword = MSNSession.User.UserPassword;
            }


            if (String.IsNullOrEmpty(newName) || String.IsNullOrEmpty(newUser))
            {
                MessageBox.Show("Informe todos os campos corretamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string testUser = String.Empty;

            //faz o teste se mudou o usuário
            if (!MSNSession.User.UserLogin.Equals(newUser))
            {
                testUser = TCPConnection.GetUserAvailable(newUser);
            }

            if (testUser.Equals(MessengerLib.Config.OKMessage) || String.IsNullOrEmpty(testUser))
            {
                UDPConnection.UpdateAccount(newName, newUser, newPassword);
                MessageBox.Show("Dados alterados com sucesso", "Minha conta", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                this.Close();
            }
            else
            {
                MessageBox.Show(testUser, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                this.userID.Focus();
            }

        }

        private void userPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            this.userPassword.Clear();
            passChanged = true;
        }
    }
}
