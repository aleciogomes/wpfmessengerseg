using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessengerLib;
using WPFMessengerServer.Control.Model;

namespace WPFMessengerServer.Core
{
    public class UDPListener
    {
        private const int channel = 1011;
        private UdpClient udpListener;

        public UDPListener()
        {
            this.udpListener = new UdpClient(channel);
        }

        public void Listen()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = null;
            string dataString = "";

            while (true)
            {
                data = udpListener.Receive(ref iep);
                dataString = MessengerLib.Encoder.GetEncoding().GetString(data, 0, data.Length);

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(dataString);
            }
        }

        private void HandleClient(object data)
        {
            string request  = data.ToString(),
                   user     = String.Empty,
                   password = String.Empty;

            string[] info = ActionHandler.GetMessage(request).Split(':');
            user = info[0];
            password = info[1];

            MSNUser msnUser = Util.GetUser(user, password);

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Action.SendMsg:


                    string destiny  = info[2];

                    //pode conter emoticons na msg
                    string msg = String.Join(":", info, 3, info.Length - 3);

                    if (msnUser != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Mensagem enviada de {0} para {1}", user, destiny));

                        if (Util.GetContact(destiny) != null)
                        {
                            Util.AddMessage(msnUser, destiny, msg);
                        }
                    }

                    break;

                case MessengerLib.Action.Logoff:

                    if (msnUser != null)
                    {
                        //auditoria
                        Console.WriteLine(String.Format("Usuário desconectado: {0}", user));

                        Util.ShutdownUser(msnUser.Login);
                    }

                    break;

                case MessengerLib.Action.UpdateAccMainInfo:

                    if (msnUser != null)
                    {

                        user = info[2];

                        msnUser = new MSNUser();
                        msnUser.Name = info[3];
                        msnUser.Login = info[4];
                        msnUser.Password = info[5];

                        //auditoria
                        Console.WriteLine(String.Format("Alterando dados da conta {0}: {1}", msnUser.Name, user));

                        Util.UpdateAccount(user, msnUser);
                    }


                    break;
                case MessengerLib.Action.UpdateAccOtherInfo:

                    //valida a conta de admin
                    if (msnUser != null)
                    {
                        //nova conta
                        msnUser = new MSNUser();
                        msnUser.Login = info[2];

                        try
                        {
                            msnUser.Expiration = DateTime.Parse(info[3]);
                        }
                        catch { }

                        try
                        {
                            msnUser.TimeAlert = int.Parse(info[4]);
                        }
                        catch { }

                        msnUser.Blocked = Convert.ToBoolean(info[5]);

                        try
                        {
                            msnUser.UnblockDate = DateTime.Parse(info[6]);
                        }
                        catch { }

                        //auditoria
                        Console.WriteLine(String.Format("Alterando outros dados da conta {0}", msnUser.Login));

                        Util.UpdateAccountOtherInfo(msnUser);
                    }

                    break;

                case MessengerLib.Action.CreateAcc:

                    //valida a conta de admin
                    if (msnUser != null)
                    {
                        //nova conta
                        msnUser = new MSNUser();

                        msnUser.Login = info[2];
                        msnUser.Name = info[3];
                        msnUser.Password = info[4];

                        try
                        {
                            msnUser.Expiration = DateTime.Parse(info[5]);
                        }
                        catch { }

                        try
                        {
                            msnUser.TimeAlert = int.Parse(info[6]);
                        }
                        catch { }

                        msnUser.Blocked = Convert.ToBoolean(info[7]);

                        try
                        {
                            msnUser.UnblockDate = DateTime.Parse(info[8]);
                        }
                        catch { }

                        //auditoria
                        Console.WriteLine(String.Format("Nova conta criada: {0}", msnUser.Login));

                        Util.CreateAccount(msnUser);
                    }

                    break;

                case MessengerLib.Action.DeleteAcc:

                    if (msnUser != null)
                    {
                        user = info[2];

                        //auditoria
                        Console.WriteLine(String.Format("Usuário excluído: {0}", user));

                        Util.DeleteAccount(user);

                    }

                    break;


            }
        }

    }
}
