using System;
using Cito.Cat.Core.Interfaces;

namespace Cito.Cat.Core.Helpers
{
    public class GoodTime : IGoodTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}