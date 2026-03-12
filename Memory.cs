using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AssaultCubeExternal
{
    public class Memory
    {
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, int dwSize, out int lpRead);
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            byte[] lpBuffer, int dwSize, out int lpWritten);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;
        private const int PROCESS_ACCESS = PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE;

        public IntPtr Handle          { get; private set; }
        public IntPtr BaseAddr        { get; private set; }
        public bool   IsAttached      => Handle != IntPtr.Zero;
        public string LastError       { get; private set; } = "";
        public bool   LastReadSuccess { get; private set; }

        public bool Attach(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) 
            { 
                LastError = "process not found"; 
                return false; 
            }

            var proc = processes[0];
            Handle = OpenProcess(PROCESS_ACCESS, false, proc.Id);
            
            if (Handle == IntPtr.Zero) 
            { 
                LastError = $"OpenProcess failed (err={GetLastError()}). Run as Administrator."; 
                return false; 
            }

            try   
            { 
                BaseAddr = proc.MainModule!.BaseAddress; 
            }
            catch (Exception ex) 
            { 
                LastError = $"MainModule exception: {ex.Message}"; 
                CloseHandle(Handle); 
                Handle = IntPtr.Zero; 
                return false; 
            }

            LastError = string.Empty;
            return true;
        }

        public void Detach() 
        { 
            if (!IsAttached) return;
            
            CloseHandle(Handle); 
            Handle = IntPtr.Zero; 
        }

        public int ReadInt(IntPtr addr)
        {
            var b = new byte[4];
            LastReadSuccess = ReadProcessMemory(Handle, addr, b, 4, out int n) && n == 4;
            return BitConverter.ToInt32(b, 0);
        }

        public float ReadFloat(IntPtr addr)
        {
            var b = new byte[4];
            LastReadSuccess = ReadProcessMemory(Handle, addr, b, 4, out int n) && n == 4;
            return BitConverter.ToSingle(b, 0);
        }

        public string ReadString(IntPtr addr, int len = 32)
        {
            var b = new byte[len];
            LastReadSuccess = ReadProcessMemory(Handle, addr, b, len, out int n) && n == len;
            return System.Text.Encoding.UTF8.GetString(b).TrimEnd('\0');
        }

        public void WriteInt(IntPtr addr, int val)
            => WriteProcessMemory(Handle, addr, BitConverter.GetBytes(val), 4, out _);

        public ViewMatrix ReadMatrix(IntPtr addr)
        {
            var b = new byte[64]; // 16 floats * 4 bytes
            ReadProcessMemory(Handle, addr, b, 64, out _);
            return new ViewMatrix
            {
                m11 = BitConverter.ToSingle(b,  0), m12 = BitConverter.ToSingle(b,  4),
                m13 = BitConverter.ToSingle(b,  8), m14 = BitConverter.ToSingle(b, 12),
                m21 = BitConverter.ToSingle(b, 16), m22 = BitConverter.ToSingle(b, 20),
                m23 = BitConverter.ToSingle(b, 24), m24 = BitConverter.ToSingle(b, 28),
                m31 = BitConverter.ToSingle(b, 32), m32 = BitConverter.ToSingle(b, 36),
                m33 = BitConverter.ToSingle(b, 40), m34 = BitConverter.ToSingle(b, 44),
                m41 = BitConverter.ToSingle(b, 48), m42 = BitConverter.ToSingle(b, 52),
                m43 = BitConverter.ToSingle(b, 56), m44 = BitConverter.ToSingle(b, 60),
            };
        }

        public void WriteFloat(IntPtr addr, float val)
            => WriteProcessMemory(Handle, addr, BitConverter.GetBytes(val), 4, out _);
    }
}
