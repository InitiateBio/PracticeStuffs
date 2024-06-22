using System;
using System.Collections.Generic;
using System.Threading;

class SnakeGame
{
    // Direction options for the snake's movement
    enum Direction { Up, Down, Left, Right }

    // Constants that define the game area size
    const int Width = 40;
    const int Height = 20;

    // Variables to keep track of the game's state
    static int score = 0;
    static bool gameover = false;
    static Direction dir = Direction.Right;
    static List<int> snakeX = new List<int>(); // Snake's X positions
    static List<int> snakeY = new List<int>(); // Snake's Y positions
    static List<Enemy> enemies = new List<Enemy>(); // List of enemies
    static Random rand = new Random(); // Random number generator for food and enemies
    static int foodX; // Food's X position
    static int foodY; // Food's Y position

    // Setup the game
    static void Initialize()
    {
        Console.CursorVisible = false; // Hide the cursor
        Console.SetWindowSize(Width + 1, Height + 1); // Set the size of the game window

        // Start the snake in the middle of the screen
        snakeX.Add(Width / 2);
        snakeY.Add(Height / 2);

        // Place the first food item
        PlaceFood();
    }

    // Draw everything on the screen
    static void Draw()
    {
        Console.Clear(); // Clear the screen

        // Draw the snake
        for (int i = 0; i < snakeX.Count; i++)
        {
            Console.SetCursorPosition(snakeX[i], snakeY[i]); // Move cursor to snake position
            Console.ForegroundColor = GetRainbowColor(i); // Set color for snake part
            Console.Write('@'); // Draw snake part
        }

        // Draw enemies
        foreach (Enemy enemy in enemies)
        {
            Console.SetCursorPosition(enemy.X, enemy.Y); // Move cursor to enemy position
            Console.ForegroundColor = ConsoleColor.Red; // Set color for enemy
            Console.Write('X'); // Draw enemy
        }

        // Draw food
        Console.SetCursorPosition(foodX, foodY); // Move cursor to food position
        Console.ForegroundColor = ConsoleColor.Green; // Set color for food
        Console.Write('o'); // Draw food

        // Draw score at the bottom of the screen
        Console.SetCursorPosition(0, Height);
        Console.Write("Score: " + score);
    }

    // Update the game's state
    static void Update()
    {
        // Move the snake
        for (int i = snakeX.Count - 1; i >= 1; i--)
        {
            // Move each part of the snake to the position of the part in front of it
            snakeX[i] = snakeX[i - 1];
            snakeY[i] = snakeY[i - 1];
        }

        // Move the head of the snake based on the direction
        switch (dir)
        {
            case Direction.Up:
                snakeY[0]--;
                break;
            case Direction.Down:
                snakeY[0]++;
                break;
            case Direction.Left:
                snakeX[0]--;
                break;
            case Direction.Right:
                snakeX[0]++;
                break;
        }

        // Check if the snake hits the walls
        if (snakeX[0] < 0 || snakeX[0] >= Width || snakeY[0] < 0 || snakeY[0] >= Height)
            gameover = true; // End the game if the snake hits the walls

        // Check if the snake hits itself
        for (int i = 1; i < snakeX.Count; i++)
        {
            if (snakeX[0] == snakeX[i] && snakeY[0] == snakeY[i])
                gameover = true; // End the game if the snake hits itself
        }

        // Check if the snake eats the food
        if (snakeX[0] == foodX && snakeY[0] == foodY)
        {
            score++; // Increase score
            PlaceFood(); // Place new food
            snakeX.Add(snakeX[snakeX.Count - 1]); // Add new part to the snake
            snakeY.Add(snakeY[snakeY.Count - 1]);

            // Add a new enemy
            AddEnemy();
        }

        // Move enemies
        foreach (Enemy enemy in enemies)
        {
            enemy.Move();
        }

        // Check if the snake hits an enemy
        foreach (Enemy enemy in enemies)
        {
            if (snakeX[0] == enemy.X && snakeY[0] == enemy.Y)
                gameover = true; // End the game if the snake hits an enemy
        }
    }

    // Place food in a random position
    static void PlaceFood()
    {
        foodX = rand.Next(0, Width);
        foodY = rand.Next(0, Height);

        // Make sure the food is not placed on the snake
        if (snakeX.Contains(foodX) && snakeY.Contains(foodY))
            PlaceFood(); // If food is on the snake, place it again

        // Make sure the food is not placed on an enemy
        foreach (Enemy enemy in enemies)
        {
            if (enemy.X == foodX && enemy.Y == foodY)
                PlaceFood(); // If food is on an enemy, place it again
        }
    }

    // Add a new enemy at a random position
    static void AddEnemy()
    {
        int enemyX, enemyY;
        do
        {
            enemyX = rand.Next(0, Width);
            enemyY = rand.Next(0, Height);
        } while (snakeX.Contains(enemyX) && snakeY.Contains(enemyY)); // Make sure enemy is not on the snake

        enemies.Add(new Enemy(enemyX, enemyY)); // Add new enemy to the list
    }

    // Get a rainbow color for the snake
    static ConsoleColor GetRainbowColor(int index)
    {
        ConsoleColor[] colors = {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
            ConsoleColor.Cyan,
            ConsoleColor.Blue,
            ConsoleColor.Magenta
        };

        return colors[index % colors.Length]; // Cycle through colors for a rainbow effect
    }

    // Class to define an enemy that moves randomly
    class Enemy
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Enemy(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Move the enemy in a random direction
        public void Move()
        {
            int dir = rand.Next(4);
            switch (dir)
            {
                case 0: // Up
                    Y--;
                    break;
                case 1: // Down
                    Y++;
                    break;
                case 2: // Left
                    X--;
                    break;
                case 3: // Right
                    X++;
                    break;
            }

            // Make sure the enemy wraps around the screen
            if (X < 0) X = Width - 1;
            if (X >= Width) X = 0;
            if (Y < 0) Y = Height - 1;
            if (Y >= Height) Y = 0;
        }
    }

    // Main method where the game starts
    static void Main(string[] args)
    {
        Initialize(); // Setup the game

        while (!SnakeGame.gameover)
        {
            Draw(); // Draw the game
            Update(); // Update the game

            // Handle keyboard input for controlling the snake
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        // If the snake is not moving down, let it move up
                        if (dir != Direction.Down)
                            dir = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        // If the snake is not moving up, let it move down
                        if (dir != Direction.Up)
                            dir = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        // If the snake is not moving right, let it move left
                        if (dir != Direction.Right)
                            dir = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        // If the snake is not moving left, let it move right
                        if (dir != Direction.Left)
                            dir = Direction.Right;
                        break;
                }
            }

            // Slow down the game so it doesn't run too fast
            Thread.Sleep(100);
        }
    }
}
