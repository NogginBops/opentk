using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace OpenTK.Platform.Native.X11
{
    internal static class Linux
    {
        // Only valid since Linux 2.3.48
        internal unsafe struct sysinfo_struct {
            public long uptime;             /* Seconds since boot */
            public nuint loads0;  /* 1, 5, and 15 minute load averages */
            public nuint loads1;  /* 1, 5, and 15 minute load averages */
            public nuint loads2;  /* 1, 5, and 15 minute load averages */
            public nuint totalram;  /* Total usable main memory size */
            public nuint freeram;   /* Available memory size */
            public nuint sharedram; /* Amount of shared memory */
            public nuint bufferram; /* Memory used by buffers */
            public nuint totalswap; /* Total swap space size */
            public nuint freeswap;  /* Swap space still available */
            public ushort procs;    /* Number of current processes */
            public nuint totalhigh; /* Total high memory size */
            public nuint freehigh;  /* Available high memory size */
            public int mem_unit;    /* Memory unit size in bytes */
            // FIXME: This is not correct!! sizeof(long) translates to sizeof(nint) in c# which can't be put in the fixed expression.
            //public fixed byte _f[20 - 2 * sizeof(ulong) - sizeof(int)];
            /* Padding to 64 bytes */
        }

        [DllImport("libc", SetLastError = true)]
        internal static extern int sysinfo(out sysinfo_struct info);

        [DllImport("libc")]
        internal static unsafe extern byte* strerror(int errnum);

        // FIXME: Can we guarantee it's a uint on all platforms?
        [Flags]
        internal enum mode_t : uint
        {
            /* RWX mask for owner */
            S_IRWXU = 0x01C0,
            /* R for owner */
            S_IRUSR = 0x0100,
            /* W for owner */
            S_IWUSR = 0x0080,
            /* X for owner */
            S_IXUSR = 0x0040,
            /* RWX mask for group */
            S_IRWXG = 0x0038,
            /* R for group */
            S_IRGRP = 0x0020,
            /* W for group */
            S_IWGRP = 0x0010,
            /* X for group */
            S_IXGRP = 0x0008,
            /* RWX mask for other */
            S_IRWXO = 0x0007,
            /* R for other */
            S_IROTH = 0x0004,
            /* W for other */
            S_IWOTH = 0x0002,
            /* X for other */
            S_IXOTH = 0x0001,
            /* set user id on execution */
            S_ISUID = 0x0800,
            /* set group id on execution */
            S_ISGID = 0x0400,
            /* save swapped text even after use */
            S_ISVTX = 0x0200,
        }

        internal enum file_flags : int 
        {
            O_ACCMODE = 0x0000_0003,
            O_RDONLY = 0x0000_0000,
            O_WRONLY = 0x0000_0001,
            O_RDWR =  0x0000_0002,
            /* not fcntl */
            O_CREAT = 0x0000_0040,
            /* not fcntl */
            O_EXCL = 0x0000_0080,
            /* not fcntl */
            O_NOCTTY = 0x0000_0100,
            /* not fcntl */
            O_TRUNC = 0x0000_0200,
            O_APPEND = 0x0000_0400,
            O_NONBLOCK = 0x0000_0800,
            /* used to be O_SYNC, see below */
            O_DSYNC = 0x0000_1000,
            /* fcntl, for BSD compatibility */
            FASYNC = 0x0000_2000,
            /* direct disk access hint */
            O_DIRECT = 0x0000_4000,
            O_LARGEFILE = 0x0000_8000,
            /* must be a directory */
            O_DIRECTORY = 0x0001_0000,
            /* don't follow links */
            O_NOFOLLOW = 0x0002_0000,
            O_NOATIME = 0x0004_0000,
            /* set close_on_exec */
            O_CLOEXEC = 0x0008_0000,
        }

        internal static unsafe int open(ReadOnlySpan<byte> pathname, file_flags flags, mode_t mode)
        {
            fixed (byte* pathnamePtr = pathname)
            {
                return open(pathnamePtr, flags, mode);
            }

            [DllImport("libc")]
            static extern int open(byte* pathname, file_flags flags, mode_t mode);
        }

        [DllImport("libc")]
        internal static extern int open(IntPtr pathname, file_flags flags, mode_t mode);

        [DllImport("libc")]
        internal static extern int close(int fd);

        [DllImport("libc")]
        internal static unsafe extern nint write(int fd, void* data, nuint size);

        // FIXME: This struct is very likely to break on 32 vs 64 bit
        // and it might break between different linux distros....
        // - Noggin_bops 2024-10-30
        internal struct stat_t
        {
            /* ID of device containing file */
            public long /* dev_t */ st_dev;
            /* inode number */
            public ulong /* ino_t */ st_ino;
            /* protection */
            public mode_t st_mode;
            /* number of hard links */
            public ulong /* nlink_t */ st_nlink;
            /* user ID of owner */
            public uint /* uid_t */ st_uid;
            /* group ID of owner */
            public uint /* gid_t */ st_gid;
            /* device ID (if special file) */
            public long /* dev_t */ st_rdev;
            /* total size, in bytes */
            public long /* off_t */ st_size;
            /* blocksize for file system I/O */
            public ulong /* blksize_t */ st_blksize;
            /* number of 512B blocks allocated */
            public ulong /* blkcnt_t */ st_blocks;
            /* time of last access */
            public ulong /* time_t */ st_atime;
            /* time of last modification */
            public ulong /* time_t */ st_mtime;
            /* time of last status change */
            public ulong /* time_t */ st_ctime;
        };

        [DllImport("libc")]
        internal static unsafe extern int fstat(int fd, out stat_t buf);
    }
}
