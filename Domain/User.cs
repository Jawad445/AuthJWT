﻿using Auth_Jwt.Infrastructure;
using System.Text.Json.Serialization;

namespace Auth_Jwt.Domain;
public class User : ArchivableEntity
{
    public User(
        string firstName,
        string lastName,
        string email,
        byte[] hash,
        byte[] salt)
    {
        FirstName = firstName;
        LastName = lastName;
        FullName = $"{FirstName} {LastName}";
        Email = email;
        Hash = hash;
        Salt = salt;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string SystemName { get; set; } = "VueExpenses";
    public string CurrencyRegionName { get; set; } = "GB";
    public bool UseDarkMode { get; set; }

    [JsonIgnore]
    public byte[] Hash { get; set; }

    [JsonIgnore]
    public byte[] Salt { get; set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public void AddRefreshToken(
        string token,
        double daysToExpire = 2)
    {
        _refreshTokens.Add(
            new RefreshToken(
                token,
                DateTime.UtcNow.AddDays(daysToExpire),
                this));
    }

    public void RemoveRefreshToken(
        string refreshToken)
    {
        _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
    }

    public bool IsValidRefreshToken(
        string refreshToken)
    {
        return _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
    }

    public void UpdateName(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        FullName = $"{FirstName} {LastName}";
    }
}
