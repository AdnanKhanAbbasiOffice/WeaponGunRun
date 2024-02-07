using System.Diagnostics.CodeAnalysis;

namespace Falcon.FalconCore.Scripts.Interfaces
{
    /**
     * Make sure the implement class is sealed and has a no arg constructor with preserve attribute, containing basic initialization.
     * The constructor is called in main thread.
     */
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IFEssential : IFMainInit
    {
        /// <summary>
        /// Called before all other FMainObj.OnGameContinue.
        /// Called in main thread.
        /// </summary>
        void OnPreContinue();

        /// <summary>
        /// Called after all other FMainObj.OnGameStop.
        /// Called in main thread.
        /// </summary>
        void OnPostStop();
    }
}

