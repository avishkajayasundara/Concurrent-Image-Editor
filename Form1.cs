using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CB007644
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        Image defaultImage = null;
        private Image image, image2, image3, image4;
        bool opened = false;
        bool picBox2IsOpened = false;
        bool picBox3IsOpened = false;
        bool picBox4IsOpened = false;
        String url1 = null;
        String url2 = null;
        String url3 = null;
        String url4 = null;
        Boolean isCubed = false;
        //  private Size originalImageSize, ModifiedImageSize;


        int cropX, cropY, cropWidth, cropHeight, oCropX, oCropY;
        public Pen cropPen;

        public DashStyle cropDashStyle = DashStyle.DashDot;
        public bool Makeselection = false;


        public bool CreateText = false;


        public Form1()
        {
            InitializeComponent();
            defaultImage = pictureBox1.Image;
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!opened)
            {
                OpenPictureBox(1);
            }
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void btnRotateLeft_Click(object sender, EventArgs e)

        {
            

            Bitmap[] rotateImages = { (Bitmap)pictureBox1.Image.Clone(), (Bitmap)pictureBox2.Image.Clone(), (Bitmap)pictureBox3.Image.Clone(), (Bitmap)pictureBox4.Image.Clone() };
            Task<Bitmap[]> t1 = Task.Factory.StartNew((arg) =>
            {
                Bitmap[] images = (Bitmap[])arg;
                Parallel.ForEach(images, currentImage =>
                {
                    try
                    {
                        currentImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            Console.WriteLine("Operations were canelled");

                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    
                });
                return images;
            }, rotateImages);
            Task t2 = t1.ContinueWith((task) =>
            {
                image = task.Result[0];
                image2 = task.Result[1];
                image3 = task.Result[2];
                image4 = task.Result[3];
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
            },TaskScheduler.FromCurrentSynchronizationContext());
           


        }
        private void BindDomainIUpDown()
        {
        }
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {
        }

        private void resizeButton_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnCrop.Enabled = false;
        }

        private void btnMakeSelection_Click(object sender, EventArgs e)
        {

            if (opened || picBox2IsOpened || picBox3IsOpened || picBox4IsOpened)
            {
                Makeselection = true;
                btnCrop.Enabled = true;
            }
            else
            {
                MessageBox.Show("No images found");
            }
               
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            Bitmap OriginalImage1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap OriginalImage2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap OriginalImage3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap OriginalImage4 = (Bitmap)pictureBox4.Image.Clone();
            Task<Bitmap> cropTask1 = Task.Factory.StartNew(() =>
            {
                
                return cropImage(OriginalImage1);
            });
            Task<Bitmap> cropTask2 = Task.Factory.StartNew(() =>
            {
                return cropImage(OriginalImage2);
            });
            Task<Bitmap> cropTask3 = Task.Factory.StartNew(() =>
            {
                return cropImage(OriginalImage3);
            });
            Task<Bitmap> cropTask4 = Task.Factory.StartNew(() =>
            {
                return cropImage(OriginalImage4);
            });
            //  TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    image = cropTask1.Result;
                    image2 = cropTask2.Result;
                    image3 = cropTask3.Result;
                    image4 = cropTask4.Result;
                    pictureBox1.Image = image;
                    pictureBox2.Image = image2;
                    pictureBox3.Image = image3;
                    pictureBox4.Image = image4;
                    btnCrop.Enabled = false;
                    pictureBox4.Image = image4;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        MessageBox.Show("Cancelling tasks");
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            
        }
        private Bitmap cropImage(Bitmap Image)
        {
            Bitmap _img = null;
            try { 
            Cursor = Cursors.Default;
            // if (cropWidth < 1)
            //{
            //   return OriginalImage;
            //}
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            //First we define a rectangle with the help of already calculated points  
            Bitmap OriginalImage = new Bitmap(Image, 448, 294);
            //Original image  
             _img = new Bitmap(cropWidth, cropHeight);
            // for cropinf image  
            Graphics g = Graphics.FromImage(_img);
            // create graphics  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //set image attributes  
            g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);                    
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException is OperationCanceledException)
                {
                    Console.WriteLine("Operations were canelled");

                }
                else
                {
                    MessageBox.Show("Error");
                }
            }
            return _img;
        }






        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap[] rotateImages = { (Bitmap)pictureBox1.Image.Clone(), (Bitmap)pictureBox2.Image.Clone(),
                (Bitmap)pictureBox3.Image.Clone(), (Bitmap)pictureBox4.Image.Clone() };
            Task<Bitmap[]> t1 = Task.Factory.StartNew((arg) =>
            {
                Bitmap[] images = (Bitmap[])arg;

                Parallel.ForEach(images, currentImage =>
                {
                    try
                    {
                        currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    catch (AggregateException ae)
                    {

                        MessageBox.Show("Error");

                    }
                });

                return images;
            }, rotateImages);

            Task t2 = t1.ContinueWith((task) =>
            {
                image = task.Result[0];
                image2 = task.Result[1];
                image3 = task.Result[2];
                image4 = task.Result[3];
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
            }, TaskScheduler.FromCurrentSynchronizationContext());


           


        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap[] rotateImages = { (Bitmap)pictureBox1.Image.Clone(), (Bitmap)pictureBox2.Image.Clone(),
                (Bitmap)pictureBox3.Image.Clone(), (Bitmap)pictureBox4.Image.Clone() };
            Task<Bitmap[]> t1 = Task.Factory.StartNew((arg) =>
            {
                Bitmap[] images = (Bitmap[])arg;

                Parallel.ForEach(images, currentImage =>
                {
                    try
                    {
                        currentImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    catch (AggregateException ae)
                    {
                        MessageBox.Show("Error");
                    }
                });
                return images;
            }, rotateImages);
            Task t2 = t1.ContinueWith((task) =>
            {
                image = task.Result[0];
                image2 = task.Result[1];
                image3 = task.Result[2];
                image4 = task.Result[3];
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap[] flipImages = { (Bitmap)pictureBox1.Image.Clone(), (Bitmap)pictureBox2.Image.Clone(),
                (Bitmap)pictureBox3.Image.Clone(), (Bitmap)pictureBox4.Image.Clone() };
            Task<Bitmap[]> t1 = Task.Factory.StartNew((arg) =>
            {
                Bitmap[] images = (Bitmap[])arg;

                Parallel.ForEach(images, currentImage =>
                {
                    try
                    {
                        currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    catch (AggregateException ae)
                    {
                        MessageBox.Show("Error");
                    }
                });
                return images;
            }, flipImages);
            Task t2 = t1.ContinueWith((task) =>
            {
                image = task.Result[0];
                image2 = task.Result[1];
                image3 = task.Result[2];
                image4 = task.Result[3];
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }  

        private void TrackBarBrightness_Scroll(object sender, EventArgs e)
        {

            Bitmap brImg1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap brImg2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap brImg3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap brImg4 = (Bitmap)pictureBox4.Image.Clone();


            List<Task> tasks = new List<Task>();

            Task brightnessTask1 = Task.Factory.StartNew((arg) =>
            {
                try { 
                Bitmap x = (Bitmap)arg;
                image = LightenDarkenImage(x);
                }
                catch (AggregateException ae)
                {
                    MessageBox.Show("Error in Tasks");

                }
            }, brImg1);
            tasks.Add(brightnessTask1);
            Task brightnessTask2 = Task.Factory.StartNew((arg) =>
            {
                try { 
                Bitmap x = (Bitmap)arg;
                image2 = LightenDarkenImage(x);
                }
                catch (AggregateException ae)
                {
                    MessageBox.Show("Error in Tasks");

                }
            }, brImg2);
            tasks.Add(brightnessTask2);

            Task brightnessTask3 = Task.Factory.StartNew((arg) =>
            {
                try { 
                Bitmap x = (Bitmap)arg;
                image3 = LightenDarkenImage(x);
                }
                catch (AggregateException ae)
                {
                    MessageBox.Show("Error in Tasks");

                }
            }, brImg3);
            tasks.Add(brightnessTask3);

            Task brightnessTask4 = Task.Factory.StartNew((arg) =>
            {
                try { 
                Bitmap x = (Bitmap)arg;
                image4 = LightenDarkenImage(x);
                }
                catch (AggregateException ae)
                {
                    MessageBox.Show("Error in Tasks");

                }
            }, brImg4);
            tasks.Add(brightnessTask4);

            Task.WaitAll(tasks.ToArray());
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                try { 
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
                }
                catch (AggregateException ae)
                {
                    MessageBox.Show("Error in Tasks");

                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);


        }

        private Bitmap LightenDarkenImage(Bitmap brImg1)
        {
            try { 
            int alpha = TrackBarBrightness.Value;
            Rectangle r = new Rectangle(0, 0, brImg1.Width, brImg1.Height);
            if (alpha > 0)
            {
                //Bitmap bmp = new Bitmap(@"Image.png");                           
                using (Graphics g = Graphics.FromImage(brImg1))
                {
                    using (Brush cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                    {
                        g.FillRectangle(cloud_brush, r);
                    }
                }
            }
            else
            {
                alpha = Math.Abs(alpha);
                using (Graphics g = Graphics.FromImage(brImg1))
                {
                    using (Brush cloud_brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                    {
                        g.FillRectangle(cloud_brush, r);
                    }
                }
            }
            }
            catch (AggregateException e)
            {
                MessageBox.Show("Error");

            }
            return brImg1;
        }

        private Bitmap changeBrightness(Bitmap brImg1, ImageAttributes imageAttributes2)
        {
            try { 
            Graphics _g1 = default(Graphics);
            //  Bitmap bm_dest = new Bitmap(Convert.ToInt32(changeBrightness.Width), Convert.ToInt32(changeBrightness.Height));
            _g1 = Graphics.FromImage(brImg1);
            _g1.DrawImage(brImg1, new Rectangle(0, 0, brImg1.Width + 1, brImg1.Height + 1), 0, 0, brImg1.Width + 1, brImg1.Height + 1, GraphicsUnit.Pixel, imageAttributes2);
            }
            catch (AggregateException e)
            {
                MessageBox.Show("Error");

            }
            return brImg1;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveImage();
        }

        private void saveImage()
        {
            try
            {
                if (opened || picBox2IsOpened || picBox3IsOpened || picBox4IsOpened)
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "image| *.jpg; *.png; *.bmp";

                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.Image.Save(save.FileName);
                    }
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox2.Image.Save(save.FileName);
                    }
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox3.Image.Save(save.FileName);
                    }
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox4.Image.Save(save.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("No images are imported...");
                }

            }
            catch (AggregateException ae)
            {
                MessageBox.Show(" save Error " + ae.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
}
        }
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            resetImage();
        }

        private void resetImage()
        {
            Task<Image> resetTask1 = Task.Factory.StartNew(() =>
            {
                Image original = null;
                if (opened)
                {
                    original = Image.FromFile(url1);
                    opened = true;
                }

                return original;
            });
            Task<Image> resetTask2 = Task.Factory.StartNew(() =>
            {
                Image original = null;
                if (picBox2IsOpened)
                {
                    original = Image.FromFile(url2);
                    opened = true;
                }

                return original;
            });
            Task<Image> resetTask3 = Task.Factory.StartNew(() =>
            {
                Image original = null;
                if (picBox3IsOpened)
                {
                    original = Image.FromFile(url3);
                    opened = true;
                }

                return original;
            });
            Task<Image> resetTask4 = Task.Factory.StartNew(() =>
            {
                Image original = null;
                if (picBox4IsOpened)
                {
                    original = Image.FromFile(url4);
                    opened = true;
                }

                return original;
            });

            if (resetTask1.Result != null)
            {
                image = resetTask1.Result;
                pictureBox1.Image = image;
            }
            if (resetTask2.Result != null)
            {
                image2 = resetTask2.Result;
                pictureBox2.Image = image2;
            }
            if (resetTask3.Result != null)
            {
                image3 = resetTask3.Result;
                pictureBox3.Image = image3;
            }
            if (resetTask4.Result != null)
            {
                image4 = resetTask4.Result;
                pictureBox4.Image = image4;
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap sepiaEffect1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap sepiaEffect2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap sepiaEffect3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap sepiaEffect4 = (Bitmap)pictureBox4.Image.Clone();
            CancellationToken token = cts.Token;
            Task<Bitmap> sepiaTask1 = Task.Factory.StartNew((arg) =>
              {
                  Bitmap img = (Bitmap)arg;
                  try
                  {
                      Task t1 = Task.Factory.StartNew(() =>
                      {
                          for (int yCoordinate = 0; yCoordinate < img.Height; yCoordinate++)
                          {
                              for (int xCoordinate = 0; xCoordinate < img.Width; xCoordinate++)
                              {
                                  Color color = img.GetPixel(xCoordinate, yCoordinate);
                                  double grayColor = ((double)(color.R + color.G + color.B)) / 3.0d;
                                  Color sepia = Color.FromArgb((byte)grayColor, (byte)(grayColor * 0.95), (byte)(grayColor * 0.82));
                                  img.SetPixel(xCoordinate, yCoordinate, sepia);
                              }
                          }
                      });

                  }
                  catch (AggregateException ae)
                  {
                      if (ae.InnerException is OperationCanceledException)
                      {
                          Console.WriteLine("Operations were canelled");

                      }
                      else
                      {
                          MessageBox.Show("Error");
                      }
                  }
                  return img;
              }, sepiaEffect1, token);
            Task<Bitmap> sepiaTask2 = Task.Factory.StartNew((arg) =>
            {
                Bitmap img = (Bitmap)arg;

                try
                {
                    for (int yCoordinate = 0; yCoordinate < img.Height; yCoordinate++)
                    {
                        for (int xCoordinate = 0; xCoordinate < img.Width; xCoordinate++)
                        {
                            Color color = img.GetPixel(xCoordinate, yCoordinate);
                            double grayColor = ((double)(color.R + color.G + color.B)) / 3.0d;
                            Color sepia = Color.FromArgb((byte)grayColor, (byte)(grayColor * 0.95), (byte)(grayColor * 0.82));
                            img.SetPixel(xCoordinate, yCoordinate, sepia);
                        }
                    }
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return img;
            }, sepiaEffect2, token);
            Task<Bitmap> sepiaTask3 = Task.Factory.StartNew((arg) =>
            {
                Bitmap img = (Bitmap)arg;

                try
                {
                    for (int yCoordinate = 0; yCoordinate < img.Height; yCoordinate++)
                    {
                        for (int xCoordinate = 0; xCoordinate < img.Width; xCoordinate++)
                        {
                            Color color = img.GetPixel(xCoordinate, yCoordinate);
                            double grayColor = ((double)(color.R + color.G + color.B)) / 3.0d;
                            Color sepia = Color.FromArgb((byte)grayColor, (byte)(grayColor * 0.95), (byte)(grayColor * 0.82));
                            img.SetPixel(xCoordinate, yCoordinate, sepia);
                        }
                    }
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return img;
            }, sepiaEffect3, token);
            Task<Bitmap> sepiaTask4 = Task.Factory.StartNew((arg) =>
            {
                Bitmap img = (Bitmap)arg;

                try
                {
                    for (int yCoordinate = 0; yCoordinate < img.Height; yCoordinate++)
                    {
                        for (int xCoordinate = 0; xCoordinate < img.Width; xCoordinate++)
                        {
                            Color color = img.GetPixel(xCoordinate, yCoordinate);
                            double grayColor = ((double)(color.R + color.G + color.B)) / 3.0d;
                            Color sepia = Color.FromArgb((byte)grayColor, (byte)(grayColor * 0.95), (byte)(grayColor * 0.82));
                            img.SetPixel(xCoordinate, yCoordinate, sepia);
                        }
                    }
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return img;
            }, sepiaEffect4, token);

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    image = sepiaTask1.Result;
                    image2 = sepiaTask2.Result;
                    image3 = sepiaTask3.Result;
                    image4 = sepiaTask4.Result;

                    pictureBox1.Image = image;
                    pictureBox2.Image = image2;
                    pictureBox3.Image = image3;
                    pictureBox4.Image = image4;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }

            }, token, TaskCreationOptions.None, TaskScheduler.Default);
        }

        private void btnMakeSelection_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void btnMakeSelection_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                cropX = e.X;
                cropY = e.Y;
                cropPen = new Pen(Color.PapayaWhip, 1);
                cropPen.DashStyle = DashStyle.DashDotDot;
            }
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            pictureBox3.Refresh();
            pictureBox4.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                pictureBox3.Refresh();
                pictureBox4.Refresh();
                cropWidth = e.X - cropX;
                cropHeight = e.Y - cropY;
                pictureBox1.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);
                pictureBox2.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);
                pictureBox3.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);
                pictureBox4.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Makeselection)
            {
                Cursor = Cursors.Default;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap negImage1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap negImage2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap negImage3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap negImage4 = (Bitmap)pictureBox4.Image.Clone();

            CancellationToken token = cts.Token;
            Task<Bitmap> negativeTask1 = Task.Factory.StartNew(() =>
            {
                Bitmap taskimage = null;
                try
                {
                    taskimage = negativeFunction(negImage1);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return taskimage;


            }, token);
            Task<Bitmap> negativeTask2 = Task.Factory.StartNew(() =>
            {
                Bitmap taskimage = null;
                try
                {
                    taskimage = negativeFunction(negImage2);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return taskimage;


            }, token);
            Task<Bitmap> negativeTask3 = Task.Factory.StartNew(() =>
            {
                Bitmap taskimage = null;
                try
                {
                    taskimage = negativeFunction(negImage3);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return taskimage;


            }, token);
            Task<Bitmap> negativeTask4 = Task.Factory.StartNew(() =>
            {
                Bitmap taskimage = null;
                try
                {
                    taskimage = negativeFunction(negImage4);
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        Console.WriteLine("Operations were canelled");

                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
                return taskimage;

            }, token);

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                image = negativeTask1.Result;
                image2 = negativeTask2.Result;
                image3 = negativeTask3.Result;
                image4 = negativeTask4.Result;
                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;

            }, token, TaskCreationOptions.None, TaskScheduler.Default);
        }

        private Bitmap negativeFunction(Bitmap negativeImage)
        {
            Color c;
            for (int i = 0; i < negativeImage.Width; i++)
            {
                for (int j = 0; j < negativeImage.Height; j++)
                {
                    c = negativeImage.GetPixel(i, j);
                    c = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
                    negativeImage.SetPixel(i, j, c);
                }
            }
            return negativeImage;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Bitmap transImage1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap transImage2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap transImage3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap transImage4 = (Bitmap)pictureBox4.Image.Clone();

            Task<Bitmap> transTask1 = Task.Factory.StartNew((arg) =>
              {
                  Image transImage = (Image)arg;
                  return CopyWithTransparency(transImage);
              }, transImage1);
            Task<Bitmap> transTask2 = Task.Factory.StartNew((arg) =>
            {
                Image transImage = (Image)arg;
                return CopyWithTransparency(transImage);
            }, transImage2);
            Task<Bitmap> transTask3 = Task.Factory.StartNew((arg) =>
            {
                Image transImage = (Image)arg;
                return CopyWithTransparency(transImage);
            }, transImage3);
            Task<Bitmap> transTask4 = Task.Factory.StartNew((arg) =>
            {
                Image transImage = (Image)arg;
                return CopyWithTransparency(transImage);
            }, transImage4);

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                image = transTask1.Result;
                image2 = transTask2.Result;
                image3 = transTask3.Result;
                image4 = transTask4.Result;

                pictureBox1.Image = image;
                pictureBox2.Image = image2;
                pictureBox3.Image = image3;
                pictureBox4.Image = image4;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);


        }



        public Bitmap CopyWithTransparency(Image sourceImage, byte alphaComponent = 100)
        {
            Bitmap bmpNew = GetArgbCopy(sourceImage);
            BitmapData bmpData = bmpNew.LockBits(new Rectangle(0, 0,
                sourceImage.Width, sourceImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            IntPtr ptr = bmpData.Scan0;

            byte[] byteBuffer = new byte[bmpData.Stride * bmpNew.Height];

            Marshal.Copy(ptr, byteBuffer, 0, byteBuffer.Length);

            for (int k = 3; k < byteBuffer.Length; k += 4)
            {
                byteBuffer[k] = alphaComponent;
            }
            Marshal.Copy(byteBuffer, 0, ptr, byteBuffer.Length);
            bmpNew.UnlockBits(bmpData);

            bmpData = null;
            byteBuffer = null;

            return bmpNew;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!picBox2IsOpened)
            {
                OpenPictureBox(2);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!picBox3IsOpened)
            {
                OpenPictureBox(3);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (!picBox4IsOpened)
            {
                OpenPictureBox(4);
            }
        }



        private void button2_Click_1(object sender, EventArgs e)
        {
            cts.Cancel();
            cts = new CancellationTokenSource();
            resetImage();
            TrackBarBrightness.Value = (TrackBarBrightness.Maximum / 2 + TrackBarBrightness.Minimum / 2);
            trackBar4.Value = 0;
            isCubed = false;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap grayImage1 = (Bitmap)pictureBox1.Image.Clone();
                Bitmap grayImage2 = (Bitmap)pictureBox2.Image.Clone();
                Bitmap grayImage3 = (Bitmap)pictureBox3.Image.Clone();
                Bitmap grayImage4 = (Bitmap)pictureBox4.Image.Clone();


                CancellationToken token = cts.Token;
                Task<Bitmap> grayTask1 = Task.Factory.StartNew(() =>
                {
                    Bitmap i = null;
                    try
                    {
                      {
                            i = greyScale(grayImage1);
                        }
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            Console.WriteLine("Operations were canelled");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    return i;
                }, token);
                Task<Bitmap> grayTask2 = Task.Factory.StartNew(() =>
                {
                    Bitmap i = null;
                    try
                    {
                        i = greyScale(grayImage2);
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            Console.WriteLine("Operations were canelled");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    return i;
                }, token);
                Task<Bitmap> grayTask3 = Task.Factory.StartNew(() =>
                {
                    Bitmap i = null;
                    try
                    {
                        i = greyScale(grayImage3);
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            Console.WriteLine("Operations were canelled");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    return i;
                }, token);
                Task<Bitmap> grayTask4 = Task.Factory.StartNew(() =>
                {
                    Bitmap i = null;
                    try
                    {
                        i = greyScale(grayImage4);
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            Console.WriteLine("Operations were canelled");

                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    return i;

                }, token);

               // TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        image = grayTask1.Result;
                        image2 = grayTask2.Result;
                        image3 = grayTask3.Result;
                        image4 = grayTask4.Result;


                        pictureBox1.Image = image;
                        pictureBox2.Image = image2;
                        pictureBox3.Image = image3;
                        pictureBox4.Image = image4;
                    }
                    catch (AggregateException ae)
                    {
                        if (ae.InnerException is OperationCanceledException)
                        {
                            MessageBox.Show("Cancelling tasks");
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                }, token, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            try
            {

                Bitmap contrastImage1 = (Bitmap)pictureBox1.Image.Clone();
                Bitmap contrastImage2 = (Bitmap)pictureBox2.Image.Clone();
                Bitmap contrastImage3 = (Bitmap)pictureBox3.Image.Clone();
                Bitmap contrastImage4 = (Bitmap)pictureBox4.Image.Clone();

                //int threshold = trackBar4.Value;

                Task<Bitmap> contrastTast1 = Task.Factory.StartNew((arg) =>
                {
                    Bitmap image = (Bitmap)arg;
                    return Contrast(image);
                    

                }, contrastImage1);
                Task<Bitmap> contrastTast2 = Task.Factory.StartNew((arg) =>
                {
                    Bitmap image = (Bitmap)arg;
                    return Contrast(image);

                    // return SetContrast(image, threshold);

                }, contrastImage2);
                Task<Bitmap> contrastTast3 = Task.Factory.StartNew((arg) =>
                {
                    Bitmap image = (Bitmap)arg;
                    return Contrast(image);

                    // return SetContrast(image, threshold);

                }, contrastImage3);
                Task<Bitmap> contrastTast4 = Task.Factory.StartNew((arg) =>
                {
                    Bitmap image = (Bitmap)arg;
                    return Contrast(image);

                    // return SetContrast(image, threshold);

                }, contrastImage4);

                TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(() =>
                {
                    image = contrastTast1.Result;
                    image2 = contrastTast2.Result;
                    image3 = contrastTast3.Result;
                    image4 = contrastTast4.Result;

                    pictureBox1.Image = image;
                    pictureBox2.Image = image2;
                    pictureBox3.Image = image3;
                    pictureBox4.Image = image4;

                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }

        }
        public Bitmap Contrast(Bitmap sourceBitmap)
        {
            Bitmap resultBitmap=null;
            try
            {
                int threshold = trackBar4.Value;
                BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                            sourceBitmap.Width, sourceBitmap.Height),
                                            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
                Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
                sourceBitmap.UnlockBits(sourceData);
                double contrastLevel = Math.Pow((100.0 + threshold) / 100.0, 2);
                double blue = 0;
                double green = 0;
                double red = 0;
                for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
                {
                    blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
                                contrastLevel) + 0.5) * 255.0;
                    green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
                                contrastLevel) + 0.5) * 255.0;
                    red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
                                contrastLevel) + 0.5) * 255.0;
                    if (blue > 255)
                    { blue = 255; }
                    else if (blue < 0)
                    { blue = 0; }
                    if (green > 255)
                    { green = 255; }
                    else if (green < 0)
                    { green = 0; }
                    if (red > 255)
                    { red = 255; }
                    else if (red < 0)
                    { red = 0; }
                    pixelBuffer[k] = (byte)blue;
                    pixelBuffer[k + 1] = (byte)green;
                    pixelBuffer[k + 2] = (byte)red;
                }
               resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


                BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                            resultBitmap.Width, resultBitmap.Height),
                                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


                Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
                resultBitmap.UnlockBits(resultData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }

            return resultBitmap;

        }
        private Bitmap SetContrast(Bitmap bmp, int threshold)
        {
            try
            {
                var lockedBitmap = new LockBitmap(bmp);
                lockedBitmap.LockBits();

                var contrast = Math.Pow((100.0 + threshold) / 100.0, 2);
                List<Task> tasks = new List<Task>();
                for (int y = 0; y < lockedBitmap.Height; y++)
                {
                    for (int x = 0; x < lockedBitmap.Width; x++)
                    {
                        var oldColor = lockedBitmap.GetPixel(x, y);
                        var red = ((((oldColor.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                        var green = ((((oldColor.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                        var blue = ((((oldColor.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                        if (red > 255) red = 255;
                        if (red < 0) red = 0;
                        if (green > 255) green = 255;
                        if (green < 0) green = 0;
                        if (blue > 255) blue = 255;
                        if (blue < 0) blue = 0;

                        var newColor = Color.FromArgb(oldColor.A, (int)red, (int)green, (int)blue);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
                lockedBitmap.UnlockBits();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }
            return bmp;

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private Bitmap greyScale(Bitmap grayScale)
        {
            try
            {
                int height = grayScale.Size.Height;
                int width = grayScale.Size.Width;
                for (int yCoordinate = 0; yCoordinate < height; yCoordinate++) //Loop through both the Y (vertical) and X (horizontal)
                {
                    for (int xCoordinate = 0; xCoordinate < width; xCoordinate++)// coordinates of the image.
                    {
                        //Get the pixel that's at our current coordinate.
                        Color color = grayScale.GetPixel(xCoordinate, yCoordinate);
                        //Calculate the gray to use for this pixel.
                        int grayColor = (color.R + color.G + color.B) / 3;
                        //Set the pixel to the new gray color.
                        grayScale.SetPixel(xCoordinate, yCoordinate, Color.FromArgb(grayColor,
                        grayColor,
                        grayColor));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }
            return grayScale;



        }

        private void button7_Click(object sender, EventArgs e)
        {
            isCubed = true;
            Bitmap cube1 = (Bitmap)pictureBox1.Image.Clone();
            Bitmap cube2 = (Bitmap)pictureBox2.Image.Clone();
            Bitmap cube3 = (Bitmap)pictureBox3.Image.Clone();
            Bitmap cube4 = (Bitmap)pictureBox4.Image.Clone();

            Task<Bitmap> cubeTask1 = Task.Factory.StartNew((arg) =>
            {
                Bitmap image = (Bitmap)arg;
                if (opened)
                {
                    return cube(image);
                }
                else
                {
                    return image;
                }
                
            }, pictureBox1.Image.Clone());
            Task<Bitmap> cubeTask2 = Task.Factory.StartNew((arg) =>
            {
                Bitmap image = (Bitmap)arg;
                if (opened)
                {
                    return cube(image);
                }
                else
                {
                    return image;
                }

            }, pictureBox2.Image.Clone());
            Task<Bitmap> cubeTask3 = Task.Factory.StartNew((arg) =>
            {
                Bitmap image = (Bitmap)arg;
                if (opened)
                {
                    return cube(image);
                }
                else
                {
                    return image;
                }

            }, pictureBox3.Image.Clone());
            Task<Bitmap> cubeTask4 = Task.Factory.StartNew((arg) =>
            {
                Bitmap image = (Bitmap)arg;
                if (opened)
                {
                    return cube(image);
                }
                else
                {
                    return image;
                }

            }, pictureBox4.Image.Clone());

            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    image = cubeTask1.Result;
                    image2 = cubeTask2.Result;
                    image3 = cubeTask3.Result;
                    image4 = cubeTask4.Result;


                    pictureBox1.Image = image;
                    pictureBox2.Image = image2;
                    pictureBox3.Image = image3;
                    pictureBox4.Image = image4;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException is OperationCanceledException)
                    {
                        MessageBox.Show("Cancelling tasks");
                    }
                    else
                    {
                        MessageBox.Show("Error");
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

        }

        private void button8_Click(object sender, EventArgs e)
        {

            if (opened || picBox2IsOpened || picBox3IsOpened || picBox4IsOpened)
            {
                Task t1 = Task.Factory.StartNew((arg) =>
                {
                    pictureBox1.Image = (Image)arg;
                    pictureBox2.Image = (Image)arg; ;
                    pictureBox3.Image = (Image)arg; ;
                    pictureBox4.Image = (Image)arg; ;
                }, defaultImage);



                Task t2 = t1.ContinueWith((task) =>
                {
                    opened = false;
                    picBox2IsOpened = false;
                    picBox3IsOpened = false;
                    picBox4IsOpened = false;
                });
            }
            else
            {
                MessageBox.Show("No images found");
            }

           
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {
            


        }

        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                try
                {
                    dlg.Title = "Open Image";
                    dlg.Filter = "files (*.bmp;*.jpg;*.jpeg,*.png,*.jfif)|*.BMP;*.JPG;*.JPEG;*.PNG;*.JFIF";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        image = new Bitmap(dlg.FileName);
                        pictureBox1.Image = image;
                        image2 = new Bitmap(dlg.FileName);
                        pictureBox2.Image = image;
                        image3 = new Bitmap(dlg.FileName);
                        pictureBox3.Image = image;
                        image4 = new Bitmap(dlg.FileName);
                        pictureBox4.Image = image;
                        picBox2IsOpened = true;
                        picBox3IsOpened = true;
                        picBox4IsOpened = true;
                        opened = true;
                        url1 = dlg.FileName;
                        url2 = dlg.FileName;
                        url3 = dlg.FileName;
                        url4 = dlg.FileName;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("ERROR.File type not supported");
                }

            }
        }
        private void btnMakeSelection_MouseDown(object sender, MouseEventArgs e)
        {

        }

        public static Bitmap GetArgbCopy(Image sourceImage)
        {
            Bitmap bmpNew = null ;
            try
            {
                 bmpNew = new Bitmap(sourceImage.Width, sourceImage.Height,
                   PixelFormat.Format32bppArgb);

                using (Graphics graphics = Graphics.FromImage(bmpNew))
                {
                    graphics.DrawImage(sourceImage, new Rectangle(0, 0,
                bmpNew.Width, bmpNew.Height), new Rectangle(0, 0,
                bmpNew.Width, bmpNew.Height), GraphicsUnit.Pixel);
                    graphics.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }
            return bmpNew;
        }

        public void OpenPictureBox(int picturebox)
        {
            try
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Open Image";
                    dlg.Filter = "files (*.bmp;*.jpg;*.jpeg,*.png,*.jfif)|*.BMP;*.JPG;*.JPEG;*.PNG;*.JFIF";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        switch (picturebox)
                        {
                            case 1:
                                image = new Bitmap(dlg.FileName);
                                pictureBox1.Image = image;
                                opened = true;
                                url1 = dlg.FileName;
                                break;
                            case 2:
                                image2 = new Bitmap(dlg.FileName);
                                pictureBox2.Image = image2;
                                picBox2IsOpened = true;
                                url2 = dlg.FileName;
                                break;
                            case 3:
                                image3 = new Bitmap(dlg.FileName);
                                pictureBox3.Image = image3;
                                picBox3IsOpened = true;
                                url3 = dlg.FileName;
                                break;
                            case 4:
                                image4 = new Bitmap(dlg.FileName);
                                pictureBox4.Image = image4;
                                picBox4IsOpened = true;
                                url4 = dlg.FileName;
                                break;
                        }

                        // Create a new Bitmap object from the picture file on disk,
                        // and assign that to the PictureBox.Image property

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR");
            }
        }
        public Bitmap cube(Bitmap cubeImage)
        {
            Graphics g;
            int z = 100, v = 40;
            Point A = new Point(10, 50);
            Point B = new Point(180, 50);
            Point C = new Point(10, 170);
            Point a = new Point(A.X + z, A.Y - v);
            Point b = new Point(B.X + z, B.Y - v);
            Point Z = new Point(B.X, C.Y);
            Point[] p3Fro = { A, B, C };
            Point[] p3Top = { a, b, A };
            Point[] p3Rig = { B, b, Z };
            Bitmap bm = new Bitmap(B.X + z, C.Y + v);
            g = Graphics.FromImage(bm);
            g.DrawImage(cubeImage, p3Fro);
            g.DrawImage(cubeImage, p3Top);
            g.DrawImage(cubeImage, p3Rig);
            g.Dispose();
           return bm;
           
        } 

    }

}
