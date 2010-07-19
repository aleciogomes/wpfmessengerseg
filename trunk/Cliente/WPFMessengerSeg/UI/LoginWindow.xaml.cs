using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using WPFMessengerSeg.Core;
using System.ComponentModel;
using System.Windows.Controls.Primitives;


namespace WPFMessengerSeg
{

    public partial class LoginWindow : Window
    {
        private int tryingLogin;
        private const int maxTryingLogin = 5;

        public LoginWindow()
        {
            MSNSession.CreateUser();

            InitializeComponent();

            KeyDown += Window_KeyDown;

            this.tryingLogin = 0;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key.ToString().Equals("Return"))
            {
                btLogin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

        }

        private void userID_GotFocus(object sender, RoutedEventArgs e)
        {
            userID.Clear();
            userID.Foreground = new SolidColorBrush(Colors.Black);
            userID.GotFocus -= userID_GotFocus;

            //userID.Text = "admin";
        }

        private void userPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            userPassword.Clear();
            userPassword.Foreground = new SolidColorBrush(Colors.Black);
            userPassword.GotFocus -= userPassword_GotFocus;

            //userPassword.Password = "admin";
        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {

            if (this.tryingLogin == maxTryingLogin)
            {
                MessageBox.Show(String.Format("{0} tentativas de acertar usuário/senha", maxTryingLogin), "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Thread.Sleep(TimeSpan.FromSeconds(30));
                return;
            }

           lblError.Visibility = Visibility.Hidden;
           userID.Focus();
           userPassword.Focus();

           if (String.IsNullOrEmpty(userPassword.Password.ToString()) || String.IsNullOrEmpty(userID.Text.ToString()))
           {
               userPassword.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
               MessageBox.Show("Informe seu ID e Senha.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
               return;
           }

           MSNSession.User.Login = userID.Text.ToString();
           MSNSession.User.Password = MessengerLib.Encoder.GenerateMD5(userPassword.Password.ToString());

           btLogin.IsEnabled = false;
           userID.IsEnabled = false;
           userPassword.IsEnabled = false;

           Duration duration = new Duration(TimeSpan.FromSeconds(1));
           DoubleAnimation doubleanimation = new DoubleAnimation(100.0, duration);
           doubleanimation.RepeatBehavior = RepeatBehavior.Forever;

           loginBar.Visibility = Visibility.Visible;
           loginBar.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);

           BackgroundWorker bw = new BackgroundWorker();
           bw.DoWork += ValidateConnect;
           bw.RunWorkerCompleted += GetConnectionValidation;
           bw.RunWorkerAsync();

        }

        private void ValidateConnect(object sender, DoWorkEventArgs e)
        {
            e.Result = TCPConnection.Login();
        }


        private void GetConnectionValidation(object sender, RunWorkerCompletedEventArgs e)
        {

            string result = e.Result.ToString();

            //verifica se o resultado é 'OK'
            bool connected = (result.Equals(MessengerLib.Config.OKMessage));

            if (connected)
            {
                if (!String.IsNullOrEmpty(TCPConnection.ExpirationWarning))
                {
                    MessageBox.Show(TCPConnection.ExpirationWarning, "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                //carega a lista de permissões
                TCPConnection.LoadFeatures(MSNSession.User);

                MainWindow main = new MainWindow();
                main.Show();

                this.Close();
            }
            else
            {
                tryingLogin++;

                btLogin.IsEnabled = true;
                userID.IsEnabled = true;
                userPassword.IsEnabled = true;
                lblError.Visibility = Visibility.Visible;
                lblError.Text = result;
                loginBar.Visibility = Visibility.Hidden;
                loginBar.BeginAnimation(ProgressBar.ValueProperty, null);
            }

        }

    }
}
