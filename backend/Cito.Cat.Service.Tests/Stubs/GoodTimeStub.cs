using System;
using System.Diagnostics.CodeAnalysis;
using Cito.Cat.Core.Interfaces;

namespace Cito.Cat.Service.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class GoodTimeStub : IGoodTime
    {
        private DateTime _now;

        public GoodTimeStub()
        {
            _now = DateTime.Now;
        }

        public DateTime Now()
        {
            return _now;
        }

        public void TravelTo(DateTime theTime)
        {
            _now = theTime;
        }
    }
}