using System;
using System.Collections.Generic;
using System.Text;


namespace AutoIt.Windows
{
    public class Management
    {
        public static IEnumerable<IntPtr> FindWindows(NativeMethods.EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            var windows = new List<IntPtr>();

            NativeMethods.EnumWindows(
                delegate(IntPtr wnd, IntPtr param)
                {
                    if (filter(wnd, param))
                    {
                        // only add the windows that pass the filter
                        windows.Add(wnd);
                    }

                    // but return true here so that we iterate all windows
                    return true;
                },
                IntPtr.Zero);

            return windows;
        }

        public static IEnumerable<IntPtr> FindWindowsWithClass(string className)
        {
            return FindWindows(
                delegate(IntPtr wnd, IntPtr param)
                {
                    var str = new StringBuilder(1024 + 1);
                    NativeMethods.GetClassName(wnd, str, str.Capacity);

                    return str.ToString() == className;
                });
        }

        public static IEnumerable<IntPtr> FindWindowsWithTitle(string title)
        {
            return FindWindows(
                delegate(IntPtr hWnd, IntPtr param)
                {
                    var builder = new StringBuilder(1024 + 1);
                    NativeMethods.GetClassName(hWnd, builder, builder.Capacity);

                    return GetWindowText(hWnd) == title;
                });
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int size = NativeMethods.GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                NativeMethods.GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return string.Empty;
        }
    }
}