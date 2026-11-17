using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartHomeApp
{
    public class SmartDevice
    {
        // [НОВЕ в Lab-4]: СТАТИЧНІ ПОЛЯ, ВЛАСТИВОСТІ ТА СТАТИЧНИЙ КОНСТРУКТОР
        private static int _deviceCounter = 0;
        private static double _electricityTariff;

        // Статична властивість для підрахунку створених об'єктів
        public static int DeviceCounter
        {
            get => _deviceCounter;
            private set => _deviceCounter = value < 0 ? 0 : value;
        }

        // Статична автовластивість з валідацією (тариф грн за 1 кВт·год)
        public static double ElectricityTariff
        {
            get => _electricityTariff;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Error: тариф на електроенергію має бути більшим за 0!");
                _electricityTariff = value;
            }
        }

        // Статичний конструктор (ініціалізує статичні члени класу при першому зверненні)
        static SmartDevice()
        {
            _electricityTariff = 4.32; // Стандартний тариф грн/кВт·год
        }

        // Метод для зменшення лічильника при видаленні об'єкта зі списку
        public static void DecrementCounter(int count = 1)
        {
            DeviceCounter -= count;
        }
        // =========================================================================

        // Приватні поля екземпляра
        private string _serialNumber;
        private string _deviceName;
        private DeviceType _type;
        private double _powerUsage;
        private bool _isActive;

        public DateTime InstallationDate { get; set; } = DateTime.Now;
        public int ErrorCount { get; private set; } = 0;

        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value), "Error: відсутнє значення!");
                if (!value.All(char.IsDigit))
                    throw new FormatException("Error: серійний номер має містити тільки цифри!");
                if (value.Length != 6)
                    throw new ArgumentException("Error: серійний номер має складатися з 6 цифр!");

                _serialNumber = value;
            }
        }

        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value), "Error: відсутнє значення!");
                if (value.Length < 3 || value.Length > 20)
                    throw new ArgumentException("Error: довжина назви має бути від 3 до 20 символів!");
                if (!value.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                    throw new FormatException("Error: назва має містити тільки букви, цифри або пробіли!");

                _deviceName = value;
            }
        }

        public DeviceType Type
        {
            get => _type;
            set
            {
                if (!Enum.IsDefined(typeof(DeviceType), value))
                    throw new ArgumentException("Error: некоректний тип пристрою!");

                _type = value;
            }
        }

        public double PowerUsage
        {
            get => _powerUsage;
            set
            {
                if (value < 0 || value > 500)
                    throw new ArgumentOutOfRangeException(nameof(value), "Error: значення потужності має бути у діапазоні [0..500] Вт!");

                _powerUsage = value;
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public string StatusSummary => $"[{_type}] {_deviceName} - {(_isActive ? "Увімкнено" : "Вимкнено")} ({_powerUsage} W)";

        // Перевантажені конструктори з ланцюговим викликом (: this)
        public SmartDevice() : this("000000", "DefaultDevice")
        {
            Console.WriteLine("-> Використано конструктор №1: SmartDevice() без параметрів.");
        }

        public SmartDevice(string serialNumber, string deviceName) : this(serialNumber, deviceName, DeviceType.SmartLight, 10.0)
        {
            Console.WriteLine("-> Використано конструктор №2: SmartDevice(serialNumber, deviceName).");
        }

        public SmartDevice(string serialNumber, string deviceName, DeviceType type, double powerUsage)
            : this(serialNumber, deviceName, type, powerUsage, false)
        {
            Console.WriteLine("-> Використано конструктор №3: SmartDevice(serialNumber, deviceName, type, powerUsage).");
        }

        // Головний конструктор (інкрементує статичний лічильник створених об'єктів)
        public SmartDevice(string serialNumber, string deviceName, DeviceType type, double powerUsage, bool isActive)
        {
            SerialNumber = serialNumber;
            DeviceName = deviceName;
            Type = type;
            PowerUsage = powerUsage;
            IsActive = isActive;

            _deviceCounter++; // Інкремент статичного лічильника
            Console.WriteLine("-> Використано конструктор №4: SmartDevice(serialNumber, deviceName, type, powerUsage, isActive).");
        }

        // Приватний метод
        private string FormatLogMessage(string action)
        {
            return $"[{DateTime.Now:HH:mm:ss}] [Пристрій '{_deviceName}']: {action}";
        }

        // Публічні методи екземпляра
        public void TogglePower()
        {
            _isActive = !_isActive;
            string status = _isActive ? "Увімкнено" : "Вимкнено";
            Console.WriteLine(FormatLogMessage($"Стан змінено на '{status}'"));
        }

        public void CalibratePower(double newUsage)
        {
            PowerUsage = newUsage;
            Console.WriteLine(FormatLogMessage($"[Overload 1] Потужність встановлено на {_powerUsage} Вт"));
        }

        public void CalibratePower(double delta, bool isPercentage)
        {
            double calculated = isPercentage
                ? _powerUsage + (_powerUsage * delta / 100.0)
                : _powerUsage + delta;

            PowerUsage = Math.Round(calculated, 2);
            string mode = isPercentage ? $"{delta}%" : $"{delta} Вт";
            Console.WriteLine(FormatLogMessage($"[Overload 2] Потужність скориговано на {mode}. Поточна: {_powerUsage} Вт"));
        }

        public void CalibratePower()
        {
            double defaultPower = _type switch
            {
                DeviceType.SmartLight => 12.0,
                DeviceType.Thermostat => 5.0,
                DeviceType.SecurityCamera => 18.0,
                DeviceType.SmartLock => 3.5,
                _ => 10.0
            };
            PowerUsage = defaultPower;
            Console.WriteLine(FormatLogMessage($"[Overload 3] Скинуто до заводської потужності ({_powerUsage} Вт)"));
        }

        public string GetDiagnosticsReport()
        {
            string log = FormatLogMessage($"Діагностика. Встановлено: {InstallationDate:dd.MM.yyyy}. Помилок: {ErrorCount}");
            return $"{log}\nСтатус: {StatusSummary}";
        }

        // [НОВЕ в Lab-4]: ПЕРЕВИЗНАЧЕННЯ ToString() ТА СТАТИЧНІ МЕТОДИ (Parse, TryParse)

        public override string ToString()
        {
            // Формат рядка, який підтримується методами Parse / TryParse
            return $"{SerialNumber},{DeviceName},{Type},{PowerUsage.ToString(CultureInfo.InvariantCulture)},{IsActive}";
        }

        // Довільний статичний метод за предметною областю
        public static string CalculateTotalPowerAndCost(List<SmartDevice> devices)
        {
            if (devices == null || devices.Count == 0)
                return "Список пристроїв порожній. Загальна потужність: 0 Вт.";

            double activePowerWatts = devices.Where(d => d.IsActive).Sum(d => d.PowerUsage);
            double dailyKwh = (activePowerWatts / 1000.0) * 24.0;
            double dailyCost = dailyKwh * ElectricityTariff;

            return $"Увімкнено пристроїв: {devices.Count(d => d.IsActive)} з {devices.Count} | " +
                   $"Сумарна потужність: {activePowerWatts:F1} Вт | " +
                   $"Витрати за добу (тариф {ElectricityTariff} грн/кВт·год): {dailyKwh:F2} кВт·год ({dailyCost:F2} грн)";
        }

        // Статичний метод Parse
        public static SmartDevice Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s), "Error: вхідний рядок не може бути порожнім!");

            string[] parts = s.Split(',');
            if (parts.Length != 5)
                throw new FormatException("Error: рядок не відповідає формату 'SerialNumber,DeviceName,Type,PowerUsage,IsActive' (має бути 5 значень через кому)!");

            string serialNumber = parts[0].Trim();
            string deviceName = parts[1].Trim();

            if (!Enum.TryParse(parts[2].Trim(), true, out DeviceType type) || !Enum.IsDefined(typeof(DeviceType), type))
                throw new FormatException($"Error: невідомий тип пристрою '{parts[2].Trim()}'! Допустимі: Thermostat, SmartLight, SecurityCamera, SmartLock.");

            if (!double.TryParse(parts[3].Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double powerUsage))
                throw new FormatException($"Error: некоректне числове значення потужності '{parts[3].Trim()}'!");

            if (!bool.TryParse(parts[4].Trim(), out bool isActive))
                throw new FormatException($"Error: некоректне булеве значення стану '{parts[4].Trim()}' (очікується true або false)!");

            // Виклик конструктора (він також перевірить обмеження властивостей та збільшить DeviceCounter)
            return new SmartDevice(serialNumber, deviceName, type, powerUsage, isActive);
        }

        // Статичний метод TryParse
        public static bool TryParse(string s, out SmartDevice device)
        {
            device = null;
            try
            {
                device = Parse(s);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TryParse Error] {ex.Message}");
                return false;
            }
        }
    }
}