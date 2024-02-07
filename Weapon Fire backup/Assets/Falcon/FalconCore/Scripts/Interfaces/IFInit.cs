using System.Collections;

namespace Falcon.FalconCore.Scripts.Interfaces
{
    /// <summary>
    /// Make sure the implement class is sealed and has a no arg constructor with preserve attribute, containing basic initialization.
    /// The constructor is called in side thread.
    /// </summary>
    public interface IFInit : IFMainInit
    {
        IEnumerator Init();
    }
}