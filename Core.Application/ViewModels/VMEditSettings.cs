using Core.Domain.Entities;
using System.Text.Json;

namespace Core.Application.ViewModels
{
    public class VMEditSettings
    {
        private readonly string _baseDirectory = null!;
        public VMEditSettings(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            TerminalSettings = LoadTerminalSettings();
            BasculaSettings = LoadBasculaSettings();
        }

        public TerminalSettings TerminalSettings { get; set; } = null!;

        public BasculaSettings BasculaSettings { get; set; } = null!;

        public VMEditSettings()
        {

        }

        private TerminalSettings LoadTerminalSettings()
        {
            var path = Path.Combine(_baseDirectory, "Data/TerminalSettings.json");
            if (!File.Exists(path))
            {
                throw new Exception($"TerminalSettings.json not found on path: {path}");
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("TerminalSettings.json is empty");
            }
            else
            {
                return JsonSerializer.Deserialize<TerminalSettings>(json) ?? throw new Exception("Json TerminalSettings invalido");
            }
        }

        private BasculaSettings LoadBasculaSettings()
        {
            var path = Path.Combine(_baseDirectory, "Data/BasculaSettings.json");
            if (!File.Exists(path))
            {
                throw new Exception($"BasculaSettings.json not found on path: {path}");
            }

            string json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("BasculaSettings.json is empty");
            }
            else
            {
                return JsonSerializer.Deserialize<BasculaSettings>(json) ?? throw new Exception("Json BasculaSettings invalido");
            }
        }

        public void SaveSettings()
        {   
            var json = JsonSerializer.Serialize(TerminalSettings);
            File.WriteAllText(Path.Combine(_baseDirectory, "Data/TerminalSettings.json"), json);
        }
    }
}
