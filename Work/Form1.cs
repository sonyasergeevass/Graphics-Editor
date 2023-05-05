using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Media.Media3D;



namespace Work
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetSize();
            pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_MouseClick);
            richTextBox1.KeyPress += new KeyPressEventHandler(richTextBox1_KeyPress);
            comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            //ToolStripMenuItem copyFileMenuItem = new ToolStripMenuItem("Сохранить файл");
            //contextMenuStrip1.Items.AddRange(new[] { copyFileMenuItem });
            //tabControl1.MouseClick += TabControl1_MouseClick;
            //skipPaint = true;
            //comboBox1.Text = "Контур";

            //comboBox1.Items.Insert(0, new ListItem("Select", "0"));
            //comboBox1.Items.Add("Выбирите цвет");
            //comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.AddRange(new string[] { "Сплошная", "Пунктир", "Штрихпунктир"});
            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = false;

            //comboBox2.Text = "Заливка";
            //comboBox1.Items.Insert(0, new ListItem("Select", "0"));
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Items.AddRange(new string[] { "Без заливки", "Сплошная", "Горизонтальная", "Сетка", "Диагональная" });
            comboBox2.SelectedIndex = 0;
            comboBox2.Enabled = false;

            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            //comboBox3.Items.AddRange(new string[] { "Диагональный" });

        }
        //public static extern bool SetProcessDPIAware();


        private class ArrayPoints
        {
            private int index = 0;
            private Point[] points;

            public ArrayPoints(int size)
            {
                if (size >= 0) { size = 2; }
                points = new Point[size];
            }

            public void SetPoint(int x, int y)
            {
                if(index >= points.Length) { index = 0; }
                points[index] = new Point(x, y);
                index++;
            }
            public void ResetPoints()
            {
                index = 0;
            }

            public int GetCountPoints() { return index; }
            public Point[] GetPoints() { return points; }

        }
        
        private bool isMouse = false;
        private ArrayPoints arrayPoints = new ArrayPoints(2);
        Bitmap map = new Bitmap(100, 100);
        Graphics graphics;
        Pen pen = new Pen(Color.Black, 3f);//стандартная ручка
        Pen pen1 = new Pen(Color.White, 3f);//ластик
        Pen pen_outline = new Pen(Color.Black, 3f);//ручка, отвечающая за контур фигур
        Brush brushka = new SolidBrush(Color.PeachPuff);
        //Brush brushka2 = new LinearGradientBrush(new Point(0, 10), new Point(200, 10), Color.PeachPuff, Color.MintCream);
        //Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, Color.Red);
        //Brush cbrush = new HatchBrush(HatchStyle.Cross, Color.Black);
        //Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Black);
        private enum DrawingTool { Pen, Line, Ellipse, Rectangle, Triangle, Eraser, RectangleCopy, BufferPaste, Text, MakeCopy};
        private DrawingTool drawingTool = DrawingTool.Pen;
        private enum Brushes { Brush1, Brush2, Horizontal, Cross, Forward_Diagonal };
        private Brushes brushes;


        private Point startPoint, endPoint;
        private bool isDrawing = false;
        private bool isBrush = false;
        private bool skipPaint = false;
        private bool isInDraw = false;
        private bool copyDot = false;
        private bool isText = false;
        private bool isCut = false;
        private string textFromTextBox = "dgdfg";


        Color gradientColor = Color.Black;
        Color colorText = Color.Black;
        private float width1 = 3;
        //private float widthText = 12;
        DashStyle dashStyle123 = DashStyle.Solid;

        Font textFont = new Font("Arial", 12);
        //Rectangle rect = new Rectangle();
        private Bitmap image;


        private void SetSize()
        {
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            map = new Bitmap(rectangle.Width, rectangle.Height);
            graphics = Graphics.FromImage(map);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            pictureBox1.Size = new Size(this.ClientSize.Width-50, this.ClientSize.Height+90);
            //tabControl1.Size = new Size(this.ClientSize.Width+100, this.ClientSize.Height - 100);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = (float)numericUpDown1.Value;
            pen1.Width = (float)numericUpDown1.Value;
            pen_outline.Width = (float)numericUpDown1.Value;
            width1 = (float)numericUpDown1.Value;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouse = true;
            startPoint = e.Location;
            //одна нижняя строка эксперементальная
            endPoint = e.Location;
            isDrawing = true;
            
        }
        //private void textBoxtext_Leave(object sender, EventArgs e)
        //{
        //    string textFromTextBox = ((TextBox)sender).Text;
        //    graphics.DrawString(textFromTextBox, new Font("Arial", 12), brushka, new Point(0, 100));
        //}

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (skipPaint)
            {
                skipPaint = false;
                return;
            }
            //if (isText)
            //{
            //    e.Graphics.DrawString(textFromTextBox, new Font("Arial", 12), brushka, new Point(0, 0));
            //}
            Pen pen123 = new Pen(gradientColor, width1);
            pen123.StartCap = LineCap.Round;
            pen123.EndCap = LineCap.Round;
            pen123.DashStyle = dashStyle123;
            if (isDrawing && drawingTool == DrawingTool.Line)
            {
                e.Graphics.DrawLine(pen123, startPoint, endPoint);
                pen123.Dispose();
            }

            else if (isDrawing && drawingTool == DrawingTool.Ellipse)
            {
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );
                if (isBrush)
                {

                    if (brushes == Brushes.Brush1)
                        e.Graphics.FillEllipse(brushka, rect);

                    else if (brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        e.Graphics.FillEllipse(hbrush, rect);
                    }
                    //else if (brushes == Brushes.Brush2)
                    //{
                    //    Brush brushka2 = new LinearGradientBrush(new Point(200, 10), startPoint, Color.White, gradientColor);
                    //    e.Graphics.FillEllipse(brushka2, rect);
                    //    brushka2.Dispose();
                    //}

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        e.Graphics.FillEllipse(cbrush, rect);

                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        e.Graphics.FillEllipse(fbrush, rect);
                    }
                }

                else { e.Graphics.DrawEllipse(pen_outline, rect); }
            }
            else if (isDrawing && drawingTool == DrawingTool.Rectangle)
            {
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );

                if (isBrush)
                {
                    if (brushes == Brushes.Brush1)
                        e.Graphics.FillRectangle(brushka, rect);
                    else if (brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        e.Graphics.FillRectangle(hbrush, rect);
                    }

                    //else if (brushes == Brushes.Brush2)
                    //{
                    //    Brush brushka2 = new LinearGradientBrush(startPoint, endPoint, Color.White, gradientColor);
                    //    graphics.FillRectangle(brushka2, rect);
                    //}

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        e.Graphics.FillRectangle(cbrush, rect);

                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        e.Graphics.FillRectangle(fbrush, rect);
                    }
                }

                else { e.Graphics.DrawRectangle(pen_outline, rect); }
            }

            else if (isDrawing && drawingTool == DrawingTool.Triangle)
            {
                Point[] pointsPOlygon = new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint };
                Point point1 = new Point(startPoint.X, startPoint.Y);
                Point point2 = new Point(startPoint.X, endPoint.Y);
                Point point3 = new Point(endPoint.X, endPoint.Y);

               

                if (isBrush)
                {
                    if (brushes == Brushes.Brush1)
                    {

                        e.Graphics.FillPolygon(brushka, pointsPOlygon);
                    }
                        

                    else if (brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        e.Graphics.FillPolygon(hbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }
                    //else if (brushes == Brushes.Brush2)
                    //{
                    //    Brush brushka2 = new LinearGradientBrush(startPoint, endPoint, Color.White, gradientColor);
                    //    e.Graphics.FillPolygon(brushka, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    //}

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        e.Graphics.FillPolygon(cbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        e.Graphics.FillPolygon(fbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }

                }            

                else
                {
                    //e.Graphics.DrawLine(pen_outline, point1, point2);
                    //e.Graphics.DrawLine(pen_outline, point2, point3);
                    //e.Graphics.DrawLine(pen_outline, point3, point1);
                    e.Graphics.DrawPolygon(pen_outline, pointsPOlygon); 
                    
                }
            }

            else if(isDrawing && drawingTool == DrawingTool.RectangleCopy)
            {
                Pen pen12 = new Pen(Color.Black, 1);
                pen12.StartCap = LineCap.Round;
                pen12.EndCap = LineCap.Round;
                pen12.DashStyle = DashStyle.Dash;
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );
                e.Graphics.DrawRectangle(pen12, rect); 
            }

            

                // Draw the bitmap
            graphics.DrawImage(map, 0, 0);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            
            isMouse = false;
            isText = false;
            arrayPoints.ResetPoints();
            //if (isInDraw && marker)
            //{
            //    isInDraw = false;
            //    marker = false;
            //    //endPoint = e.Location;
            //    //this.Invalidate();
            //    pictureBox1.Image = map;
            //    endPoint = e.Location;
            //    Pen pen12 = new Pen(Color.Black, 1);
            //    pen12.StartCap = LineCap.Round;
            //    pen12.EndCap = LineCap.Round;
            //    pen12.DashStyle = DashStyle.Dash;
            //    Rectangle recti = new Rectangle(
            //        Math.Min(startPoint.X, endPoint.X),
            //        Math.Min(startPoint.Y, endPoint.Y),
            //        Math.Abs(endPoint.X - startPoint.X),
            //        Math.Abs(endPoint.Y - startPoint.Y)
            //        );
            //    graphics.DrawRectangle(pen12, recti);

            //    //int x = Math.Min(startPoint.X, endPoint.X);
            //    //int y = Math.Min(startPoint.Y, endPoint.Y);
            //    //int width = Math.Abs(startPoint.X - endPoint.X);
            //    //int height = Math.Abs(startPoint.Y - endPoint.Y);

            //    //Bitmap bmpScreenCapture = new Bitmap(width, height);
            //    //Graphics g = Graphics.FromImage(bmpScreenCapture);
            //    //g.CopyFromScreen(x, y, 0, 0, bmpScreenCapture.Size);

            //    //Clipboard.SetImage(bmpScreenCapture);

            //    //pictureBox1.Image = Clipboard.GetImage();
            //}


            if (isDrawing && drawingTool == DrawingTool.Line)
            {
                pictureBox1.Image = map;
                endPoint = e.Location;
                isDrawing = false;
                graphics = Graphics.FromImage(map);
                Pen pen123 = new Pen(gradientColor, width1);
                pen123.StartCap = LineCap.Round;
                pen123.EndCap = LineCap.Round;
                pen123.DashStyle = dashStyle123;
                graphics.DrawLine(pen123, startPoint, endPoint);
                pen123.Dispose();
                //----------------------------
                //pictureBox1.Image = map;
                //endPoint = e.Location;
                //graphics.DrawLine(pen, startPoint, endPoint); // рисуем линию
                ////graph_objects.Add(graphics);
                //pictureBox1.Invalidate(); // перерисовываем изображение
                //isDrawing = false; // завершаем режим рисования линии
                //------------------------------
            }

            else if (isDrawing && drawingTool == DrawingTool.Ellipse)
            {
                pictureBox1.Image = map;
                endPoint = e.Location;
                isDrawing = false;
                graphics = Graphics.FromImage(map);
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );
                if(isBrush)
                {

                    if (brushes == Brushes.Brush1)
                        graphics.FillEllipse(brushka, rect);

                    else if(brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        graphics.FillEllipse(hbrush, rect);
                    }
                    else if (brushes == Brushes.Brush2)
                    {
                        Brush brushka2 = new LinearGradientBrush(startPoint, endPoint, Color.White, gradientColor);
                        graphics.FillEllipse(brushka2, rect);
                    }

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        graphics.FillEllipse(cbrush, rect);
                        
                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        graphics.FillEllipse(fbrush, rect);
                    }
                }
                    
                else { graphics.DrawEllipse(pen_outline, rect); }
                
                //pictureBox1.Invalidate(); // перерисовываем изображение
                //isDrawing = false;// завершаем режим рисования линии
                //isBrush = false;
            }

            else if (isDrawing && drawingTool == DrawingTool.Rectangle)
            {
                pictureBox1.Image = map;
                endPoint = e.Location;
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );

                if (isBrush)
                {
                    if (brushes == Brushes.Brush1)
                        graphics.FillRectangle(brushka, rect);
                    else if (brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        graphics.FillRectangle(hbrush, rect);
                    }

                    else if (brushes == Brushes.Brush2){
                        Brush brushka2 = new LinearGradientBrush(startPoint, endPoint, Color.White, gradientColor);
                        graphics.FillRectangle(brushka2, rect); 
                    }

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        graphics.FillRectangle(cbrush, rect);

                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        graphics.FillRectangle(fbrush, rect);
                    }
                }

                else { graphics.DrawRectangle(pen_outline, rect); }

                pictureBox1.Invalidate(); // перерисовываем изображение
                isDrawing = false; // завершаем режим рисования линии
            }
            else if (isDrawing && drawingTool == DrawingTool.Triangle)
            {
                pictureBox1.Image = map;
                endPoint = e.Location;


                if (isBrush)
                {
                    if (brushes == Brushes.Brush1)
                        graphics.FillPolygon(brushka, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });

                    else if (brushes == Brushes.Horizontal)
                    {
                        Brush hbrush = new HatchBrush(HatchStyle.Horizontal, Color.Black, gradientColor);
                        graphics.FillPolygon(hbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }
                    else if (brushes == Brushes.Brush2)
                    {
                        Brush brushka2 = new LinearGradientBrush(startPoint, endPoint, Color.White, gradientColor);
                        graphics.FillPolygon(brushka, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint }); 
                    }

                    else if (brushes == Brushes.Cross)
                    {
                        Brush cbrush = new HatchBrush(HatchStyle.Cross, gradientColor);
                        graphics.FillPolygon(cbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }

                    else if (brushes == Brushes.Forward_Diagonal)
                    {
                        Brush fbrush = new HatchBrush(HatchStyle.ForwardDiagonal, gradientColor);
                        graphics.FillPolygon(fbrush, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint });
                    }

                }

                else { graphics.DrawPolygon(pen_outline, new Point[] { startPoint, new Point(startPoint.X, endPoint.Y), endPoint }); }// рисуем треугольник
                pictureBox1.Invalidate(); // перерисовываем изображение
                isDrawing = false;// вызываем метод Invalidate для обновления pictureBox1
                //isMouse = false;
            }

            else if (isDrawing && drawingTool == DrawingTool.RectangleCopy )
            {

                pictureBox1.Image = map;
                endPoint = e.Location;
                Rectangle rect = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y)
                    );

                if(isCut)
                {
                    Brush br = new SolidBrush(Color.White);
                    graphics.FillRectangle(br, rect);
                }

                int width = Math.Abs(endPoint.X - startPoint.X)-2;
                int height = Math.Abs(endPoint.Y - startPoint.Y)-2;
                image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(image);
                //g.CopyFromScreen(this.Left + panel1.Left + titleBarHeight, this.Top + panel1.Top + borderWidth, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                g.CopyFromScreen(Math.Min(startPoint.X, endPoint.X)+1, Math.Min(startPoint.Y, endPoint.Y)+toolStrip1.Height, 0, 0, image.Size, CopyPixelOperation.SourceCopy);
                //g.CopyFromScreen(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), new Point(0, 0), image.Size);
                //g.CopyFromScreen(pictureBox1.PointToScreen(rect.Location), new Point(0, 0), image.Size);
                Clipboard.SetImage(image);



                //pictureBox1.Invalidate(); // перерисовываем изображение
                //isDrawing = false; // завершаем режим рисования
                //Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                ////Bitmap bmpScreenCapture = new Bitmap(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y));
                //Graphics g = Graphics.FromImage(bmpScreenCapture);
                //g.CopyFromScreen(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.X, endPoint.X), 200, 200, bmpScreenCapture.Size);
                //Clipboard.SetImage(bmpScreenCapture);
                //pictureBox1.Image = Clipboard.GetImage();
                //Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                //Graphics g = Graphics.FromImage(bmp);
                //g.CopyFromScreen(rect.Location, Point.Empty, rect.Size);
                //Clipboard.SetImage(bmp);
                pictureBox1.Invalidate();
                isDrawing = false;
               
            }

            else if (isDrawing && drawingTool == DrawingTool.BufferPaste)
            {
                skipPaint = true;
                //PictureBox pictureBox = new PictureBox();
                //pictureBox.Image = image;
                //pictureBox1.Controls.Add(pictureBox); // измененный код
                //pictureBox1.Invalidate();
                //pictureBox1.Image = Clipboard.GetImage();
                //if (Clipboard.ContainsImage())
                //{
                //    Image image = Clipboard.GetImage();
                //    if (pictureBox1 != null)
                //    {
                //        pictureBox1.Image = image;
                //    }
                //}
                Image clipboardImage = Clipboard.GetImage();
                if (clipboardImage != null)
                {
                    Image pasteImage = new Bitmap(clipboardImage);
                    graphics.DrawImage(pasteImage, new Point(e.Location.X - clipboardImage.Width / 2, e.Location.Y - clipboardImage.Height / 2));


                    //PictureBox pictureBox = new PictureBox();
                    //pictureBox.Image = pasteImage;
                    //pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    //pictureBox1.Controls.Add(pictureBox); // измененный код
                    //pictureBox1.Invalidate(); // измененный код
                    //pictureBox.Location = new Point(e.Location.X - clipboardImage.Width / 2, e.Location.Y - clipboardImage.Height / 2);
                }
            }
            isDrawing = false;
            pictureBox1.Invalidate();
            isMouse = false;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //isMouse = true;
            //skipPaint = true;
            //if (drawingTool == DrawingTool.BufferPaste)
            //{
            //    Image clipboardImage = Clipboard.GetImage();
            //    if (clipboardImage != null)
            //    {
            //        Image pasteImage = new Bitmap(clipboardImage);
            //        PictureBox pictureBox = new PictureBox();
            //        pictureBox.Image = pasteImage;
            //        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            //        pictureBox.Location = new Point(e.Location.X - clipboardImage.Width / 2, e.Location.Y - clipboardImage.Height / 2);
            //        pictureBox1.Controls.Add(pictureBox); // измененный код
            //        pictureBox1.Invalidate(); // измененный код
            //    }
            //}
            if (isText && drawingTool == DrawingTool.Text)
            {
                //isDrawing = false
                Brush brushText = new SolidBrush(colorText);
                graphics.DrawString(textFromTextBox, textFont, brushText, e.Location);
            }
            pictureBox1.Image = map;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouse) { return; }

            if (drawingTool == DrawingTool.Pen)
            {
                arrayPoints.SetPoint(e.X, e.Y);
                if (arrayPoints.GetCountPoints() >= 2)
                {
                    //if (isHbrush)
                    //    graphics.FillClosedCurve(hbrush, arrayPoints.GetPoints());
                    //else { graphics.DrawLines(pen, arrayPoints.GetPoints()); }

                    graphics.DrawLines(pen, arrayPoints.GetPoints());
                    pictureBox1.Image = map;
                    arrayPoints.SetPoint(e.X, e.Y);
                }
            }
            else if (drawingTool == DrawingTool.Line || drawingTool == DrawingTool.Ellipse || drawingTool == DrawingTool.Rectangle || drawingTool == DrawingTool.Triangle || drawingTool == DrawingTool.RectangleCopy)
            {
                //pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                //graphics = Graphics.FromImage(pictureBox1.Image);
                //graphics.DrawImage(map, 0, 0);

                //graphics.DrawLine(pen, startPoint, endPoint);
                //endPoint = e.Location;
                //pictureBox1.Refresh();
                //graph_objects.Add(graphics);

                //верный вариант продолжени коода для моего компьютера
                if (isDrawing)
                {
                    endPoint = e.Location;
                    pictureBox1.Invalidate();
                }
                //выше норм

            }
            else if (drawingTool == DrawingTool.Eraser)
            {

                arrayPoints.SetPoint(e.X, e.Y);
                if (arrayPoints.GetCountPoints() >= 2)
                { 
                    graphics.DrawLines(pen1, arrayPoints.GetPoints());
                    pictureBox1.Image = map;
                    arrayPoints.SetPoint(e.X, e.Y);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //skipPaint = true;
                pen.Color = button.BackColor;
                brushka = new SolidBrush(button.BackColor);
                gradientColor = button.BackColor;
                pen_outline.Color = button.BackColor;
                //brushka2 = new LinearGradientBrush(new Point(0, 10), new Point(200, 10), Color.PeachPuff, button.BackColor);
                //hbrush = new HatchBrush(HatchStyle.Horizontal, Color.PeachPuff, button.BackColor);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            skipPaint = true;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                brushka = new SolidBrush(colorDialog1.Color);
                gradientColor = colorDialog1.Color;
                pen_outline.Color = colorDialog1.Color;
                ((Button)sender).BackColor = colorDialog1.Color;
            } 
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            graphics.Clear(pictureBox1.BackColor);
            pictureBox1.Image = map;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPG(*.JPG)|*.jpg";
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        { 
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                map = new Bitmap(openFileDialog1.FileName);
                graphics = Graphics.FromImage(map);
                pictureBox1.Image = map;
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            
            drawingTool = DrawingTool.Line;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //copyDot = false;
            drawingTool = DrawingTool.Pen;
            pen.DashStyle = DashStyle.Solid;
            //pen.Color = System.Drawing.Color.PeachPuff;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            drawingTool = DrawingTool.Rectangle;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            drawingTool = DrawingTool.Triangle;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = false;
            pen_outline.DashStyle = DashStyle.DashDot;
            dashStyle123 = DashStyle.DashDot;


        }

        private void button24_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = false;
            pen_outline.DashStyle = DashStyle.Dot;
            dashStyle123 = DashStyle.Dot;
        }


        private void button29_Click(object sender, EventArgs e)
        {
            
            //skipPaint = true;
            isBrush = true;
            brushes = Brushes.Brush1;  
            
        }

        private void button28_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = true;
            brushes = Brushes.Brush2;
        }


        private void button25_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = false;
            pen_outline.DashStyle = DashStyle.Solid;
            dashStyle123 = DashStyle.Solid;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = false;
 
        }

        private void button26_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = true;
            brushes = Brushes.Horizontal;

        }

        private void button30_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = true;
            brushes = Brushes.Cross;
        }

        private void button31_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            isBrush = true;
            brushes = Brushes.Forward_Diagonal;
        }

        private void button32_Click(object sender, EventArgs e) //ereaser
        {
            drawingTool = DrawingTool.Eraser;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2ID inputForm = new Form2ID();
            DialogResult result = inputForm.ShowDialog();
        } 

        private void сохранитьИзображениеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPG(*.JPG)|*.jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void открытьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                map = new Bitmap(openFileDialog1.FileName);
                graphics = Graphics.FromImage(map);
                pictureBox1.Image = map;
            }
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2ID inputForm = new Form2ID();
            DialogResult result = inputForm.ShowDialog();
        }

        private void saveFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPG(*.JPG)|*.jpg";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                map = new Bitmap(openFileDialog1.FileName);
                graphics = Graphics.FromImage(map);
                pictureBox1.Image = map;
            }
        }

        private void оПрограммеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Form2ID inputForm = new Form2ID();
            //DialogResult result = inputForm.ShowDialog();
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();

            if (selectedState == "Сплошная")
            {
                isBrush = false;
                pen_outline.DashStyle = DashStyle.Solid;
                dashStyle123 = DashStyle.Solid;
            }
            else if (selectedState == "Пунктир")
            {
                isBrush = false;
                pen_outline.DashStyle = DashStyle.Dot;
                dashStyle123 = DashStyle.Dot;
            }
            else
            {
                isBrush = false;
                pen_outline.DashStyle = DashStyle.DashDot;
                dashStyle123 = DashStyle.DashDot;
            }

        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox2.SelectedItem.ToString();
            if (selectedState == "Без заливки")
            {
                isBrush = false;
            }
            else if (selectedState == "Сплошная")
            {
                isBrush = true;
                brushes = Brushes.Brush1;
            }
            else if(selectedState == "Горизонтальная")
            {
                isBrush = true;
                brushes = Brushes.Horizontal;
            }
            else if (selectedState == "Сетка")
            {
                isBrush = true;
                brushes = Brushes.Cross;
            }
            else
            {
                isBrush = true;
                brushes = Brushes.Forward_Diagonal;
            }
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            isCut = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            //isInDraw = true;
            drawingTool = DrawingTool.RectangleCopy;
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            isCut = false;
            copyDot = true;
            drawingTool = DrawingTool.BufferPaste;
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            isText = true;
            //graphics.DrawString(textFromTextBox, new Font("Arial", 12), brushka, new Point(0, 0));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                comboBox3.Items.Add(family.Name);
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            //isText = true;
            //drawingTool = DrawingTool.Text;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isText = true;
            drawingTool = DrawingTool.Text;
            RichTextBox rtb = (RichTextBox)sender;

            // Сохранить текст в переменную
            textFromTextBox = rtb.Text;
        }
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Добавить символ к тексту
            //textFromTextBox += e.KeyChar;
        }

        private void button23_Click_2(object sender, EventArgs e)
        {
            skipPaint = true;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colorText = colorDialog1.Color;
                ((Button)sender).BackColor = colorDialog1.Color;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Font font = new Font(comboBox3.SelectedItem.ToString(), 12);
            richTextBox1.Font = font;
            textFont = font;
            //string selectedState = comboBox3.SelectedItem.ToString();
            //if(selectedState == "Диагональный")
            //{
            //    richTextBox1.SelectionFont = new Font("Consolas", 10f, FontStyle.Strikeout);
            //}
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            FontDialog fontDialog1 = new FontDialog();
            if (fontDialog1.ShowDialog() == DialogResult.OK && richTextBox1.SelectionLength > 0)
            {
                richTextBox1.SelectionFont = fontDialog1.Font;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            //widthText = (float)numericUpDown2.Value;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button24_Click_2(object sender, EventArgs e)
        {
            isCut = true;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            drawingTool = DrawingTool.RectangleCopy;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //skipPaint = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            drawingTool = DrawingTool.Ellipse;
        }

    }
}
