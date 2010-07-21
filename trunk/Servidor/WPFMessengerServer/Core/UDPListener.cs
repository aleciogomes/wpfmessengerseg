using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessengerLib;
using WPFMessengerServer.Control.Model;
using MessengerLib.Handler;

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
                dataString = MessengerLib.Util.Encoder.GetEncoding().GetString(data, 0, data.Length);

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(dataString);
            }
        }

        private void HandleClient(object data)
        {
            string request  = data.ToString(),
                   userLogin     = String.Empty,
                   userPassword = String.Empty;

            string[] info = ActionHandler.GetMessage(request).Split(':');
            userLogin = info[0];
            userPassword = info[1];

            MSNUser msnUser = Util.GetUser(userLogin, userPassword);

            switch (ActionHandler.GetAction(request))
            {
                case MessengerLib.Handler.Action.SendMsg:

                    string destiny  = info[2];

                    //pode conter emoticons na msg
                    string msg = String.Join(":", info, 3, info.Length - 3);

                    if (msnUser != null)
                    {
                        //auditoria
                        Util.RegEvent(msnUser.Login, String.Format("Mensagem enviada para {0}", destiny));
                        Console.WriteLine(String.Format("Mensagem enviada de {0} para {1}: {2}", msnUser.Login, destiny, msg));

                        if (Util.GetContact(destiny) != null)
                        {
                            Util.AddMessage(msnUser, destiny, msg);
                        }
                    }
                    else
                    {
                        Util.RegEvent(userLogin, "Tentativa de enviar mensagem sem login do sistema");
                    }

                    break;

                case MessengerLib.Handler.Action.Logoff:

                    if (msnUser != null)
                    {
                        //auditoria
                        Util.RegEvent(msnUser.Login, "Logoff efetuado");
                        Console.WriteLine(String.Format("Usuário desconectado: {0}", msnUser.Login));

                        Util.ShutdownUser(msnUser.Login);
                    }
                    else
                    {
                        Util.RegEvent(userLogin, "Tentativa de realizar logoff sem login do sistema");
                    }

                    break;

                case MessengerLib.Handler.Action.UpdateAccMainInfo:

                    if (msnUser != null)
                    {

                        userLogin = info[2];

                        msnUser = new MSNUser();
                        msnUser.Name = info[3];
                        msnUser.Login = info[4];
                        msnUser.Password = info[5];

                        //auditoria
                        Util.RegEvent(userLogin, String.Format("Dados da conta {0} alterados", msnUser.Login));
                        Console.WriteLine(String.Format("Alterando dados da conta {0}: {1}", msnUser.Name, userLogin));

                        Util.UpdateAccount(userLogin, msnUser);
                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de alterar dados da conta {0} sem login do sistema", info[3]));
                    }

                    break;
                case MessengerLib.Handler.Action.UpdateAccOtherInfo:

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
                        Util.RegEvent(userLogin, String.Format("Dados adicionas da conta {0} alterados", msnUser.Login));
                        Console.WriteLine(String.Format("Alterando outros dados da conta {0}", msnUser.Login));

                        Util.UpdateAccountOtherInfo(msnUser);
                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de alterar dados da conta {0} sem login do sistema", info[2]));
                    }

                    break;

                case MessengerLib.Handler.Action.UpdatePermissions:

                    //valida a conta de admin
                    if (msnUser != null)
                    {

                        msnUser = new MSNUser();
                        msnUser.Login = info[2];
                        msnUser.ID = int.Parse(info[3]);

                        IList<string> listOperation = new List<string>();

                        //busca as permissões
                        if(info.Length > 3)
                        {
                            for (int i = 3; i < info.Length; i++ )
                            {
                                listOperation.Add(info[i]);
                            }
                        }

                        //auditoria
                        Util.RegEvent(userLogin, String.Format("Permissões da conta {0} alteradas", msnUser.Login));
                        Console.WriteLine(String.Format("Permissões da conta conta {0} alteradas", msnUser.Login));

                        Util.UpdatePermissions(msnUser, listOperation);
                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de alterar permissões do usuário {0} sem login do sistema", info[2]));
                    }

                    break;
                
                case MessengerLib.Handler.Action.CreateAcc:

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
                        Util.RegEvent(userLogin, String.Format("Nova conta {0} criada", msnUser.Login));
                        Console.WriteLine(String.Format("Nova conta criada: {0}", msnUser.Login));

                        Util.CreateAccount(msnUser);
                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de criar conta {0} sem login do sistema", info[2]));
                    }

                    break;

                case MessengerLib.Handler.Action.DeleteAcc:

                    if (msnUser != null)
                    {

                        userLogin = info[2];

                        //auditoria
                        Util.RegEvent(msnUser.Login, String.Format("Conta {0} excluída", userLogin));
                        Console.WriteLine(String.Format("Usuário excluído: {0}", userLogin));

                        Util.DeleteAccount(userLogin);

                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de excluir conta {0} sem login do sistema", info[2]));
                    }

                    break;

                case MessengerLib.Handler.Action.EventInvalidPassword:

                    if (msnUser != null)
                    {
                        userLogin = info[2];

                        //auditoria
                        Util.RegEvent(msnUser.Login, String.Format("Tentativa de atribur senha fraca para conta {0}", userLogin));
                        Console.WriteLine(String.Format("Tentativa de atribur senha fraca para conta {0}", userLogin));
                    }
                    else
                    {
                        Util.RegEvent(userLogin, String.Format("Tentativa de simular erro de senha para conta {0} sem login do sistema", info[2]));
                    }

                    break;

                case MessengerLib.Handler.Action.EventSendEmoticonInMsg:

                    if (msnUser != null)
                    {
                        //auditoria
                        Util.RegEvent(msnUser.Login, "Enviou mensagem com emoticon");
                        Console.WriteLine(String.Format("Enviou mensagem com emoticon: {0}", msnUser.Login));
                    }
                    else
                    {
                        Util.RegEvent(userLogin, "Tentativa de simular envio de emoticon sem login do sistema");
                    }

                    break;


                case MessengerLib.Handler.Action.EventRecEmoticonInMsg:

                    if (msnUser != null)
                    {
                        //auditoria
                        Util.RegEvent(msnUser.Login, "Recebeu mensagem com emoticon");
                        Console.WriteLine(String.Format("Recebeu mensagem com emoticon: {0}", msnUser.Login));
                    }
                    else
                    {
                        Util.RegEvent(userLogin, "Tentativa de simular recebimento de emoticon sem login do sistema");
                    }

                    break;
            }
        }

    }
}
