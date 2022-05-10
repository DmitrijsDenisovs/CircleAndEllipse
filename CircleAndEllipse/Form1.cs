using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CircleAndEllipse
{
    public partial class Form1 : Form
    {
        int x;
        int y;
        int x2;
        int y2;
        int x3;
        int y3;

        int pointNumber = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void CircleButton_Click(object sender, EventArgs e)
        {
            if (GetCoords())
            {
                int r = (int) Math.Ceiling(Math.Sqrt(Math.Pow(x2 - x, 2) + Math.Pow(y2 - y, 2)));
                DrawCircle(Color.Red, x, y, r);
            }
            else
            {
                MessageBox.Show("Coordinates must be positive integer numbers", "Invalid input");
            }
        }

        private void EllipseButton_Click(object sender, EventArgs e)
        {
            bool valid = GetCoords();

            if (Int32.TryParse(x3TextBox.Text, out x3))
            {
                if (x3 < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (Int32.TryParse(y3TextBox.Text, out y3))
            {
                if (y3 < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (valid)
            {
                double r1 = Math.Sqrt(Math.Pow(x3 - x, 2) + Math.Pow(y3 - y, 2));
                double r2 = Math.Sqrt(Math.Pow(x3 - x2, 2) + Math.Pow(y3 - y2, 2));
                DrawEllipse(Color.Purple, x, y, x2, y2, r1, r2);
            }
            else
            {
                MessageBox.Show("Coordinates and radiuses must be positive integer numbers", "Invalid input");
            }
        }

        private void DrawPoint(Color color, int x, int y)
        {
            SolidBrush brush = new SolidBrush(color);

            this.CreateGraphics().FillRectangle(brush, x, y, 1, 1);
        }

 

        private void DrawCircle(Color color, int centerX, int centerY, int radius)
        {
            //initialize start of one octant
            int x = radius;

            int radiusError = 1 - radius;

            for (int y = 0; y < x; ++y)
            {
                DrawPoint(color, x + centerX, y + centerY);
                DrawPoint(color, centerX - x, y + centerY);
                DrawPoint(color, x + centerX, centerY - y);
                DrawPoint(color, centerX - x, centerY - y);
                DrawPoint(color, y + centerX, x + centerY);
                DrawPoint(color, centerX - y, x + centerY);
                DrawPoint(color, y + centerX, centerY - x);
                DrawPoint(color, centerX - y, centerY - x);

                if (radiusError <= 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    --x;
                    radiusError += 2 * y - 2 * x + 1;
                }

            }
        }

        private void DrawEllipse(Color color, int x1, int y1, int x2, int y2, double r1, double r2)
        {
            List<Tuple<double, double>> coordsToRotate = new List<Tuple<double, double>>();
            double focalDist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) / 2.0;
            double biggerHalfaxis = (r1 + r2) / 2.0;
            double eccentricity = focalDist / biggerHalfaxis;

            if (Math.Round(eccentricity, 10) >= 1.0)
            {
                MessageBox.Show("Given ellipse does not exist");
                return;
            }

            double lesserHalfaxis = biggerHalfaxis * Math.Sqrt(1.0 - Math.Pow(eccentricity, 2));

            double yc = (y1 + y2) / 2.0;
            double xc = (x1 + x2) / 2.0;
            double rx = biggerHalfaxis;
            double ry = lesserHalfaxis; 

            double dx, dy, d1, d2, x, y;

            x = 0;
            y = ry;

            d1 = (ry * ry) - (rx * rx * ry) + (0.25 * rx * rx);
            dx = 2 * ry * ry * x;
            dy = 2 * rx * rx * y;

            while (dx < dy)
            {
                coordsToRotate.Add(new Tuple<double, double>((x + xc), (y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((-x + xc), (y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((x + xc), (-y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((-x + xc), (-y + yc)));

                if (d1 < 0)
                {
                    ++x;
                    dx = dx + (2 * ry * ry);
                    d1 = d1 + dx + (ry * ry);
                }
                else
                {
                    ++x;
                    --y;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d1 = d1 + dx - dy + (ry * ry);
                }
            }

            d2 = ((ry * ry) * ((x + 0.5) * (x + 0.5))) + ((rx * rx) * ((y - 1) * (y - 1))) - (rx * rx * ry * ry);

            while (y >= 0)
            {
                coordsToRotate.Add(new Tuple<double, double>((x + xc), (y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((-x + xc), (y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((x + xc), (-y + yc)));
                coordsToRotate.Add(new Tuple<double, double>((-x + xc), (-y + yc)));

                if (d2 > 0)
                {
                    --y;
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + (rx * rx) - dy;
                }
                else
                {
                    --y;
                    ++x;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + dx - dy + (rx * rx);
                }
            }
            double k = (double) (x2 - x1) / (double)(y2 - y1);
            double angle = y1 == y2 ? 0 : Math.PI / 2.0 - Math.Atan(k);

            List<Tuple<int, int>> rotatedCoords = RotateCoords(coordsToRotate, angle, xc, yc);

            foreach(Tuple<int, int> coords in rotatedCoords)
            {
                DrawPoint(Color.Blue, coords.Item1, coords.Item2);
            }
        }

        private List<Tuple<int, int>> RotateCoords(List<Tuple<double, double>> coordsToRotate, double angle, double xc = 0, double yc = 0)
        {
            double sine = Math.Sin(angle);
            double cosine = Math.Cos(angle);

            List<Tuple<int, int>> rotatedCoords = new List<Tuple<int, int>>();

            foreach (Tuple<double, double> coords in coordsToRotate)
            {
                double xToUpdate = coords.Item1 - xc;
                double yToUpdate = coords.Item2 - yc;
                double newX = xToUpdate * cosine - yToUpdate * sine;
                double newY = xToUpdate * sine + yToUpdate * cosine;
                rotatedCoords.Add(new Tuple<int, int>((int)(newX + xc), (int)(newY + yc)));
            }

            return rotatedCoords;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            int x = this.PointToClient(Cursor.Position).X;
            int y = this.PointToClient(Cursor.Position).Y;
            this.CreateGraphics().DrawRectangle(new Pen(Color.Black, 5), new Rectangle(x, y, 1, 1));

            switch (pointNumber)
            {
                default:
                    break;
                case 1:
                    xTextBox.Text = x.ToString();
                    yTextBox.Text = y.ToString();
                    break;
                case 2:
                    x2TextBox.Text = x.ToString();
                    y2TextBox.Text = y.ToString();
                    break;
                case 3:
                    x3TextBox.Text = x.ToString();
                    y3TextBox.Text = y.ToString();
                    break;
            }
         

            if (++pointNumber > 3)
            {
                pointNumber = 1;
            }
        }

        private bool GetCoords()
        {
            bool valid = true;

            if (Int32.TryParse(xTextBox.Text, out x))
            {
                if (x < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (Int32.TryParse(yTextBox.Text, out y))
            {
                if (y < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }

            if (Int32.TryParse(x2TextBox.Text, out x2))
            {
                if (x2 < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }


            if (Int32.TryParse(y2TextBox.Text, out y2))
            {
                if (y2 < 0)
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
            return valid;
        }
    }
}
