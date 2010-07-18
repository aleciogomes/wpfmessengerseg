using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using WPFMessengerSeg.Core;
using WPFMessengerSeg.Core.events;
using WPFMessengerSeg.UI;

namespace WPFMessengerSeg
{
    public partial class MainWindow : Window
    {

        public IList<MSNUser> listUsers;

        private string rootTitleOnline;
        private string rootTitleOffline;

        private Dictionary<string, MSNUser> dicOnlineUsers;
        private Dictionary<string, MSNUser> dicOfflineUsers;

        private TalkManager talkManager;
        private TimeSpan timeRefreshUsers;

        private bool firstRefresh = true;

        public MainWindow()
        {
            InitializeComponent();

            Closing += Window_Closing;

            this.rootTitleOnline = treeItemRootOnline.Header.ToString();
            this.rootTitleOffline = treeItemRootOffline.Header.ToString();

            this.dicOnlineUsers = new Dictionary<string, MSNUser>();
            this.dicOfflineUsers = new Dictionary<string, MSNUser>();

            this.lblUsuario.Text= MSNSession.User.Login.ToString();
            this.lblNome.Text = MSNSession.User.Name;

            //atualiza lista de usuários a cada 6 segundos
            this.timeRefreshUsers = TimeSpan.FromSeconds(6);

            this.talkManager = new TalkManager(this);
            this.LoadRSS();

            this.InitalizeTree();

            //verifica permissões de admin
            if (1 == 1)
            {
                this.EnableAdminOptions();
            }

        }

        #region Listagem de usuários

        private void InitializeRefresher()
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


        #region Construção da lista

        private void InitalizeTree()
        {
            /* 
             * Não é possível enviar msg para todos por enquanto
             * 
            MSNUser user = new MSNUser();
            user.Login = String.Empty;
            user.Name = "Todos os usuários";

            TreeViewItem node = new TreeViewItem();
            node.Header = user.Name;
            node.FontSize = 12;
            node.Foreground = new SolidColorBrush(Colors.LimeGreen);
            node.PreviewMouseDoubleClick += ShowTalkWindow;
            treeItemRootOnline.Items.Add(node);

            this.dicOnlineUsers.Add(node.Header.ToString(), user);
            this.talkManager.UserList.Add(user.Login, user);
            */

            this.treeItemRootOnline.IsExpanded = true;
            this.treeItemRootOnline.Header = rootTitleOnline.Replace("(0)", String.Format("({0})", treeItemRootOnline.Items.Count));

            this.treeItemRootOffline.IsExpanded = true;
        }

        public void LoadTreeView(object sender, RunWorkerCompletedEventArgs e)
        {

            TreeViewItem node;
            String userDisplay = null;

            foreach (MSNUser user in listUsers)
            {

                userDisplay = MessengerLib.Config.FormatUserDisplay(user.Name, user.Login);

                //se não é o usuário logado
                if (user.Login != MSNSession.User.Login)
                {
                    node = new TreeViewItem();
                    node.Header = userDisplay;
                    node.FontSize = 12;

                    if (user.Online)
                    {
                        AddOnline(user, node);
                    }
                    else
                    {
                        AddOffline(user, node);
                    }

                    //Console.WriteLine(String.Format("Usuário adicionado: {0}", user.UserName));

                }
            }

            this.RefreshGroupHeader();

            InitializeRefresher();
        }

        private void RefreshGroupHeader()
        {
            treeItemRootOnline.Header = rootTitleOnline.Replace("(0)", String.Format("({0})", treeItemRootOnline.Items.Count));
            treeItemRootOnline.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Header", System.ComponentModel.ListSortDirection.Ascending));

            treeItemRootOffline.Header = rootTitleOffline.Replace("(0)", String.Format("({0})", treeItemRootOffline.Items.Count));
            treeItemRootOffline.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Header", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void RemoveDeletedUser(object sender, Arguments e)
        {
            this.RemoveItemByHeader(treeItemRootOnline, e.Header);
            this.RemoveItemByHeader(treeItemRootOffline, e.Header);
            this.RefreshGroupHeader();
        }

        private void AddOnline(MSNUser user, TreeViewItem node)
        {
            
            string header = node.Header.ToString(); 

            //garante que o usuário não seja mostrado como um usuário OFFLINE
            dicOfflineUsers.Remove(header);
            this.RemoveItemByHeader(treeItemRootOffline, header);

            if (!dicOnlineUsers.ContainsKey(header))
            {
                node.PreviewMouseDoubleClick += ShowTalkWindow;

                if (!talkManager.UserList.ContainsKey(user.Login))
                {
                    talkManager.UserList.Add(user.Login, user);
                }

                treeItemRootOnline.Items.Add(node);
                dicOnlineUsers.Add(header, user);

            }
        }

        private void AddOffline(MSNUser user, TreeViewItem node)
        {
            string header = node.Header.ToString(); 

            //garante que o usuário não seja mostrado como um usuário ONLINE
            dicOnlineUsers.Remove(header);
            this.RemoveItemByHeader(treeItemRootOnline, header);

            //verifica se já está adicionado
            if (!dicOfflineUsers.ContainsKey(header))
            {
                //verifica se o usuário logado pode falar com usuários offline
                if (1 == 2)
                {
                    node.PreviewMouseDoubleClick += ShowTalkWindow;

                    if (!talkManager.UserList.ContainsKey(user.Login))
                    {
                        talkManager.UserList.Add(user.Login, user);
                    }
                }
                node.Foreground = new SolidColorBrush(Colors.Gray);
                treeItemRootOffline.Items.Add(node);
                dicOfflineUsers.Add(header, user);

            }

        }

        private void RemoveItemByHeader(TreeViewItem tree,  string header)
        {

            TreeViewItem searchItem = null;

            foreach (TreeViewItem item in tree.Items)
            {
                if ((item.Header.ToString().Equals(header)))
                {
                    searchItem = item;
                    break;
                }
            }

            if (searchItem != null)
            {
                tree.Items.Remove(searchItem);
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeRefresher();
        }

        #endregion

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Essa ação finalizará todas as conversas e fechará o programa.\nDeseja continuar?","Finalizar WPFMessenger", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                //faz o logoff no server
                UDPConnection.Logoff();

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

            TalkWindow selectedWindow = talkManager.addTalk(dicOnlineUsers[selectedItem.Header.ToString()]);
            selectedWindow.Show();
            selectedWindow.Focus();
        }

        private void EnableAdminOptions()
        {
            this.menuAdmin.Visibility = Visibility.Visible;
            this.menuAuditoria.Visibility = Visibility.Visible;
        }

        #region Ações do menu

        private void Logoff_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow accWindow = new AccountWindow();
            accWindow.Owner = this;

            accWindow.ShowDialog();

            this.lblUsuario.Text = MSNSession.User.Login;
            this.lblNome.Text = MSNSession.User.Name;

        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow manage = new AdminWindow();
            manage.UserDeleted += new DeleteUserHandler(this.RemoveDeletedUser);
            manage.Owner = this;
            manage.ShowDialog();
        }

        private void ChangeUsersProp_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow properties = new ConfigWindow();
            properties.Owner = this;
            properties.ShowDialog();
        }

        private void Auditor_Click(object sender, RoutedEventArgs e)
        {
            AuditorWindow auditor = new AuditorWindow();
            auditor.Owner = this;
            auditor.ShowDialog();
        }

        #endregion

    }
}
