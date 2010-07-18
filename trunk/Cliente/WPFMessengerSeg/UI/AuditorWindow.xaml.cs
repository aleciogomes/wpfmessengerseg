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
    /// Interaction logic for AuditorWindow.xaml
    /// </summary>
    public partial class AuditorWindow : Window
    {
        public AuditorWindow()
        {
            InitializeComponent();

            this.LoadDates();
        }

        private void LoadDates()
        {
            this.comboLogs.Items.Clear();

            //atualiza a lista
            string[] listDates = TCPConnection.GetLogDates();

            foreach (string date in listDates)
            {
                this.comboLogs.Items.Add(date);
            }

            this.comboLogs.SelectedIndex = -1;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime logDate = DateTime.Parse(this.comboLogs.SelectedItem.ToString());

            IList<MessengerLib.Core.MSNLog> listLog = TCPConnection.GetLog(logDate);

            Binding binding = new Binding();
            binding.Source = listLog;
            listEvents.SetBinding(ListView.ItemsSourceProperty, binding);

        }

    }
}
