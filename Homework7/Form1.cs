using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
namespace Homework7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap a, b;
        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog tomato =new OpenFileDialog();
            if(tomato.ShowDialog()==DialogResult.OK)
            {
                a = new Bitmap(tomato.FileName);
                b = new Bitmap(tomato.FileName);
                pictureBox1.Image = a;
                pictureBox2.Image = b;
            }
        }
        private void button2_Click(object sender, EventArgs e)//膨脹按鈕
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("請先Load圖片");
                return;
            }
            b = big(b);
            pictureBox2.Image = b;
        }
        private void button3_Click(object sender, EventArgs e)//侵蝕按鈕
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("請先Load圖片");
                return;
            }
            b = small(b);
            pictureBox2.Image = b;
        }
        private void button4_Click(object sender, EventArgs e)//open按鈕
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("請先Load圖片");
                return;
            }
            b = small(b);
            b = big(b);
            pictureBox2.Image = b;
        }
        private void button5_Click(object sender, EventArgs e)//closing按鈕
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("請先Load圖片");
                return;
            }
            b = big(b);
            b = small(b);
            pictureBox2.Image = b;
        }
        private void button6_Click(object sender, EventArgs e)//儲存圖片
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("請先Load圖片");
                return;
            }

            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "所有檔案|*.*|BMP File|*.bmp|JPEG File|*.jpg|GIF File|*.gif|PNG File|*.png|TIFF File|*.tiff";
                saveFileDialog1.FilterIndex = 3;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName != "")
                {
                    Bitmap processedBitmap = b;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case 2:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case 3:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case 4:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 5:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                            break;
                    }
                }
            }
        }
        Bitmap big(Bitmap input)//膨脹
        {
            int[,,] source = GetRGBData(input);
            int[,,] output = GetRGBData(input);
            int filter_size = comboBox1.SelectedIndex*2+3,filter_temp =filter_size/ 2,flag;
            for(int i=filter_temp;i<a.Width- filter_temp; i++)
            {
                for(int j= filter_temp ; j < a.Height - filter_temp; j++)
                {
                    flag = 0;
                    for (int inner_i = i - filter_temp; inner_i <= i + filter_temp; inner_i++)
                        for (int inner_j = j - filter_temp; inner_j <= j + filter_temp; inner_j++)
                            if(source[inner_i,inner_j,0]<150)
                                flag = 1;
                    if (flag == 1)
                        for (int k = 0; k < 3; k++)
                            if(flag==1)
                                output[i, j, k] = 0;
                }

            }
            return setRGBData(output);
        }
        Bitmap small(Bitmap input) //侵蝕
        {
            int[,,] source = GetRGBData(input);
            int[,,] output = new int[input.Width, input.Height,3];
            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                        output[i, j, k] = 255;
            int filter_size = comboBox1.SelectedIndex*2+3, filter_temp = filter_size / 2, flag;
            int filter_sqrt = filter_size * filter_size;
            for (int i = filter_temp; i < a.Width - filter_temp; i++)
            {
                for (int j = filter_temp; j < a.Height - filter_temp; j++)
                {
                    flag = 0;
                    for (int inner_i = i - filter_temp; inner_i <= i + filter_temp; inner_i++)
                        for (int inner_j = j - filter_temp; inner_j <= j + filter_temp; inner_j++)
                            if (source[inner_i, inner_j, 0] < 150)
                                flag ++;
                    if (flag == filter_sqrt)
                        for (int k = 0; k < 3; k++)
                            output[i, j, k] = 0;
                }

            }
            return setRGBData(output);
        }
        public static int[,,] GetRGBData(Bitmap bitImg)
        {
            int height = bitImg.Height;
            int width = bitImg.Width;
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // get the starting memory place
            IntPtr imgPtr = bitmapData.Scan0;
            //scan width
            int stride = bitmapData.Stride;
            //scan ectual
            int widthByte = width * 3;
            // the byte num of padding
            int skipByte = stride - widthByte;
            //set the place to save values
            int[,,] rgbData = new int[width, height, 3];
            #region
            unsafe//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        //B channel
                        rgbData[i, j, 2] = p[0];
                        p++;
                        //g channel
                        rgbData[i, j, 1] = p[0];
                        p++;
                        //R channel
                        rgbData[i, j, 0] = p[0];
                        p++;
                    }
                    p += skipByte;
                }
            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return rgbData;
        }



        public static Bitmap setRGBData(int[,,] rgbData)
        {
            Bitmap bitImg;
            int width = rgbData.GetLength(0);
            int height = rgbData.GetLength(1);
            bitImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// 24bit per pixel 8x8x8
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //get image starting place
            IntPtr imgPtr = bitmapData.Scan0;
            //image scan width
            int stride = bitmapData.Stride;
            int widthByte = width * 3;
            int skipByte = stride - widthByte;
            #region
            unsafe
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        p[0] = (byte)rgbData[i, j, 2];
                        p++;
                        p[0] = (byte)rgbData[i, j, 1];
                        p++;
                        p[0] = (byte)rgbData[i, j, 0];
                        p++;
                    }
                    p += skipByte;
                }

            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return bitImg;
        }
    }
}
