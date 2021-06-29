using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace SnakeGame3
{
    public partial class SnakeGame : Form
    {
        int columns;
        int rows;
        int snakeX;
        int snakeY;
        int startX;
        int startY;
        int snakeLength;
        int startLength;
        int score;
        string direction = "";
        int[,] snakeArray;
        int[,] foodArray;
        bool gameOver = false;
        string filePath = AppDomain.CurrentDomain.BaseDirectory + "SavedScores.txt";
        string readFile;
        Timer timer = new Timer();
        Random random = new Random();
        Queue<Point> snakePointQueue = new Queue<Point>();
        List<Point> foodPoint = new List<Point>();

        public SnakeGame()
        {
            InitializeComponent();
            columns = 40; rows = 40; //grid
            snakeArray = new int[columns, rows];
            foodArray = new int[columns, rows];
            startX = columns / 2;
            startY = rows / 2;
            snakeX = startX;
            snakeY = startY;
            StartTimer();
            this.DoubleBuffered = true; //prevents flickering
            NewGame();
        }
        private void StartTimer()
        {
            timer.Interval = 30; //snake speed
            timer.Tick += Update;
            timer.Start();
        }
        private void SnakeGame_Paint(object sender, PaintEventArgs e) //drawing snake and food
        {
            int counter = 1;
            Graphics gfx = e.Graphics;
            foreach (Point point in snakePointQueue.ToList())
            {
                if (counter++ == snakePointQueue.Count())
                {
                    gfx.FillRectangle(Brushes.DarkGreen, point.X * this.Size.Width / columns, point.Y * this.Size.Height / rows, 20, 20);
                }
                else
                    gfx.FillRectangle(Brushes.LightGreen, point.X * this.Size.Width / columns, point.Y * this.Size.Height / rows, 20, 20);
            }
            foreach (Point point in foodPoint.ToList())
            {
                gfx.FillRectangle(Brushes.Red, point.X * this.Size.Width / columns, point.Y * this.Size.Height / rows, 20, 20);
            }
        }

        private void SnakeGame_KeyDown(object sender, KeyEventArgs e) //movement direction
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "down")
                        direction = "up";
                    break;
                case Keys.Down:
                    if (direction != "up")
                        direction = "down";
                    break;
                case Keys.Right:
                    if (direction != "left")
                        direction = "right";
                    break;
                case Keys.Left:
                    if (direction != "right")
                        direction = "left";
                    break;
                case Keys.R:
                    if (gameOver == true)
                        NewGame();
                    gameOver = false;
                    break;
                default:
                    break;
            }
        }
        private void NewGame()
        {
            direction = "";
            Array.Clear(snakeArray, 0, snakeArray.Length);
            Array.Clear(foodArray, 0, foodArray.Length);
            snakePointQueue.Clear();
            foodPoint.Clear();
            snakeX = startX;
            snakeY = startY;
            snakeLength = 0;
            startLength = 4;
            snakeArray[snakeX, snakeY] = 1;
            snakePointQueue.Enqueue(new Point(snakeX, snakeY));
            Point fPoint = new Point(random.Next(columns) - 1, random.Next(rows) - 3);
            foodPoint.Add(fPoint);
            foodArray[fPoint.X, fPoint.Y] = 1;
            timer.Enabled = true;
        }
        private void Update(object sender, EventArgs e)
        {
            if (direction != "")
            {
                switch (direction)
                {
                    case "left":
                        if (snakeX != 0) { snakeX--; }
                        else { snakeX = columns - 1; } //going through border
                        break;
                    case "right":
                        if (snakeX != columns - 1) { snakeX++; }
                        else { snakeX = 0; }
                        break;
                    case "up":
                        if (snakeY != 0) { snakeY--; }
                        else { snakeY = rows - 3; }
                        break;
                    case "down":
                        if (snakeY != rows - 3) { snakeY++; }
                        else { snakeY = 0; }
                        break;
                    default:
                        break;
                }
                if (snakeArray[snakeX, snakeY] == 0)
                {
                    snakeArray[snakeX, snakeY] = 1;
                    snakePointQueue.Enqueue(new Point(snakeX, snakeY)); //snake movement
                }
                else //eating yourself (collision)
                {
                    timer.Enabled = false;
                    score = snakeLength - startLength;
                    gameOver = true;
                    SaveScores();
                    MessageBox.Show(" Przegrales!\n Twoj wynik: " + score.ToString() + "\n \"R\" resetuje gre" + "\n" + readFile);
                }
                if (foodArray[snakeX, snakeY] != 1 && snakeLength >= startLength) //food not eaten
                {
                    Point dequeue = snakePointQueue.Dequeue();
                    snakeArray[dequeue.X, dequeue.Y] = 0;
                }
                else if (snakeLength < startLength) //start game
                {
                    snakeLength++;
                }
                else //food eaten
                {
                    snakeLength++;
                    foodArray[snakeX, snakeY] = 0;
                    Point fPoint = new Point(random.Next(columns - 1), random.Next(rows - 3));
                    foodPoint[0] = fPoint;
                    foodArray[fPoint.X, fPoint.Y] = 1;
                }
            }
            Refresh();
        }
        private void SaveScores()
        {
            var lineCount = 1;
            if (!File.Exists(filePath))
            {
                using (var scoreToFile = File.CreateText(filePath))//creating .txt file
                {
                    scoreToFile.WriteLine(lineCount + ". Gra - " + score + " Punktow");
                }
            }
            else
            {
                using (var reader = File.OpenText(filePath))
                {
                    while (reader.ReadLine() != null) { lineCount++; } //reading number of lines from file
                }
                using (var scoreToFile = File.AppendText(filePath))//creating new lines in file
                {
                    scoreToFile.WriteLine(lineCount + ". Gra - " + score + " Punktow");
                }
            }
            readFile = File.ReadAllText(filePath);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
