using System;
using System.Collections.Generic;

namespace MessengerLib.Core
{
    public class MSNFeature
    {
        public MSNFeature()
        {
            this.ListOperation = new List<MSNOperation>();
        }

        public int ID { get; set; }
        public Feature Name { get; set; }
        public IList<MSNOperation> ListOperation { get; set; } 
    }
}
