using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace coords
{
    public partial class Form1 : Form
    {
        delegate void SetTextCallback(string labelText);

        public string labelText;
        private Thread demoThread = null;
        System.Timers.Timer myTimer = new System.Timers.Timer();

        public Form1()
        {
            InitializeComponent();

            myTimer.Elapsed += new ElapsedEventHandler(updateCoords);
            myTimer.Interval = 100;
            myTimer.Start();
        }

        private void updateCoords(object source, ElapsedEventArgs e)
        {
            labelText = "";
            labelText += Cursor.Position.X;
            labelText += ", ";
            labelText += Cursor.Position.Y;

            //Console.WriteLine(labelText);

            this.demoThread = new Thread(new ThreadStart(this.ThreadProcSafe));
            this.demoThread.Start();
        }
        private void ThreadProcSafe()
        {
            this.SetText(labelText);
        }

        private void SetText(string labelText)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.coordsLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                try
                {
                    this.Invoke(d, new object[] { labelText });
                }
                catch (ObjectDisposedException oe)
                {
                    Console.WriteLine("Caught: " + oe.Message);
                }
            }
            else
            {
                this.coordsLabel.Text = labelText;
            }
        }
        public void formCloser(object sender, EventArgs e)
        {
            myTimer.Stop();
            Application.Exit();
        }
    }
}
