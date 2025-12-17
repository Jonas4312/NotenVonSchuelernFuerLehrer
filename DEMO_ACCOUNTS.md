# Demo-Accounts

Diese Accounts werden beim Start der Anwendung automatisch erstellt (Seeding).

## Seed-Modi

Die Anwendung unterstützt zwei Seed-Modi, die über die Umgebungsvariable `SEED_MODE` gesteuert werden:

### Full-Modus (Standard)
```bash
docker compose up -d
# oder explizit:
SEED_MODE=full docker compose up -d
```
Erstellt 5 Lehrer, 3 Klassen, 89 Schüler und Noten.

### Minimal-Modus
```bash
SEED_MODE=minimal docker compose up -d
```
Erstellt nur einen Admin-Lehrer ohne Klassen, Fächer oder Schüler.

| Benutzername | Passwort | Beschreibung |
|--------------|----------|--------------|
| `admin` | `Admin123!` | Leerer Account zum Aufbau eigener Daten |

---

## Full-Modus: Alle Demo-Accounts

**Allgemeines Passwort für alle Demo-Accounts:** `Passwort123`

---

## Übersicht

| Benutzername | Name | Fächer | Klassen | Besonderheit |
|--------------|------|--------|---------|--------------|
| `mschmidt` | Maria Schmidt | Mathematik, Deutsch, Englisch | 10A, 10B, 9A | Vollzugriff auf alle Fächer und Klassen |
| `tmueller` | Thomas Müller | Mathematik | 10A, 10B | Nur Mathe-Noten eintragen |
| `sweber` | Sabine Weber | Deutsch, Englisch | 9A | Nur eine Klasse |
| `kfischer` | Klaus Fischer | *(keine)* | *(keine)* | Kann nur Noten einsehen (Read-Only) |
| `phofmann` | Petra Hofmann | Englisch | 10A | Nur ein Fach in einer Klasse |

---

## Detaillierte Beschreibung

### Maria Schmidt (`mschmidt`)
- **Fächer:** Mathematik, Deutsch, Englisch
- **Klassen:** 10A (35 Schüler), 10B (29 Schüler), 9A (25 Schüler)
- **Berechtigungen:** Vollzugriff
  - ✅ Kann Noten in allen drei Fächern für alle Schüler eintragen
  - ✅ Kann alle Schüler in allen Klassen sehen
  - ✅ Kann Fächer, Klassen, Schüler und Lehrer verwalten

### Thomas Müller (`tmueller`)
- **Fächer:** Mathematik
- **Klassen:** 10A, 10B
- **Berechtigungen:** Eingeschränkt auf Mathematik
  - ✅ Kann Mathe-Noten für Schüler in 10A und 10B eintragen
  - ❌ Kann keine Deutsch- oder Englisch-Noten eintragen
  - ❌ Sieht die Klasse 9A nicht

### Sabine Weber (`sweber`)
- **Fächer:** Deutsch, Englisch
- **Klassen:** 9A
- **Berechtigungen:** Eingeschränkt auf 9A
  - ✅ Kann Deutsch- und Englisch-Noten für Schüler in 9A eintragen
  - ❌ Sieht die Klassen 10A und 10B nicht
  - ❌ Kann keine Mathe-Noten eintragen

### Klaus Fischer (`kfischer`)
- **Fächer:** *(keine zugeordnet)*
- **Klassen:** *(keine zugeordnet)*
- **Berechtigungen:** Nur Lesezugriff
  - ⚠️ Sieht Warnung "Lesezugriff" im Dashboard
  - ❌ Kann keine Noten eintragen oder bearbeiten
  - ❌ Sieht keine Klassen oder Schüler
  - ✅ Kann die Verwaltung (Fächer, Klassen, Schüler, Lehrer) einsehen

### Petra Hofmann (`phofmann`)
- **Fächer:** Englisch
- **Klassen:** 10A
- **Berechtigungen:** Minimal
  - ✅ Kann nur Englisch-Noten für Schüler in 10A eintragen
  - ❌ Sieht nur die Klasse 10A
  - ❌ Kann keine Mathe- oder Deutsch-Noten eintragen

---

## Testszenarien

### Vollzugriff testen
→ Login als `mschmidt` / `Passwort123`

### Eingeschränkten Fach-Zugriff testen
→ Login als `tmueller` / `Passwort123`
- Nur Mathematik sichtbar im Fach-Dropdown

### Eingeschränkten Klassen-Zugriff testen
→ Login als `sweber` / `Passwort123`
- Nur Klasse 9A im Klassen-Dropdown

### Read-Only Modus testen (Neuer Lehrer ohne Zuordnungen)
→ Login als `kfischer` / `Passwort123`
- Warnung "Lesezugriff" wird angezeigt
- Keine Klassen verfügbar

### Minimalen Zugriff testen
→ Login als `phofmann` / `Passwort123`
- Nur 10A sichtbar, nur Englisch als Fach

---

## Zusätzliche Demo-Daten

### Klassen
| Klasse | Kürzel | Anzahl Schüler |
|--------|--------|----------------|
| 10A | 10A | 35 |
| 10B | 10B | 29 |
| 9A | 9A | 25 |

### Fächer
| Fach | Kürzel |
|------|--------|
| Mathematik | MA |
| Deutsch | DE |
| Englisch | EN |

### Noten
Alle Schüler haben bereits 1-3 zufällige Noten (1-6) pro Fach mit Datum zwischen September und November 2023.
