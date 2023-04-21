using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleGridEditor.Classes
{
    internal class Grid
    {
        public int x { get; set; }

        public int y { get; set; }

        public string Symbole { get; set; }

        public bool DoubleSpace { get; set; }

        [JsonConstructor]
        public Grid(int X, int Y, bool doubleSpace = false)
        {
            x = X;
            y = Y;
            DoubleSpace = doubleSpace;
        }

        public void SetSymbole(string symbole)
        {
            if (DoubleSpace && symbole.Length == 1)
            {
                Symbole = $"{symbole} ";
                Console.WriteLine(symbole);
            }
            else
                Symbole = symbole;
        }

        public string GetSymbole()
        {
            return Symbole;
        }

        public void Clear()
        {
            if (DoubleSpace)
                Symbole = "  ";
            else
                Symbole = " ";
        }

        public void SetDoubleSpace(bool spacing)
        {
            DoubleSpace = spacing;
        }
    }
}
