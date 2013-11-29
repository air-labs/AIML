using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolRecognition
{
    public class BaseMaker
    {
        public double[][] Inputs { get; private set; }
        public double[][] Answers { get; private set; }
        public int InputSize { get; private set; }
        public int OutputSize { get; private set; }

        static int size = 16;
        Bitmap bigBitmap;
        Bitmap bitmap;
        Graphics graphics;
        public  Font Font = new Font("Arial", (int)(size * 0.8));
        Random rnd=new Random();

        public string Symbols;
        public float MinAngle;
        public float MaxAngle;
        public float DeltaAngle;

        public double[] GenerateSymbol(char symbol, float angle)
        {
            graphics.Clear(Color.White);
            graphics.ResetTransform();
            graphics.TranslateTransform(size / 2, size / 2);
            graphics.RotateTransform(angle);
            graphics.TranslateTransform(-size / 2, -size / 2);
            graphics.DrawString(symbol.ToString(), Font, Brushes.Black, new Rectangle(0, 0, size, size),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            var inputVector = new double[InputSize];
            int ptr = 0;
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    inputVector[ptr++] = bitmap.GetPixel(x, y).R > 100 ? 0 : 1;
            return inputVector;
        }

        public double[] GenerateRandom(int index)
        {
            var angle = rnd.NextDouble() * (MaxAngle - MinAngle) + MinAngle;
            return GenerateSymbol(Symbols[index], (float)angle);
        }

        public void Generate()
        {
            InputSize = size * size;
            OutputSize = Symbols.Length;
            var scaleSize = size * 3;
            int rotationsCount = (int)((MaxAngle - MinAngle) / DeltaAngle)+1;

            bigBitmap = new Bitmap((rotationsCount) * scaleSize, scaleSize * Symbols.Length);
            bitmap = new Bitmap(size, size);

     

            graphics = Graphics.FromImage(bitmap);
            var bigG = Graphics.FromImage(bigBitmap);

            var inputs = new List<double[]>();
            var answers=new List<double[]>();

            for (int letterIndex = 0; letterIndex < Symbols.Length; letterIndex++)
            {
                var answerVector = Enumerable.Range(0, Symbols.Length).Select(z => letterIndex == z ? 1.0 : 0.0).ToArray();
                var angle = MinAngle;
                for (int angleIndex = 0; angleIndex < rotationsCount; angleIndex++)
                {
                    var inputVector = GenerateSymbol(Symbols[letterIndex], angle);
                    bigG.DrawImage(bitmap, angleIndex * scaleSize, letterIndex * scaleSize, scaleSize, scaleSize);
                    bigG.DrawRectangle(Pens.Black, angleIndex * scaleSize, letterIndex * scaleSize, scaleSize, scaleSize);
                    inputs.Add(inputVector);
                    answers.Add(answerVector);
                    angle += DeltaAngle;
                 //   ShowBitmap(bitmap);
                }
            }
            Inputs=inputs.ToArray();
            Answers=answers.ToArray();
          
        }

        public void ShowBitmap()
        {
            var form = new Form()
             {
                 ClientSize=bigBitmap.Size,
        Controls = 
                        {
                            new PictureBox
                            {
                                Image=bigBitmap,
                                Dock = DockStyle.Fill
                                
                            }
                        }
              };
            Application.Run(form);
        }
    }
}