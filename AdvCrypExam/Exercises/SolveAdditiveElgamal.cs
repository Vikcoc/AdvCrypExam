namespace AdvCrypExam.Exercises
{
    public static class SolveAdditiveElgamal
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(nameof(SolveAdditiveElgamal) + "/elgamal", SolveElgamal);
        }

        public static string SolveElgamal(int mod, int g, int h, int c1, int c2)
        {
            var res = $"""
                In additive Elgamal h = g * x thus if we multiply with g^-1 we find x
                {SolveAlgorithms.EuclidModuloInverse(mod, g)}


                """;

            var divg = SolveAlgorithms.GetEuclidModuloInverse(mod, g);
            var x = h * divg;

            res += $"""
                x = g^-1 * h mod N = {divg} * {h} mod {mod} = {x} mod {mod}
                """;

            if (x % mod != x)
            {
                x %= mod;
                res += $" = {x} mod {mod}";
            }

            res += "\n\n";

            res += $"""
                In additive Elgamal m = c2 - c1 * x mod N = {c2} - {c1} * {x} mod {mod} = {c2} - {c1 * x} mod {mod}
                """;

            var m = c1 * x;
            if (m % mod != m)
            {
                m %= mod;
                res += $" = {c2} - {m} mod {mod}";
            }

            m = c2 - m;
            res += $" = {m} mod {mod}";

            if (m % mod != m)
            {
                m %= mod;
                res += $" = {m} mod {mod}";
            }

            if (m < 0)
            {
                m += mod;
                res += $" = {m} mod {mod}";
            }

            return res;
        }
    }

}
