﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using MessengerLib.Handler;
using Microsoft.Win32;
using WPFMessengerSeg.Core;
using WPFMessengerSeg.Core.events;
using WPFMessengerSeg.Core.util;
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

        //permissões
        private bool offlineTalk = false;
        private bool sendMsg = false;
        private bool recMsg = false;

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

            //carrega as permissões do usuário apenas uma vez
            this.ReadPermissions();
        }

        #region Listagem de usuários

        private void InitializeRefresher()
        {
            if (this.recMsg)
            {
                //verifica se existem novas mensagens para o usuário logado
                talkManager.IntializerMsgRefresher();
            }

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

            listUsers = TCPConnection.GetListUsers(true);
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

                userDisplay = MessengerLib.Util.Config.FormatUserDisplay(user.Name, user.Login);

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

                    if (!talkManager.UserList.ContainsKey(user.Login))
                    {
                        talkManager.UserList.Add(user.Login, user);
                    }

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
                //verifica se o usuário logado pode falar com usuários
                if(this.sendMsg)
                {
                    node.PreviewMouseDoubleClick += ShowTalkWindow;
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
                if (this.offlineTalk)
                {
                    node.PreviewMouseDoubleClick += ShowTalkWindow;
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

            MSNUser user;
            dicOfflineUsers.TryGetValue(selectedItem.Header.ToString(), out user);

            if(user == null){
                dicOnlineUsers.TryGetValue(selectedItem.Header.ToString(), out user);
            }

            TalkWindow selectedWindow = talkManager.addTalk(user);
            selectedWindow.Show();
            selectedWindow.Focus();
        }

        private void ReadPermissions()
        {
            bool manageUser,
                 configUsers,
                 events;

            manageUser = MSNUser.HasFeature(MSNSession.User, Operation.RegUsers);
            configUsers = MSNUser.HasFeature(MSNSession.User, Operation.ChangeProp);
            events = MSNUser.HasFeature(MSNSession.User, Operation.Auditor);

            if (manageUser || manageUser)
            {
                this.menuAdmin.Visibility = Visibility.Visible;

                if (manageUser)
                {
                    this.menuManageUsers.Visibility = Visibility.Visible;
                }

                if (manageUser)
                {
                    this.menuConfigUsers.Visibility = Visibility.Visible;
                }
            }

            if (events)
            {
                this.menuAuditoria.Visibility = Visibility.Visible;
                this.menuEvents.Visibility = Visibility.Visible;
            }

            //se ainda não possui chave privada/publica
            if (String.IsNullOrEmpty(MSNSession.User.SignaturePublicKey))
            {
                this.menuAssinatura.Visibility = Visibility.Visible;
            }

            this.offlineTalk = MSNUser.HasFeature(MSNSession.User, Operation.SendMsgOffUser);
            this.sendMsg = MSNUser.HasFeature(MSNSession.User, Operation.SendMsg);
            this.recMsg = MSNUser.HasFeature(MSNSession.User, Operation.RecMsg);

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

        private void DigitalSignature_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.DefaultExt = Signature.KEY_EXT;
            dialog.Filter = String.Format("Arquivo de assinatura do WPFMessenger ({0})|*{1}", Signature.KEY_EXT, Signature.KEY_EXT);

            if (dialog.ShowDialog() == true)
            {
                Signature signature = new Signature(true);
                signature.GenerateSignatureFile(dialog.FileName);
                this.menuAssinatura.Visibility = Visibility.Collapsed;
                MessageBox.Show("Assinatura digital criada! Mantenha esse arquivo em local seguro, pois ele será utilizado para assinar dados durante a exportação de informações.", "Assinatura digital", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void ExportContactsWithKey_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MSNSession.User.SignaturePublicKey))
            {
                ExportContacts_Click(true);
            }
            else
            {
                MessageBox.Show("Você ainda não possui uma assinatura digital. Crie uma antes de realizar essa operação novamente.", "Exportação de contatos", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ExportContactsNoKey_Click(object sender, RoutedEventArgs e)
        {
            ExportContacts_Click(false);
        }

        private void ExportContacts_Click(bool sign)
        {

            if(sign && String.IsNullOrEmpty(MSNSession.User.SignaturePrivateKey))
            {
                MessageBox.Show("Selecione o arquivo que contém sua assinatura digital", "Exportação de contatos", MessageBoxButton.OK, MessageBoxImage.Information);

                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.DefaultExt = Signature.KEY_EXT;
                openDialog.Filter = String.Format("Arquivo de assinatura do WPFMessenger ({0})|*{1}", Signature.KEY_EXT, Signature.KEY_EXT);

                if (openDialog.ShowDialog() == true)
                {
                    new Signature(false).ReadSginatureFile(openDialog.FileName);
                }
                else
                {
                    //sai da função
                    return;
                }
            }

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.DefaultExt   = Report.REPORT_EXT;
            dialog.Filter       = String.Format("Lista de contatos do WPFMessenger ({0})|*{1}", Report.REPORT_EXT, Report.REPORT_EXT);

            if (dialog.ShowDialog() == true)
            {
                IList<int> listContacts = new List<int>();

                foreach (KeyValuePair<string, MSNUser> kvp in dicOnlineUsers)
                {
                    listContacts.Add(kvp.Value.ID);
                }

                foreach (KeyValuePair<string, MSNUser> kvp in dicOfflineUsers)
                {
                    listContacts.Add(kvp.Value.ID);
                }

                new Report().GenerateContactReport(listContacts, dialog.FileName, sign);
                UDPConnection.ExportContactList();
                MessageBox.Show("Lista de contatos exportada com sucesso!", "Exportação de contatos", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportContactsWithKey_Click(object sender, RoutedEventArgs e)
        {
            ImportContacts_Click(true);
        }

        private void ImportContactsNoKey_Click(object sender, RoutedEventArgs e)
        {
            ImportContacts_Click(false);
        }

        private void ImportContacts_Click(bool validateSignature)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = Report.REPORT_EXT;
            dialog.Filter = String.Format("Lista de contatos do WPFMessenger ({0})|*{1}", Report.REPORT_EXT, Report.REPORT_EXT);

            if (dialog.ShowDialog() == true)
            {
                Report importReport = new Report();

                string title = "Importação de contatos";

                if (importReport.ImportContactReport(dialog.FileName, validateSignature))
                {

                    #region Remove da lista os contatos que o usuário já tem

                    foreach (KeyValuePair<string, MSNUser> kvp in dicOnlineUsers)
                    {
                        importReport.ImportedValues.Remove(kvp.Value.ID.ToString());
                    }

                    foreach (KeyValuePair<string, MSNUser> kvp in dicOfflineUsers)
                    {
                        importReport.ImportedValues.Remove(kvp.Value.ID.ToString());
                    }

                    #endregion

                    importReport.ImportedValues.Remove(MSNSession.User.ID.ToString());

                    if (importReport.ImportedValues.Count > 0)
                    {
                        UDPConnection.AddContacts(importReport.ImportedValues);
                        MessageBox.Show("A lista de contatos selecionada foi importada.", title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("A lista de contatos selecionada foi importada, porém você já possui todos os contatos da lista.", title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    if(importReport.InvalidSignature)
                    {
                        MessageBox.Show("A lista de contatos selecionada não foi importada. A assinatura do arquivo não é válida.", title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if(importReport.SignatureNotFound)
                    {
                        MessageBox.Show("A lista de contatos selecionada não possui assinatura.", title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(String.Format("A lista de contatos selecionada não foi importada. Foram encontrados problemas de {0} no arquivo.", importReport.InvalidContent ? "confidencialidade" : "integridade"), title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

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

        private void btFavorito_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationWindow authentication = new AuthenticationWindow();
            authentication.Owner = this;
            authentication.ShowDialog();

            //credenciais OK
            if((bool) authentication.DialogResult)
            {
                MessageBox.Show(talkManager.TopTalker, "TopTalker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
