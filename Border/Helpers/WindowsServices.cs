using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Border.Helpers
{
    public static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);



        const int VK_OEM_4 = 0xDB; // [
        const int VK_OEM_5 = 0xDC; // \
        const int VK_OEM_6 = 0xDD; // ]


        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hwnd, int index, int modifiers, int hotkey);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hwnd, int index);

        public static void SetWindowExTransparent(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            SetWindowExTransparent(hwnd);
        }
        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            WindowTransparent = true;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        internal static void Initialize(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            var hwndSource = HwndSource.FromHwnd(hwnd);
            hwndSource.AddHook(HotKeyHook);
        }

        public static bool WindowTransparent { get; private set; } = false;
        public static void UnsetWindowExTransparent(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            UnsetWindowExTransparent(hwnd);
        }
        public static void UnsetWindowExTransparent(IntPtr hwnd)
        {
            WindowTransparent = false;
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
        }

        public static IntPtr HotKeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    //switch (wParam.ToInt32())
                    //{
                        //case 0:
                        //case 1:
                            var k = new Hotkey(
                                    (lParam.ToInt32() >> 16), 
                                    (lParam.ToInt32() & MOD_ALT) > 0, 
                                    (lParam.ToInt32() & MOD_SHIFT) > 0, 
                                    (lParam.ToInt32() & MOD_CONTROL) > 0
                                );
                    var a= (lParam.ToInt32() & MOD_ALT) > 0;
                    var b =(lParam.ToInt32() & MOD_SHIFT) > 0;
                    var c =(lParam.ToInt32() & MOD_CONTROL) > 0;
                            if (Hotkeys.ContainsKey(k))
                            {
                                Hotkeys[k].Callback.Invoke();
                            }
                            handled = true; // TODO: Find a way to not always capture the hotkey
                            break;
                    //}break;
            }
            return IntPtr.Zero;
        }
        const int MOD_NOREPEAT = 0x4000;
        const int MOD_ALT = 0x0001;
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int MOD_WIN = 0x0008;

        public struct Hotkey
        {


            public int Key;
            public int Mod;

            public Hotkey(int key, bool alt, bool shift, bool ctrl)
            {
                this.Key = key;
                Mod = MOD_NOREPEAT;
                if (alt)
                {
                    Mod |= MOD_ALT;
                }
                if (ctrl)
                {
                    Mod |= MOD_CONTROL;
                }
                if (shift)
                {
                    Mod |= MOD_SHIFT;
                }
            }
        }

        public delegate void HotkeyActivated();
        private struct HotkeyData
        {
            public HotkeyActivated Callback;
            public int ID;
        }
        private static Dictionary<Hotkey, HotkeyData> Hotkeys = new Dictionary<Hotkey, HotkeyData>();

        private static int nextKeyId = 0;
        public static int RegisterKey(Hotkey hotkey, HotkeyActivated callback, Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (RegisterHotKey(hwnd, nextKeyId, hotkey.Mod, hotkey.Key))
            {
                Hotkeys.Add(hotkey, new HotkeyData { Callback = callback, ID = nextKeyId });
            }
            else
            {
                return -1;
            }
            return nextKeyId++; // return then increase
        }

        public static bool UnregisterKey(Hotkey hotkey, Window window)
        {
            if (Hotkeys.ContainsKey(hotkey))
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                return UnregisterHotKey(hwnd, Hotkeys[hotkey].ID);
            }
            return false;
        }

        public static void UnregisterAllKeys(Window window)
        {

            foreach(var kv in Hotkeys)
            {
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                var asdf = UnregisterHotKey(hwnd, kv.Value.ID);
            }
        }
    }


    /// <summary>
    /// v0.1:
    /// Read and show Build orders (from JSON )
    /// 
    /// Add Warning for bans
    /// Custom Keys
    /// Save Window settings (location and size)
    /// close button
    /// helper texts
    /// Change background color and transparency
    /// v0.2:
    /// Timer, with error margin, will blink when past time
    /// Options menu
    /// Remote locations (e.g. github gist/pastebin gist)
    /// v0.3+
    /// Build Order Editor
    /// Build Order Verifier (timings, and costs)
    /// Read game state
    /// Conditions & Automatic advancing current goal
    /// Helpful stats
    /// APM
    /// Vertical Layout
    /// Records (speedrunner mode)
    /// Icons
    /// </summary>
}