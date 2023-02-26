namespace CircuitBreakerSample 
{
    public static class Config 
    {
        
        public static int CircuitOpenTimeout => 4000;
        public static int CircuitClosedErrorLimit = 1;
    }
}