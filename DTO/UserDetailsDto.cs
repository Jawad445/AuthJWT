using System.Globalization;

namespace Auth_Jwt.DTO;

public class UserDetailsDto
{
    protected UserDetailsDto()
    {
    }

    public UserDetailsDto(
        string systemName,
        string currencyRegionName,
        bool useDarkMode)
    {
        SystemName = systemName;
        CurrencyRegionName = currencyRegionName;
        UseDarkMode = useDarkMode;
        Theme = useDarkMode ? "dark" : "light";
        DisplayCurrency = new RegionInfo(currencyRegionName).CurrencySymbol;
    }

    public string SystemName { get; set; }
    public string CurrencyRegionName { get; set; }
    public bool UseDarkMode { get; set; }
    public string Theme { get; set; }
    public string DisplayCurrency { get; set; }
}
