using System;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Native.X11
{
    struct UdevPtr
    {
        public IntPtr Value;
    }

    struct UdevMonitorPtr
    {
        public IntPtr Value;
    }

    struct UdevEnumeratePtr
    {
        public IntPtr Value;
    }

    struct UdevListEntryPtr
    {
        public IntPtr Value;
    }

    struct UdevDevicePtr
    {
        public IntPtr Value;
    }

    internal static class Libudev
    {
        private const string UDEV = "udev";

        static Libudev()
        {
            DllResolver.InitLoader();
        }

        internal static unsafe int strlen(byte* str)
        {
            if (str == null)
                return 0;

            int length = 0;
            while(str[length++] != 0);

            return length - 1;
        }

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_new();

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_ref(UdevPtr udev);
 
        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_unref(UdevPtr udev);

        // udev_monitor

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevMonitorPtr udev_monitor_ref(UdevMonitorPtr udev_monitor);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevMonitorPtr udev_monitor_unref(UdevMonitorPtr udev_monitor);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_monitor_get_udev(UdevMonitorPtr udev_monitor);

        internal static unsafe UdevMonitorPtr udev_monitor_new_from_netlink(UdevPtr udev, ReadOnlySpan<byte> name)
        {
            fixed (byte* namePtr = name)
            {
                return udev_monitor_new_from_netlink(udev, namePtr);
            }

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern UdevMonitorPtr udev_monitor_new_from_netlink(UdevPtr udev, byte* name);
        }

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_monitor_enable_receiving(UdevMonitorPtr udev_monitor);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_monitor_set_receive_buffer_size(UdevMonitorPtr udev_monitor, int size);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_monitor_get_fd(UdevMonitorPtr udev_monitor);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevDevicePtr udev_monitor_receive_device(UdevMonitorPtr udev_monitor);

        internal static unsafe int udev_monitor_filter_add_match_subsystem_devtype(UdevMonitorPtr udev_monitor, ReadOnlySpan<byte> subsystem, ReadOnlySpan<byte> devtype)
        {
            fixed (byte* subsystemPtr = subsystem)
            fixed (byte* devtypePtr = devtype)
            {
                return udev_monitor_filter_add_match_subsystem_devtype(udev_monitor, subsystemPtr, devtypePtr);
            }

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern int udev_monitor_filter_add_match_subsystem_devtype(UdevMonitorPtr udev_monitor, byte* subsystem, byte* devtype);
        }

        internal static unsafe int udev_monitor_filter_add_match_tag(UdevMonitorPtr udev_monitor, ReadOnlySpan<byte> tag)
        {
            fixed (byte* tagPtr = tag)
            {
                return udev_monitor_filter_add_match_tag(udev_monitor, tagPtr);
            }

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern int udev_monitor_filter_add_match_tag(UdevMonitorPtr udev_monitor, byte* tag);
        }

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_monitor_filter_update(UdevMonitorPtr udev_monitor);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_monitor_filter_remove(UdevMonitorPtr udev_monitor);

        // udev_enumerate

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]  
        internal static extern UdevEnumeratePtr udev_enumerate_ref(UdevEnumeratePtr udev_enumerate);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevEnumeratePtr udev_enumerate_unref(UdevEnumeratePtr udev_enumerate);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_enumerate_get_udev(UdevEnumeratePtr udev_enumerate);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevEnumeratePtr udev_enumerate_new(UdevPtr udev);

        internal static unsafe int udev_enumerate_add_match_subsystem(UdevEnumeratePtr udev_enumerate, ReadOnlySpan<byte> subsystem)
        {
            fixed (byte* subsystemPtr = subsystem)
            {
                return udev_enumerate_add_match_subsystem(udev_enumerate, subsystemPtr);
            }

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static unsafe extern int udev_enumerate_add_match_subsystem(UdevEnumeratePtr udev_enumerate, byte* subsystem);
        }

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int udev_enumerate_scan_devices(UdevEnumeratePtr udev_enumerate);

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevListEntryPtr udev_enumerate_get_list_entry(UdevEnumeratePtr udev_enumerate);

        // udev_list

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevListEntryPtr udev_list_entry_get_next(UdevListEntryPtr list_entry);
        internal static unsafe UdevListEntryPtr udev_list_entry_get_by_name(UdevListEntryPtr list_entry, ReadOnlySpan<byte> name)
        {
            fixed (byte* namePtr = name)
            {
                return udev_list_entry_get_by_name(list_entry, namePtr);
            }

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static unsafe extern UdevListEntryPtr udev_list_entry_get_by_name(UdevListEntryPtr list_entry, byte* name);
        }
        internal static unsafe string? udev_list_entry_get_name(UdevListEntryPtr list_entry)
        {
            byte* retPtr = udev_list_entry_get_name(list_entry);
            string? str = Marshal.PtrToStringUTF8((IntPtr)retPtr);
            return str;

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern byte* udev_list_entry_get_name(UdevListEntryPtr list_entry);
        }
        internal static unsafe string? udev_list_entry_get_value(UdevListEntryPtr list_entry)
        {
            byte* retPtr = udev_list_entry_get_value(list_entry);
            string? str = Marshal.PtrToStringUTF8((IntPtr)retPtr);
            return str;

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern byte* udev_list_entry_get_value(UdevListEntryPtr list_entry);
        }
    
        // udev_device

        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevDevicePtr udev_device_ref(UdevDevicePtr udev_device);
        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevDevicePtr udev_device_unref(UdevDevicePtr udev_device);
        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern UdevPtr udev_device_get_udev(UdevDevicePtr udev_device);

        internal static unsafe UdevDevicePtr udev_device_new_from_syspath(UdevPtr udev, string syspath)
        {
            byte* syspathPtr = (byte*)Marshal.StringToCoTaskMemUTF8(syspath);
            UdevDevicePtr ret = udev_device_new_from_syspath(udev, syspathPtr);
            Marshal.FreeCoTaskMem((IntPtr)syspathPtr);
            return ret;

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern UdevDevicePtr udev_device_new_from_syspath(UdevPtr udev, byte* syspath);
        }
    
        [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern IntPtr /* const char* */ udev_device_get_devnode(UdevDevicePtr udev_device);

        internal static unsafe ReadOnlySpan<byte> udev_device_get_property_value(UdevDevicePtr udev_device, ReadOnlySpan<byte> key)
        {
            byte* retPtr;
            fixed (byte* keyPtr = key)
            {
                retPtr = udev_device_get_property_value(udev_device, keyPtr);
            }

            if (retPtr == null)
                return ReadOnlySpan<byte>.Empty;
            else
                return new ReadOnlySpan<byte>(retPtr, strlen(retPtr));
            
            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern byte* udev_device_get_property_value(UdevDevicePtr udev_device, byte* key);
        }
    
        internal static unsafe ReadOnlySpan<byte> udev_device_get_action(UdevDevicePtr udev_device)
        {
            byte* ret = udev_device_get_action(udev_device);

            if (ret == null)
                return ReadOnlySpan<byte>.Empty;
            else
                return new ReadOnlySpan<byte>(ret, strlen(ret));

            [DllImport(UDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern byte* udev_device_get_action(UdevDevicePtr udev_device);
        }
    }
}