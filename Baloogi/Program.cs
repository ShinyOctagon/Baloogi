using System.Net.Http.Json;

HttpClient client = new();

bool running = true;

while (running)
{
    Console.WriteLine("=== ONLINE SCOREBOARD ===");
    Console.WriteLine("1. Submit Score");
    Console.WriteLine("2. View Scoreboard");
    Console.WriteLine("3. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            await SubmitScore();
            break;

        case "2":
            await ViewScoreboard();
            break;

        case "3":
            running = false;
            Console.WriteLine("Goodbye!");
            break;

        default:
            Console.WriteLine("Invalid choice.");
            break;
    }

    Console.WriteLine();
}

async Task SubmitScore()
{
    //I'ma write a cool name here yahoo
    Console.Write("Name: ");
    string? name = Console.ReadLine();
    
    //dis is de umm da umm
    Console.Write("Score: ");
    string? scoreInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Name cannot be blank.");
        return;
    }

    if (!int.TryParse(scoreInput, out int score) || score < 0)
    {
        Console.WriteLine("Invalid score.");
        return;
    }

    ScoreEntry entry = new ScoreEntry
    {
        Name = name,
        Score = score
    };

    try
    {
        //I want to make the "..." move but maybe stoopid
        Console.WriteLine("Submitting...");
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "https://hooks.zapier.com/hooks/catch/8338993/ujs9jj9/",
            entry
        );

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Score submitted!");
        }
        else
        {
            Console.WriteLine("Could not submit score.");
        }
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Could not connect to the server.");
    }
}

async Task ViewScoreboard()
{
    try
    {
        Console.WriteLine("Loading scoreboard...");

        List<ScoreEntry>? scores =
            await client.GetFromJsonAsync<List<ScoreEntry>>(
                "https://script.google.com/macros/s/AKfycbys5aEPMvNCutyhNYYCcQcCjzsi2UtqNspmKyCH-AicJxJbCJMrAoT0LUaYaXhTWA8n/exec"
            );

        if (scores == null || scores.Count == 0)
        {
            Console.WriteLine("No scores found.");
            return;
        }

        List<ScoreEntry> sortedScores = scores
            .OrderByDescending(s => s.Score)
            .Take(10)
            .ToList();

        Console.WriteLine("=== LEADERBOARD ===");

        for (int i = 0; i < sortedScores.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {sortedScores[i].Name,-12} {sortedScores[i].Score}");
        }
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Could not connect to the server.");
    }
}

public class ScoreEntry
{
    public string Name { get; set; } = "";
    public int Score { get; set; }
}