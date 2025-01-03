using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MappaMasolo
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<string>> osztalyok = new Dictionary<string, List<string>>();
        private Dictionary<string, string> osztalyUtvonalak = new Dictionary<string, string>();
        private CancellationTokenSource cancelTokenSource;

        private readonly string alapCelKonyvtar = "Z:\\elérési út\\célhely";

        public MainWindow()
        {
            InitializeComponent();
            BetoltOsztalyok();
        }

        private void BetoltOsztalyok()
        {
            // Beágyazott erőforrásként tárolt névsor beolvasása
            var lines = ReadEmbeddedResource("MappaMasolo.NEVSOR.txt").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            FeldolgozOsztalyok(lines);
        }

        private string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Resource not found: " + resourceName);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void FeldolgozOsztalyok(string[] lines)
        {
            string currentClass = null;

            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("Osztály:"))
                {
                    currentClass = line.Substring(8).Trim();
                    osztalyok[currentClass] = new List<string>();
                }
                else if (line.StartsWith("Útvonal:"))
                {
                    if (currentClass == null) throw new InvalidOperationException("Az útvonal megadása előtt nem szerepel osztály.");
                    osztalyUtvonalak[currentClass] = line.Substring(8).Trim();
                }
                else
                {
                    if (currentClass == null) throw new InvalidOperationException("A diák megadása előtt nem szerepel osztály.");
                    osztalyok[currentClass].Add(line);
                }
            }

            osztalyComboBox.ItemsSource = osztalyok.Keys;
        }

        private string NormalizaltMappaNev(string nev)
        {
            var dict = new Dictionary<char, char>
            {
                { 'á', 'a' }, { 'é', 'e' }, { 'í', 'i' }, { 'ó', 'o' }, { 'ö', 'o' }, { 'ő', 'o' },
                { 'ú', 'u' }, { 'ü', 'u' }, { 'ű', 'u' }, { 'Á', 'A' }, { 'É', 'E' }, { 'Í', 'I' },
                { 'Ó', 'O' }, { 'Ö', 'O' }, { 'Ő', 'O' }, { 'Ú', 'U' }, { 'Ü', 'U' }, { 'Ű', 'U' }
            };

            var normalized = new StringBuilder();

            foreach (var c in nev)
            {
                if (dict.ContainsKey(c))
                {
                    normalized.Append(dict[c]);
                }
                else if (char.IsWhiteSpace(c))
                {
                    normalized.Append('_');
                }
                else
                {
                    normalized.Append(c);
                }
            }

            return normalized.ToString();
        }

        private void OsztalyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (osztalyComboBox.SelectedItem is string selectedClass)
            {
                diakListaPanel.Children.Clear();

                foreach (var diak in osztalyok[selectedClass])
                {
                    var checkBox = new CheckBox { Content = diak, IsChecked = true };
                    diakListaPanel.Children.Add(checkBox);
                }
            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var child in diakListaPanel.Children)
            {
                if (child is CheckBox checkBox)
                {
                    checkBox.IsChecked = true;
                }
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var child in diakListaPanel.Children)
            {
                if (child is CheckBox checkBox)
                {
                    checkBox.IsChecked = false;
                }
            }
        }

        private async void InditasButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(osztalyComboBox.SelectedItem is string selectedClass))
            {
                MessageBox.Show("Válassz ki egy osztályt!");
                return;
            }

            var diakok = diakListaPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            if (!diakok.Any())
            {
                MessageBox.Show("Nincs kijelölt diák a másoláshoz.");
                return;
            }

            string forrasUtvonal = osztalyUtvonalak[selectedClass];
            // string celUtvonal = Path.Combine(alapCelKonyvtar, DateTime.Now.ToString("yyyy-MM-dd"), selectedClass);
            string celUtvonal = Path.Combine(alapCelKonyvtar, selectedClass);

            Directory.CreateDirectory(celUtvonal);

            cancelTokenSource = new CancellationTokenSource();
            var token = cancelTokenSource.Token;

            try
            {
                haladasBar.Value = 0;
                aktualisFajlText.Text = "";

                var mappaNevLista = diakok.Select(NormalizaltMappaNev).ToList();
                var fajlok = new List<string>();

                foreach (var mappaNev in mappaNevLista)
                {
                    var mappak = Directory.GetDirectories(forrasUtvonal, $"{mappaNev}*");
                    foreach (var mappa in mappak)
                    {
                        fajlok.AddRange(Directory.EnumerateFiles(mappa, "*", SearchOption.AllDirectories));
                    }
                }

                int totalFiles = fajlok.Count;

                for (int i = 0; i < totalFiles; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        aktualisFajlText.Text = "Másolás megszakítva.";
                        break;
                    }

                    string sourceFile = fajlok[i];
                    string destFile = sourceFile.Replace(forrasUtvonal, celUtvonal);

                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, true);

                    aktualisFajlText.Text = sourceFile;
                    haladasBar.Value = ((double)(i + 1) / totalFiles) * 100;

                    await Task.Delay(50); // Animáció kedvéért
                }

                if (!token.IsCancellationRequested)
                {
                    aktualisFajlText.Text = "Másolás kész.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt: {ex.Message}");
            }
        }

        private void MegszakitasButton_Click(object sender, RoutedEventArgs e)
        {
            cancelTokenSource?.Cancel();
        }
    }
}
