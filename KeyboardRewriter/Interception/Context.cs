using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace KeyboardRewriter.Interception
{
    public class Context : SafeHandleZeroOrMinusOneIsInvalid
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int InterceptionPredicate(int device);

        private static class NativeMethods
        {
            [DllImport("interception.dll", EntryPoint = "interception_create_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern Context CreateContext();

            [DllImport("interception.dll", EntryPoint = "interception_destroy_context", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyContext(IntPtr handle);


            [DllImport("interception.dll", EntryPoint = "interception_get_hardware_id", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint GetHardwareId(IntPtr handle, int device, [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 3)]
                StringBuilder hardwareIdBuffer, uint bufferSize);


            [DllImport("interception.dll", EntryPoint = "interception_set_filter", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SetFilter(IntPtr handle, InterceptionPredicate predicate, Filter filter);


            [DllImport("interception.dll", EntryPoint = "interception_is_invalid", CallingConvention = CallingConvention.Cdecl)]
            public static extern int IsInvalid(int device);

            [DllImport("interception.dll", EntryPoint = "interception_is_keyboard", CallingConvention = CallingConvention.Cdecl)]
            public static extern int IsKeyboard(int device);

            [DllImport("interception.dll", EntryPoint = "interception_is_mouse", CallingConvention = CallingConvention.Cdecl)]
            public static extern int IsMouse(int device);


            [DllImport("interception.dll", EntryPoint = "interception_wait", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Wait(IntPtr handle);

            [DllImport("interception.dll", EntryPoint = "interception_wait_with_timeout", CallingConvention = CallingConvention.Cdecl)]
            public static extern int WaitWithTimeout(IntPtr handle, ulong milliseconds);


            [DllImport("interception.dll", EntryPoint = "interception_send", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Send(IntPtr handle, int device, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                KeyStroke[] strokes, uint nStrokes);

            [DllImport("interception.dll", EntryPoint = "interception_receive", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Receive(IntPtr handle, int device, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                KeyStroke[] strokes, uint nStrokes);

            [DllImport("interception.dll", EntryPoint = "interception_send", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Send(IntPtr handle, int device, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                MouseStroke[] strokes, uint nStrokes);

            [DllImport("interception.dll", EntryPoint = "interception_receive", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Receive(IntPtr handle, int device, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
                MouseStroke[] strokes, uint nStrokes);
        }

        public static Context Create()
        {
            var context = NativeMethods.CreateContext();

            context.RetrieveHardwareIds();

            return context;
        }

        private Context() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.DestroyContext(handle);
            return true;
        }

        public const int MaxDevices = 20;

        private void RetrieveHardwareIds()
        {
            var hardwareIds = new List<(string HardwareId, int DeviceId)>();

            var stringBuilder = new StringBuilder(1024);

            for (int device = 1; device <= MaxDevices; device++)
            {
                stringBuilder.Clear();

                var result = NativeMethods.GetHardwareId(handle, device, stringBuilder, (uint) stringBuilder.Capacity);
                if (result == 0)
                    continue;

                hardwareIds.Add((HardwareId: stringBuilder.ToString(), DeviceId: device));
            }

            HardwareIds = hardwareIds.AsReadOnly();
        }

        public IReadOnlyCollection<(string HardwareId, int DeviceId)> HardwareIds { get; private set; }


        public void SetFilter(InterceptionPredicate devicePredicate, Filter filter)
        {
            NativeMethods.SetFilter(handle, devicePredicate, filter);
        }

        public DeviceIdentifier Wait()
        {
            return new DeviceIdentifier(NativeMethods.Wait(handle));
        }

        public DeviceIdentifier WaitWithTimeout(ulong milliseconds)
        {
            return new DeviceIdentifier(NativeMethods.WaitWithTimeout(handle, milliseconds));
        }

        public bool IsKeyboard(DeviceIdentifier deviceIdentifier)
        {
            return NativeMethods.IsKeyboard(deviceIdentifier.Id) != 0;
        }

        public bool IsMouse(DeviceIdentifier deviceIdentifier)
        {
            return NativeMethods.IsMouse(deviceIdentifier.Id) != 0;
        }

        public int Send(DeviceIdentifier deviceIdentifier, KeyStroke[] strokes)
        {
            return Send(deviceIdentifier, strokes, strokes.Length);
        }

        public int Send(DeviceIdentifier deviceIdentifier, KeyStroke[] strokes, int nStrokes)
        {
            return NativeMethods.Send(handle, deviceIdentifier.Id, strokes, (uint) nStrokes);
        }

        public int Receive(DeviceIdentifier deviceIdentifier, KeyStroke[] strokes)
        {
            return NativeMethods.Receive(handle, deviceIdentifier.Id, strokes, (uint) strokes.Length);
        }

        public int Send(DeviceIdentifier deviceIdentifier, MouseStroke[] strokes)
        {
            return Send(deviceIdentifier, strokes, strokes.Length);
        }

        public int Send(DeviceIdentifier deviceIdentifier, MouseStroke[] strokes, int nStrokes)
        {
            return NativeMethods.Send(handle, deviceIdentifier.Id, strokes, (uint) nStrokes);
        }

        public int Receive(DeviceIdentifier deviceIdentifier, MouseStroke[] strokes)
        {
            return NativeMethods.Receive(handle, deviceIdentifier.Id, strokes, (uint) strokes.Length);
        }
    }
}