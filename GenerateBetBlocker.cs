string outputFile = "bet-blocker.txt";

// Ignorar verificação SSL. Por algum motivo, o GitHub está retornando um certificado inválido. 
HttpClientHandler handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};

var client = new HttpClient(handler)
{
    BaseAddress = new Uri("https://raw.githubusercontent.com")
};

var request = new HttpRequestMessage(HttpMethod.Get, "/bet-blocker/bet-blocker/refs/heads/main/blocklist.txt");

var response = await client.SendAsync(request);
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine("Erro ao obter a lista de bloqueios.");
    return;
}

string[] baseContent = [
    "# Bet-Blocker",
    $"# Version: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
    "# Source: https://github.com/bet-blocker/bet-blocker/blob/main/blocklist.txt",
    "",
    "||*.bet.br^"
];

var newContent = await response.Content.ReadAsStringAsync();
var newLines = newContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
using var writer = new StreamWriter(outputFile);

foreach (var line in baseContent)
{
    await writer.WriteLineAsync(line);
}

foreach (var line in newLines)
{
    string trimmed = line.Trim();
    if (!string.IsNullOrWhiteSpace(trimmed))
    {
        await writer.WriteLineAsync($"||{trimmed}^");
    }
}

Console.WriteLine($"Blocklist processada e salva em {outputFile}");
