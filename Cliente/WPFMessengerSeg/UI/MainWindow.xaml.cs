using System.Collections.Generic;
using System.Windows;
using WPFMessengerSeg.Core;
using System.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows.Documents;
using WPFMessengerSeg.UI;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media;
using System.Windows.Interop;

namespace WPFMessengerSeg
{
    public partial class MainWindow : Window
    {

        public IList<MSNUser> listUsers;

        private string rootTitle;

        private Dictionary<string, MSNUser> dicTreeItems;

        private TalkManager talkManager;

        //atualiza lista de usuários a cada 6 segundos
        private TimeSpan timeRefreshUsers;

        private bool firstRefresh = true;

        public MainWindow()
        {
            InitializeComponent();

            Closing += Window_Closing;

            this.rootTitle = treeItemRootOnline.Header.ToString();

            this.dicTreeItems = new Dictionary<string, MSNUser>();

            this.lblUsuario.Text= MSNSession.User.UserLogin.ToString();
            this.lblNome.Text = MSNSession.User.UserName;

            this.timeRefreshUsers = TimeSpan.FromSeconds(6);

            this.talkManager = new TalkManager(this);
            this.LoadRSS();

            //cria o usuário 'TODOS'
            MSNUser user = new MSNUser();
            user.UserLogin = String.Empty;
            user.UserName = "Todos os usuários";

            TreeViewItem node = new TreeViewItem();
            node.Header = user.UserName;
            node.FontSize = 12;
            node.Foreground = new SolidColorBrush(Colors.LimeGreen);
            node.PreviewMouseDoubleClick += ShowTalkWindow;
            treeItemRootOnline.Items.Add(node);

            this.dicTreeItems.Add(node.Header.ToString(), user);
            this.talkManager.UserList.Add(user.UserLogin, user);

            this.treeItemRootOnline.IsExpanded = true;
            this.treeItemRootOnline.Header = rootTitle.Replace("(0)", String.Format("({0})", treeItemRootOnline.Items.Count));

        }

        #region Busca listagem de usuários

        private void IntializerRefresher()
        {

            //verifica se existem novas mensagens para o usuário logado
            talkManager.IntializerMsgRefresher();

            //verifica se existem novos usuários logados
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += RefreshUsers;
            bw.RunWorkerCompleted += LoadTreeView;
            bw.RunWorkerAsync();
        }

        private void RefreshUsers(object sender, DoWorkEventArgs e)
        {
            if (!firstRefresh)
            {
                Thread.Sleep(timeRefreshUsers);
            }
            else
            {
                firstRefresh = false;
            }

            //Console.WriteLine(String.Format("Atualizou lista de usuários: {0}", System.DateTime.Now));

            listUsers = TCPConnection.GetListUsers();
        }


        public void LoadTreeView(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeViewItem node;

            String userDisplay = null;

            foreach (MSNUser user in listUsers)
            {
                userDisplay = FormatUserDisplay(user);

                if (!dicTreeItems.ContainsKey(userDisplay) && user.UserLogin != MSNSession.User.UserLogin)
                {
                    node = new TreeViewItem();
                    node.Header = userDisplay;
                    node.FontSize = 12;
                    node.PreviewMouseDoubleClick += ShowTalkWindow;
                    treeItemRootOnline.Items.Add(node);

                    dicTreeItems.Add(node.Header.ToString(), user);

                    if(!talkManager.UserList.ContainsKey(user.UserLogin)){
                        talkManager.UserList.Add(user.UserLogin, user);
                    }

                    //Console.WriteLine(String.Format("Usuário adicionado: {0}", user.UserName));
                }
            }

            treeItemRootOnline.Header = rootTitle.Replace("(0)", String.Format("({0})", treeItemRootOnline.Items.Count));
            treeItemRootOnline.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Header", System.ComponentModel.ListSortDirection.Ascending));

            IntializerRefresher();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntializerRefresher();
        }

        #endregion

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Essa ação finalizará todas as conversas e fechará o programa.\nDeseja continuar?","Finalizar WPFMessenger", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                //faz o logoff no server
                UDPConnection udp = new UDPConnection();
                udp.Logoff();

                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LoadRSS()
        {
            RSSReader r = new RSSReader();
            r.Read();

            Run texto = null;
            Hyperlink link = null;
            TextBlock block;

            int startIndex;

            RSSNews news;

            for (int i = 0; i < r.ListNews.Count && i < 5; i++)
            {

                news = r.ListNews[i];

                startIndex = (news.Title.Length-1 >= 45 ? 45 : news.Title.Length-1);

                texto = new Run(String.Format("{0}...", news.Title.Remove(startIndex)));
                link = new Hyperlink(texto);
                link.NavigateUri = new Uri(news.Link);
                link.RequestNavigate += Hyperlink_RequestNavigate;

                link.Style = (Style)FindResource("LinkStyle");

                block = new TextBlock();
                block.Style = (Style)FindResource("TBStyle");
                block.Inlines.Add(link);
                panelRSS.Children.Add(block);
            }
        }

        private void ShowTalkWindow(object sender, RoutedEventArgs e)
        {

            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            TreeViewItem selectedItem = (TreeViewItem)treeUsers.SelectedItem;

            TalkWindow selectedWindow = talkManager.addTalk(dicTreeItems[selectedItem.Header.ToString()]);
            selectedWindow.Show();
            selectedWindow.Focus();
        }

        private String FormatUserDisplay(MSNUser user)
        {
            return String.Format("{0} (id: {1})", user.UserName, user.UserLogin);
        }

        private void Sair(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
