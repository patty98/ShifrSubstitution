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
using System.Numerics;
namespace DES_image
{
    public partial class Form1 : Form
    {
        Bitmap image1;
         int block = 8;
        int len_number = 8;
        List<int[]> tablesRand_inv = new List<int[]>();
        List<int[]> tablesRand = new List<int[]>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void LoadImage()
        {
            image1 = new Bitmap(@"D:\dog.bmp", true);
            pictureBox1.Image = image1;
            pictureBox2.Image = image1;
            Bitmap image_enc = image1;
            int z = 0;
            Bitmap bmap = (Bitmap)image1.Clone();
            Color[] c = new Color[bmap.Height * bmap.Width];
            byte[,] b_ar = new byte[bmap.Height, bmap.Width];
            byte[] copy_arr = new byte[bmap.Height * bmap.Width];
            Color[] copy1 = new Color[bmap.Height * bmap.Width];
            Color[] copy = new Color[bmap.Height * bmap.Width];

            int[] keys = new int[block];
            int x = 0;
            int j = 0;

            for (int i = 0; i < image1.Width; i++) //получаем значение светотени(оттенка цвета) пикселя
            {
                for (j = 0; j < image1.Height; j++)
                {
                    c[x] = image1.GetPixel(i, j);
                    x++;
                }
            }


            keys = GenerateKeys(keys);

           /* StreamWriter sw2 = new StreamWriter(@"D:\\R3_correl_im.txt");
            int n = 0;

            for (int k = 0; k < 2000; k++)

            {
                for (int i = 0; i < c.Length - k; i++)
                {

                    copy[i + k] = c[i];

                }
                x = 0;
                for (int i = 0; i < k; i++)
                {
                    copy[i] = System.Drawing.Color.FromArgb(c[i + j].A, 0, 0, 0);
                }

                BigInteger Rk1 = new BigInteger();
                BigInteger Rsum = new BigInteger();
                Rsum = 0;
                Rk1 = 0;
                int mid = Dispersion(c, k);

                for (n = 0; n < c.Length - k; n++)
                {

                    if (n + k < c.Length - k)
                    {
                        Rk1 += (copy[n].G - mid) * (c[n].G - mid);

                    }
                    else
                    {
                        break;
                    }


                }
                sw2.WriteLine(Rk1 + "\t" + k);
                Rk1 = 0;
                Rsum = 0;
                n = 0;
            }
            sw2.Close();*/



            c = EncryptIm(c, keys);


          /*  StreamWriter sw1 = new StreamWriter(@"D:\\R3_correl_encode.txt");

            n = 0;
            for (int k = 0; k < 2000; k++)

            {
                for (int i = 0; i < c.Length - k; i++)
                {

                    copy[i + k] = c[i];

                }
                x = 0;
                for (int i = 0; i < k; i++)
                {
                    copy[i] = System.Drawing.Color.FromArgb(c[i + j].A, 0, 0, 0);
                }

                BigInteger Rk1 = new BigInteger();
                BigInteger Rsum = new BigInteger();
                Rsum = 0;
                Rk1 = 0;
                int mid = Dispersion(c, k);

                for (n = 0; n < c.Length - k; n++)
                {

                    if (n + k < c.Length - k)
                    {
                        Rk1 += (copy[n].G - mid) * (c[n].G - mid);

                    }
                    else
                    {
                        break;
                    }


                }
                sw1.WriteLine(Rk1 + "\t" + k);
                Rk1 = 0;
                Rsum = 0;
                n = 0;
            }
            sw1.Close();
            */

            x = 0;
            for (int i = 0; i < pictureBox1.Image.Width; i++)
            {
                for (j = 0; j < pictureBox1.Image.Height; j++)
                {
                    ((Bitmap)pictureBox1.Image).SetPixel(i, j, c[x]);
                    x++;
                }
            }
            pictureBox1.Refresh();
            pictureBox1.Image.Save("D:\\encode2.jpg");

            c = DecryptIm(c, keys);
            
            x = 0;
            for (int i = 0; i < pictureBox2.Image.Width; i++)
            {
                for (j = 0; j < pictureBox2.Image.Height; j++)
                {
                    ((Bitmap)pictureBox2.Image).SetPixel(i, j, c[x]);
                    x++;
                }
            }
            pictureBox2.Refresh();

        }

        private int [] GenerateKeys(int [] keys)
        {

            Random rand = new Random(unchecked((int)(DateTime.Now.Ticks)));
            keys = Enumerable.Range(0, 256).OrderBy(p => rand.Next()).Take(keys.Length).ToArray();
            return keys;

        }
        private int Dispersion(Color[] c, int k)
        {
            BigInteger mid = new BigInteger();
            mid = 0;
            int i = 0;
            int r = 0;
            for (; i < c.Length - k; i++)
            {
                mid += c[i].G;
            }
            r = (int)mid / c.Length;
            return r;

        }
        private Color [] EncryptIm(Color [] c, int [] keys)
        {
            int[] read = new int[256];
            int C = 0;
            int v1 = 0;
            int i = 0;
            int v2 = 0;
            for (int j = 0; j < c.Length;)
            {

                for (; i < block; i++)
                {
                    try
                    {
                        if (i == 0)
                        {
                            C = keys[i] ^ c[i + j].G;
                            c[i + j] = Color.FromArgb(c[i + j].A, C, C, C);
                            v1 = C % 32;
                            v2 = v1;
                            continue;
                        }

                        C = keys[i] ^ c[i + j].G;
                        read = ReadFile(v1, read,0);
                        C = read[C];
                        c[i + j] = Color.FromArgb(c[i + j].A, C, C, C);
                        v2 = C % 32;
                        v2 = v1 ^ v2;
                        v1 = v2;
                    }
                    catch
                    {
                        break;
                    }
                    read = new int[256];

                }
                j += i;
                i = 0;
                
            }

                

            return c;
        }
        private Color[] DecryptIm(Color[] c, int[] keys)
        {

            int[] read = new int[256];
            int B = 0;
            int v1 = 0;
            int i = 0;
            int v2 = 0;
            int b = 0;
            for (int j = 0; j < c.Length;)
            {

                for (; i < block; i++)
                {
                    try
                    {
                        if (i == 0)
                        {
                            v1 = c[i+j].G % 32;
                            B= keys[i] ^ c[i + j].G;
                            c[i + j] = Color.FromArgb(c[i + j].A,B,B,B);
                            v2 = v1;
                            continue;
                        }

                        b = c[i + j].G;
                        read = ReadFile(31-v1, read, 1);
                        B = read[c[i + j].G];
                        B = keys[i] ^ B;
                        c[i + j] = Color.FromArgb(c[i + j].A, B,B,B);
                        v2 = b % 32;
                        v2 = v1 ^ v2;
                        v1 = v2;
                    }
                    catch
                    {
                        break;
                    }
                    read = new int[256];
                }
                j += i;
                i = 0;
            }

            return c;

        }
        private int [] ReadFile(int v, int [] read, int point)
        {

            string path = "D:\\T" + v.ToString()+".txt";
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
         {
                while (!sr.EndOfStream)
                {
                    string[] p = sr.ReadLine().Split('\t');
                   
                    int ind = Convert.ToInt16(p[0]);
                    read[ind] = Convert.ToInt16(p[1]);
                   
                }
        }
            return read;
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadImage();
        }
    }
    }
