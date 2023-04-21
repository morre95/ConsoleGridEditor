using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleGridEditor.Classes
{
    internal class GridEditor
    {

        static string DefaultDir { get; set; } = Path.GetFullPath(@"grids\");
        public static Grid[,] Editor(int gridRows, int gridColumns, bool useDoubleSpace = false)
        {
            Grid[,] editGrid = PopulateGrid(gridRows, gridColumns, useDoubleSpace);

            int x = 1;
            int y = 1;

            CLearScreen();

            DrawGrid(gridRows, gridColumns, editGrid, x, y);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (x <= 0) x = gridRows - 1;
                    else x = Math.Max(0, x - 1);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (x >= gridRows - 1) x = 0;
                    else x = Math.Min(gridRows - 1, x + 1);
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (y <= 0) y = gridColumns - 1;
                    else y = Math.Max(0, y - 1);
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (y >= gridColumns - 1) y = 0;
                    else y = Math.Min(gridColumns - 1, y + 1);
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    if (editGrid[x, y].DoubleSpace)
                    {
                        if (editGrid[x, y].GetSymbole() != "  ")
                            editGrid[x, y].SetSymbole("  ");
                        else
                            editGrid[x, y].SetSymbole("* ");
                    }
                    else
                    {
                        if (editGrid[x, y].GetSymbole() != " ")
                            editGrid[x, y].SetSymbole(" ");
                        else
                            editGrid[x, y].SetSymbole("*");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    FieldInfo[] fields = typeof(Emoji).GetFields(BindingFlags.Public | BindingFlags.Static);
                    int currentIndex = 0;
                    int totalFields = fields.Length;

                    while (true)
                    {
                        Console.Clear();

                        FieldInfo currentField = fields[currentIndex];
                        Console.WriteLine(currentField.Name.Replace('_', ' '));
                        Console.WriteLine(currentField.GetValue(null));

                        ConsoleKeyInfo key = Console.ReadKey();

                        if (key.Key == ConsoleKey.UpArrow)
                        {
                            currentIndex = (currentIndex - 1 + totalFields) % totalFields;
                        }
                        else if (key.Key == ConsoleKey.DownArrow)
                        {
                            currentIndex = (currentIndex + 1) % totalFields;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                    editGrid[x, y].SetSymbole(fields[currentIndex].GetValue(null).ToString());
                    if (editGrid[x, y].DoubleSpace && editGrid[x, y].GetSymbole().Length == 1) editGrid[x, y].Symbole += " ";
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    return editGrid;
                }
                else if (keyInfo.Key == ConsoleKey.L)
                {
                    Grid[,] temp = LoadFromFile();

                    gridRows = temp.GetLength(0);
                    gridColumns = temp.GetLength(1);
                    editGrid = PopulateGrid(gridRows, gridColumns, useDoubleSpace);
                    editGrid = temp;
                }
                else if (keyInfo.Key == ConsoleKey.S)
                {
                    SaveToFile(editGrid);
                }

                CLearScreen();
                DrawGrid(gridRows, gridColumns, editGrid, x, y);
            }
        }

        private static void CLearScreen()
        {
            // Clears the screen and the scrollback buffer in xterm-compatible terminals.
            Console.Clear(); Console.WriteLine("\x1b[3J");
        }

        private static Grid[,] PopulateGrid(int gridRows, int gridColumns, bool useDoubleSpace = false)
        {
            Grid[,] editGrid = new Grid[gridRows, gridColumns];

            for (int rows = 0; rows < gridRows; rows++)
            {
                for (int columns = 0; columns < gridColumns; columns++)
                {
                    Grid grid = new Grid(columns, rows, useDoubleSpace);
                    if (grid.x == 0 || grid.x > gridColumns - 2 || grid.y == 0 || grid.y > gridRows - 2)
                    {
                        grid.SetSymbole("*");
                    }
                    else
                    {
                        grid.Clear();
                    }

                    editGrid[rows, columns] = grid;
                }
            }

            return editGrid;
        }

        static void DrawGrid(int gridRows, int gringColumns, Grid[,] editGrid, int x, int y)
        {
            string toPrint = "";
            for (int col = 0; col < gridRows; col++)
            {
                for (int row = 0; row < gringColumns; row++)
                {
                    if (col == x && row == y)
                    {
                        if (editGrid[col, row].DoubleSpace)
                        {
                            toPrint += "+ ";
                        }
                        else
                        {
                            toPrint += "+";
                        }
                    }
                    else
                    {
                        toPrint += editGrid[col, row].GetSymbole();
                    }

                }
                toPrint += "\n";
            }
            Console.WriteLine(toPrint);
            Console.WriteLine("Move with arrow keys");
            Console.WriteLine("Spacebar = add wall, Enter to select emoji");
            Console.WriteLine("L = Load from file, S = Save to file");
        }

        static void SaveToFile(Grid[,] editGrid)
        {
            Console.Write("Filename: ");
            string fileName = Console.ReadLine()!;

            List<Grid[]> rows = new List<Grid[]>();
            for (int i = 0; i < editGrid.GetLength(0); i++)
            {
                Grid[] row = new Grid[editGrid.GetLength(1)];
                for (int j = 0; j < editGrid.GetLength(1); j++)
                {
                    row[j] = editGrid[i, j];
                }
                rows.Add(row);
            }

            if (!Directory.Exists(DefaultDir)) Directory.CreateDirectory(DefaultDir);

            string jsonString = JsonSerializer.Serialize(rows);
            File.WriteAllText(DefaultDir + fileName + ".json", jsonString);
        }

        static string[] GetJsonFiles()
        {
            return Directory.GetFiles(DefaultDir, "*.json", SearchOption.AllDirectories);
        }

        static string SelectFile()
        {
            string[] allJsonFIles = GetJsonFiles();

            int numberOfFiles = allJsonFIles.Length;
            int index = 0;

            while (true)
            {
                Console.Clear();

                Console.WriteLine($"File {Emoji.Point_Right} {Path.GetFileName(allJsonFIles[index]).Replace(".json", "")}");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.UpArrow)
                {
                    index = (index - 1 + numberOfFiles) % numberOfFiles;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    index = (index + 1) % numberOfFiles;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }

            return allJsonFIles[index];
        }

        static Grid[,] LoadFromFile()
        {
            string fileName = SelectFile();
            string jsonString = File.ReadAllText(fileName);

            List<Grid[]> rows = JsonSerializer.Deserialize<List<Grid[]>>(jsonString)!;
            Grid[,] grid = new Grid[rows.Count, rows[0].Length];
            for (int i = 0; i < rows.Count; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    grid[i, j] = rows[i][j];
                }
            }

            return grid;
        }
    }
}
