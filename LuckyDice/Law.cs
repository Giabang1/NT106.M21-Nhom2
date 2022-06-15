using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LuckyDice
{
    
    public partial class Law : Form
    {
        protected string[] pFileNames;
        protected int currentImage = -1;
        List<Image> LawList = new List<Image>();
        public Law()
        {
            InitializeComponent();
        }

        private void Law_Load(object sender, EventArgs e)
        {
            Import();
            currentImage++;
            pictureBoxLaw.Image = LawList[currentImage];
        }
        void Import()
        {
            LawList.Add(Image.FromFile(@"law\1.jpg"));
            LawList.Add(Image.FromFile(@"law\2.jpg"));
            LawList.Add(Image.FromFile(@"law\3.jpg"));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            CharacterSelected characterSelected = new CharacterSelected();
            characterSelected.Location = this.Location;
            characterSelected.Show();
            this.Hide();
        }

        private void btnNextLaw_Click(object sender, EventArgs e)
        {
            currentImage++;
            if (currentImage == LawList.Count())
                currentImage = 0;
            pictureBoxLaw.Image = LawList[currentImage];
        }
    }
}
