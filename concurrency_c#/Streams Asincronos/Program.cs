await foreach (var names in NamesGenerate())
{
    Console.WriteLine(names);
}

async IAsyncEnumerable<string> NamesGenerate()
{
    yield return "Vicente";
    await Task.Delay(2000);
    yield return "Diana";
}