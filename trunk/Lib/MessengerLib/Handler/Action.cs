using System;

namespace MessengerLib.Handler
{
    public static class ActionHandler
    {

        private const char separator = '/';

        public static Action GetAction(string input)
        {
            string action = input.Split(separator)[0];
            return (Action)Enum.Parse(typeof(Action), action, true);
        }

        public static string GetMessage(string input)
        {
            return input.Split(new char[]{separator}, 2)[1];
        }

        public static string FormatAction(Action action, string message)
        {
            return String.Format("{0}{1}{2}", action.ToString(), separator, message);
        }

    }

    public enum Action
    {
        //TCP
         Login
        ,ConfirmLogin
        ,GetUsers
        ,GetUserInfo
        ,GetMsg
        ,GetFeatures
        ,GetLogDates
        ,GetLog
        ,UserAvailable

        //UDP
        ,Logoff
        ,AddContacts
        ,CreateAcc
        ,DeleteAcc
        ,UpdatePermissions
        ,UpdateAccMainInfo
        ,UpdateAccOtherInfo
        ,SendMsg
        ,EventSendEmoticonInMsg
        ,EventRecEmoticonInMsg
        ,EventInvalidPassword
        ,EventInvalidConfig
        ,EventExportContacts
        ,EventImportContactsFail
        ,SaveMotherBoardID
    }
}
