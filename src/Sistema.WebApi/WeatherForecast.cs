namespace Sistema.WebApi;

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
    // https://www.tiktok.com/@shuralifes/video/7535873119576231174?is_from_webapp=1&sender_device=pc
    // https://www.tiktok.com/@sharkys009/video/7547218854653562117?is_from_webapp=1&sender_device=pc
    //https://www.tiktok.com/@lily_.350/video/7535155518629760263?is_from_webapp=1&sender_device=pc
    //https://www.tiktok.com/@zerito_deidad/video/7543983847642369286?is_from_webapp=1&sender_device=pc
}

