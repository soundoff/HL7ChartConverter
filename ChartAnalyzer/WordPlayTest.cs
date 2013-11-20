using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ChartConverter
{
    public partial class WordPlayTest : Form, IView
    {
        public WordPlayTest()
        {
            InitializeComponent();
            Controller.Instance.WireUp();
            Controller.Instance.RegisterView(this);
        }

        public void SetStatusText(string status)
        {
            txtOutput.Text =  "\r\n" + status + txtOutput.Text;
        }

        [STAThreadAttribute]
        private void btnGo_Click(object sender, EventArgs e)
        {
            ThreadStart me = delegate { Controller.Instance.RaiseBeginProcessing(this, Action.ProcessCharts); };
            Thread glass = new Thread(me);
            glass.SetApartmentState(ApartmentState.STA);
            glass.Start();
        }

        private void WordPlayTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            Controller.Instance.RaiseCleanUpEvent();
        }

        [STAThreadAttribute]
        private void btnExport_Click(object sender, EventArgs e)
        {
            ThreadStart me = delegate { Controller.Instance.RaiseBeginProcessing(this, Action.ConvertCharts); };
            Thread glass = new Thread(me);
            glass.SetApartmentState(ApartmentState.STA);
            glass.Start();
        }

        [STAThreadAttribute]
        private void btnDocToTif_Click(object sender, EventArgs e)
        {
            ThreadStart me = delegate { Controller.Instance.RaiseBeginProcessing(this, Action.ConvertDocToTif); };
            Thread glass = new Thread(me);
            glass.SetApartmentState(ApartmentState.STA);
            glass.Start();
        }

        public void ShowMessage(object sender, MessagingEventArgs e)
        {
            if (e.Message.Length > 0)
            {
                if (InvokeRequired)
                {
                    txtOutput.Invoke((MethodInvoker)delegate { SetStatusText(e.Message); } );
                }
                else
                {
                    SetStatusText(e.Message);
                }
            }
        }

        public void UpdateStatusCount(int Current, int Total)
        {
            lblStatusCount.Text = String.Format("{0} of {1}", Current, Total);
        }

        public void SetStartTime(DateTime start)
        {
            lblStartDate.Text = start.ToString();
        }

        public void SetFinishTime(DateTime finish)
        {
            lblFinishDate.Text = finish.ToString();
        }

        public void SetCurrentFile(string filename)
        {
            lblCurrentFile.Text = filename;
        }

        public void DisplayStartTime(DateTime start)
        {
            if (InvokeRequired)
            {
                txtOutput.Invoke((MethodInvoker)delegate { SetStartTime(start); });
            }
            else
            {
                SetStartTime(start);
            }
        }

        public void DisplayFinishTime(DateTime finish)
        {
            if (InvokeRequired)
            {
                txtOutput.Invoke((MethodInvoker)delegate { SetFinishTime(finish); });
            }
            else
            {
                SetFinishTime(finish);
            }
        }

        public void DisplayStatusCount(int current, int total)
        {
            if (InvokeRequired)
            {
                txtOutput.Invoke((MethodInvoker)delegate { UpdateStatusCount(current, total); });
            }
            else
            {
                UpdateStatusCount(current, total);
            }
        }

        public void DisplayCurrentFile(string filename)
        {
            if (InvokeRequired)
            {
                txtOutput.Invoke((MethodInvoker)delegate { SetCurrentFile(filename); });
            }
            else
            {
                SetCurrentFile(filename);
            }
        }
    }
}
