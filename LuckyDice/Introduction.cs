using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuckyDice
{
    public partial class Introduction : Form
    {
        public Introduction()
        {
            InitializeComponent();
        }

        private void Introduction_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
        }
        int loadingSpeed = 4;
        float initPercentage = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            initPercentage += loadingSpeed;
            float percentage = initPercentage / picDice.Height * 100;
            label1.Text = ((int)percentage).ToString() + " %";
            panel1.Location = new Point(panel1.Location.X, panel1.Location.Y + loadingSpeed);
            if (panel1.Location.Y > picDice.Location.Y + picDice.Height)
            {
                label1.Text = "100 %";
                new LuckyDice.Law().Show();
                this.timer1.Stop();
                this.Hide();

            }
        }
    }
    
    
}
