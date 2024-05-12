using System.Runtime.Serialization.Json;

public class Words
{
    /// <summary>
    /// Возвращает случайное слово из файла длиной length. Если length = int.MaxValue, то возвращаются слова длиной больше 7
    /// </summary>
    public static string GetRandomWord(int length)
    {
        Random random = new Random();

        string[] words = GetWords().Where(x => length != int.MaxValue ? x.Length == length : x.Length >= 7).ToArray();

        return words[random.Next(words.Length)];
    }

    /// <summary>
    /// Возвращает массив всех слов из файла
    /// </summary>
    /// <returns></returns>
    private static string[] GetWords()
    {
        return File.ReadAllLines(@"..\..\..\russianWords.txt");
    }
}
