/*
 *ALUNOS:
 * - Alécio J. Gomes Neto
 * - Luiz Diego Aquino
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Atividade01Cliente
{
    public class Server
    {

        private const string password = "senhaForte123@!@#$";
        private const string user = "usuario01";

        /*
         * CONEXÃO CLIENTE/SERVIDOR
         * {
        */

            static void Main(string[] args)
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Any, 1012);
                tcpListener.Start();

                while (true)
                {
                    //aguarda a conexão do cliente
                    TcpClient client = tcpListener.AcceptTcpClient();

                    //trata conexão com cliente
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(client);
                }
            }

            static void HandleClient(object client)
            {
                TcpClient tcpClient = (TcpClient)client;
                NetworkStream clientStream = tcpClient.GetStream();

                byte[] message = new byte[4096];
                int qtdBytes;

                while (true)
                {
                    qtdBytes = 0;

                    try
                    {
                        //aguarda receber a mensagem
                        qtdBytes = clientStream.Read(message, 0, 4096);
                    }
                    catch
                    {
                        break;
                    }

                    if (qtdBytes == 0)
                    {
                        //cliente desconectado
                        break;
                    }

                    //mensagem recebida
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string input = encoder.GetString(message, 0, qtdBytes);

                    bool valid = ValidateUser(input.Split(':')[0], input.Split(':')[1]);

                    //resposta do servidor
                    SendAnswer(valid, tcpClient);
                }

                tcpClient.Close();
            }

            static void SendAnswer(bool validLogin, TcpClient tcpClient)
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                String serverAnswer = String.Empty;

                if (validLogin)
                {
                    serverAnswer = "Bem vindo!";
                }
                else
                {
                    serverAnswer = "Usuario/senha invalidos";
                }

                NetworkStream stream = tcpClient.GetStream();
                byte[] buffer = encoder.GetBytes(serverAnswer);

                //envia Resposta
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }

        /*
         *}
        */

        /*
         * VALIDAÇÃO SENHA
         * {
        */
            static bool ValidateUser(string inputUser, string inputPass)
            {
                string validPassword = GenerateMD5(password);

                if (inputUser.Equals(user) && inputPass.Equals(validPassword))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            static string GenerateMD5(string input)
            {
                //calcula o MD5 hash a partir da string
                MD5 md5 = MD5.Create();
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                byte[] hash = md5.ComputeHash(bytes);

                //converte o array de bytes em uma string haxadecimal
                StringBuilder finalString = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    finalString.Append(hash[i].ToString("X2"));
                }

                return finalString.ToString();
            }
        /*
         * }
        */

    }
}
