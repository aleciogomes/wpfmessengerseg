﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using WPFMessengerSeg.Core.util;
using WPFMessengerSeg.UI;

namespace WPFMessengerSeg.Core
{
    public class TalkManager
    {

        private Dictionary<int, int> dicChats;
        public string TopTalker;
        private int topTalkerCount;

        private MainWindow main;

        private MediaPlayer mp;

        private IList<MSNMessage> messageList;

        public Dictionary<string, TalkWindow> TalkList { get; set; }
        public Dictionary<string, MSNUser> UserList { get; set; }

        public TalkManager(MainWindow main)
        {
            TalkList = new Dictionary<string, TalkWindow>();
            UserList = new Dictionary<string, MSNUser>();
            this.main = main;

            mp = new MediaPlayer();
            Uri mp3Adress = new Uri(@".\resources\alert.mp3", UriKind.Relative);
            mp.Open(mp3Adress);

            this.dicChats = new Dictionary<int, int>();

            this.TopTalker = "Nenhuma mensagem foi recebida até o momento";
            this.topTalkerCount = 0;
        }

        public TalkWindow addTalk(MSNUser destiny)
        {
            TalkWindow selectedWindow;

            if (!TalkList.ContainsKey(destiny.Login))
            {
                selectedWindow = new TalkWindow(destiny);
                selectedWindow.Owner = main;
                TalkList.Add(destiny.Login, selectedWindow);
            }
            else
            {
                selectedWindow = TalkList[destiny.Login];
            }

            return selectedWindow;
        }

        public void IntializerMsgRefresher()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += GetMessages;
            bw.RunWorkerCompleted += RefreshMessagesBoard;
            bw.RunWorkerAsync();
        }

        private void GetMessages(object sender, DoWorkEventArgs e)
        {
            messageList = TCPConnection.GetMyMessages();
        }


        private void RefreshMessagesBoard(object sender, RunWorkerCompletedEventArgs e)
        {

            MSNUser forwarder = null;

            foreach (MSNMessage mensagem in messageList)
            {
                UserList.TryGetValue(mensagem.Forwarder, out forwarder);

                if (forwarder != null)
                {
                    this.ActivateWindow(forwarder, mensagem.Message);
                }
            }

        }

        private void ActivateWindow(MSNUser ownerUser, string message)
        {

            mp.Stop();
            mp.Play();

            TalkWindow window = null;
            TalkList.TryGetValue(ownerUser.Login, out window);

            if (window == null)
            {
                window = addTalk(ownerUser);   
            }

            if (!window.IsVisible)
            {
                window.WindowState = WindowState.Minimized;
                window.Visibility = Visibility.Visible;
                
                Win32.Flash(window);

            }

            window.InsertMessage(ownerUser, message, false);

            if (!this.dicChats.ContainsKey(ownerUser.ID))
            {
                this.dicChats.Add(ownerUser.ID, 0);
            }
            this.dicChats[ownerUser.ID]++;

            if (this.dicChats[ownerUser.ID] > this.topTalkerCount)
            {
                this.TopTalker = ownerUser.Name;
            }
        }

    }
}
