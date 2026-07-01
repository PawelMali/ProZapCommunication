namespace ProZapCommunication.Web.Models;

// Model Agregujący (Dla interfejsu UI i Eksportu)
// To jest kluczowy model dla "przeglądarki całego procesu". 
// Pozwoli nam wyświetlić dane ze wszystkich stacji dla jednego numeru SN w jednym wierszu (płaska struktura).
public class ProductionProcessReport
{
    // Główne identyfikatory relacyjne (Master)
    public string SN { get; set; } = string.Empty;           // Podany przez użytkownika
    public string? Mfs_SN { get; set; }                      // Znaleziony na Stacji 2
    public string? Motor_SN { get; set; }                    // Znaleziony na Stacji 7

    public int FinalStatus { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalAttempts { get; set; }

    // --- HISTORIA LINII GŁÓWNEJ (Detail) ---
    public List<Station1Result> Station1History { get; set; } = new();
    public List<Station2Result> Station2History { get; set; } = new();
    public List<Station3Result> Station3History { get; set; } = new();
    public List<Station4Result> Station4History { get; set; } = new();

    // --- HISTORIA LINII SUB-ZESPOŁU (Detail) ---
    public List<Station5Result> Station5History { get; set; } = new();
    public List<Station6Result> Station6History { get; set; } = new();
    public List<Station7Result> Station7History { get; set; } = new();
}
