namespace AdvCrypExam.Exercises
{
    public static class SolveCipolla
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(nameof(SolveShamirSecret) + "/issqr", IsSquare);
            endpoints.MapGet(nameof(SolveShamirSecret) + "/suitablea", FindSuitableA);
            endpoints.MapGet(nameof(SolveShamirSecret) + "/cipolla", Cipolla);
        }

        public static int GetIsSquare(int mod, int n)
        {
            return SolveAlgorithms.GetFastExp(mod, n, (mod - 1) / 2);
        }
        public static string IsSquare(int mod, int n)
        {
            var res = $"""
                Using Euler's Criterion x is square in Fp iff x^((p-1)/2) = 1 mod p
                So {n} is square in F{mod} iff {n}^(({mod}-1)/2) = 1 mod {mod} = {n}^{(mod - 1) / 2} mod {mod}
                Using fast exp
                {SolveAlgorithms.FastExp(mod, n, (mod - 1) / 2)}
                """;

            return res;
        }

        public static string FindSuitableA(int mod, int n)
        {
            return Enumerable
                .Range(0, mod)
                .Where(x => (x * x) % mod != n)
                .Select(x => (x, (x * x - n) % mod))
                .Select(x => (x.x, x.Item2 < 0 ? mod + x.Item2 : x.Item2))
                .Where(x => GetIsSquare(mod, x.Item2) != 1)
                .Select(x => $"a = {x.x} => a^2 - n = {x.x * x.x} - {n} = {x.Item2} => {IsSquare(mod, x.Item2)}")
                .Aggregate((a, b) => a + "\n\n" + b);
        }

        public static string Cipolla(int mod, int n, int a)
        {
            var ws = (a * a - n) % mod;
            ws = ws < 0 ? ws + mod : ws;
            if (GetIsSquare(mod, ws) == 1)
                return $"{ws} is a square";

            var res = $"""
                p = {mod}
                n = {n}
                a = {a}
                w = sqrt(a^2 - n) = sqrt({ws})
                x = (w + a)^((p - 1)/2) = (w + {a})^(({mod} + 1)/2) = (w + {a})^{(mod + 1) / 2}
                Using fast exp

                """;
            var exp = (mod + 1) / 2;
            //from fast exp
            res += $"\n{exp} = {exp.ToString("b")}";

            var powers = exp.ToString("b").Reverse().Select((c, i) => (keep: c == '1', index: i)).Where(x => x.keep).Select(x => x.index).ToArray();

            res += $" = {powers.Select(x => "2^" + x).Aggregate((a, b) => a + " + " + b)}";

            res += $"\nw^{exp} mod {mod} = {powers.Select(x => "w^2^" + x).Aggregate((a, b) => a + " * " + b)}";

            res += $"\nw^{exp} mod {mod} = {powers.Select(x => "w^" + (1 << x)).Aggregate((a, b) => a + " * " + b)}";

            res += $"\n";

            var pw = new (int free, int coef)[powers[^1] + 1];
            pw[0] = (a, 1);
            for (var i = 1; i < pw.Length; i++)
            {
                pw[i] = ((pw[i - 1].free * pw[i - 1].free + pw[i - 1].coef * pw[i - 1].coef * ws) % mod, (2 * pw[i - 1].free * pw[i - 1].coef) % mod);
                res += $"\nw^{1 << i} mod {mod} = {pw[i - 1].coef * pw[i - 1].coef} * w^2 + {2 * pw[i - 1].free * pw[i - 1].coef} * w + {pw[i - 1].free * pw[i - 1].free}" +
                    $" = {(pw[i - 1].coef * pw[i - 1].coef) % mod} * {ws} + {pw[i].coef} * w + {(pw[i - 1].free * pw[i - 1].free) % mod}" +
                    $" = {pw[i].coef} * w + {pw[i].free}";

                //var aux = (free: pw[i].free > mod / 2 ? mod * -1 + pw[i].free : pw[i].free, coef: pw[i].coef > mod / 2 ? mod * -1 + pw[i].coef : pw[i].coef);

                //aux = (free: pw[i].free > mod / 2 ? mod * -1 + pw[i].free : pw[i].free, coef: pw[i].coef > mod / 2 ? mod * -1 + pw[i].coef : pw[i].coef);
                //if (pw[i] != aux)
                //{
                //    pw[i] = aux;
                //    res += $" = {pw[i].coef} * w + {pw[i].free}";
                //}
            }

            res += $"\n";

            res += $"\nw^{exp} mod {mod} = {powers.Select(x => $"({pw[x].coef} * w + {pw[x].free})").Aggregate((a, b) => a + " * " + b)}";

            res += """

                don't forget about doing the multiplications above to get 1 number;
                also don't forget to do p - x to get the other square root;
                """;

            return res;
        }
    }
}
