using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Drawing;

namespace AdvCrypExam.Exercises
{
    public static class SolveShamirSecret
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(nameof(SolveShamirSecret) + "/shamir", SolveShamir);
        }

        public static string SolveShamir(int mod, int deg, [FromBody] List<Point> values)
        {
            if (values.Count <= deg)
                return "Cannot give a definite answer";


            int[,] m = new int[deg + 1, deg + 2];
            for (int i = 0; i < m.GetLength(0); i++)
                for (int j = 0; j < m.GetLength(1); j++)
                    m[i, j] = SolveAlgorithms.GetFastExp(mod, values[i].X, j);

            for (int i = 0; i < m.GetLength(0); i++)
                m[i, m.GetLength(1) - 1] = values[i].Y;

            var res = $"""
                Let P(x) = s + {Enumerable.Range(1, m.GetLength(1) - 2).Select(x => "a" + x + " * x^" + x).Aggregate((a, b) => a + " + " + b)}
                Which gives the following system of ecuations over Z{mod}:
                (print the system based on the polynom and the matrix below)

                Which can be solved using the matrix:
                {PrintMatrix(m)}

                (If not all numbers (except 0) have modular inverse GG)
                (Otherwise we can compute on the draft paper the inverse)

                """;

            for (int i = 0; i < m.GetLength(0); i++)
            {
                if (m[i, i] != 1)
                {
                    int inverse = SolveAlgorithms.GetEuclidModuloInverse(mod, m[i, i]);
                    res += $"Multiply line {i + 1} with {inverse} to get:\n";
                    for (int j = i; j < m.GetLength(1); j++)
                        m[i, j] = (m[i, j] * inverse) % mod;
                    res += PrintMatrix(m) + "\n\n";
                }

                res += Enumerable.Range(0, m.GetLength(0)).Except([i]).Select(x => $"L{x + 1} - {m[x, i]} * L{i + 1}").Aggregate((a, b) => a + "; " + b) + "\n";

                foreach (int j in Enumerable.Range(0, m.GetLength(0)).Except([i]))
                {
                    var fact = m[j, i];
                    for (int k = i; k < m.GetLength(1); k++)
                    {
                        m[j, k] = (m[j, k] + (mod - (m[i, k] * fact) % mod)) % mod;
                    }
                }
                res += PrintMatrix(m) + "\n\n";

            }

            return res;
        }



        public static string PrintMatrix(int[,] m)
        {
            return new Matrix<int>(m).Chunk(m.GetLength(1))
                .Select(x => "|" + x.Select(y => y.ToString().PadLeft(4, ' ')).Aggregate((a, b) => a + ", " + b) + "|")
                .Aggregate((a, b) => a + "\n" + b);
        }

        public class Matrix<T> : IEnumerable<T>
        {
            private T[,] _matrix;

            public Matrix(T[,] matrix)
            {
                _matrix = matrix;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0; i < _matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < _matrix.GetLength(1); j++)
                    {
                        yield return _matrix[i, j];
                    }
                }
            }

            public static implicit operator Matrix<T>(T[,] val) => new Matrix<T>(val);

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int GetLength(int dimension) => _matrix.GetLength(dimension);
        }
    }
}
