﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Flicker;

public class WindowManager
{
    
   [DllImport("user32.dll")]
   static extern IntPtr GetForegroundWindow();

   [DllImport("user32.dll")]
   static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
   
   [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
   public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


   private string GetWindowTitle(IntPtr windowHandle)
   {
      const int nChars = 256;
      StringBuilder buff = new StringBuilder(nChars);

      if (GetWindowText(windowHandle, buff, nChars) > 0)
      {
         return buff.ToString();
      }

      return "";
   }

   private Screen GetScreenSize(IntPtr hwnd)
   {
      return Screen.FromHandle(hwnd);
   }

   private void SetWindowSize(double x, double y, double widthPercent, double heightPercent)
   {
      const short SWP_NOMOVE = 0X2;
      const short SWP_NOSIZE = 1;
      const short SWP_NOZORDER = 0X4;
      const int SWP_SHOWWINDOW = 0x0040;
      
      var handle = GetForegroundWindow();
      var screen = GetScreenSize(handle);

      var width = screen.WorkingArea.Width;
      var height = screen.WorkingArea.Height;
      
      Debug.WriteLine($"Width: ${width} Height: ${height}");

      int boundsX = screen.Bounds.X;
      int boundsY = screen.Bounds.Y;

      if (handle != IntPtr.Zero)
      {
         Debug.WriteLine($"Modifying window {GetWindowTitle(handle)}");
         int flags = SWP_NOZORDER | SWP_SHOWWINDOW;
         if (widthPercent == 0 || heightPercent == 0)
         {
            flags |= SWP_NOSIZE;
         }
         
         SetWindowPos(handle, 0, (int)Math.Floor(width * x) + boundsX, (int)Math.Floor(height * y) + boundsY, (int)Math.Ceiling(widthPercent * width), (int) Math.Ceiling(height * heightPercent), flags);
      }
   }

   public bool HandleShortcuts(string keyCode)
   {
      switch (keyCode)
      {
         // 1st, 2nd, 3rds
         case "J":
            SetWindowSize(0, 0, 0.33, 1.0);
            return true;
         case "K":
            SetWindowSize(0.33, 0, 0.33, 1.0);
            return true;
         case "L":
            SetWindowSize(0.66, 0, 0.33, 1.0);
            return true;
         
         // Corners
         case "T":
            SetWindowSize(0, 0, 0.5, 0.5);
            return true;
         case "Y":
            SetWindowSize(0.5, 0, 0.5, 0.5);
            return true;
         case "G":
            SetWindowSize(0, 0.5, 0.5, 0.5);
            return true;
         case "H":
            SetWindowSize(0.5, 0.5, 0.5, 0.5);
            return true;
         
         case "Left":
            SetWindowSize(0.0, 0.0, 0.5, 1.0);
            return true;
         case "Right":
            SetWindowSize(0.5, 0.0, 0.5, 1.0);
            return true;
      }

      return false;
   }
}