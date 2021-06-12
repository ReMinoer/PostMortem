using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PostMortem.Windows.Dumper
{
    // http://www.debuginfo.com/articles/effminidumps.html
    // https://github.com/dotnet/diagnostics/blob/main/src/Tools/dotnet-dump/Dumper.Windows.cs

    static public class WindowsProcessDumper
    {
        static public void Dump(string dumpFilePath, ProcessDumpType type) => Dump(Process.GetCurrentProcess(), dumpFilePath, type);
        static public void Dump(Process process, string dumpFilePath, ProcessDumpType type)
        {
            try
            {
                using (var stream = new FileStream(dumpFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                {
                    Native.MINIDUMP_TYPE nativeDumpType = GetNativeDumpType(type);
                    IntPtr exceptionPointers = process.Id == Process.GetCurrentProcess().Id
                        ? GetExceptionPointers()
                        : IntPtr.Zero;

                    for (int i = 0; i < 5; i++)
                    {
                        if (exceptionPointers != IntPtr.Zero)
                        {
                            var exceptionInfo = new Native.MINIDUMP_EXCEPTION_INFORMATION
                            {
                                ThreadId = Native.GetCurrentThreadId(),
                                ExceptionPointers = exceptionPointers,
                                ClientPointers = false
                            };

                            if (Native.MiniDumpWriteDump(process.Handle, (uint)process.Id, stream.SafeFileHandle, nativeDumpType, ref exceptionInfo, IntPtr.Zero, IntPtr.Zero))
                                break;
                        }
                        else
                        {
                            if (Native.MiniDumpWriteDump(process.Handle, (uint)process.Id, stream.SafeFileHandle, nativeDumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
                                break;
                        }

                        // Retry the write dump on ERROR_PARTIAL_COPY
                        int err = Marshal.GetHRForLastWin32Error();
                        if (err != Native.ERROR_PARTIAL_COPY)
                            Marshal.ThrowExceptionForHR(err);
                    }
                }
            }
            catch (Exception e)
            {
                throw new ProcessDumpingException("An exception occurred during process dumping.", e);
            }
        }

        static private Native.MINIDUMP_TYPE GetNativeDumpType(ProcessDumpType dumpType)
        {
            switch (dumpType)
            {
                case ProcessDumpType.Full:
                    return Native.MINIDUMP_TYPE.MiniDumpWithFullMemory
                        | Native.MINIDUMP_TYPE.MiniDumpWithDataSegs
                        | Native.MINIDUMP_TYPE.MiniDumpWithHandleData
                        | Native.MINIDUMP_TYPE.MiniDumpWithUnloadedModules
                        | Native.MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo
                        | Native.MINIDUMP_TYPE.MiniDumpWithThreadInfo
                        | Native.MINIDUMP_TYPE.MiniDumpWithTokenInformation;
                case ProcessDumpType.Heap:
                    return Native.MINIDUMP_TYPE.MiniDumpWithPrivateReadWriteMemory
                        | Native.MINIDUMP_TYPE.MiniDumpWithDataSegs
                        | Native.MINIDUMP_TYPE.MiniDumpWithHandleData
                        | Native.MINIDUMP_TYPE.MiniDumpWithUnloadedModules
                        | Native.MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo
                        | Native.MINIDUMP_TYPE.MiniDumpWithThreadInfo
                        | Native.MINIDUMP_TYPE.MiniDumpWithTokenInformation;
                case ProcessDumpType.Mini:
                    return Native.MINIDUMP_TYPE.MiniDumpWithThreadInfo;
                default:
                    throw new NotSupportedException();
            }
        }

        static private IntPtr GetExceptionPointers()
        {
            // Not accessible directly in .NET Standard
            var method = typeof(Marshal).GetMethod(
                "GetExceptionPointers",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                null,
                Type.EmptyTypes,
                null
            );
            return (IntPtr?)method?.Invoke(null, null) ?? IntPtr.Zero;
        }

        // ReSharper disable All
        static private class Native
        {
            public const int ERROR_PARTIAL_COPY = unchecked((int)0x8007012b);

            [DllImport("Dbghelp.dll", SetLastError = true)]
            static public extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, SafeFileHandle hFile, MINIDUMP_TYPE DumpType, ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);
            [DllImport("Dbghelp.dll", SetLastError = true)]
            static public extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, SafeFileHandle hFile, MINIDUMP_TYPE DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

            [DllImport("Kernel32.dll")]
            public static extern uint GetCurrentThreadId();

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct MINIDUMP_EXCEPTION_INFORMATION
            {
                public uint ThreadId;
                public IntPtr ExceptionPointers;
                [MarshalAs(UnmanagedType.Bool)]
                public bool ClientPointers;
            }

            [Flags]
            public enum MINIDUMP_TYPE : uint
            {
                MiniDumpNormal = 0,
                MiniDumpWithDataSegs = 1 << 0,
                MiniDumpWithFullMemory = 1 << 1,
                MiniDumpWithHandleData = 1 << 2,
                MiniDumpFilterMemory = 1 << 3,
                MiniDumpScanMemory = 1 << 4,
                MiniDumpWithUnloadedModules = 1 << 5,
                MiniDumpWithIndirectlyReferencedMemory = 1 << 6,
                MiniDumpFilterModulePaths = 1 << 7,
                MiniDumpWithProcessThreadData = 1 << 8,
                MiniDumpWithPrivateReadWriteMemory = 1 << 9,
                MiniDumpWithoutOptionalData = 1 << 10,
                MiniDumpWithFullMemoryInfo = 1 << 11,
                MiniDumpWithThreadInfo = 1 << 12,
                MiniDumpWithCodeSegs = 1 << 13,
                MiniDumpWithoutAuxiliaryState = 1 << 14,
                MiniDumpWithFullAuxiliaryState = 1 << 15,
                MiniDumpWithPrivateWriteCopyMemory = 1 << 16,
                MiniDumpIgnoreInaccessibleMemory = 1 << 17,
                MiniDumpWithTokenInformation = 1 << 18,
                MiniDumpWithModuleHeaders = 1 << 19,
                MiniDumpFilterTriage = 1 << 20,
                MiniDumpWithAvxXStateContext = 1 << 21,
                MiniDumpWithIptTrace = 1 << 22,
                MiniDumpValidTypeFlags = (-1) ^ ((~1) << 22)
            }
        }
    }
}