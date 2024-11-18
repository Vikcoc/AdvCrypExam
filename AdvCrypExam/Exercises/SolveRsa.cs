namespace AdvCrypExam.Exercises
{
    public static class SolveRsa
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet(nameof(SolveRsa) + "/lambda", Lambda);
            endpoints.MapGet(nameof(SolveRsa) + "/rsa", BreakRsa);

            //φ uppercase Φ, lowercase φ or ϕ;
        }

        public static int GetLambda(int n)
        {
            var fact = SolveAlgorithms.GetFactorization(n).Take(2).ToArray();
            return SolveAlgorithms.GetLcm(fact[0].Index - 1, fact[1].Index - 1);
        }

        public static string Lambda(int n)
        {
            var fact = SolveAlgorithms.GetFactorization(n).Take(2).ToArray();
            var a = fact[0].Index;
            var b = fact[1].Index;
            return $"λ({n}) = lcm({a} - 1, {b} - 1) = {SolveAlgorithms.GetLcm(a - 1, b - 1)}";
        }

        public static string BreakRsa(int mod, int e, int c)
        {
            var res = $"""
                We factorise {mod} to obtain λ
                {SolveAlgorithms.Factorization(mod)}

                {Lambda(mod)}
                

                """;
            var lambda = GetLambda(mod);
            res += $"""
                We know that the private key d = e^-1 mod λ({mod}) thus we use the Extended Euclid algorithm for the inverse
                {SolveAlgorithms.EuclidModuloInverse(lambda, e)}


                """;
            var d = SolveAlgorithms.GetEuclidModuloInverse(lambda, e);

            res += $"""
                The clear message m = c^d mod N = {c}^{d} mod {mod} thus we use fast exponentiation
                {SolveAlgorithms.FastExp(mod, c, d)}

                So m = {SolveAlgorithms.GetFastExp(mod, c, d)}
                """;

            return res;
        }
    }
}
