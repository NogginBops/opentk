using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Platform
{
    /// <summary>
    /// Interface for interacting with joysticks.
    /// </summary>
    /// <seealso cref="Toolkit.Joystick"/>
    public interface IJoystickComponent : IPalComponent
    {
        // FIXME: Should we have separate gamepad and joystick components?

        // FIXME: Document the expectations for this API, and what all of the functions are supposed to do.

        // FIXME: Add some way to query if joysticks are connected or not?

        // FIXME: Get joystick from player index (and make index different from player index?)

        // FIXME: Way to query/enumerate all of the joysticks currently connected.

        /// <summary>
        /// The recommended deadzone value for the left analog stick.
        /// </summary>
        public float LeftDeadzone { get; }

        /// <summary>
        /// The recommended deadzone value for the right analog stick.
        /// </summary>
        public float RightDeadzone { get; }

        /// <summary>
        /// The recommended threshold for considering the left or right trigger pressed.
        /// </summary>
        public float TriggerThreshold { get; }

        /// <summary>
        /// Checks wether a joystick with the specific index is present on the system or not.
        /// </summary>
        /// <param name="index">The index of the joystick.</param>
        /// <returns>If a joystick with the specified index is connected.</returns>
        public bool IsConnected(int index);

        /// <summary>
        /// Opens a handle to a specific joystick.
        /// </summary>
        /// <param name="index">The player index of the joystick to open.</param>
        /// <returns>The opened joystick handle.</returns>
        public JoystickHandle Open(int index);

        /// <summary>
        /// Closes a handle to a joystick.
        /// </summary>
        /// <param name="handle">The joystick handle.</param>
        public void Close(JoystickHandle handle);

        public Guid GetGuid(JoystickHandle handle);

        /// <summary>
        /// Gets a human readable name for the joystick.
        /// </summary>
        /// <param name="handle">The joystick to get the name of.</param>
        /// <returns>The human readable name of the joystick.</returns>
        public string GetName(JoystickHandle handle);

        /// <summary>
        /// Gets the value of a specific joystick axis.
        /// This value is in the range [-1, 1] for analog sticks, and [0, 1] for triggers.
        /// FIXME: Document what direction negative and positive directions go...
        /// </summary>
        /// <param name="handle">A handle to a joystick.</param>
        /// <param name="axis">The joystick axis to get.</param>
        /// <returns>The joystick axis value.</returns>
        public float GetAxis(JoystickHandle handle, JoystickAxis axis);

        /// <summary>
        /// Get the pressed state of a specific joystick button.
        /// </summary>
        /// <param name="handle">A handle to a joystick.</param>
        /// <param name="button">The joystick button to get.</param>
        /// <returns>True if the specified button is pressed or false if the button is released.</returns>
        public bool GetButton(JoystickHandle handle, JoystickButton button);

        public bool SetVibration(JoystickHandle handle, float lowFreqIntensity, float highFreqIntensity);

        public bool TryGetBatteryInfo(JoystickHandle handle, out GamepadBatteryInfo batteryInfo);


        /*
        SDL_GUID SDL_CreateJoystickGUID(Uint16 bus, Uint16 vendor, Uint16 product, Uint16 version, const char *vendor_name, const char *product_name, Uint8 driver_signature, Uint8 driver_data)
        {
            SDL_GUID guid;
            Uint16 *guid16 = (Uint16 *)guid.data;
            Uint16 crc = 0;

            SDL_zero(guid);

            if (vendor_name && *vendor_name && product_name && *product_name) {
                crc = SDL_crc16(crc, vendor_name, SDL_strlen(vendor_name));
                crc = SDL_crc16(crc, " ", 1);
                crc = SDL_crc16(crc, product_name, SDL_strlen(product_name));
            } else if (product_name) {
                crc = SDL_crc16(crc, product_name, SDL_strlen(product_name));
            }

            // We only need 16 bits for each of these; space them out to fill 128.
            // Byteswap so devices get same GUID on little/big endian platforms.
            *guid16++ = SDL_Swap16LE(bus);
            *guid16++ = SDL_Swap16LE(crc);

            if (vendor) {
                *guid16++ = SDL_Swap16LE(vendor);
                *guid16++ = 0;
                *guid16++ = SDL_Swap16LE(product);
                *guid16++ = 0;
                *guid16++ = SDL_Swap16LE(version);
                guid.data[14] = driver_signature;
                guid.data[15] = driver_data;
            } else {
                size_t available_space = sizeof(guid.data) - 4;

                if (driver_signature) {
                    available_space -= 2;
                    guid.data[14] = driver_signature;
                    guid.data[15] = driver_data;
                }
                if (product_name) {
                    SDL_strlcpy((char *)guid16, product_name, available_space);
                }
            }
            return guid;
        }
        */
        internal static Guid CreateSDLCompatibleJoystickGUID(ushort bus, ushort vendor, ushort product, ushort version)
        {
            Span<byte> guid = stackalloc byte[16];
            Span<ushort> guidu16 = MemoryMarshal.Cast<byte, ushort>(guid);

            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(0), bus);
            // FIXME: SDL uses a crc16 of the product name here??
            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(2), 0);

            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(4), vendor);
            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(6), 0);
            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(8), product);
            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(10), 0);
            BinaryPrimitives.WriteUInt16LittleEndian(guid.Slice(12), version);
            guid[14] = 0;
            guid[15] = 0;

            return new Guid(guid);
        }
    }
}
