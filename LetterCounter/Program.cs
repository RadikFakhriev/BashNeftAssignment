using System.Text.RegularExpressions;

namespace LetterCounter
{
    internal class Program
    {
        private static CountdownEvent doneCountDownEvent;
        private static CancellationTokenSource cts;
        private static readonly Regex SearchFilesRegex = new Regex(@"(\d+)?(\w)?(\.txt)");

        static void Main(string[] args)
        {
            using var consoleInput = new StreamReader(Console.OpenStandardInput());

            var threadInfo = new ThreadInfo();
            Console.WriteLine("Входная директория: ");
            threadInfo.InputPath = consoleInput.ReadLine();
            Console.WriteLine("Директория для результатов: ");
            threadInfo.OutputPath = consoleInput.ReadLine();
            cts = new CancellationTokenSource();
            Console.CancelKeyPress += CloseHandler;

            DirectoryInfo sourceDirectory = new DirectoryInfo(threadInfo.InputPath);
            var fileCount = sourceDirectory.GetFiles()
                .Count(fl => SearchFilesRegex.IsMatch(fl.Name));
            doneCountDownEvent = new CountdownEvent(fileCount);

            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                if (SearchFilesRegex.IsMatch(file.Name))
                {
                    threadInfo.FilePath = file.FullName;
                    threadInfo.FileName = file.Name;
                    threadInfo.CancelToken = cts.Token;

                    ThreadPool.QueueUserWorkItem(CalculateResultJob, threadInfo.ShallowCopy());
                }
            }

            while (true)
            {
                if (doneCountDownEvent.CurrentCount == 0)
                {
                    Console.WriteLine("\nЗадача завершена.");
                    Console.WriteLine($"В директории для результатов создано " +
                        $"{doneCountDownEvent.InitialCount - doneCountDownEvent.CurrentCount} файлов");
                    Environment.Exit(0);
                }
                Thread.Sleep(200);
            }
        }

        static void OnJobEnd()
        {
            doneCountDownEvent.Signal();
            int totalProgress = (int)
                   ((double)(doneCountDownEvent.InitialCount - doneCountDownEvent.CurrentCount) / doneCountDownEvent.InitialCount * 100);
            Console.Write("\r Обработано {0}%   ", totalProgress);
        }

        static void CalculateResultJob(object state)
        {
            try
            {
                ThreadInfo threadInfo = state as ThreadInfo;
                if (!threadInfo.CancelToken.IsCancellationRequested)
                {
                    string[] fileLines = File.ReadAllLines(threadInfo.FilePath);
                    var result = fileLines.Sum(line => line.ToCharArray().Count(ch => char.IsLetter(ch)));

                    using (StreamWriter outputFile = 
                        new StreamWriter(Path.Combine(threadInfo.OutputPath, $"{threadInfo.FileName}")))
                    {
                        outputFile.WriteLine(result);
                    }
                    Thread.Sleep(0);
                    OnJobEnd();
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании файла с результатом подсчёта букв!");
                Console.WriteLine(ex.Message);
            }
        }

        static void CloseHandler(object sender, ConsoleCancelEventArgs args)
        {
            var isCtrlC = args.SpecialKey == ConsoleSpecialKey.ControlC;

            if (isCtrlC)
            {
                args.Cancel = true;
            }

            cts.Cancel();
            cts.Dispose();

            Console.WriteLine("\nАктивные задачи завершены. ");
            Console.WriteLine($"В директории для результатов создано " +
                $"{doneCountDownEvent.InitialCount - doneCountDownEvent.CurrentCount} файлов");
            Console.WriteLine("Программа завершит выполнение через 2 секунды...");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
