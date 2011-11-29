using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CellularAutomata
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void UpdateState() //USED ONLY IN A NEW THREAD
        {
            while (continueUpdate)
            {
                this.DrawLines();
                this.NextState();
                this.DrawRects();
                Thread.Sleep(this.updateRate);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text != "Stop")
            {
                continueUpdate = true;
                button1.Text = "Stop";
                this.updateState = new Thread(this.UpdateState);
                this.updateState.Start();
                // Loop until worker thread activates.
                while (!updateState.IsAlive);
            }
            else
            {
                continueUpdate = false;
                button1.Text = "Start";
                updateState.Join();
            }
            
        }

        public void button1Text(string newText)
        {
            button1.Text = newText;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Text = "Start";
            button1.Show();
            if (updateState == null)
            {
                updateState = new Thread(this.UpdateState);
            }
            continueUpdate = false;
            if(updateState.IsAlive) //wait for the thread to stop
                updateState.Join();
            this.DrawLines();
            this.InitializeRects();
            this.SetupInitialState();
            this.DrawRects();
            updateRate = (int)((1000 / trackBar1.Value));
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            updateRate = (int)((1000 / trackBar1.Value));
            if (trackBar1.Value == 1)
                label1.Text = "1 Update per second";
            else
                label1.Text = trackBar1.Value.ToString() + " Updates per second";
           // label1.Text = this.updateRate.ToString();
            //label2.Text = this.trackBar1.Value.ToString();
        }
    }
}
