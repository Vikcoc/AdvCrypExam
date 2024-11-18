using Microsoft.AspNetCore.Mvc;

namespace AdvCrypExam.Exercises
{
    public static class SolveAlgorithms
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(nameof(SolveAlgorithms) + "/factorization", Factorization);
            endpoints.MapGet(nameof(SolveAlgorithms) + "/gcd", Gcd);
            endpoints.MapGet(nameof(SolveAlgorithms) + "/lcm", Lcm);
            endpoints.MapGet(nameof(SolveAlgorithms) + "/inverse", EuclidModuloInverse);
            endpoints.MapGet(nameof(SolveAlgorithms) + "/fastexp", FastExp);
            endpoints.MapGet(nameof(SolveAlgorithms) + "/fastmod", FastMod);
            endpoints.MapPost(nameof(SolveAlgorithms) + "/crt", Crt);



        }

        public static IEnumerable<(int Index, int Appearance)> GetFactorization(int number)
        {
            var app = new int[number + 1];
            int i;
            for (i = 0; i < app.Length; i++)
                app[i] = 0;

            i = 2;
            while (number > 1 && i <= number)
            {
                if (number % i == 0)
                {
                    app[i]++;
                    number = number / i;
                }
                else
                {
                    i++;
                }
            }

            return app.Select((x, i) => (Index: i, Appearance: x)).Where(x => x.Appearance > 0);
        }
        public static string Factorization(int number)
        {
            var app = GetFactorization(number);

            return number + " = " + app.Select(x => x.Index + "^" + x.Appearance).DefaultIfEmpty().Aggregate((a, b) => a + " * " + b);
        }

        public static int GetGcd(int a, int b)
        {
            var c = Math.Min(a, b);
            while (c > 1 && (a % c != 0 || b % c != 0))
                c--;
            return c;
        }
        public static string Gcd(int a, int b)
        {
            var c = GetGcd(a, b);
            return $"""
                gcd({a},{b}) = {c}
                {Factorization(a)}
                {Factorization(b)}
                {Factorization(c)}
                """;
        }
        public static int GetLcm(int a, int b)
        {
            return a * b / GetGcd(a, b);
        }
        public static string Lcm(int a, int b)
        {
            return $"""
                lcm({a}, {b}) = {a} * {b} / gcd({a}, {b})
                = {a * b} / {GetGcd(a, b)}
                = {GetLcm(a, b)}
                """;
        }

        public static int GetEuclidModuloInverse(int mod, int num)
        {
            if (GetGcd(num, mod) != 1)
                return -1;

            var lst = new List<(int a, int b, int c, int d)>();

            lst.Add((mod, mod / num, num, mod % num));

            while (lst.Last().d > 1)
                lst.Add((lst.Last().c, lst.Last().c / lst.Last().d, lst.Last().d, lst.Last().c % lst.Last().d));

            lst.Reverse();

            lst[0] = (lst[0].d, lst[0].a, -1 * lst[0].b, lst[0].c);
            for (int i = 1; i < lst.Count; i++)
                lst[i] = (lst[i - 1].c, lst[i].a, lst[i - 1].a + lst[i - 1].c * -1 * lst[i].b, lst[i - 1].b);
            var inverse = lst[^1].c;

            if (inverse % mod != inverse)
                inverse = inverse % mod;

            if (inverse < 0)
                inverse = mod + inverse;

            return inverse;
        }

        public static string EuclidModuloInverse(int mod, int num)
        {
            var res = Gcd(num, mod) + "\n";

            if (GetGcd(num, mod) != 1)
                return res;

            var lst = new List<(int a, int b, int c, int d)>();

            lst.Add((mod, mod / num, num, mod % num));
            res += $"\n{lst.Last().a} = {lst.Last().b} * {lst.Last().c} + {lst.Last().d}";

            while (lst.Last().d > 1)
            {
                lst.Add((lst.Last().c, lst.Last().c / lst.Last().d, lst.Last().d, lst.Last().c % lst.Last().d));
                res += $"\n{lst.Last().a} = {lst.Last().b} * {lst.Last().c} + {lst.Last().d}";
            }

            res = res + "\n";
            lst.Reverse();

            res += $"\n{lst[0].d} = {lst[0].d} * {lst[0].a} + {-1 * lst[0].b} * {lst[0].c} mod {mod}";
            lst[0] = (lst[0].d, lst[0].a, -1 * lst[0].b, lst[0].c);
            for (int i = 1; i < lst.Count; i++)
            {
                res += $"\n = {lst[i - 1].a} * {lst[i - 1].b} + {lst[i - 1].c} * ({lst[i].a} + {-1 * lst[i].b} * {lst[i].c})";
                lst[i] = (lst[i - 1].c, lst[i].a, lst[i - 1].a + lst[i - 1].c * -1 * lst[i].b, lst[i - 1].b);

                res += $" = {lst[i].a} * {lst[i].b} + {lst[i].c} * {lst[i].d} mod {mod}";
            }

            res += $"\n = {lst[^1].c} * {lst[^1].d}";

            var inverse = lst[^1].c;

            if (inverse % mod != inverse)
            {
                inverse = inverse % mod;
                res += $"\n = {inverse} * {lst[^1].d}";
            }

            if (inverse < 0)
            {
                inverse = mod + inverse;
                res += $"\n = {inverse} * {lst[^1].d}";
            }

            return res;
        }

        public static int GetFastExp(int mod, int n, int exp)
        {
            var p = 1;
            for (int i = 0; i < exp; i++)
            {
                p *= n;
                p %= mod;
            }
            return p;
        }
        public static string FastExp(int mod, int n, int exp)
        {
            var res = $"{n}^{exp} mod {mod}";

            res += $"\n{exp} = {exp.ToString("b")}";

            var powers = exp.ToString("b").Reverse().Select((c, i) => (keep: c == '1', index: i)).Where(x => x.keep).Select(x => x.index).ToArray();

            res += $" = {powers.Select(x => "2^" + x).Aggregate((a, b) => a + " + " + b)}";

            res += $"\n{n}^{exp} mod {mod} = {powers.Select(x => n + "^2^" + x).Aggregate((a, b) => a + " * " + b)}";

            res += $"\n{n}^{exp} mod {mod} = {powers.Select(x => n + "^" + (1 << x)).Aggregate((a, b) => a + " * " + b)}";

            res += $"\n";

            var pw = new int[powers[^1] + 1];
            pw[0] = n;
            if (pw[0] > mod / 2)
            {
                pw[0] = mod * -1 + pw[0];
                res += $"\n{n}^{1 << 0} mod {mod} = {pw[0]}";
            }
            for (var i = 1; i < pw.Length; i++)
            {
                pw[i] = (pw[i - 1] * pw[i - 1]);
                res += $"\n{n}^{1 << i} mod {mod} = {pw[i]}";
                if (pw[i] % mod != pw[i])
                {
                    pw[i] = pw[i] % mod;
                    res += $" = {pw[i]}";
                }
                if (pw[i] > mod / 2)
                {
                    pw[i] = mod * -1 + pw[i];
                    res += $" = {pw[i]}";
                }
            }

            res += $"\n";

            res += $"\n{n}^{exp} mod {mod} = {powers.Select(x => pw[x].ToString()).Aggregate((a, b) => a + " * " + b)}";

            var rax = powers.Select(x => pw[x]).Aggregate((a, b) => a * b) % mod;

            res += $" = {rax}";

            if (rax % mod != rax)
            {
                rax = rax % mod;
                res += $" = {rax}";
            }

            if (rax < 0)
            {
                rax = mod + rax;
                res += $" = {rax}";
            }

            return res;
        }

        public static int FastMod(int mod, int n) => n % mod;

        public static int GetCrt(List<Cmod> cmods)
        {
            var bigm = cmods.Select(x => x.Mod).Aggregate((a, b) => a * b);
            var things = cmods.Select(x => (x.N, (bigm / x.Mod), GetEuclidModuloInverse(x.Mod, (bigm / x.Mod) % x.Mod))).ToList();
            return things.Select(x => (x.N * x.Item2 * x.Item3) % bigm).Aggregate((a, b) => (a + b) % bigm);
        }

        public static string Crt([FromBody] List<Cmod> cmods)
        {
            var bigm = cmods.Select(x => x.Mod).Aggregate((a, b) => a * b);
            var res = $"""
                {cmods.Select(x => $"x = {x.N} mod {x.Mod}").Aggregate((a, b) => a + "\n" + b)}


                M = {cmods.Select(x => x.Mod.ToString()).Aggregate((a, b) => a + " * " + b)} = {bigm}
                {Enumerable.Range(0, cmods.Count)
                .Select(x => (x, cmods.Where((y, i) => x != i)))
                .Select(x => $"M{x.x + 1} = {x.Item2.Select(y => y.Mod.ToString()).Aggregate((a, b) => a + " * " + b)} = {bigm / cmods[x.x].Mod}")
                .Aggregate((a, b) => a + "\n" + b)}


                """;
            var things = cmods.Select(x => (x.N, (bigm / x.Mod), GetEuclidModuloInverse(x.Mod, (bigm / x.Mod) % x.Mod))).ToList();
            res += $"""
                {Enumerable.Range(0, cmods.Count)
                .Select(x => $"y{x + 1} = {things[x].Item2}^-1 mod {cmods[x].Mod} = {things[x].Item2 % cmods[x].Mod}^-1 mod {cmods[x].Mod}\n{EuclidModuloInverse(cmods[x].Mod, things[x].Item2 % cmods[x].Mod)}")
                .Aggregate((a, b) => a + "\n" + b)}
                
                x = {things.Select(x => $"({x.N} * {x.Item2} * {x.Item3})").Aggregate((a, b) => a + " + " + b)} mod {bigm}
                  = {things.Select(x => (x.N * x.Item2 * x.Item3) % bigm).Aggregate((a, b) => (a + b) % bigm)}
                """;

            return res;
        }
        public struct Cmod
        {
            public int Mod { get; set; }
            public int N { get; set; }
        }
    }
}
