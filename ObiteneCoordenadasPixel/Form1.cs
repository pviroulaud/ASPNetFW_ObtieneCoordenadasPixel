using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Collections;
namespace ObiteneCoordenadasPixel
{
    public partial class Form1 : Form
    {
        Bitmap IM,IMv;
        RectangleF TamañoVisible;
        double escala = 1;
        PointF xy = new PointF();
        Graphics GR;
        PointF[] dots;
        PointF PuntoSelectado = new PointF();
        bool editdot = false;
        string ARCHimagen;

        ArrayList Puntos = new ArrayList();

        public Form1()
        {
            InitializeComponent();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (openFileDialog1.Filter == "Archivo de sesion de trabajo (*.scn)|*.scn")
            {
                System.IO.StreamReader rd = new System.IO.StreamReader(openFileDialog1.FileName);
                ARCHimagen = rd.ReadLine();
                IM = new Bitmap(ARCHimagen);
                GR = Graphics.FromImage(IM);

                toolStripComboBox1.SelectedIndex = 0;
                TamañoVisible.X = 0;
                TamañoVisible.Y = 0;
                TamañoVisible.Width = pictureBox1.Width;
                TamañoVisible.Height = pictureBox1.Height;
                IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                hScrollBar1.Maximum = IM.Width - (int)TamañoVisible.Width;
                vScrollBar1.Maximum = IM.Height - (int)TamañoVisible.Height;
                pictureBox1.BackgroundImage = IMv; 

                while (!rd.EndOfStream)
                {
                    string[] lin= rd.ReadLine().Split(new char[] {','});
                    xy.X=(float)(Convert.ToDouble(lin[0]));
                    xy.Y=(float)(Convert.ToDouble(lin[1]));
                    Puntos.Add(xy);
                    dataGridView1.Rows.Add(Puntos.Count-1, xy.X, xy.Y);
                }
                rd.Close();
               
                RedibujarLineas();

            }
            else
            {
                ARCHimagen = openFileDialog1.FileName;
                IM = new Bitmap(ARCHimagen);
                GR = Graphics.FromImage(IM);

                toolStripComboBox1.SelectedIndex = 0;
                TamañoVisible.X = 0;
                TamañoVisible.Y = 0;
                TamañoVisible.Width = pictureBox1.Width;
                TamañoVisible.Height = pictureBox1.Height;
                IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                hScrollBar1.Maximum = IM.Width - (int)TamañoVisible.Width;
                vScrollBar1.Maximum = IM.Height - (int)TamañoVisible.Height;
                pictureBox1.BackgroundImage = IMv; 
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AjustarTamaño();
        }
        private void AjustarTamaño()
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                pictureBox1.Width = 3 * (this.Width - 10) / 4;
                hScrollBar1.Width = pictureBox1.Width;
                vScrollBar1.Left = pictureBox1.Left + pictureBox1.Width + 5;
                dataGridView1.Left = vScrollBar1.Left + vScrollBar1.Width + 5;
                dataGridView1.Width = (1 * (this.Width - 10) / 4) - 60;

                pictureBox1.Height = this.Height - 80 - hScrollBar1.Height;
                hScrollBar1.Top = pictureBox1.Top + pictureBox1.Height + 5;
                vScrollBar1.Height = pictureBox1.Height;
                dataGridView1.Height = pictureBox1.Height;

                TamañoVisible.X = 0;
                TamañoVisible.Y = 0;
                TamañoVisible.Width = pictureBox1.Width;
                TamañoVisible.Height = pictureBox1.Height;
                if (IMv != null)
                {
                    IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    hScrollBar1.Maximum = IM.Width - (int)TamañoVisible.Width;
                    vScrollBar1.Maximum = IM.Height - (int)TamañoVisible.Height;
                    pictureBox1.BackgroundImage = IMv;
                }
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            AjustarTamaño();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripTextBox2.Text = (TamañoVisible.X + (e.X / escala)).ToString() + " - " + (TamañoVisible.Y + (e.Y / escala)).ToString();
            xy.X = (float)(TamañoVisible.X + (e.X / escala));
            xy.Y = (float)(TamañoVisible.Y + (e.Y / escala));
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            TamañoVisible.X = hScrollBar1.Value ;/// (float)escala;
            if (IMv != null)
            {
                IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                pictureBox1.BackgroundImage = IMv; 
            }

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            TamañoVisible.Y = vScrollBar1.Value ;/// (float)escala;
            if (IMv != null)
            {
                IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                pictureBox1.BackgroundImage = IMv; 
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedItem != null)
            {
                escala = Convert.ToDouble(toolStripComboBox1.SelectedItem) / 100;
                TamañoVisible.Width = (int)(pictureBox1.Width / escala);
                TamañoVisible.Height = (int)(pictureBox1.Height / escala);

                if (TamañoVisible.X + TamañoVisible.Width > IM.Width) 
                { TamañoVisible.X = IM.Width - TamañoVisible.Width; }
                if (TamañoVisible.Y + TamañoVisible.Height > IM.Height) 
                { TamañoVisible.Y = IM.Height - TamañoVisible.Height; }
                
                if (IMv != null)
                {
                    IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    hScrollBar1.Maximum = IM.Width - (int)(TamañoVisible.Width);
                    vScrollBar1.Maximum = IM.Height - (int)TamañoVisible.Height;
                    pictureBox1.BackgroundImage = IMv; 
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (editdot)
            {
                Puntos[dataGridView1.SelectedRows[0].Index] = xy;
                dataGridView1.SelectedRows[0].Cells[1].Value = xy.X;
                dataGridView1.SelectedRows[0].Cells[2].Value = xy.Y;
                IM = new Bitmap(openFileDialog1.FileName);
                GR = Graphics.FromImage(IM);
                PuntoSelectado.X = (float)(dataGridView1.SelectedRows[0].Cells[1].Value);
                PuntoSelectado.Y = (float)(dataGridView1.SelectedRows[0].Cells[2].Value);
                if ((PuntoSelectado.X != -1) && (PuntoSelectado.Y != -1))
                {
                    GR.FillEllipse(Brushes.Blue, PuntoSelectado.X - 5, PuntoSelectado.Y - 5, 10, 10);
                }
                editdot = false;
            }
            else
            {
                Puntos.Add(xy);

                dataGridView1.Rows.Add(Puntos.Count-1, xy.X, xy.Y); //AppendText(xy.X.ToString() + " - " + xy.Y.ToString() + "\n");
                
            }
            RedibujarLineas();
            
        }
        private void RedibujarLineas()
        {
            int n = 0;
            

            if (Puntos.Count > 1)
            {
                dots = new PointF[Puntos.Count];
                foreach (PointF P in Puntos)
                {
                    dots[n] = P;
                    GR.DrawString("[" + n.ToString() + "]", new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Blue, dots[n].X+10,dots[n].Y);
                    n++;
                }
                GR.DrawLines(new Pen(Color.Red, 1), dots);
                
                IMv = IM.Clone(TamañoVisible, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                pictureBox1.BackgroundImage = IMv;
            }
        }
        private void guardarCoordenadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt";
            saveFileDialog1.ShowDialog();

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            System.IO.StreamWriter wr = new System.IO.StreamWriter(saveFileDialog1.FileName);
            
            if (saveFileDialog1.Filter == "Archivo de sesion de trabajo (*.scn)|*.scn")
            {
                wr.WriteLine(ARCHimagen);
                foreach (PointF P in dots)
                {
                    wr.WriteLine(P.X.ToString().Replace(',', '.') + "," + P.Y.ToString().Replace(',', '.'));                    
                }
                wr.Close();
            }
            else
            {
                float MAXY = float.MinValue;
                float MAXX = float.MinValue;
                float minY = float.MaxValue;
                float minX = float.MaxValue;

                int n = 0;
                //System.IO.StreamWriter wr = new System.IO.StreamWriter(saveFileDialog1.FileName);
                string[] nombre = saveFileDialog1.FileName.Split(new char[] { '\\', '.' });

                wr.WriteLine("/* Codigo generado por la aplicacion ObtieneCoordenadasPixel.exe V1.0 */");
                wr.WriteLine("/* Desarrolado por Pablo Viroulaud. año 2017 */");
                wr.WriteLine();

                wr.WriteLine("private PointF[] " + nombre[nombre.Length - 2] + " = new PointF[" + dots.Length + "]; // Declaracion del vector de puntos.");
                wr.WriteLine();
                wr.WriteLine("// Asignacion de valores a los puntos del vector.");
                foreach (PointF P in dots)
                {
                    if (P.X > MAXX) { MAXX = P.X; }
                    if (P.Y > MAXY) { MAXY = P.Y; }
                    if (P.X < minX) { minX = P.X; }
                    if (P.Y < minY) { minY = P.Y; }

                    wr.WriteLine(nombre[nombre.Length - 2] + "[" + n.ToString() + "].X=" + P.X.ToString().Replace(',', '.') + "f;");
                    wr.WriteLine(nombre[nombre.Length - 2] + "[" + n.ToString() + "].Y=" + P.Y.ToString().Replace(',', '.') + "f;");
                    n++;
                }
                wr.WriteLine();
                wr.WriteLine("// Maximo valor X= " + MAXX.ToString());
                wr.WriteLine("// Minimo valor X= " + minX.ToString());
                wr.WriteLine("// Maximo valor Y= " + MAXY.ToString());
                wr.WriteLine("// Minimo valor Y= " + minY.ToString());
                wr.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows[0].Cells[1].Value.ToString() != "")
            {
                if (dataGridView1.SelectedRows[0].Index != dataGridView1.Rows.Count)
                {
                    PuntoSelectado.X = (float)(float.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()));
                    PuntoSelectado.Y = (float)(float.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString()));
                    
                    IM = new Bitmap(ARCHimagen);
                    GR = Graphics.FromImage(IM);

                    if ((PuntoSelectado.X != -1) && (PuntoSelectado.Y != -1))
                    {
                        GR.FillEllipse(Brushes.Blue, PuntoSelectado.X - 5, PuntoSelectado.Y - 5, 10, 10);
                    }
                    RedibujarLineas();
                }
                else
                {
                    IM = new Bitmap(ARCHimagen);
                    GR = Graphics.FromImage(IM);
                    PuntoSelectado.X = -1;
                    PuntoSelectado.Y = -1;
                    RedibujarLineas();
                }

            }
        }

        private void modificarPuntoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult R = MessageBox.Show("Seleccione la nueva unbicacion para el punto N°" + dataGridView1.SelectedRows[0].Index + "\n(X,Y) = (" + dataGridView1.SelectedRows[0].Cells[1].Value.ToString() + "," + dataGridView1.SelectedRows[0].Cells[2].Value.ToString()+")");
            if (R == System.Windows.Forms.DialogResult.OK)
            {
                editdot = true;
            }
            else
            {
                editdot = false;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            //dataGridView1.SelectedRows[0].Cells[1].Value = xy.X;
            //dataGridView1.SelectedRows[0].Cells[2].Value = xy.Y;
            IM = new Bitmap(openFileDialog1.FileName);
            GR = Graphics.FromImage(IM);
            PuntoSelectado.X = (float)(float.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString()));
            PuntoSelectado.Y = (float)(float.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString()));
            Puntos[dataGridView1.SelectedRows[0].Index] = PuntoSelectado;
            if ((PuntoSelectado.X != -1) && (PuntoSelectado.Y != -1))
            {
                GR.FillEllipse(Brushes.Blue, PuntoSelectado.X - 5, PuntoSelectado.Y - 5, 10, 10);
            }
            RedibujarLineas();
        }

        private void guardarEstaSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Archivo de sesion de trabajo (*.scn)|*.scn";
            saveFileDialog1.FileName = "";
            saveFileDialog1.ShowDialog();
        }

        private void abrirSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivo de sesion de trabajo (*.scn)|*.scn";
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
        }

        
    }
}
