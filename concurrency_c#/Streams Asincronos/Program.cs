var cancelationTokenSource = new CancellationTokenSource();

try
{
    await foreach (var names in NamesGenerate(cancelationTokenSource.Token))
    {
        Console.WriteLine(names);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Operacion cancelada");
}
finally
{
    cancelationTokenSource?.Dispose();
}

Console.WriteLine("Final");

async IAsyncEnumerable<string> NamesGenerate(CancellationToken token = default)
{
    yield return "Vicente";
    await Task.Delay(2000, token);
    yield return "Diana";
    await Task.Delay(2000, token);
    yield return "Belen";
}