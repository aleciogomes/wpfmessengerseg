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
    /// Interaction logic for AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        public AuthenticationWindow()
        {
            InitializeComponent();
        }

        private void btAutenticar_Click(object sender, RoutedEventArgs e)
        {
            string result = TCPConnection.ConfirmLogin(this.userID.Text, MessengerLib.Encoder.GenerateMD5(this.userPassword.Password));

            if (!result.Equals(MessengerLib.Config.OKMessage))
            {
                MessageBox.Show(result, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;
            }
            else
            {
                this.DialogResult = true;
            }

            this.Close();
        }
    }
}
