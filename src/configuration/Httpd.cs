namespace livemap.configuration;

public class Httpd {
    public bool Enabled { get; set; } = true;

    public int Port { get; set; } = 8080;

    public string Host { get; set; } = "localhost";
}
