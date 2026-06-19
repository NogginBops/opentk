using OpenTK.Core.Utility;

namespace OpenTK.Platform
{
    /// <summary>
    /// Common interface for PAL2 extension classes.
    /// </summary>
    public interface IPalExtension
    {
        /// <summary>
        /// Name of the extension.
        /// </summary>
        /// <remarks>
        /// Although not strictly enforced, keep extension to match the regex /\w+_\w+/ much like Khronos extension
        /// strings. OTK_ and EXT_ are reserved for OpenTK use.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// The logger for this extension.
        /// </summary>
        /// <seealso cref="ILogger"/>
        /// <seealso cref="ToolkitOptions.Logger"/>
        ILogger? Logger { get; set; }

        /// <summary>
        /// Initialize the extension.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uninitialize the extension.
        /// </summary>
        void Uninitialize();
    }
}
