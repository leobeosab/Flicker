/*
 * Most of this code was ripped off of Stephen Toub's Article found here
 * https://learn.microsoft.com/en-us/archive/blogs/toub/low-level-keyboard-hook-in-c
 * Thanks Stephen!
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Flicker;

public class KeyboardHandler
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    private static HotKeySet hotkeySet;
    private static KeysConverter converter;

    private static WindowManager windowManager;

    public KeyboardHandler()
    {
        converter = new KeysConverter();
        windowManager = new WindowManager();
        hotkeySet = new HotKeySet(new List<HotKey>
        {
            new("LShiftKey"),
            new("LWin")
        });
    }
    
    public void Init()
    {
        _hookID = SetHook(_proc);
    }

    public void Destroy()
    {
        UnhookWindowsHookEx(_hookID);
    }
    
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static bool CheckForResize(Keys vkCode)
    {
        if (!hotkeySet.IsSetPressed())
        {
            return false;
        }

        string? keyValue = converter.ConvertToString((object)vkCode);
        return windowManager.HandleShortcuts(keyValue);
    }
    
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            hotkeySet.TriggerKey((Keys)vkCode, true);
            Debug.WriteLine((object)(Keys)vkCode + " Down");

            if (CheckForResize((Keys)vkCode))
            {
                return new IntPtr(1);
            }
        }
        else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            hotkeySet.TriggerKey((Keys)vkCode, false);
            Debug.WriteLine((object)(Keys)vkCode + " Up");
        }
        
        // in case we processed the message, prevent the system from passing the message to the rest of the hook chain
        // return result.ToInt32() == 0 ? CallNextHookEx(_hookId, nCode, wParam, lParam) : result;
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}