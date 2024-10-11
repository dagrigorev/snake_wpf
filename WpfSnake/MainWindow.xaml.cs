using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 20;
        private const int SnakeStartLength = 5;
        private const int GameSpeed = 80; // Increased update rate (lower is faster)
        private const double MovementIncrement = 2; // Smaller movement steps

        private enum Direction { Up, Down, Left, Right }
        private Direction currentDirection = Direction.Right;

        private List<Ellipse> snakeParts = new List<Ellipse>();
        private Ellipse food;
        private Random random = new Random();
        private DispatcherTimer gameTimer;
        private int score = 0;

        public MainWindow()
        {
            InitializeComponent();
            StartGame();
        }

        private void StartGame()
        {
            GameCanvas.Children.Clear();
            snakeParts.Clear();
            score = 0;
            UpdateScore();

            currentDirection = Direction.Right;

            // Initialize snake
            for (int i = 0; i < SnakeStartLength; i++)
            {
                AddSnakePart(100 - (i * SnakeSquareSize), 100);
            }

            // Initialize food
            food = new Ellipse
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = Brushes.Red
            };
            GameCanvas.Children.Add(food);
            SpawnFood();

            // Start game loop
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(GameSpeed) };
            gameTimer.Tick += GameTick;
            gameTimer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            MoveSnake();

            if (CheckCollision())
            {
                EndGame();
                return;
            }

            // Check for food collision
            if (Canvas.GetLeft(snakeParts[0]) == Canvas.GetLeft(food) &&
                Canvas.GetTop(snakeParts[0]) == Canvas.GetTop(food))
            {
                GrowSnake();
                SpawnFood();
                score++;
                UpdateScore();
            }
        }

        private void MoveSnake()
        {
            // Move each snake part to the position of the one before it
            for (int i = snakeParts.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(snakeParts[i], Canvas.GetLeft(snakeParts[i - 1]));
                Canvas.SetTop(snakeParts[i], Canvas.GetTop(snakeParts[i - 1]));
            }

            // Move the head based on the current direction
            switch (currentDirection)
            {
                case Direction.Up:
                    Canvas.SetTop(snakeParts[0], Canvas.GetTop(snakeParts[0]) - SnakeSquareSize);
                    break;
                case Direction.Down:
                    Canvas.SetTop(snakeParts[0], Canvas.GetTop(snakeParts[0]) + SnakeSquareSize);
                    break;
                case Direction.Left:
                    Canvas.SetLeft(snakeParts[0], Canvas.GetLeft(snakeParts[0]) - SnakeSquareSize);
                    break;
                case Direction.Right:
                    Canvas.SetLeft(snakeParts[0], Canvas.GetLeft(snakeParts[0]) + SnakeSquareSize);
                    break;
            }
        }

        private void GrowSnake()
        {
            // Add a new part at the end of the snake
            AddSnakePart(
                (int)Canvas.GetLeft(snakeParts[snakeParts.Count - 1]),
                (int)Canvas.GetTop(snakeParts[snakeParts.Count - 1])
            );
        }

        private void AddSnakePart(int x, int y)
        {
            var ellipse = new Ellipse
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = Brushes.Green
            };

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            GameCanvas.Children.Add(ellipse);
            snakeParts.Add(ellipse);
        }

        private bool CheckCollision()
        {
            // Check wall collisions
            if (Canvas.GetLeft(snakeParts[0]) < 0 ||
                Canvas.GetLeft(snakeParts[0]) >= GameCanvas.ActualWidth - SnakeSquareSize ||
                Canvas.GetTop(snakeParts[0]) < 0 ||
                Canvas.GetTop(snakeParts[0]) >= GameCanvas.ActualHeight - SnakeSquareSize)
            {
                return true;
            }

            // Check self-collision
            for (int i = 1; i < snakeParts.Count; i++)
            {
                if (Canvas.GetLeft(snakeParts[0]) == Canvas.GetLeft(snakeParts[i]) &&
                    Canvas.GetTop(snakeParts[0]) == Canvas.GetTop(snakeParts[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private void EndGame()
        {
            gameTimer.Stop();
            MessageBox.Show($"Game Over! Your score: {score}", "Snake Game", MessageBoxButton.OK, MessageBoxImage.Information);
            StartGame();
        }

        private void SpawnFood()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            Canvas.SetLeft(food, random.Next(0, maxX) * SnakeSquareSize);
            Canvas.SetTop(food, random.Next(0, maxY) * SnakeSquareSize);
        }

        private void UpdateScore()
        {
            ScoreText.Text = $"Score: {score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (currentDirection != Direction.Down)
                        currentDirection = Direction.Up;
                    break;
                case Key.Down:
                    if (currentDirection != Direction.Up)
                        currentDirection = Direction.Down;
                    break;
                case Key.Left:
                    if (currentDirection != Direction.Right)
                        currentDirection = Direction.Left;
                    break;
                case Key.Right:
                    if (currentDirection != Direction.Left)
                        currentDirection = Direction.Right;
                    break;
            }
        }
    }
}