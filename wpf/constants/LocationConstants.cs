namespace wpf.constants
{
    public static class LocationConstants
    {
        // Central list of allowed locations for updating kapal location.
        // Keep simple strings so they can be used directly in UI and DB updates.
        public static readonly string[] AllowedLocations = new[]
        {
            "Jakarta",
            "Semarang",
            "Bali",
            "Turkey",
            "London",
            "Banyuwangi"
        };
    }
}
