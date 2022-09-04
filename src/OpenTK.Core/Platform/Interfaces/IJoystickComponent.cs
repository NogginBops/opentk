using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Core.Platform
{
    public interface IJoystickComponent : IPalComponent
    {
        /// <summary>
        /// The number of Joysticks that can be used simultainiously.
        /// </summary>
        int MaxSupportedJoysticks { get; }

        /// <summary>
        /// Fills a span with the currently active Joystick handles.
        /// These handles are only valid until the next
        /// FIXME: cref to connected and disconnected events.
        /// <para/>
        /// Pass <c>default</c> to get the number of active joysticks.
        /// </summary>
        /// <param name="handle">A span of handles to fill.</param>
        /// <returns>
        /// The number of handles placed into the span
        /// or the number of active joysticks if <c>default</c> is passed as <paramref name="handle"/>.
        /// </returns>
        int GetActiveJoysticks(Span<JoystickHandle> handle);

        string GetJoystickName(JoystickHandle handle);
    }
}
