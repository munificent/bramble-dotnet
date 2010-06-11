using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bramble.Core
{
    //### bob: need to implement other ctors
    public class UnexpectedEnumValueException : Exception
    {
        public UnexpectedEnumValueException(object value)
            : base("The enum value \"" + value.ToString() + "\" in type \"" + value.GetType().Name + "\" was not expected.")
        {
        }
    }
}
