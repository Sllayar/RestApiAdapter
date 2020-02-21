using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FavoritMotors.Integration.RestApiAdapter
{
    public interface IExceptionHendler
    {
        bool Process(Exception ex);
    }
}
