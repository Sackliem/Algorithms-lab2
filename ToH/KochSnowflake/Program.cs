using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KochSnowflake
{
    public class KochSnowflake : Form
    {
        private const int Width = 1280;
        private const int Height = 1000;

        public KochSnowflake()
        {
            this.Text = "Koch Snowflake";
            this.Size = new Size(Width, Height);
            this.Paint += new PaintEventHandler(this.DrawSnowflake);
        }

        private void DrawSnowflake(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Blue, 1);

            // Исходные вершины равностороннего треугольника
            PointF pointA = new PointF(200, 700);
            PointF pointB = new PointF(800, 700);
            PointF pointC = new PointF(500, 700-(float)(600 * Math.Sqrt(3) / 2)); // ахуетительное вычисление третьей вершины

            // Рисуем три стороны треугольника с рекурсивными линиями Коха, применяя повороты
            DrawKochLine(g, pen, pointA, pointB, 10); // первая сторона
            DrawKochLine(g, pen, pointB, pointC, 10); // вторая сторона
            DrawKochLine(g, pen, pointC, pointA, 10); // третья сторона с поворотом
        }

        private void DrawKochLine(Graphics g, Pen pen, PointF p1, PointF p2, int level)
        {
            if (level == 0)
            {
                g.DrawLine(pen, p1, p2); // Рисуем прямую линию, если достигли конца рекурсии
            }
            else
            {
                float dx = (p2.X - p1.X) / 3;
                float dy = (p2.Y - p1.Y) / 3;

                PointF pA = new PointF(p1.X + dx, p1.Y + dy);
                PointF pB = new PointF(p1.X + 2 * dx, p1.Y + 2 * dy);

                // Находим точку для "выступа" фрактала Коха
                float midX = (p1.X + p2.X) / 2;
                float midY = (p1.Y + p2.Y) / 2;

                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

                // Изменяем угол на +Math.PI / 3, чтобы выступ пошел в противоположную сторону
                float newX = midX + (float)(length * Math.Cos(angle + Math.PI / 3));
                float newY = midY + (float)(length * Math.Sin(angle + Math.PI / 3));

                PointF pC = new PointF(newX, newY);

                // Рекурсивно рисуем 4 части линии Коха
                DrawKochLine(g, pen, p1, pA, level - 1);
                DrawKochLine(g, pen, pA, pC, level - 1);
                DrawKochLine(g, pen, pC, pB, level - 1);
                DrawKochLine(g, pen, pB, p2, level - 1);
            }
        }

        // Функция для поворота точки относительно другой на определённый угол
        private PointF RotatePoint(PointF center, PointF point, float angleDegrees)
        {
            double angleRadians = angleDegrees * Math.PI / 180;
            float cosTheta = (float)Math.Cos(angleRadians);
            float sinTheta = (float)Math.Sin(angleRadians);

            float dx = point.X - center.X;
            float dy = point.Y - center.Y;

            return new PointF(
                center.X + dx * cosTheta - dy * sinTheta,
                center.Y + dx * sinTheta + dy * cosTheta
            );
        }



        [STAThread]
        public static void Main()
        {
            Application.Run(new KochSnowflake());
        }
    }
}
