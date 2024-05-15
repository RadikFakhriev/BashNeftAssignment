# Тестовое задание в БашНИПИнефть
### Условие
Необходимо написать на C# консольную программу-сервис, которая параллельно в несколько потоков  обрабатывает текстовые файлы в определенной папке.

- В папке создается файл с расширением *.TXT (копируется или перемещается в эту папку), программа должна посчитать количество **букв** в данном файле и в другой папке создать файл с таким же именем и записать в него полученное значение.
- Файлов может быть много, поэтому работу нужно распараллелить;
- При первом запуске необходимо обработать все находящиеся в папке файлы;
- Входная папка и папка для результатов должны задаваться параметрами командной строки при запуске программы;
- При штатном завершении приложения (например, через Ctrl-C, а не через «убийство» процесса в TaskManager или закрытии окна) нужно дождаться завершения активных в данный момент задач обработки файлов.


Для удобства тестирования решения была написана ещё одна консольная программа для создания файлов с тестовыми данными. В решении (которое находится в папке LetterCounter) есть второй проект - *FilesGenerator*, который следует запустить перед 1м запуском LetterCounter (в случае отсутствия тестовых данных)

### Подробности реализации
- .NET 8
- ThreadPool, управляемые потоки
