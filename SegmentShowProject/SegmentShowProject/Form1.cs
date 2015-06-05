using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dicom.Imaging;
using Dicom;
using System.IO;

namespace SegmentShowProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }
        string[] allCTFilePath = new string[]{};
        Image img;
        List<Image> images = new List<Image>();
        int currentCTFileNumb = 0;
        string currentFilePath;
        string sName; string sHospital;
        private System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
        Point startPoint = new Point();
        bool boolMove = false;
        bool boolScale = false;
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                if (path == @"D:\1049664802\FileRecv\2\CT")
                {
                    allCTFilePath = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    //for (int i = 0; i < allCTFilePath.Length;i++ ) {
                    //    var image = new DicomImage(allCTFilePath[i]);
                    //    string pathString = @"D:\1049664802\FileRecv\CTImages\" + i + ".jpg";
                    //    image.RenderImage().Save(pathString);
                    //    img = Image.FromFile(pathString);
                    //    images.Add(img);
                    //    Console.WriteLine(i);
                    //}
                    currentCTFileNumb = 0;
                    img = images[currentCTFileNumb];                    
                    pictureBox1.Refresh();
                }
                else if (System.IO.Path.GetFileName(path) != "1")
                {
                    MessageBox.Show("目录不存在");
                }
                else {
                    if (path.Length < 10)
                    {
                        MessageBox.Show("文件名不符合要求");
                    }
                    if (path.Length < 9)
                    {
                        MessageBox.Show("文件不是合法的bmp");
                    }
                    if (path.Length < 8)
                    {
                        MessageBox.Show("bmp缺少文件");
                    }
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            img = Image.FromFile(@"D:\1049664802\FileRecv\3.jpg");
            //if(img!=null){
            //    currentFilePath = allCTFilePath[currentCTFileNumb];
            //    sName = DicomFile.Open(currentFilePath).Dataset.Get<String>(DicomTag.PatientName);
            //    sHospital = DicomFile.Open(currentFilePath).Dataset.Get<String>(DicomTag.InstitutionName);
            //    textBox1.Text = sName;
            //    textBox2.Text = sHospital;
            //}
                e.Graphics.Transform = matrix;
                e.Graphics.DrawImage(img,(pictureBox1.Width-img.Width)/2,(pictureBox1.Height-img.Height)/2);
        }
        /// <summary>
        /// 鼠标滚轮浏览图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Console.WriteLine(e.Delta);
            if (e.Delta > 0)
            {
                currentCTFileNumb -= 1;
                if (currentCTFileNumb < 0)
                {
                    currentCTFileNumb = 0;
                    return;
                }
                img = images[currentCTFileNumb];  
                pictureBox1.Refresh();
            }
            else if (e.Delta < 0)
            {
                currentCTFileNumb += 1;
                if (currentCTFileNumb >= images.Count)
                {
                    currentCTFileNumb = images.Count - 1;
                    return;
                }
                img = images[currentCTFileNumb];  
                pictureBox1.Refresh();
            }
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 鼠标右键是平移，左键是放缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Focus();
            if(e.Button == MouseButtons.Right){
                startPoint.X = e.X;
                startPoint.Y = e.Y;
                boolMove = true;
            }
            if(e.Button == MouseButtons.Left){
                startPoint.X = e.X;
                startPoint.Y = e.Y;
                boolScale = true;
            }
        }
        /// <summary>
        /// 实现平移和放缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(boolMove == true){
                matrix.Translate(e.X-startPoint.X,e.Y-startPoint.Y);
                pictureBox1.Refresh();
            }

            if(boolScale == true){
                float scaleX = (float)(e.X - startPoint.X) / 100;
                float scaleY = (float)(e.Y - startPoint.Y) / 100;
                matrix.Scale(1+scaleX,1+scaleY);
                matrix.Translate(-256*scaleX,-256*scaleY);
                
                pictureBox1.Refresh();
            }
                startPoint.X = e.X;
                startPoint.Y = e.Y;

        }
        /// <summary>
        /// 鼠标抬起结束操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            boolMove = false;
            boolScale = false;
        }
    }
}
