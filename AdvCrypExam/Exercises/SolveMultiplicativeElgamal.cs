namespace AdvCrypExam.Exercises
{
    public static class SolveMultiplicativeElgamal
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(nameof(SolveMultiplicativeElgamal) + "/elgamal", SolveElgamal);
        }
        public static string SolveElgamal(int mod, int g, int h, int c1, int c2)
        {
            var res = $"""
                In elgamal private key x = random
                Public key h = g^x
                Temporary key y = random
                encrypted message (c1, c2) = (g^y, h^y * m) = (g^y, g^(x * y) * m)
                m = (c1^x)^-1 * c2 mod p
                
                We find x by brute force g^n = {g}^n = {g}
                """;

            var x = 1;
            var pow = g;
            for (x = 1; x <= mod && pow != h; x++) // for increments before evaluating
            {
                pow = (pow * g) % mod;

                res += $", {pow}";
            }

            res += "\n";

            if (pow != h)
            {
                res += "couldn't find x";
                return res;
            }

            res += $"""
                We fast exp c1^x mod p = {c1}^{x} mod {mod}
                {SolveAlgorithms.FastExp(mod, c1, x)}

                """;

            var invc1 = SolveAlgorithms.GetFastExp(mod, c1, x);

            res += $"""
                Then we do the modulo inverse: (c1^x)^-1 = {invc1}^-1 mod {mod}
                {SolveAlgorithms.EuclidModuloInverse(mod, invc1)}

                """;

            invc1 = SolveAlgorithms.GetEuclidModuloInverse(mod, invc1);

            res += $"""
                Then we multiply with c2
                m = (c1^x)^-1 * c2 mod p = {invc1} * {c2} mod {mod} = {invc1 * c2} mod {mod}
                """;

            invc1 = invc1 * c2;

            if (invc1 % mod != invc1)
            {
                invc1 %= mod;
                res += $" = {invc1} mod {mod}";
            }

            return res;
        }
    }
}
