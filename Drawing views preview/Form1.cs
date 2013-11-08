using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using TS3d = Tekla.Structures.Geometry3d;

namespace Drawing_views_preview
{
    public partial class form1 : Form
    {
        private Bitmap buffer;
        
        public form1()
        {
            InitializeComponent();
            panel1_Resize(this, null); // Initialize buffer 
            this.StartPosition = FormStartPosition.CenterScreen;
            //this.MinimumSize = new System.Drawing.Size(297, 210); //A4
            //this.MaximumSize = new System.Drawing.Size(1189, 841); //A0
            //this.AutoSize = true; this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            // Resize the buffer, if it is growing 
            if (buffer == null ||
                buffer.Width < panel1.Width ||
                buffer.Height < panel1.Height)
            {
                Bitmap newBuffer = new Bitmap(panel1.Width, panel1.Height);
                if (buffer != null)
                    using (Graphics bufferGrph = Graphics.FromImage(newBuffer))
                        bufferGrph.DrawImageUnscaled(buffer, Point.Empty);
                buffer = newBuffer;
            }
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Draw the buffer into the panel 
            e.Graphics.DrawImageUnscaled(buffer, Point.Empty);
        }


        private void btnPreview2_Click(object sender, EventArgs e)
        {
            // Draw into the buffer when button is clicked 
            PaintBlueRectangle();
        }


        private void PaintBlueRectangle()
        {
            ArrayList ItemList = new ArrayList();
            Drawing ActivatedDrawing;
            bool DrawingFound = false;

            DrawingHandler MyDrawingHandler = new DrawingHandler();

            if (MyDrawingHandler.GetConnectionStatus())
            {
                DrawingEnumerator SelectedDrawings = MyDrawingHandler.GetDrawingSelector().GetSelected();

                while (SelectedDrawings.MoveNext() && !DrawingFound)
                {
                    ActivatedDrawing = SelectedDrawings.Current;
                    double sheetHeight = ActivatedDrawing.Layout.SheetSize.Height;
                    double sheetWidth = ActivatedDrawing.Layout.SheetSize.Width;
                   
                    double minX = 0.0; 
                    double minY = 0.0; 
                    double maxX = sheetWidth; 
                    double maxY = sheetHeight;
                    
                    TSD.ContainerView currentSheet = ActivatedDrawing.GetSheet();
                    TSD.DrawingObjectEnumerator viewEnum = currentSheet.GetViews();
                    while (viewEnum.MoveNext())
                    {
                        TSD.View currentView = viewEnum.Current as TSD.View;
                        RectangleBoundingBox ViewAABB = currentView.GetAxisAlignedBoundingBox();
                        double rbbx = ViewAABB.UpperLeft.X;
                        double rbby = ViewAABB.UpperLeft.Y;
                        double rbbW = ViewAABB.Width;
                        double rbbH = ViewAABB.Height;
                        double viewScale = currentView.Attributes.Scale;
                        //MessageBox.Show("rbbx=" + Convert.ToString(rbbx) + ", rbby=" + Convert.ToString(rbby) + ", rbbW=" + Convert.ToString(rbbW) + ", rbbH=" + Convert.ToString(rbbH));
                        ItemList.Add(rbbx);
                        ItemList.Add(rbby);
                        ItemList.Add(rbbW);
                        ItemList.Add(rbbH);
                        ItemList.Add(viewScale);
                        
                        if (ViewAABB.UpperLeft.X < minX)
                        {
                            minX = ViewAABB.UpperLeft.X;
                        }
                        if (ViewAABB.LowerLeft.Y < minY)
                        {
                            minY = ViewAABB.LowerLeft.Y;
                        }
                        if (ViewAABB.UpperRight.X > maxX)
                        {
                            maxX = ViewAABB.UpperRight.X;                           
                        }
                        if (ViewAABB.UpperLeft.Y > maxY)
                        {
                            maxY = ViewAABB.UpperLeft.Y;
                        }
                    }

                    double bufferWidth = Math.Abs(minX) + maxX;
                    double bufferHeight = Math.Abs(minY) + maxY;

                    //panel1.Width = (int)Convert.ToInt32(bufferWidth);
                    //panel1.Height = (int)Convert.ToInt32(bufferHeight);
                    //btnPreview2.Location = new Point(panel1.Width - btnPreview2.Size.Width - 15, panel1.Height + btnPreview2.Size.Height);
                    //checkBox1.Location = new Point((int)Convert.ToInt32(0.2 * panel1.Width), panel1.Height + checkBox1.Size.Height + 20);
                    this.Size = new System.Drawing.Size(panel1.Width + 20, panel1.Height + btnPreview2.Size.Height + 20);
                    buffer = new Bitmap((int)Convert.ToInt32(bufferWidth), (int)Convert.ToInt32(bufferHeight));




////////////////////////////////////////////////////////////////////////////////////////
                    /*
                                   double scaleFactorX = 297 / bufferWidth;
                                   double scaleFactorY = 210 / bufferHeight;
                                   double scaleFactor = 0;
                                   if (scaleFactorX <= scaleFactorY)
                                   {
                                       scaleFactor = scaleFactorX;
                                   }else{
                                       scaleFactor=scaleFactorY;
                                   }
               

                                                       panel1.Width = (int)Convert.ToInt32(bufferWidth * scaleFactor);
                                                       panel1.Height = (int)Convert.ToInt32(bufferHeight * scaleFactor);                   
                                                       btnPreview2.Location = new Point(panel1.Width - btnPreview2.Size.Width - 15, panel1.Height + btnPreview2.Size.Height);
                                                       checkBox1.Location = new Point((int)Convert.ToInt32(0.2*panel1.Width), panel1.Height + checkBox1.Size.Height + 20);
                                                       this.Size = new System.Drawing.Size(panel1.Width + 20, panel1.Height + btnPreview2.Size.Height + 20);
                                                       buffer = new Bitmap((int)Convert.ToInt32(bufferWidth * scaleFactor), (int)Convert.ToInt32(bufferHeight * scaleFactor));
                                      */
 //////////////////////////////////////////////////////////////////////////////////////

                    

                    // Draw blue rectangle into the buffer 
                    using (Graphics bufferGrph = Graphics.FromImage(buffer))
                    {
                        Pen drwPen = new Pen(Color.DarkBlue, 4);
                        bufferGrph.Clear(Color.White);
                        System.Drawing.Rectangle myRectangle = new System.Drawing.Rectangle((int)Convert.ToInt32(Math.Abs(minX)), (int)Convert.ToInt32(Math.Abs(maxY)-sheetHeight), (int)Convert.ToInt32(sheetWidth), (int)Convert.ToInt32(sheetHeight));
                       
                        bufferGrph.DrawRectangle(drwPen, myRectangle);

                        for (int i = 0; i <= ItemList.Count / 5 - 1; i++)
                        {
                            int w = (int)Convert.ToInt32(ItemList[5 * i + 2]);
                            int h = (int)Convert.ToInt32(ItemList[5 * i + 3]) ;
                            int x = (int)Convert.ToInt32(ItemList[5 * i]) + (int)Convert.ToInt32(Math.Abs(minX));
                            int y = (int)Convert.ToInt32(Math.Abs(maxY) - Convert.ToDouble(ItemList[5 * i + 1]));
                            bufferGrph.DrawRectangle(new Pen(Color.Blue, 2), x, y, w, h);

                            if (checkBox1.Checked)
                            {
                                int sx, sy;
                                // Create string to draw.
                                String drawString = "Scale:" + Convert.ToString(ItemList[5 * i + 4]);
                                // Create font and brush.
                                Font drawFont = new Font("Arial", 8);
                                SolidBrush drawBrush = new SolidBrush(Color.Black);
                                // Set format of string.
                                StringFormat drawFormat = new StringFormat();
                                if (h > w)
                                {
                                    // Create point for upper-left corner of drawing.
                                    sx = x + (int)(0.5 * w); sy = y + (int)(0.25 * h);
                                    drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                                }
                                else
                                {
                                    // Create point for upper-left corner of drawing.
                                    sx = x + (int)(0.25 * w); sy = y + (int)(0.5 * h);
                                    drawFormat.FormatFlags = StringFormatFlags.DisplayFormatControl;
                                }
                                // Draw string to screen.
                                bufferGrph.DrawString(drawString, drawFont, drawBrush, sx, sy, drawFormat);
                            }
                        }
                        //Be sure the dispose of the pench
                        drwPen.Dispose();
                    }
                    // Invalidate the panel. This will lead to a call of 'panel1_Paint' 
                    panel1.Invalidate();
                    //only one loop
                    DrawingFound = true;
                }
            }
        }

        private void form1_Load(object sender, EventArgs e) { }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { }
    }
}
