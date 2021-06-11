using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PostMortem.Windows.MiniDump
{
    // https://github.com/Elskom/Sdk/blob/main/MiniDump/MiniDump/MiniDump.cs
    // http://www.debuginfo.com/articles/effminidumps.html

    static public class MiniDumpGenerator
    {
        static public void SaveUnhandledExceptionMiniDump(string outputPath, MiniDumpTypes dumpType)
        {
            var exceptionInformation = new MINIDUMP_EXCEPTION_INFORMATION
            {
                ClientPointers = false,
                ExceptionPointers = GetExceptionPointers(),
                ThreadId = SafeNativeMethods.GetCurrentThreadId(),
            };

            using (FileStream fileStream = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                _ = SafeNativeMethods.MiniDumpWriteDump(
                    new Microsoft.Win32.SafeHandles.SafeFileHandle(SafeNativeMethods.GetCurrentProcess(), true),
                    SafeNativeMethods.GetCurrentProcessId(),
                    fileStream.SafeFileHandle,
                    dumpType,
                    ref exceptionInformation,
                    default,
                    default);

                int error = Marshal.GetLastWin32Error();
                if (error > 0)
                    throw new MiniDumpGenerationException($"Mini dump generation failed with error code {error}!");
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
                null);
            return (IntPtr?)method?.Invoke(null, null) ?? IntPtr.Zero;
        }

        static private class SafeNativeMethods
        {
            [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            static public extern bool MiniDumpWriteDump(SafeHandle hProcess, uint processId, SafeHandle hFile, MiniDumpTypes DumpType, ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam, IntPtr UserStreamParam, IntPtr CallackParam);

            [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            static public extern uint GetCurrentThreadId();

            [DllImport("kernel32.dll", ExactSpelling = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            static public extern uint GetCurrentProcessId();

            [DllImport("kernel32.dll", ExactSpelling = true)]
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            static public extern IntPtr GetCurrentProcess();
        }

        // Pack=4 is important! So it works also for x64!
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct MINIDUMP_EXCEPTION_INFORMATION
        {
            internal uint ThreadId;
            internal IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool ClientPointers;
        }
    }
}