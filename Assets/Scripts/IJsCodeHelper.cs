using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// This is only for JS (mobile) because keyboard does not show up
    /// so this is to get the query param from JS code to C# code and insert game code into
    /// the game code input field
    /// </summary>
    public interface IJsCodeHelper : IDisposable
    {
        void GetCode();
    }
}
