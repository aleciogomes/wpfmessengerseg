using System;
using System.Windows;
using WPFMessengerSeg.Core;
using System.Text;
using MessengerLib.Core;
using System.Net.Sockets;
using System.Net;

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

            this.userID.Text = MSNSession.User.Login;

            //coloca a senha como 'X'
            this.userPassword.Password = new StringBuilder().Append('X', MSNSession.User.Login.Length).ToString();

            this.userName.Text = MSNSession.User.Name;

            if (MSNSession.User.Expiration != null)
            {
                this.userExpiration.Text = MSNSession.User.Expiration.ToString();
            }
            else
            {
                this.userExpiration.Text = "Sem limite";
            }

            this.serverURL.Text = MSNConfig.ServerURL;
            this.tcpPort.Text = MSNConfig.TCPPort.ToString();
            this.udpPort.Text = MSNConfig.UDPPort.ToString();

        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btAlterar_Click(object sender, RoutedEventArgs e)
        {

            if (ValidateConnection())
            {

                string newName = this.userName.Text.ToString(),
                       newUser = this.userID.Text.ToString(),
                       newPassword = this.userPassword.Password.ToString();

                string result = MSNUser.ValidateChanges(MSNSession.User.Login, newName, newUser, passChanged, newPassword, this.userPassword2.Password.ToString());

                if (!String.IsNullOrEmpty(result))
                {
                    MessageBox.Show(result, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (passChanged)
                    {
                        newPassword = MessengerLib.Util.Encoder.GenerateMD5(newPassword);
                    }
                    else
                    {
                        //mantém a senha
                        newPassword = MSNSession.User.Password;
                    }

                    UDPConnection.UpdateAccount(MSNSession.User.Login, newName, newUser, newPassword);
                    MessageBox.Show("Dados alterados com sucesso", "Minha conta", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    this.Close();
                }
            }
        }

        private bool ValidateConnection()
        {
            bool validTCP = true,
                 validUDP = true;

            int tcpPort = 0,
                udpPort = 0;

            IPEndPoint serverEndPoint = null;

            #region Teste TCP

            TcpClient clientTCP = new TcpClient();
            try
            {
                tcpPort = int.Parse(this.tcpPort.Text);
                serverEndPoint = new IPEndPoint(IPAddress.Parse(this.serverURL.Text), tcpPort);
                clientTCP.Connect(serverEndPoint);

            }
            catch
            {
                validTCP = false;
            }
            finally
            {
                clientTCP.Close();
            }

            #endregion

            #region Teste UDP

            UdpClient clientUDP = new UdpClient();

            try
            {
                udpPort = int.Parse(this.udpPort.Text);
                serverEndPoint = new IPEndPoint(IPAddress.Parse(this.serverURL.Text), udpPort);
                byte[] data = MessengerLib.Util.Encoder.GetEncoding().GetBytes("testing udp");
                clientUDP.Send(data, data.Length, serverEndPoint);
            }
            catch 
            {
                validUDP = false;
            }
            finally
            {
                clientUDP.Close();
            }

            #endregion

            if (!validUDP || !validTCP)
            {
                MessageBox.Show("Configurações de conexão inválidas. Verifique os dados informados.", "Configurações de conexão", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //atualiza arquivo de configuração
            MSNConfig.UpdateConfig(this.serverURL.Text, tcpPort, udpPort);

            return true;
        }

        private void userPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            this.userPassword.Clear();
            passChanged = true;
        }
    }
}
