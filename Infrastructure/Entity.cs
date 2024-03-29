﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth_Jwt.Infrastructure;

public interface IEntity
{
    int Id { get; }
}

public interface IArchivableEntity
{
    bool Archived { get; }
    void Archive();
    void UnArchive();
}

public class Entity : IEntity
{
    [JsonIgnore]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ArchivableEntity : Entity, IArchivableEntity
{
    public bool Archived { get; set; }

    public void Archive()
    {
        Archived = true;
    }

    public void UnArchive()
    {
        Archived = false;
    }
}
