//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable All

namespace AutoIt.Windows
{
    internal static class NativeMethods
    {
        internal static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        internal const uint SWP_NOSIZE = 0x0001;
        internal const uint SWP_NOMOVE = 0x0002;
        internal const uint SWP_NOACTIVATE = 0x0010;

        internal const int WM_GETTEXT = 0x000D;
        internal const int WM_GETTEXTLENGTH = 0x000E;

        internal const int SW_HIDE = 0;
        internal const int SW_SHOWNORMAL = 1;
        internal const int SW_SHOWMINIMIZED = 2;
        internal const int SW_SHOWMAXIMIZED = 3;
        internal const int SW_SHOWNOACTIVATE = 4;
        internal const int SW_RESTORE = 9;
        internal const int SW_SHOWDEFAULT = 10;

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("User32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        [DllImport("User32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

        [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
        internal static extern void GetClassName(IntPtr hWnd, StringBuilder s, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);
    }
}