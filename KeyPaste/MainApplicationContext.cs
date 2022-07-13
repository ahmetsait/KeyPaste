using KeyPaste.Properties;
using KeyPaste.WinApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KeyPaste
{
    public class MainApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public MainApplicationContext()
        {
            Settings.Current = Settings.Load();
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.AppIcon,
                ContextMenu = new ContextMenu(new[] {
                    new MenuItem("Auto-Type Clipboard", MenuItem_Paste_Click) { DefaultItem = true },
                    new MenuItem("Exit", MenuItem_Exit_Click),
                }),
                Visible = true,
            };
            
            trayIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MenuItem_Paste_Click(sender, e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            trayIcon.Dispose();
            base.Dispose(disposing);
        }

        protected override void ExitThreadCore()
        {
            Settings.Current.Save();
            base.ExitThreadCore();
        }

        private static string GetWindowTitle(UIntPtr hWnd)
        {
            int length = User32.GetWindowTextLength(hWnd);
            StringBuilder text = new StringBuilder(length + 1);
            User32.GetWindowText(hWnd, text, text.Capacity);
            return text.ToString();
        }

        private static string GetWindowClassName(UIntPtr hWnd)
        {
            StringBuilder text = new StringBuilder(512);
            User32.GetClassName(hWnd, text, text.Capacity);
            return text.ToString();
        }

        private static List<UIntPtr> EnumWindows(UIntPtr hWnd)
        {
            var windowList = new List<UIntPtr>();

            bool OnWindowEnum(UIntPtr hwnd, UIntPtr lparam)
            {
                windowList.Add(hwnd);
                return true;
            }

            User32.EnumWindows(OnWindowEnum, hWnd);
            return windowList;
        }

        private static Process GetProcessFromWindowHandle(UIntPtr hWnd)
        {
            User32.GetWindowThreadProcessId(hWnd, out int pid);
            return Process.GetProcessById(pid);
        }

        volatile bool typing = false;

        private void AutoTypeText(object obj)
        {
            typing = true;
            string text = (string)obj;
            
            // HACK: Make last window gain focus again.
            using (Form dummy = new Form())
            {
                dummy.Show();
                dummy.Activate();
                dummy.WindowState = FormWindowState.Minimized;
            }
            Thread.Sleep(1);
            UIntPtr hWnd = User32.GetForegroundWindow();

            UIntPtr ei = User32.GetMessageExtraInfo();
            INPUT[] inputSequence = new INPUT[2];
            for (int i = 0; i < text.Length && typing; i++)
            {
                inputSequence[0] = new INPUT();
                inputSequence[0].type = InputType.INPUT_KEYBOARD;
                inputSequence[0].U.ki.wScan = (ScanCode)text[i];
                inputSequence[0].U.ki.dwFlags = KeyEventFlags.KEYEVENTF_UNICODE;
                inputSequence[0].U.ki.dwExtraInfo = ei;
                inputSequence[1] = inputSequence[0];
                inputSequence[1].U.ki.dwFlags |= KeyEventFlags.KEYEVENTF_KEYUP;
                User32.SendInput((uint)inputSequence.Length, inputSequence, INPUT.Size);
                Thread.Sleep(1);
            }
            typing = false;
        }

        private void MenuItem_Paste_Click(object sender, EventArgs e)
        {
            if (typing)
                typing = false;
            else
            {
                Thread t = new Thread(new ParameterizedThreadStart(AutoTypeText));
                t.Start(Clipboard.GetText());
            }
        }

        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
