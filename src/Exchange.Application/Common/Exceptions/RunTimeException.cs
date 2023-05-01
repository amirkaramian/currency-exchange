using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Application.Common.Exceptions;
public class RunTimeException : Exception
{
    public RunTimeException()
       : base()
    {
    }

    public RunTimeException(string message)
        : base(message)
    {
    }

    public RunTimeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
