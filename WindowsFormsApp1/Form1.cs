using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        List<Point> Coordinates;
        bool flag;//флаг существования рисунка
        Graphics g;
        Color borderColor;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            flag = false;
            Coordinates = new List<Point>();
            borderColor = Color.FromArgb(255,0,0,255);
        }

        private void PutPixel(Color color, int x, int y) //рисовать пиксель
        {
            if (x > 0 && x < bmp.Width && y > 0 && y < bmp.Height) bmp.SetPixel(x, y, color);
        }

        public void Bresenham4Line(Color clr, int x0, int y0, int x1, int y1)
        {
            //Изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
            //Направление приращения
            int sx = (x1 >= x0) ? (1) : (-1);
            int sy = (y1 >= y0) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                PutPixel(clr, x0, y0);
                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;
                    PutPixel(clr, x, y);
                    x += sx;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                PutPixel(clr, x0, y0);
                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;
                    PutPixel(clr, x, y);
                    y += sy;
                }
            }
        }

        int LineFill(int x, int y, int dir, int preXL, int preXR)
        {
            int xl = x, xr = x;
            Color color = bmp.GetPixel(xl,y);
            do
            {
                try
                {
                    color = bmp.GetPixel(--xl, y);
                }
                catch
                {
                    ArgumentOutOfRangeException e = new ArgumentOutOfRangeException();
                    MessageBox.Show(e.Message);
                    Environment.Exit(Environment.ExitCode);
                }
            } while (color != borderColor);

            do
            {
                color = bmp.GetPixel(++xr, y);
            } while (color != borderColor);

            xl++; xr--;
            Bresenham4Line(color, xl, y, xr, y);
            for(x = xl;x<=xr;x++)
            {
                color = bmp.GetPixel(x, y + dir);
                if (color != borderColor) { x = LineFill(x, y + dir, dir, xl, xr); }
            }
            for (x = xl; x < preXL; x++)
            {
                color = bmp.GetPixel(x, y - dir);
                if(color!=borderColor) {x = LineFill(x,y-dir,-dir,xl,xr); }
            }
            for(x = preXR;x<xr;x++)
            {
                color = bmp.GetPixel(x, y - dir);
                if (color != borderColor) {x = LineFill(x,y-dir,-dir,xl,xr); }
            }
            return xr;
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (!flag)
            {
                var p = new Point(e.X, e.Y);
                if (e.Button == MouseButtons.Left)
                {
                    
                    Coordinates.Add(p);//добавляем в листок с точками
                    if (Coordinates.Count == 1)//если 1 точка
                    {
                        PutPixel(Color.Black, e.X, e.Y);//делаем пиксель черным
                    }
                    else //если есть точки
                    {
                        Bresenham4Line(Color.Black, Coordinates[Coordinates.Count - 1].X, Coordinates[Coordinates.Count - 1].Y, //соединяет 2 последние точки
                                       Coordinates[Coordinates.Count - 2].X, Coordinates[Coordinates.Count - 2].Y);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    for (int i = 0; i < Coordinates.Count() - 1; i++)
                    {
                        Bresenham4Line(Color.Blue, Coordinates[i].X, Coordinates[i].Y, //соединяет 2 последние точки
                                       Coordinates[i + 1].X, Coordinates[i + 1].Y);
                    }
                    Bresenham4Line(Color.Blue, Coordinates[Coordinates.Count - 1].X, Coordinates[Coordinates.Count - 1].Y, //соединяет 2 последние точки
                                       Coordinates[0].X, Coordinates[0].Y);
                    pictureBox1.Image = bmp; //отображаем
                    LineFill(p.X, p.Y, 1, p.X, p.Y);
                    flag = true;
                }
                pictureBox1.Image = bmp; //отображаем
            }
        }
        private void pictureBox1_ButtonClick(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space)
            {
                g = Graphics.FromImage(pictureBox1.Image);
                Coordinates.Clear();
                g.Clear(Color.White);//очищаем область
                pictureBox1.Image = bmp; //отображаем
                flag = false;

            }
        }
    }
}
