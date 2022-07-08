using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyPaste.Properties;
using KeyPaste.WinApi;
using System.Diagnostics;
using System.Threading;

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
                AutoTypeClipboardText();
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

        private static void AutoTypeClipboardText()
        {
            string text = Clipboard.GetText();
            
            // HACK: Make last window gain focus again.
            using (Form dummy = new Form())
            {
                dummy.Show();
                dummy.Activate();
                dummy.WindowState = FormWindowState.Minimized;
            }
            UIntPtr hWnd = User32.GetForegroundWindow();

            INPUT[] inputSequence = new INPUT[text.Length * 2];
            UIntPtr ei = User32.GetMessageExtraInfo();
            for (int i = 0; i < text.Length; i++)
            {
                INPUT input = new INPUT();
                input.type = InputType.INPUT_KEYBOARD;
                input.U.ki.wScan = (ScanCode)text[i];
                input.U.ki.dwFlags = KeyEventFlags.KEYEVENTF_UNICODE;
                input.U.ki.dwExtraInfo = ei;
                inputSequence[i * 2] = input;
                input.U.ki.dwFlags |= KeyEventFlags.KEYEVENTF_KEYUP;
                inputSequence[i * 2 + 1] = input;
            }
            User32.SendInput((uint)inputSequence.Length, inputSequence, INPUT.Size);
        }

        private void MenuItem_Paste_Click(object sender, EventArgs e)
        {
            AutoTypeClipboardText();
        }

        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
