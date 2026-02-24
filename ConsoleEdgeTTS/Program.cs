using Edge_tts_sharp;
using Edge_tts_sharp.Model;

class Program
{
    static void Main(string[] args)
    {

        if (args.Length < 1)
        {
            Console.WriteLine("""
用法: tts <文本内容> <语言名称>
     tts <文本内容>
示例: tts "你好，世界" zh-CN 
     tts "Hola, mundo" es-ES
     tts "Hola, mundo"
未检测到参数，自动使用 demo 参数 "你好，世界" zh-CN ...
""");
            args = new[] { "你好，世界", "zh-CN" };
        }
        else if (args.Length < 2)
        {
            args = new[] { args[0], "es-ES" };
        }

        string text = args[0];
        string languageName = args[1];
        int rate = 0;
        string safeText = string.Concat($"{languageName}_{text}".Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "Voice", $"{safeText}.mp3");
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Voice")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Voice"));
        }

        Edge_tts.Await = true;
        var voice = Edge_tts.GetVoice().FirstOrDefault(i => i.Name.Contains(languageName) || i.ShortName.Contains(languageName)|| i.FriendlyName.Contains(languageName));
        if (voice == null)
        {
            Console.WriteLine($"未找到语言: {languageName}");
            return;
        }

        PlayOption option = new PlayOption
        {
            Rate = rate,
            Text = text,
        };

        ManualResetEvent waitHandle = new ManualResetEvent(false);

        Edge_tts.Invoke(option, voice, (_binary) =>
        {
            File.WriteAllBytes(outputFile, _binary.ToArray());
            Console.WriteLine($"已保存: {outputFile}");
            waitHandle.Set();
        });

        waitHandle.WaitOne();
    }
}
