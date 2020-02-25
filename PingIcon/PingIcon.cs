using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;

namespace PingIcon
{
    class PingIcon
    {
        public static NotifyIcon nIcon = new NotifyIcon();
        private static string strping;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string ClassName, string WindowText);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        [STAThread]
        static void Main()
        {

            Task updatePingTask = new Task(updatePing);
            updatePingTask.Start();
            

            using (Ping p = new Ping())
            {
                strping = new Ping().Send("1.1.1.1").RoundtripTime.ToString();

            }

            using (Icon icon = CreateTextIcon(strping))
            {
                nIcon.Icon = icon;
                MenuItem menuItem1 = new MenuItem("Exit");

                MenuItem[] menuList = new MenuItem[]{ menuItem1};
                menuItem1.Click += new System.EventHandler(menuItem1_Click);

                ContextMenu cMenu = new ContextMenu(menuList);
                nIcon.ContextMenu = cMenu;

                nIcon.Visible = true;

                Application.Run();
                Application.ApplicationExit += new EventHandler(OnApplicationExit);

                nIcon.Visible = false;
            }

        }

        public static Icon CreateTextIcon(string str)
        {
            int fontsize;
            if (str.Length >= 3)
            {
                fontsize = 10;
            }
            else
            {
                fontsize = 16;
            }
            Font fontToUse = new Font("Microsoft Sans Serif", fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.White);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str, fontToUse, brushToUse, -4, -2);
            IntPtr hIcon = bitmapText.GetHicon();
            Icon icon = System.Drawing.Icon.FromHandle(hIcon);
            brushToUse.Dispose();
            bitmapText.Dispose();
            g.Dispose();

            return icon;
            
        }

        public static void updatePing()
        {
            while (true)
            {
                using (Ping p = new Ping())
                {
                    try
                    {
                        var ping = p.Send("1.1.1.1").RoundtripTime.ToString();
                        if(nIcon.Icon != null) nIcon.Icon.Dispose();
                        Icon icon = CreateTextIcon(ping);
                        nIcon.Icon = icon;
                        DestroyIcon(icon.Handle);

                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }

                Thread.Sleep(1000);

            }
        }

        private static void menuItem1_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            nIcon.Dispose();
            Environment.Exit(1);
        }

    }
}
