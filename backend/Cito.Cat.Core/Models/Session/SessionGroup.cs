using System;
using System.Collections.Generic;

namespace Cito.Cat.Core.Models.Session
{
    public class SessionGroup 
    {
        public Guid Id { get; set; }
        
        public List<string> ItemIds { get; set; }

        public SessionGroup()
        {
            ItemIds = new List<string>();
        }
    }
}