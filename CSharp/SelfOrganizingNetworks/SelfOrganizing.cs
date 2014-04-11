using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Common;

namespace SelfOrganizingNetworks
{
    static class SelfOrganizing
    {
        static SelfOrganizingTaskV0 task = new SelfOrganizingTaskV5();


        static double[][] Inputs;
        static Random rnd = new Random(1);
        static DistanceNetwork network;
        static SOMLearning learning;
        static int iterationsBetweenDrawing=10;

        #region Обучение сети 


        static void Learning()
        {
            for (int i = 0; i<iterationsBetweenDrawing ; i++)
                learning.Run(Inputs[rnd.Next(Inputs.Length)]);
            
            
             map = new MapElement[task.NetworkWidth, task.NetworkHeight];
             int number = 0;
             for (int x = 0; x < task.NetworkWidth; x++)
                 for (int y = 0; y < task.NetworkHeight; y++)
                 {
                     var neuron = network.Layers[0].Neurons[x * task.NetworkHeight + y];
                     map[x, y] = new MapElement { X = (float)neuron.Weights[0], Y = (float)neuron.Weights[1], Id = number++ };
                 }

             foreach (var e in Inputs)
             {
                 network.Compute(e);
                 var winner = network.GetWinner();
                 map[winner / task.NetworkHeight, winner % task.NetworkHeight].IsActive = true;
             }
        }


  
    
        #endregion

        static Form form; 
        static MapElement[,] map;
        static MyUserControl pointsPanel;
        static MyUserControl networkPanel;
        static MyUserControl networkGraphControl;
        static int[,] space;
        static Bitmap spaceBitmap;
        static int selected = -1;
       
        
        #region Рисование

        static Color GetColor(int mapX, int mapY)
        {
            return Color.FromArgb(200 - 200 * mapY / task.NetworkHeight, 150, 200 - 200 * mapX / task.NetworkWidth);
        }

        static Brush GetBrush(MapElement element)
        {
            if (element.Id == selected) return Brushes.Magenta;
            else if (element.IsActive) return new SolidBrush(GetColor(element.MapX,element.MapY));
            else return Brushes.LightGray;
        }

       
       
        static void DrawGraph(object sender, PaintEventArgs args)
        {
            if (map == null) return;
            var g = args.Graphics;
            var W = pointsPanel.ClientSize.Width - 20;
            var H = pointsPanel.ClientSize.Height - 20;
            g.Clear(Color.White);
            g.TranslateTransform(10, 10);
            var pen=new Pen(Color.FromArgb(100,Color.LightGray));
            foreach (var e in map)
            {
                if (e.MapX != task.NetworkWidth - 1)
                    g.DrawLine(pen, W * e.X, H * e.Y, W * map[e.MapX + 1, e.MapY].X, H * map[e.MapX + 1, e.MapY].Y);
                if (e.MapY != task.NetworkHeight - 1)
                    g.DrawLine(pen, W * e.X, H * e.Y, W * map[e.MapX, e.MapY + 1].X, H * map[e.MapX, e.MapY + 1].Y);
            }

            foreach(var e in map)
            {
                g.FillEllipse(GetBrush(e),
                    e.X*W-3,
                    e.Y*W-3,
                    6,
                    6);
                
            }
        }

        static void DrawPoints(object sender, PaintEventArgs aegs)
        {
            var g = aegs.Graphics;
            g.Clear(Color.White);

            var W = pointsPanel.ClientSize.Width;
            var H = pointsPanel.ClientSize.Height;

            if (spaceBitmap != null)
            {
                g.DrawImage(spaceBitmap, 0, 0);

                if (selected != -1)
                {
                    var highlight=new SolidBrush(Color.FromArgb(100,Color.White));
                    for (int x = 0; x < W; x++)
                        for (int y = 0; y < H; y++)
                            if (space[x, y] == selected)
                                g.FillRectangle(highlight, x, y, 1, 1);
                }
            };


            foreach (var e in Inputs)
            {
                g.FillEllipse(Brushes.Black,
                    (int)(W * e[0]) - 2,
                    (int)(H * e[1]) - 2,
                    4, 4);
            }



        }

        static void DrawConnection(Graphics g, MapElement n1, MapElement n2)
        {
            if (!n1.IsActive || !n2.IsActive) return;
            var distance = Math.Sqrt(Math.Pow(n1.X - n2.X, 2) + Math.Pow(n1.Y - n2.Y, 2));
            distance = Math.Min(1, distance * 5);
            var Pen = new Pen(Color.FromArgb((int)(distance * 128 + 120), GetColor(n1.MapX,n1.MapY)),2);
            g.DrawLine(Pen, n1.DisplayLocation, n2.DisplayLocation);
        }

        static void DrawNetwork(object sender, PaintEventArgs aegs)
        {
            if (map == null) return;
            var W = pointsPanel.ClientSize.Width - 20;
            var H = pointsPanel.ClientSize.Height - 20;
            
            var g = aegs.Graphics;
            for (int x = 0; x < task.NetworkWidth; x++)
                for (int y = 0; y < task.NetworkHeight; y++)
                    map[x, y].DisplayLocation = new Point(10 + x * W / task.NetworkWidth, 10 + y * H / task.NetworkHeight);

            for (int x = 0; x < task.NetworkWidth; x++)
                for (int y = 0; y < task.NetworkHeight; y++)
                {
                    if (x != task.NetworkWidth - 1) DrawConnection(g, map[x, y], map[x + 1, y]);
                    if (y != task.NetworkHeight - 1) DrawConnection(g, map[x, y], map[x, y + 1]);
                    g.FillEllipse(
                       GetBrush(map[x, y]),
                       map[x, y].DisplayLocation.X - 5,
                       map[x, y].DisplayLocation.Y - 5,
                       10, 10);
                }
        }
        #endregion
        #region Исследование после обучения

        static System.Windows.Forms.Timer timer;
        static bool paused;

        static void PauseResume(object sender, EventArgs e)
        {
            paused = !paused;
            if (paused)
            {
                timer.Stop();
                MakeSpace();
                pointsPanel.MouseMove += PointMouseMove;
                networkPanel.MouseMove += NetworkMouseMove;
            }
            else
            {
                selected = -1;
                spaceBitmap = null;
                timer.Start();
            }
            form.Invalidate(true);
        }


        static void MakeSpace()
        {
            var Colors = new[] { Color.Orange, Color.LightGray, Color.LightGreen, Color.LightBlue, Color.LightYellow, Color.LightCoral, Color.LightCyan };

            space = new int[pointsPanel.ClientSize.Width, pointsPanel.ClientSize.Height];
            spaceBitmap = new Bitmap(pointsPanel.ClientSize.Width,pointsPanel.ClientSize.Height);
            for (int x=0;x<spaceBitmap.Width;x++)
                for (int y = 0; y < spaceBitmap.Height; y++)
                {
                    network.Compute(new double[] { (double)x / spaceBitmap.Width, (double)y / spaceBitmap.Height });
                    var winner = network.GetWinner();
                    space[x, y] = winner;
                    var n = map.Cast<MapElement>().Where(z => z.Id == winner).First();
                    if (n.IsActive)
                        spaceBitmap.SetPixel(x, y, GetColor(n.MapX,n.MapY));
                }

        }


        static void PointMouseMove(object sender, MouseEventArgs e)
        {
            selected = space[e.X,e.Y];
            form.Invalidate(true);
        }
       

        static void NetworkMouseMove(object sender, MouseEventArgs e)
        {
            if (map == null) return;
            foreach (var m in map)
            {
                if (Math.Abs(e.X - m.DisplayLocation.X) < 5 && Math.Abs(e.Y - m.DisplayLocation.Y) < 5)
                {
                    if (selected!=m.Id)
                    {
                        selected = m.Id;
                        form.Invalidate(true);
                    }
                    return;
                }
            }
            if (selected != -1)
            {
                selected = -1;
                form.Invalidate(true);
            }

        }
        #endregion

        class MapElement
        {
            public float X;
            public float Y;
            public int Id;
            public Point DisplayLocation;
            public bool IsActive;
            public int MapX { get { return Id / task.NetworkHeight; } }
            public int MapY { get { return Id % task.NetworkHeight; } }

        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            network = new DistanceNetwork(2, task.NetworkWidth * task.NetworkHeight);
            for (int x=0;x<task.NetworkWidth;x++)
                for (int y = 0; y < task.NetworkHeight; y++)
                {
                    var n = network.Layers[0].Neurons[x * task.NetworkHeight + y];
                    n.Weights[0] = rnd.NextDouble() * 0.2 + 0.4;
                    n.Weights[1] = rnd.NextDouble() * 0.2 + 0.4;
                }
            learning = new SOMLearning(network, task.NetworkWidth, task.NetworkHeight);
            learning.LearningRadius = task.LearningRadius;
            learning.LearningRate = task.LearningRate;


            Inputs = task.GenerateInputs().ToArray();
         
            pointsPanel = new MyUserControl() { Dock= DockStyle.Fill};
            pointsPanel.Paint += DrawPoints;
            networkPanel = new MyUserControl() { Dock = DockStyle.Fill  };
            networkPanel.Paint += DrawNetwork;
            networkGraphControl = new MyUserControl { Dock = DockStyle.Fill  };
            networkGraphControl.Paint += DrawGraph;
            var pauseButton = new Button { Text = "Pause/Resume" };
            pauseButton.Click+=PauseResume;

            var table = new TableLayoutPanel() { Dock = DockStyle.Fill, RowCount=2, ColumnCount=2 };
            table.Controls.Add(pointsPanel, 0, 0);
            table.Controls.Add(networkPanel, 0, 1);
            table.Controls.Add(networkGraphControl, 1, 0);
            table.Controls.Add(pauseButton,1,1);
           // table.Controls.Add(pause, 1, 1);
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

          



            form = new Form()
            {
                ClientSize = new Size(600, 600),
                Controls = 
                {
                   table
                }
            };

            timer = new System.Windows.Forms.Timer();
            timer.Tick += (sender, args) => { Learning(); form.Invalidate(true); };
            timer.Interval = 100;
            timer.Start();


            Application.Run(form);

        }

      

       
    }
}
