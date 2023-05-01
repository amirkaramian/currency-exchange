using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Exceptions;
internal class UnsupportedCurrencyException : Exception
{
    public UnsupportedCurrencyException(string code)
        : base($"Currency \"{code}\" is unsupported.")
    {
    }
}
