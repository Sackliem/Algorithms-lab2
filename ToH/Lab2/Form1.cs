using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private int numberOfDisks = 0; 
        private Stack<int>[] rods; 
        private int movesMade = 0;
        private int movingDisk = -1; 
        private int movingDiskX; 
        private int movingDiskY; 
        private Button buttonStop; 
        private CancellationTokenSource cancellationTokenSource; 

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; 

            buttonStop = new Button();
            buttonStop.Text = "Остановить";
            buttonStop.Location = new Point(253, 500); 
            buttonStop.Click += ButtonStop_Click;
            this.Controls.Add(buttonStop);

            rods = new Stack<int>[3];
            for (int i = 0; i < 3; i++)
            {
                rods[i] = new Stack<int>();
            }
            for (int i = numberOfDisks; i > 0; i--)
            {
                rods[0].Push(i);
            }

            this.Paint += new PaintEventHandler(Form1_Paint);
            textBox2.Text = "Диски";
            textBox2.ForeColor = Color.Gray;
            textBox2.GotFocus += RemoveText;
            textBox2.LostFocus += AddText;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawRods(e.Graphics);
            if (movingDisk >= 0)
            {
                // Отрисовываем перемещаемый диск
                e.Graphics.FillRectangle(Brushes.LightSlateGray, movingDiskX, movingDiskY, movingDisk * 20, 12);
            }
        }

        private void DrawRods(Graphics graphics)
        {
            int rodWidth = 10;
            int rodHeight = 150;
            int baseHeight = 300;
            int spacing = 200;

            for (int i = 0; i < rods.Length; i++)
            {
                int x = i * spacing + 100; 
                graphics.FillRectangle(Brushes.Black, x - rodWidth / 2, baseHeight - rodHeight, rodWidth, rodHeight);

                int diskOffset = 0; 
                foreach (var disk in rods[i])
                {
                    graphics.FillRectangle(Brushes.Gray, x - disk * 10, baseHeight - rodHeight + diskOffset - 10, disk * 20, 12);
                    diskOffset += 20;
                }
            }
        }

        private async Task MoveDiskWithAnimation(int fromRod, int toRod)
        {
            if (rods[fromRod].Count > 0)
            {
                int disk = rods[fromRod].Pop();
                int targetX = (toRod * 200 + 100) - disk * 10; 
                int fromX = (fromRod * 200 + 100) - disk * 10; 
                movingDiskX = fromX; 
                int y = 130; 
                movingDiskY = y;
                
                movingDisk = disk;

                for (int i = 0; i < 20; i++)
                {
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    y -= 5; 
                    movingDiskY = y;
                    this.Invalidate(); 
                    await Task.Delay(30);
                }

                movingDiskX = targetX; 
                for (int i = 0; i < 20; i++)
                {
                    this.cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    y += 5; 
                    movingDiskY = y;
                    this.Invalidate(); 
                    await Task.Delay(30); 
                }

                rods[toRod].Push(disk);
                movesMade++;
                movingDisk = -1; 
                this.Invalidate(); 
            }
        }

        private async Task SolveHanoi(int n, int fromRod, int toRod, int tempRod)
        {
            if (n > 0)
            {
                await SolveHanoi(n - 1, fromRod, tempRod, toRod);
                await MoveDiskWithAnimation(fromRod, toRod);
                await SolveHanoi(n - 1, tempRod, toRod, fromRod);
            }
        }

        private async void buttonMove_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int newNumberOfDisks) && newNumberOfDisks > 0)
            {
                numberOfDisks = newNumberOfDisks;

                rods = new Stack<int>[3];
                for (int i = 0; i < 3; i++)
                {
                    rods[i] = new Stack<int>();
                }
                for (int i = numberOfDisks; i > 0; i--)
                {
                    rods[0].Push(i);
                }

                movesMade = 0; 
                cancellationTokenSource = new CancellationTokenSource();
                await SolveHanoi(numberOfDisks, 0, 2, 1); 
                MessageBox.Show($"Сделано движений: {movesMade}");
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите натуральное число дисков."); 
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel(); 
        }

        private void RemoveText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "количество дисков")
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black;
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "количество дисков";
                textBox.ForeColor = Color.Gray;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonMove_Click(sender, e);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
    }
}