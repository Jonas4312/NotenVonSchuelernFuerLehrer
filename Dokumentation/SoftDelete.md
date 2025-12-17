# Soft Delete

Die Anwendung verwendet Soft Deletes für alle Entitäten. Das bedeutet, dass Datensätze beim Löschen nicht physisch aus der Datenbank entfernt werden, sondern nur als gelöscht markiert werden.

## Betroffene Entitäten

| Entität | Tabelle | IsDeleted-Spalte |
|---------|---------|------------------|
| Fach | `Fach` | ✅ |
| Klasse | `Klasse` | ✅ |
| Schüler | `Schueler` | ✅ |
| Lehrer | `Lehrer` | ✅ |
| Note | `Note` | ✅ |

## Implementierung

### 1. Entity-Property

Jede Entität hat eine `IsDeleted`-Property:

```csharp
public bool IsDeleted { get; set; }
```

### 2. Query-Filter (automatisches Ausfiltern)

In den Entity-Konfigurationen wird ein globaler Query-Filter definiert, der gelöschte Datensätze automatisch aus allen Abfragen ausfiltert:

```csharp
// Beispiel: FachConfiguration.cs
builder.HasQueryFilter(f => !f.IsDeleted);
```

Dadurch wird bei jeder Datenbankabfrage automatisch `WHERE NOT IsDeleted` hinzugefügt.

### 3. Delete-Handler

Die Lösch-Handler setzen nur das `IsDeleted`-Flag:

```csharp
// Statt: _context.Fach.Remove(fach);
fach.IsDeleted = true;
await _context.SaveChangesAsync();
```

## Vorteile

- **Datenwiederherstellung**: Gelöschte Daten können bei Bedarf wiederhergestellt werden
- **Audit-Trail**: Historische Daten bleiben erhalten
- **Referenzielle Integrität**: Keine Probleme mit Foreign-Key-Verletzungen
- **Automatisches Filtern**: Durch Query-Filter erscheinen gelöschte Daten nirgends im Frontend

## Datenbankabfragen

### Alle gelöschten Schüler anzeigen

```sql
SELECT * FROM Schueler WHERE IsDeleted = 1;
```

### Gelöschten Schüler wiederherstellen

```sql
UPDATE Schueler SET IsDeleted = 0 WHERE Id = 'guid-hier';
```

### Alle gelöschten Datensätze anzeigen

```sql
-- Fächer
SELECT Id, Bezeichnung, IsDeleted FROM Fach WHERE IsDeleted = 1;

-- Klassen
SELECT Id, Bezeichnung, IsDeleted FROM Klasse WHERE IsDeleted = 1;

-- Schüler
SELECT Id, Vorname, Nachname, IsDeleted FROM Schueler WHERE IsDeleted = 1;

-- Lehrer
SELECT Id, Vorname, Nachname, Benutzername, IsDeleted FROM Lehrer WHERE IsDeleted = 1;

-- Noten
SELECT Id, Wert, SchuelerId, FachId, IsDeleted FROM Note WHERE IsDeleted = 1;
```

## Hinweise

### Query-Filter umgehen (für Admin-Zwecke)

Falls man auch gelöschte Datensätze abfragen möchte, kann man den Query-Filter mit `IgnoreQueryFilters()` umgehen:

```csharp
var alleSchuelerInklusiveGeloeschte = await _context.Schueler
    .IgnoreQueryFilters()
    .ToListAsync();
```

### Migration

Die Soft Delete-Spalten wurden mit der Migration `SoftDelete` hinzugefügt. Die Migration fügt allen Tabellen eine `IsDeleted`-Spalte (TINYINT(1), Default: 0) hinzu.

```bash
# Migration erstellen (bereits erledigt)
dotnet ef migrations add SoftDelete --project ..\NotenVonSchuelernFuerLehrer.Domain.Model

# Migration anwenden (passiert automatisch beim Container-Start)
dotnet ef database update
```
