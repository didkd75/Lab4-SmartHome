using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHomeApp
{
    internal class Program
    {
        private static List<SmartDevice> devices = new List<SmartDevice>();
        private static int maxCapacity = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Write("Введіть максимальну кількість пристроїв N (N > 0): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out maxCapacity) && maxCapacity > 0)
                {
                    break;
                }
                Console.WriteLine("Error: має бути ціле додатне число!");
            }

            bool exitProgram = false;
            while (!exitProgram)
            {
                Console.WriteLine("\n=== МЕНЮ КЕРУВАННЯ РОЗУМНИМ ДОМОМ (Lab-4) ===");
                Console.WriteLine("1 – Додати об'єкт");
                Console.WriteLine("2 – Переглянути всі об'єкти");
                Console.WriteLine("3 – Знайти об'єкт");
                Console.WriteLine("4 – Продемонструвати поведінку");
                Console.WriteLine("5 – Видалити об'єкт");
                Console.WriteLine("6 – Продемонструвати static-методи");
                Console.WriteLine("0 – Вийти з програми");
                Console.Write("Оберіть пункт меню: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": AddDeviceMenu(); break;
                    case "2": ShowAllDevices(); break;
                    case "3": SearchDeviceMenu(); break;
                    case "4": DemonstrateBehaviorMenu(); break;
                    case "5": DeleteDeviceMenu(); break;
                    case "6": DemonstrateStaticMethodsMenu(); break;
                    case "0":
                        exitProgram = true;
                        Console.WriteLine("Програму успішно завершено. До побачення!");
                        break;
                    default:
                        Console.WriteLine("Некоректний вибір! Спробуйте ще раз.");
                        break;
                }
            }
        }

        // Допоміжні методи безпечного зчитування (без створення зайвих об'єктів, щоб не збивати DeviceCounter)
        private static string ReadValidSerial()
        {
            while (true)
            {
                Console.Write("Введіть серійний номер (6 цифр): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.All(char.IsDigit) && input.Length == 6)
                    return input;
                Console.WriteLine("Error: серійний номер має складатися з 6 цифр!");
            }
        }

        private static string ReadValidName()
        {
            while (true)
            {
                Console.Write("Введіть назву пристрою (3-20 символів): ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input) && input.Length >= 3 && input.Length <= 20 &&
                    input.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                    return input;
                Console.WriteLine("Error: назва має бути від 3 до 20 символів (букви, цифри, пробіли)!");
            }
        }

        private static DeviceType ReadValidType()
        {
            while (true)
            {
                Console.WriteLine("Оберіть тип: 1 - Thermostat, 2 - SmartLight, 3 - SecurityCamera, 4 - SmartLock");
                Console.Write("Ваш вибір: ");
                if (int.TryParse(Console.ReadLine(), out int val) && Enum.IsDefined(typeof(DeviceType), val))
                    return (DeviceType)val;
                Console.WriteLine("Error: некоректний тип пристрою!");
            }
        }

        private static double ReadValidPower()
        {
            while (true)
            {
                Console.Write("Введіть потужність Вт (0..500): ");
                if (double.TryParse(Console.ReadLine(), out double val) && val >= 0 && val <= 500)
                    return val;
                Console.WriteLine("Error: значення потужності має бути у діапазоні [0..500] Вт!");
            }
        }

        // Пункт 1: Додавання об'єкта (додано пункт 6 — створення через рядок Parse/TryParse)
        private static void AddDeviceMenu()
        {
            if (devices.Count >= maxCapacity)
            {
                Console.WriteLine($"Error: досягнуто ліміт пристроїв N = {maxCapacity}. Додавання неможливе!");
                return;
            }

            Console.WriteLine("Оберіть спосіб створення об'єкта:");
            Console.WriteLine("1 – Конструктор без параметрів: SmartDevice()");
            Console.WriteLine("2 – Конструктор з 2 параметрами: SmartDevice(serialNumber, deviceName)");
            Console.WriteLine("3 – Конструктор з 4 параметрами: SmartDevice(serialNumber, deviceName, type, powerUsage)");
            Console.WriteLine("4 – Конструктор з 5 параметрами: SmartDevice(serialNumber, deviceName, type, powerUsage, isActive)");
            Console.WriteLine("5 – Автоматична генерація випадкового об'єкта");
            Console.WriteLine("6 – Введення об'єкта через рядок (TryParse)");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                SmartDevice newDevice = null;

                switch (choice)
                {
                    case "1":
                        newDevice = new SmartDevice();
                        break;

                    case "2":
                        string sn2 = ReadValidSerial();
                        string name2 = ReadValidName();
                        Console.WriteLine();
                        newDevice = new SmartDevice(sn2, name2);
                        break;

                    case "3":
                        string sn3 = ReadValidSerial();
                        string name3 = ReadValidName();
                        DeviceType type3 = ReadValidType();
                        double power3 = ReadValidPower();
                        Console.WriteLine();
                        newDevice = new SmartDevice(sn3, name3, type3, power3);
                        break;

                    case "4":
                        string sn4 = ReadValidSerial();
                        string name4 = ReadValidName();
                        DeviceType type4 = ReadValidType();
                        double power4 = ReadValidPower();
                        Console.Write("Пристрій увімкнений? (1 - Так, 0 - Ні): ");
                        bool active4 = Console.ReadLine() == "1";
                        Console.WriteLine();
                        newDevice = new SmartDevice(sn4, name4, type4, power4, active4);
                        break;

                    case "5":
                        Random rnd = new Random();
                        string autoSn = rnd.Next(100000, 999999).ToString();
                        string autoName = "SmartNode" + rnd.Next(10, 99);
                        DeviceType autoType = (DeviceType)rnd.Next(1, 5);
                        double autoPower = Math.Round(rnd.NextDouble() * 150, 1);
                        bool autoActive = rnd.Next(0, 2) == 1;
                        newDevice = new SmartDevice(autoSn, autoName, autoType, autoPower, autoActive);
                        break;

                    case "6":
                        Console.WriteLine("Формат рядка: SerialNumber,DeviceName,Type,PowerUsage,IsActive");
                        Console.WriteLine("Приклад: 123456,HallLamp,SmartLight,15.5,true");
                        Console.Write("Введіть рядок: ");
                        string rawString = Console.ReadLine();

                        if (SmartDevice.TryParse(rawString, out newDevice))
                        {
                            Console.WriteLine("Рядок успішно розпізнано через TryParse!");
                        }
                        else
                        {
                            Console.WriteLine("Не вдалося створити об'єкт з введеного рядка.");
                            return;
                        }
                        break;

                    default:
                        Console.WriteLine("Некоректний вибір!");
                        return;
                }

                devices.Add(newDevice);
                Console.WriteLine("Об'єкт успішно створено та додано до колекції!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка створення об'єкта: {ex.Message}");
            }
        }

        // Пункт 2: Перегляд усіх об'єктів + виведення статичного лічильника
        private static void ShowAllDevices()
        {
            Console.WriteLine($"[Статичний лічильник] Всього створених об'єктів SmartDevice.DeviceCounter: {SmartDevice.DeviceCounter}");
            PrintTable(devices);
        }

        private static void PrintTable(List<SmartDevice> list)
        {
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта не знайдено.");
                return;
            }

            Console.WriteLine(new string('-', 88));
            Console.WriteLine($"| {"№",-3} | {"Серійний номер",-14} | {"Назва пристрою",-16} | {"Тип",-14} | {"Потужність",-10} | {"Стан",-12} |");
            Console.WriteLine(new string('-', 88));

            for (int i = 0; i < list.Count; i++)
            {
                int index = devices.IndexOf(list[i]) + 1;
                string state = list[i].IsActive ? "Увімкнено" : "Вимкнено";
                Console.WriteLine($"| {index,-3} | {list[i].SerialNumber,-14} | {list[i].DeviceName,-16} | {list[i].Type,-14} | {list[i].PowerUsage + "W",-10} | {state,-12} |");
            }
            Console.WriteLine(new string('-', 88));
        }

        private static void SearchDeviceMenu()
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта немає в системі.");
                return;
            }

            Console.WriteLine("Оберіть характеристику для пошуку:");
            Console.WriteLine("1 – За серійним номером");
            Console.WriteLine("2 – За типом пристрою");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            List<SmartDevice> results = new List<SmartDevice>();

            if (choice == "1")
            {
                Console.Write("Введіть серійний номер для пошуку: ");
                string sn = Console.ReadLine();
                results = devices.Where(d => d.SerialNumber.Equals(sn, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else if (choice == "2")
            {
                Console.Write("Введіть назву типу (Thermostat, SmartLight, SecurityCamera, SmartLock): ");
                string typeStr = Console.ReadLine();
                results = devices.Where(d => d.Type.ToString().Equals(typeStr, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            PrintTable(results);
        }

        private static void DemonstrateBehaviorMenu()
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта немає для демонстрації.");
                return;
            }

            ShowAllDevices();
            Console.Write("Введіть № пристрою у таблиці: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= devices.Count)
            {
                SmartDevice dev = devices[index - 1];
                Console.WriteLine($"\n--- Демонстрація поведінки об'єкта '{dev.DeviceName}' ---");

                dev.TogglePower();

                while (true)
                {
                    Console.Write("\n[Перевантаження 1] Введіть нову точну потужність (0..500 Вт): ");
                    try
                    {
                        if (double.TryParse(Console.ReadLine(), out double p))
                        {
                            dev.CalibratePower(p);
                            break;
                        }
                        throw new FormatException("Error: некоректне число!");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                while (true)
                {
                    Console.Write("\n[Перевантаження 2] Введіть зміну потужності у відсотках (напр. 10 або -20): ");
                    try
                    {
                        if (double.TryParse(Console.ReadLine(), out double percent))
                        {
                            dev.CalibratePower(percent, true);
                            break;
                        }
                        throw new FormatException("Error: некоректне число!");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                Console.WriteLine("\n[Перевантаження 3] Скидання потужності до заводського значення:");
                dev.CalibratePower();

                Console.WriteLine("\n" + dev.GetDiagnosticsReport());
            }
            else
            {
                Console.WriteLine("Error: некоректний порядковий номер!");
            }
        }

        // Пункт 5: Видалення об'єкта з декрементом статичного лічильника
        private static void DeleteDeviceMenu()
        {
            if (devices.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта немає в системі.");
                return;
            }

            Console.WriteLine("1 – За порядковим номером");
            Console.WriteLine("2 – За характеристикою (серійний номер)");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введіть порядковий номер: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= devices.Count)
                {
                    devices.RemoveAt(index - 1);
                    SmartDevice.DecrementCounter(1);
                    Console.WriteLine("Об'єкт успішно видалено! Статичний лічильник зменшено.");
                }
                else
                {
                    Console.WriteLine("Error: некоректне значення порядкового номера!");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Введіть серійний номер: ");
                string sn = Console.ReadLine();
                int count = devices.RemoveAll(d => d.SerialNumber.Equals(sn, StringComparison.OrdinalIgnoreCase));

                if (count > 0)
                {
                    SmartDevice.DecrementCounter(count);
                    Console.WriteLine($"Успішно видалено об'єктів: {count}. Статичний лічильник оновлено.");
                }
                else
                {
                    Console.WriteLine("Відсутність об'єктів для видалення із заданою характеристикою.");
                }
            }
        }

        // [НОВЕ в Lab-4]: ПУНКТ МЕНЮ 6 — ДЕМОНСТРАЦІЯ STATIC-МЕТОДІВ ТА ToString()
        private static void DemonstrateStaticMethodsMenu()
        {
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ STATIC-ЧЛЕНІВ ТА МЕТОДУ ToString() ===");

            // 1. Статичні властивості
            Console.WriteLine($"\n1. Поточне значення SmartDevice.DeviceCounter: {SmartDevice.DeviceCounter}");
            Console.WriteLine($"2. Поточний тариф SmartDevice.ElectricityTariff: {SmartDevice.ElectricityTariff} грн/кВт·год");

            // 2. Статичний метод предметної області CalculateTotalPowerAndCost
            Console.WriteLine("\n3. Виклик статичного методу SmartDevice.CalculateTotalPowerAndCost(devices):");
            Console.WriteLine("   -> " + SmartDevice.CalculateTotalPowerAndCost(devices));

            // 3. Зміна статичного тарифу та перерахунок
            Console.Write("\nБажаєте змінити тариф електроенергії для перевірки static-властивості? (1 - Так, 0 - Ні): ");
            if (Console.ReadLine() == "1")
            {
                Console.Write("Введіть новий тариф (грн/кВт·год): ");
                try
                {
                    if (double.TryParse(Console.ReadLine(), out double newTariff))
                    {
                        SmartDevice.ElectricityTariff = newTariff;
                        Console.WriteLine("   Оновлений розрахунок -> " + SmartDevice.CalculateTotalPowerAndCost(devices));
                    }
                    else
                    {
                        Console.WriteLine("   Error: некоректне число!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   Виключення: {ex.Message}");
                }
            }

            // 4. Демонстрація ToString() на існуючому або тестовому об'єкті
            Console.WriteLine("\n4. Демонстрація перевизначеного методу ToString():");
            if (devices.Count > 0)
            {
                Console.WriteLine($"   Перший об'єкт у колекції ToString() -> \"{devices[0].ToString()}\"");
            }
            else
            {
                Console.WriteLine("   (Колекція порожня — додайте об'єкт для перегляду його ToString())");
            }

            // 5. Демонстрація Parse та TryParse
            Console.WriteLine("\n5. Демонстрація роботи SmartDevice.Parse() та SmartDevice.TryParse():");
            string validTest = "777888,KitchenCam,SecurityCamera,24.5,true";
            string invalidTest = "12,BadName,UnknownType,-50,notabool";

            Console.WriteLine($"   Тест некоректного рядка: \"{invalidTest}\"");
            bool failResult = SmartDevice.TryParse(invalidTest, out _);
            Console.WriteLine($"   Результат TryParse: {failResult}");

            Console.WriteLine($"\n   Тест коректного рядка: \"{validTest}\"");
            if (SmartDevice.TryParse(validTest, out SmartDevice parsedDev))
            {
                Console.WriteLine($"   Результат TryParse: True -> Об'єкт створено: {parsedDev.StatusSummary}");
                // Видаляємо тимчасовий тестовий об'єкт із лічильника, щоб не спотворювати реальну кількість у списку
                SmartDevice.DecrementCounter(1);
            }
        }
    }
}