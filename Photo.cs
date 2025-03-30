using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Aplicațíe_pentru_prelucrarea_de_fotografii
{


    public partial class Photo : Form
    {
        public PictureBox pictureBox_photo; // Modifică nivelul de acces la public pentru PictureBox
        private bool paint = false;
        private Point py, px;
        private Graphics g;
        private float initialPenSize = 1;
        private float initialEraserSize = 1;
        private Pen p;
        private Pen eraser ;
        int index;
        int x, y, sX, sY, cX, cY;
        ColorDialog cd = new ColorDialog();
        public Color new_color = Color.Black;

        public Photo()
        {
            InitializeComponent();
            pictureBox_photo = pictureBox; // Asigură-te că ai corecta referință către PictureBox-ul existent
            p = new Pen(new_color, initialPenSize);
            eraser = new Pen(Color.White,initialEraserSize);
        }

        // Metoda pentru incarcarea imaginii în PictureBox-ul din formularul copil
        public void LoadImage(string imagePath)
        {
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize; // Seteaza modul de redimensionare la dimensiunile imaginii
            pictureBox.Image = Image.FromFile(imagePath);
            pictureBox.Visible = true;

            CheckZoomAndFit();
        }

        private void CheckZoomAndFit()
        {
            if (pictureBox.Image != null)
            {
                if (pictureBox.Image.Width > this.ClientSize.Width || pictureBox.Image.Height > this.ClientSize.Height)
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Width = this.ClientSize.Width;
                    pictureBox.Height = this.ClientSize.Height;
                }
                else
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox.Width = pictureBox.Image.Width;
                    pictureBox.Height = pictureBox.Image.Height;
                }

                pictureBox.Left = (this.ClientSize.Width - pictureBox.Width) / 2;
                pictureBox.Top = (this.ClientSize.Height - pictureBox.Height) / 2;
            }
        }

        private void Photo_Load(object sender, EventArgs e)
        {

        }

        private void Photo_Resize(object sender, EventArgs e)
        {
            CheckZoomAndFit();

        }

        // Metoda pentru începerea desenului cu creionul
        public void StartPencilDrawing()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 1;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StartEraserDrawing()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 2;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DrawingEllipse()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 3;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DrawingRectangle()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 4;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DrawingLine()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 5;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DrawingBezier()
        {
            if (pictureBox_photo.Image != null)
            {
                g = Graphics.FromImage(pictureBox_photo.Image);
                index = 6;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void DrawingColor()
        {
            if (pictureBox_photo.Image != null)
            {
                cd.ShowDialog();
                new_color = cd.Color;
                p.Color = cd.Color;

            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public Color GetColor()
        {
            return new_color;
        }
        public void UpdateColor()
        {
            p.Color = new_color; // Set the pen color using the value passed from the parent form
        }
        public void IncreasePenSize()
        {

            if (initialPenSize < 10)
            {
                initialPenSize = (float)(initialPenSize + 0.5);
                p = new Pen(new_color, initialPenSize);

                initialEraserSize = (float)(initialEraserSize + 0.5);
                eraser = new Pen(Color.White, initialEraserSize);
            }
            else
            {
                initialPenSize++; // Crește cu 1 când ajunge la 10
                p = new Pen(new_color, initialPenSize);

                initialEraserSize++;
                eraser = new Pen(Color.White, initialEraserSize);
            }
        }

        public void DecreasePenSize()
        {
            if (initialPenSize > 0.5 && initialPenSize <= 10)
            {
                initialPenSize = (float)(initialPenSize - 0.5);
                p = new Pen(new_color, initialPenSize);

                initialEraserSize = (float)(initialEraserSize - 0.5);
                eraser = new Pen(Color.White, initialEraserSize);
            }
            else if (initialPenSize > 10)
            {
                initialPenSize--;
                p = new Pen(new_color, initialPenSize);

                initialEraserSize--;
                eraser = new Pen(Color.White, initialEraserSize);
            }
        }
        public float GetCurrentPenSize()
        {
            return initialPenSize;
        }

        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }
        private bool AreColorsSimilar(Color color1, Color color2, int threshold)
        {
            int redDifference = Math.Abs(color1.R - color2.R);
            int greenDifference = Math.Abs(color1.G - color2.G);
            int blueDifference = Math.Abs(color1.B - color2.B);

            return redDifference <= threshold && greenDifference <= threshold && blueDifference <= threshold;
        }

        public void Fill(Bitmap bm, int x, int y, Color new_clr, int threshold)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr || AreColorsSimilar(old_color, new_clr, threshold)) { return; }

            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 7)
            {
                Point point = set_point(pictureBox, e.Location);
                int colorThreshold = 30; // Ajustează acest prag în funcție de necesități
                Fill((Bitmap)pictureBox_photo.Image, point.X, point.Y, new_color, colorThreshold);
            }
        }

        public void FillColor()
        {
            if (pictureBox_photo.Image != null)
            {
                index = 7;
            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void EnablePictureBoxMouseDown(bool enable)
        {
            if (enable)
            {
                pictureBox_photo.MouseDown += pictureBox_MouseDown;
            }
            else
            {
                pictureBox_photo.MouseDown -= pictureBox_MouseDown;
            }
        }

        public void EnablePictureBoxMouseMove(bool enable)
        {
            if (enable)
            {
                pictureBox_photo.MouseMove += pictureBox_MouseMove;
            }
            else
            {
                pictureBox_photo.MouseMove -= pictureBox_MouseMove;
            }
        }
        public void EnablePictureBoxMouseUp(bool enable)
        {
            if (enable)
            {
                pictureBox_photo.MouseUp += pictureBox_MouseUp;
            }
            else
            {
                pictureBox_photo.MouseUp -= pictureBox_MouseUp;
            }
        }
        // Evenimentul pentru apăsarea butonului mouse-ului pentru desenare
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox_photo.Image != null)
            {
                paint = true;
                // Obține dimensiunile reale ale imaginii
                float scaleX = (float)pictureBox_photo.Image.Width / pictureBox_photo.Width;
                float scaleY = (float)pictureBox_photo.Image.Height / pictureBox_photo.Height;

                // Transformă coordonatele mouse-ului în coordonatele imaginii originale
                py = new Point((int)(e.X * scaleX), (int)(e.Y * scaleY));

                cX = (int)(e.X * scaleX);
                cY = (int)(e.Y * scaleY);

            }
            else
            {
                MessageBox.Show("No image in the PictureBox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evenimentul pentru mișcarea mouse-ului pentru desenare
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            // Obține dimensiunile reale ale imaginii
            float scaleX = (float)pictureBox_photo.Image.Width / pictureBox_photo.Width;
            float scaleY = (float)pictureBox_photo.Image.Height / pictureBox_photo.Height;
            if (paint)
            {
                if (index == 1)
                {
                    px = new Point((int)(e.X * scaleX), (int)(e.Y * scaleY));

                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2)
                {
                    px = new Point((int)(e.X * scaleX), (int)(e.Y * scaleY));

                    g.DrawLine(eraser, px, py);
                    py = px;
                }
            }

            x = (int)(e.X * scaleX);
            y = (int)(e.Y * scaleY);
            sX = (int)(e.X * scaleX) - cX;
            sX = (int)(e.Y * scaleY) - cY;

            pictureBox_photo.Refresh();
        }

        // Evenimentul pentru eliberarea butonului mouse-ului
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;

            sX = x - cX;
            sY = y - cY;

            if (index == 3)
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if (index == 4)
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            if (index == 5)
            {
                g.DrawLine(p, cX, cY, x, y);
            }
            if (index == 6)
            {
                // Define the four points for the Bezier curve
                Point startPoint = new Point(cX, cY);
                Point controlPoint1 = new Point(x, cY);
                Point controlPoint2 = new Point(cX, y);
                Point endPoint = new Point(x, y);

                // Draw Bezier curve
                g.DrawBezier(p, startPoint, controlPoint1, controlPoint2, endPoint);
            }


        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
                if (index == 6)
                {
                    // Define the four points for the Bezier curve
                    Point startPoint = new Point(cX, cY);
                    Point controlPoint1 = new Point(x, cY);
                    Point controlPoint2 = new Point(cX, y);
                    Point endPoint = new Point(x, y);

                    // Draw Bezier curve
                    g.DrawBezier(p, startPoint, controlPoint1, controlPoint2, endPoint);
                }
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }

        Label[] textLabel = new Label[100];
        Panel[] textPanel = new Panel[100];
        int c = 0;
        private int lastAddedIndex = -1;

        public void AdaugaTextPeImagine(int width, int height, string fontName, float fontSize, Color textColor, bool boldActivated, bool italicActivated)
        {
            if (pictureBox_photo.Image != null)
            {
                Image image = pictureBox_photo.Image;

                FontStyle fontStyle = FontStyle.Regular;

                if (boldActivated)
                {
                    fontStyle |= FontStyle.Bold;
                }

                if (italicActivated)
                {
                    fontStyle |= FontStyle.Italic;
                }

                Font textFont = new Font(fontName, fontSize, fontStyle);

                pictureBox_photo.Image = image;

                textPanel[c] = new Panel();
                textLabel[c] = new Label();

                textLabel[c].Size = new Size(width, height);
                textLabel[c].Text = "Click Here";
                textLabel[c].TextAlign = ContentAlignment.MiddleCenter;
                textLabel[c].Location = new Point(0, 0); // Centrare relativă la panel
                textLabel[c].BackColor = Color.Transparent; // Setează fundalul labelului ca fiind transparent
                textLabel[c].Font = textFont;
                textLabel[c].MouseDown += Photo_MouseDown;
                textLabel[c].MouseUp += Photo_MouseUp;
                textLabel[c].MouseMove += Photo_MouseMove;
                textLabel[c].Click += Photo_Click;
                textLabel[c].Name = c + " false c0";

                textPanel[c].Size = new Size(width, height);
                textPanel[c].Name = "Click Here";
                textPanel[c].Location = new Point(10, 10);
                textPanel[c].BackColor = Color.White; // Setează fundalul panelului ca fiind alb
                textPanel[c].Controls.Add(textLabel[c]);

                textLabel[c].Location = new Point((textPanel[c].Width - textLabel[c].Width) / 2, (textPanel[c].Height - textLabel[c].Height) / 2);
                pictureBox_photo.Controls.Add(textPanel[c]);
                textPanel[c++].BringToFront();
                lastAddedIndex++; // Actualizează contorul de elemente adăugate
                this.Cursor = Cursors.Default;
            }
        }


        int val_x, val_y;

        private void Photo_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label l = sender as Label;
                for (int i = 0; i < c; i++)
                {
                    if (textLabel[i].Name != l.Name)
                    {
                        textLabel[i].Name = textLabel[i].Name.Replace("c1", "c0").Replace("true", "false");
                        textLabel[i].BackColor = Color.White;
                    }
                }
                l.Name = l.Name.Replace("false", "true");
                l.Cursor = Cursors.SizeAll;
                val_x = e.X;
                val_y = e.Y;
            }
        }
        private void Photo_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label l = sender as Label;
                l.Name = l.Name.Replace("true", "false");
                l.Cursor = Cursors.Default;
            }
        }
        private void Photo_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label l = sender as Label;
                if (l.Name.Contains("true"))
                {
                    l.BringToFront();

                    Point newLocation = pictureBox_photo.PointToClient(MousePosition);
                    int panelIndex = Convert.ToInt32(l.Name.Split(' ')[0]);
                    Point panelLocation = textPanel[panelIndex].Location;
                    Size panelSize = textPanel[panelIndex].Size;

                    // Verifică dacă noua poziție depășește marginile imaginii
                    if (newLocation.X >= 0 && newLocation.Y >= 0 &&
                        newLocation.X + panelSize.Width <= pictureBox_photo.Width &&
                        newLocation.Y + panelSize.Height <= pictureBox_photo.Height)
                    {
                        textPanel[panelIndex].Location = newLocation;
                    }
                    else
                    {
                        // Ajustează poziția textului pentru a rămâne în interiorul imaginii
                        int x = Math.Max(0, Math.Min(newLocation.X, pictureBox_photo.Width - panelSize.Width));
                        int y = Math.Max(0, Math.Min(newLocation.Y, pictureBox_photo.Height - panelSize.Height));
                        textPanel[panelIndex].Location = new Point(x, y);
                    }
                }
            }
        }
        private void Photo_Click(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                Label l = sender as Label;
                for (int i = 0; i < c; i++)
                {
                    if (textLabel[i].Name != l.Name)
                        textLabel[i].Name = textLabel[i].Name.Replace("c1", "c0").Replace("true", "false");
                }
                if (l.Name.Contains("c1"))
                {
                    l.Name = l.Name.Replace("c1", "c0");
                    l.BackColor = Color.White;
                }
                else
                {
                    l.Name = l.Name.Replace("c0", "c1");
                    l.BackColor = Color.Green;
                    this.Focus();
                }
            }
        }
        public void Photo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine($"Tastă apăsată: {e.KeyChar}");

            foreach (Control control in pictureBox_photo.Controls)
            {
                if (control is Panel panel && panel.Name.Contains("c1"))
                {
                    foreach (Control innerControl in panel.Controls)
                    {
                        if (innerControl is Label label)
                        {
                            if (label.Text != "" && e.KeyChar == '\b') // '\b' reprezintă tastele Backspace
                            {
                                label.Text = label.Text.Remove(label.Text.Length - 1);
                                if (panel.Name.Length > 5 && panel.Name.Substring(panel.Name.Length - 4) == "&nl;")
                                {
                                    panel.Name = panel.Name.Remove(panel.Name.Length - 4, 4);
                                    panel.Name = panel.Name.Remove(panel.Name.Length - 1);
                                }
                                else
                                    panel.Name = panel.Name.Remove(panel.Name.Length - 1);
                            }
                            else if (e.KeyChar != '\b')
                            {
                                label.Text += e.KeyChar;
                                panel.Name += e.KeyChar;
                            }

                            if (e.KeyChar == '\r') // '\r' reprezintă tasta Enter
                                panel.Name += "&nl;";
                        }
                    }
                }
            }
        }

        public void RemoveLastTextAdded()
        {
            if (lastAddedIndex >= 0)
            {
                pictureBox_photo.Controls.Remove(textPanel[lastAddedIndex]);
                textPanel[lastAddedIndex] = null;
                textLabel[lastAddedIndex] = null;
                lastAddedIndex--;
                c--;
            }
        }

    }
}
