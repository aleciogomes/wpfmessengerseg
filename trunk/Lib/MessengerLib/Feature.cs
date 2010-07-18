using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessengerLib
{
    public class FeatureHandler
    {
        public static Feature GetFeature(string feature)
        {
            return (Feature)Enum.Parse(typeof(Feature), feature, true);
        }
    }

    public enum Feature
    {
         Main
        ,Chat
    }

}
