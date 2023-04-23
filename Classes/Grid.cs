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
        /// <summary>
        /// Row
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// Column
        /// </summary>
        public int y { get; set; }

        public string Symbole { get; set; }

        // TBD: Undra om inte hela iden med double space bör skrotas. Efteroms det blir förvirrande och så gör det koden svårhanterad. Antingen så skrotas alla emojis med två tecken eller så är det alltid double space
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
                Symbole = $"{symbole} ";
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
