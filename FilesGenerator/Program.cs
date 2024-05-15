namespace FilesGenerator
{
    internal class Program
    {
        private static CountdownEvent doneCountDownEvent;
        private static CancellationTokenSource cts;

        static void Main(string[] args)
        {
            using var consoleInput = new StreamReader(Console.OpenStandardInput());

            var threadInfo = new ThreadInfo();
            int inputAttempts = 0;
            int fileCount = 0;

            do
            {
                if (inputAttempts > 0)
                    Console.WriteLine("Некорректный ввод, попробуйте ещё раз -> ");
                else
                    Console.WriteLine("Введите путь до директории, в которой будут созданы файлы:");
                threadInfo.OutPath = consoleInput.ReadLine();
                inputAttempts++;
            } while (!Directory.Exists(threadInfo.OutPath));

            inputAttempts = 0;

            Console.WriteLine("Введите кол-во файлов, которые нужно создать:");

            while (!int.TryParse(consoleInput.ReadLine(), out fileCount))
            {
                Console.WriteLine("Некорректный ввод, попробуйте ещё раз -> ");
            }

            Console.WriteLine("Введите максимальную длину строки:");
            int maxLineLen = 0;

            while (!int.TryParse(consoleInput.ReadLine(), out maxLineLen))
            {
                Console.WriteLine("Некорректный ввод, попробуйте ещё раз -> ");
            }

            threadInfo.MaxLineLength = maxLineLen;

            Console.WriteLine("Введите максимальное кол-во строк:");
            int maxLineCount = 0;

            while (!int.TryParse(consoleInput.ReadLine(), out maxLineCount))
            {
                Console.WriteLine("Некорректный ввод, попробуйте ещё раз -> ");
            }

            threadInfo.MaxLineCount = maxLineCount;

            doneCountDownEvent = new CountdownEvent(fileCount);
            cts = new CancellationTokenSource();
            Console.CancelKeyPress += CloseHandler;

            Console.WriteLine("Выполняется задача создания файлов...");

            Task task = Task.Run(() =>
            {
                for (int i = 1; i <= fileCount; i++)
                {
                    threadInfo.FileName = i.ToString();
                    threadInfo.CancelToken = cts.Token;

                    ThreadPool.QueueUserWorkItem(CreateFileJob, threadInfo.ShallowCopy());
                }
            }, cts.Token);

            while (true)
            {
                if (doneCountDownEvent.CurrentCount == 0)
                {
                    Console.WriteLine("\nЗадача завершена.");
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
            Console.Write("\r Создано {0}%   ", totalProgress);
        }

        static void CreateFileJob(object state)
        {
            try
            {
                ThreadInfo threadInfo = state as ThreadInfo;
                if (!threadInfo.CancelToken.IsCancellationRequested)
                {
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(threadInfo.OutPath, $"{threadInfo.FileName}.txt")))
                    {
                        var linesCount = Random.Shared.Next(1, threadInfo.MaxLineCount);

                        for (int i = 0; i < linesCount; i++)
                        {
                            var lineLen = Random.Shared.Next(1, threadInfo.MaxLineLength);
                            outputFile.WriteLine(ContentExtensions.RandomString(lineLen));
                        }
                    }
                    Thread.Sleep(0);
                    OnJobEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании файла с контентом!");
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

            Console.WriteLine("\nАктивные задачи завершены. Программа завершит выполнение через 2 секунды... ");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}
