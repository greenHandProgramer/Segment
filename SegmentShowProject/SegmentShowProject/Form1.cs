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
using System.Drawing.Imaging;
using System.Threading;

namespace SegmentShowProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            
        }
        private string[] listCTFiles = new string[]{};
        private string[] listBMPFiles = new string[] { };
        private Image img;
        private int currentFileNumb = 0;
        private string currentFilePath;
        private string sName; string sHospital;
        private System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix();
        private System.Drawing.Drawing2D.Matrix matrixString = new System.Drawing.Drawing2D.Matrix();
        private Point startPoint = new Point();
        private bool boolMove = false;
        private bool boolScale = false;
        private string dicomIndirectory ;
        //private string bmpIndirectory = @"D:\1049664802\FileRecv\2\1";
        private Bitmap bmp;
        private float alpha = 0.5f;
        private int windowWidth = 1200;
        private int windowCenter = -450;

        private void Form1_Load(object sender, EventArgs e)
        {
            dicomIndirectory = @"D:\1049664802\FileRecv";
            //listBMPFiles = Directory.GetFiles(bmpIndirectory, "*", SearchOption.AllDirectories);
            //Array.Sort(listBMPFiles,new FileNameComparer1());
            //listCTFiles = Directory.GetFiles(dicomIndirectory, "*", SearchOption.AllDirectories);
            //Array.Sort(listCTFiles, new FileNameComparer1());
            //trackBar1.Minimum = 0;
            //trackBar1.Maximum = listCTFiles.Length - 1;
            //this.trackBar1.Value = 10 / listCTFiles.Length;
            //trackBar2.Minimum = 1;
            //trackBar2.Maximum = 10;
            //currentFileNumb = 0;
            //showCurrentImage(currentFileNumb);
            //showCurrentBMP(currentFileNumb);
            trackBar2.Value = 5;
            Form2 f2 = new Form2();
            f2.Show();
        }

        private string upDirectory;
        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = dicomIndirectory;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dicomIndirectory = fbd.SelectedPath;
                upDirectory = System.IO.Path.GetDirectoryName(dicomIndirectory);
                //if (System.IO.Path.GetFileName(upDirectory) != "")
                //{
                //    MessageBox.Show("目录不存在");
                //    return;
                //}

                string[] listFolders = Directory.GetFiles(upDirectory, "*", SearchOption.AllDirectories);
                bool isHaveBMP = false;
                bool isHaveCT = false;
                for (int i = 0; i < listFolders.Length;i++ ) {
                    if(listFolders[i]=="1"){
                        isHaveBMP = true;
                    } 
                    if (listFolders[i] == "CT")
                    {
                        isHaveCT = true;
                    }
                }

                if(!isHaveBMP&&isHaveCT){
                    MessageBox.Show("文件名不符合要求");
                    return;
                }

                listBMPFiles = Directory.GetFiles(upDirectory+"\\1", "*", SearchOption.AllDirectories);
                Array.Sort(listBMPFiles,new FileNameComparer1());
                for (int i = 0; i < listBMPFiles.Length;i++ ) {
                    string extension = (new FileInfo(listBMPFiles[i])).Extension;
                    if (extension == ".db")
                    {
                        System.IO.File.Delete(listBMPFiles[i]);
                        continue;
                    }
                    if(extension != ".bmp"){
                        MessageBox.Show("文件不是合法的bmp");
                        return;
                    }
                }
                        Console.WriteLine(listBMPFiles.Length.ToString());

                listCTFiles = Directory.GetFiles(upDirectory+"\\CT", "*", SearchOption.AllDirectories);
                Array.Sort(listCTFiles, new FileNameComparer1());
                if(listBMPFiles.Length!=listCTFiles.Length){
                    MessageBox.Show("bmp缺少文件");
                    return;
                }
  
                trackBar1.Minimum = 0;
                trackBar1.Maximum = listCTFiles.Length-1;
                this.trackBar1.Value = 10 / listCTFiles.Length;
                trackBar2.Minimum = 1;
                trackBar2.Maximum = 10;
                currentFileNumb = 0;
                currentFilePath = listCTFiles[currentFileNumb];
                showCurrentImage(currentFileNumb);
                showCurrentBMP(currentFileNumb);
                SetInitialPictureBoxScaleLevel(bmp, pictureBox1, ref matrix);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            if(img!=null&&isCTShow){
                e.Graphics.Transform = matrix;
                e.Graphics.DrawImage(img,0,0);
                e.Graphics.Transform = matrixString;
                matrixString.Scale(1.0f, 1.0f);                
                drawString(sender,e);
            }
            if(bmp!=null&&isBMPShow){
                e.Graphics.Transform = matrix;
                drawimage(e.Graphics,bmp,alpha,0,0);
            }

        }

        private void drawString(object sender, PaintEventArgs e)
        {
            if (currentFilePath == null)
            {
                return;
            }
            else { 
                sName = DicomFile.Open(currentFilePath).Dataset.Get<String>(DicomTag.PatientName);
                sHospital = DicomFile.Open(currentFilePath).Dataset.Get<String>(DicomTag.InstitutionName);
                // Create font and brush.
                Font drawFont = new Font("Arial", 9);
                SolidBrush drawBrush = new SolidBrush(Color.White);

                // Create rectangle for drawing.
                float x = 0F;
                float y = 0F;
                float width = 200.0F;
                float height = 50.0F;
                //RectangleF drawRect1 = new RectangleF(x, y, width, height);
                //RectangleF drawRect2 = new RectangleF(x, y+16, width, height);

                // Set format of string.
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Center;

                e.Graphics.DrawString(sName, drawFont, drawBrush, new RectangleF(x , y , width, height));
                e.Graphics.DrawString(sHospital, drawFont, drawBrush, new RectangleF(x + 380, y , width, height));
                e.Graphics.DrawString("W:"+windowWidth, drawFont, drawBrush, new RectangleF(x , y+12, width, height));
                e.Graphics.DrawString("L:" + windowCenter, drawFont, drawBrush, new RectangleF(x , y +24, width, height));


            }
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
                currentFileNumb -= 1;
                if (currentFileNumb < 0)
                {
                    currentFileNumb = 0;
                    return;
                }
                showCurrentImage(currentFileNumb);
                showCurrentBMP(currentFileNumb);
            }
            else if (e.Delta < 0)
            {
                currentFileNumb += 1;
                if (currentFileNumb >= listBMPFiles.Length)
                {
                    currentFileNumb = listBMPFiles.Length - 1;
                    return;
                }
                showCurrentImage(currentFileNumb);
                showCurrentBMP(currentFileNumb);
            }
        }

        private PointF startPointOnImageCoord;
        private PointF startPointOnControlCoord;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.pictureBox1.Focus();
            if(e.Button == MouseButtons.Right){
                startPoint = new Point(e.X,e.Y);
                boolMove = true;
            }
            if(e.Button == MouseButtons.Left&isChangedWindow == false){
                startPoint.X = e.X;
                startPoint.Y = e.Y;
                boolScale = true;
            }
            if(e.Button == MouseButtons.Left&isChangedWindow == true){
                startPoint = new Point(e.X,e.Y);
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
                this.Cursor = System.Windows.Forms.Cursors.Hand;
                matrix.Translate(e.X-startPoint.X,e.Y-startPoint.Y);
                pictureBox1.Refresh();
            }

            if(boolScale == true){
                //float scaleX = (float)(e.X - startPoint.X) / 100;
                //float scaleY = (float)(e.Y - startPoint.Y) / 100;
                //matrix.Scale(1+scaleX,1+scaleY);
                //matrix.Translate(-256*scaleX,-256*scaleY);
                //pictureBox1.Refresh();
                this.Cursor = System.Windows.Forms.Cursors.Cross;
                startPointOnControlCoord = new PointF(startPoint.X, startPoint.Y);
                startPointOnImageCoord = Control2Image(matrix, new Point(startPoint.X, startPoint.Y));

                pictureBox1.Cursor = System.Windows.Forms.Cursors.SizeAll;
                PointF startPointOnControlCoordBefore = Image2Control(matrix, startPointOnImageCoord);

                int dx = e.X - startPoint.X;
                int dy = e.Y - startPoint.Y;
                matrix.Scale((300 + dx) / 300f, (300 + dx) / 300f);

                PointF startPointOnControlCoordAfter = Image2Control(matrix, startPointOnImageCoord);

                float diffX = (startPointOnControlCoordAfter.X - startPointOnControlCoordBefore.X) / matrix.Elements[3];
                float diffY = (startPointOnControlCoordAfter.Y - startPointOnControlCoordBefore.Y) / matrix.Elements[3];

                //matrix.Scale(-3, -3);
                matrix.Translate(-diffX, -diffY);
                pictureBox1.Refresh();
            }
            if(isChangedWindow == true&&e.Button == MouseButtons.Left)
            {
                this.Cursor = System.Windows.Forms.Cursors.NoMove2D;
                int changeX = e.X - startPoint.X;
                int changeY = e.Y - startPoint.Y;
                windowWidth += changeY;
                windowCenter += changeX;
                showCurrentImage(currentFileNumb);
                showCurrentBMP(currentFileNumb);
            }
                startPoint.X = e.X;
                startPoint.Y = e.Y;

        }

        private static System.Drawing.PointF Control2Image(System.Drawing.Drawing2D.Matrix image2controlMatrix, System.Drawing.PointF controlP)
        {
            PointF[] point = new PointF[] { controlP };
            image2controlMatrix.Invert();
            image2controlMatrix.TransformPoints(point);
            image2controlMatrix.Invert();
            return point[0];
        }
        private static System.Drawing.PointF Image2Control(System.Drawing.Drawing2D.Matrix image2controlMatrix, System.Drawing.PointF controlP)
        {
            PointF[] point = new PointF[] { controlP };
            image2controlMatrix.TransformPoints(point);
            return point[0];
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            boolMove = false;
            boolScale = false;
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentFileNumb = trackBar1.Value;
            Console.WriteLine(currentFileNumb.ToString());
            showCurrentImage(currentFileNumb);
            showCurrentBMP(currentFileNumb);
        }

        private DicomImage _dicomImage;

        private void showCurrentImage(int currentCTFileNumb) {
            if(isCTShow == false){
                return;
            }
            _dicomImage = new DicomImage(listCTFiles[currentCTFileNumb]);
            _dicomImage.WindowCenter = windowCenter;
            _dicomImage.WindowWidth = windowWidth;
            img = _dicomImage.RenderImage();
            currentFilePath = listCTFiles[currentFileNumb];
            //pictureBox1.Refresh();

        }

        private void showCurrentBMP(int currentCTFileNumb) {
            bmp = (Bitmap)Image.FromFile(listBMPFiles[currentCTFileNumb]);
            Bitmap imgtarget = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height),PixelFormat.Format24bppRgb);
            Color color = bmp.GetPixel(1, 1);
            for (int i=0; i < bmp.Width;i++ )
            {
                for (int j = 0; j < bmp.Height;j++ ) {
                    if(bmp.GetPixel(i,j)!=color){
                        imgtarget.SetPixel(i, j, Color.Red);
                    }
                }
            }
            imgtarget.MakeTransparent(color);//使黑色区域变成透明的
            bmp = imgtarget;
            pictureBox1.Refresh();
        }

        public static void drawimage(Graphics ge, Bitmap b, float alpha, int startX, int startY)
        {
            float[][] ptsArray ={  new float[] {1, 0, 0, 0, 0},   new float[] {0, 1, 0, 0, 0},   new float[] {0, 0, 1, 0, 0},  new float[] {0, 0, 0, alpha, 0}, //注意：此处为0.5f，图像为半透明
            new float[] {0, 0, 0, 0, 1}};
            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            ImageAttributes imgAttributes = new ImageAttributes(); //设置图像的颜色属性
            imgAttributes.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap); //画图像
            ge.DrawImage(b, new Rectangle(startX, startY, b.Width, b.Height), 0, 0, b.Width, b.Height, GraphicsUnit.Pixel, imgAttributes);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            alpha =(float) 0.1 * trackBar2.Value;
            showCurrentImage(currentFileNumb);
            showCurrentBMP(currentFileNumb);
        } 

        private bool isChangedWindow = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isChangedWindow) { 
                isChangedWindow = true;
                button1.BackColor = Color.Gray;
                return;
            }
            if(isChangedWindow){
                isChangedWindow = false;
                button1.BackColor = Color.White;
                return;
            }
        }

        private bool isCTShow = true;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                isCTShow = false;
            }
            else {
                isCTShow = true;
            }
            pictureBox1.Refresh();
        }
        private bool isBMPShow = true;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                isBMPShow = false;
            }
            else {
                isBMPShow = true;
            }
            pictureBox1.Refresh();
        }

        public void SetInitialPictureBoxScaleLevel(Bitmap bitmap, System.Windows.Forms.PictureBox pictureBox, ref System.Drawing.Drawing2D.Matrix matrix)
        {
            if (bitmap == null || matrix == null)
            {
                return;
            }

            if (bitmap.Width == 0 || bitmap.Height == 0 || pictureBox.Width == 0 || pictureBox.Height == 0)
            {
                return;
            }
            //Console.WriteLine(pictureBox.Width + "  " + pictureBox.Height);
            matrix = new System.Drawing.Drawing2D.Matrix();
            float a = (float)pictureBox.Width / bitmap.Width;
            float b = (float)pictureBox.Height / bitmap.Height;
            Console.WriteLine(" SetInitialPictureBoxScaleLevel :" + a + " " + b);
            if (a <= b)
            {
                matrix.Scale(a, a);
                PointF start = new PointF((float)(bitmap.Width * a / 2f), (float)(bitmap.Height * a / 2f));
                PointF end = new PointF((float)(pictureBox.Width / 2f), (float)(pictureBox.Height / 2f));

                matrix.Invert();
                var pts = new PointF[] { start, end };
                matrix.TransformPoints(pts);
                matrix.Invert();
                matrix.Translate(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
            }
            else
            {
                matrix.Scale(b, b);

                PointF start = new PointF(bitmap.Width * b / 2f, bitmap.Height * b / 2f);
                PointF end = new PointF(pictureBox.Width / 2f, pictureBox.Height / 2f);
                matrix.Invert();
                var pts = new PointF[] { start, end };
                matrix.TransformPoints(pts);
                matrix.Invert();
                matrix.Translate(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y);
            }
        }

        private void btnSaveResult_Click(object sender, EventArgs e)
        {
            int index = 0;
            while (trackBar1.Value < trackBar1.Maximum)
            {
                Bitmap bitmap = new Bitmap(512,512);
                pictureBox1.DrawToBitmap(bitmap , new Rectangle(0 ,0 , 512 ,512));
                if(System.IO.Directory.Exists(upDirectory+"\\Result")){
                    bitmap.Save(upDirectory+"\\Result\\"+index+".jpg",ImageFormat.Jpeg);
                }
                else{
                    System.IO.Directory.CreateDirectory(upDirectory+"\\Result");
                    bitmap.Save(upDirectory+"\\Result\\"+index+".jpg");
                }
                trackBar1.Value++;

                trackBar1_Scroll(sender, e);
                Thread.Sleep(100); 
                index++;
            }
            
        }
    }
}
