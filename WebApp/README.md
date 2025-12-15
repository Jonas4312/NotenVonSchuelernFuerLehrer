# Notenverwaltung - Frontend

Eine React-Anwendung zur Dokumentation der mündlichen Mitarbeit von Schülern.

## 🚀 Features

- **Klassenübersicht**: Wählen Sie eine Klasse aus dem Dropdown
- **Schülerliste**: Übersichtliche Kacheln mit Namen, Bild und Durchschnittsnote
- **Notendetails**: Vollständige Notenübersicht pro Schüler mit Fachdurchschnitten
- **CRUD-Operationen**: Noten hinzufügen, bearbeiten und löschen
- **Responsive Design**: Optimiert für Desktop, Tablet und Mobile
- **Barrierefreiheit**: ARIA-Labels, Keyboard-Navigation, Farbkontraste

## 🛠️ Tech Stack

- **React 19** - UI Framework
- **TypeScript** - Typsicherheit
- **Vite** - Build Tool & Dev Server
- **CSS Modules** - Scoped Styling
- **Lucide React** - Icons
- **Axios** - HTTP Client (für Backend-Anbindung)

## 📦 Installation

```bash
cd WebApp
npm install
```

## 🏃 Entwicklung

```bash
npm run dev
```

Öffne [http://localhost:3000](http://localhost:3000) im Browser.

## 🏗️ Build

```bash
npm run build
```

Die Build-Artefakte werden im `dist/` Ordner erstellt.

## 📁 Projektstruktur

```
src/
├── components/           # Wiederverwendbare UI-Komponenten
│   ├── Header/          # App-Header mit Logo
│   ├── KlasseSelector/  # Dropdown für Klassenauswahl
│   ├── SchuelerList/    # Liste aller Schüler
│   ├── SchuelerCard/    # Einzelne Schülerkarte
│   ├── SchuelerDetail/  # Detailansicht eines Schülers
│   ├── NotenTable/      # Tabelle aller Noten
│   ├── NoteModal/       # Dialog zum Hinzufügen/Bearbeiten
│   └── ConfirmDialog/   # Bestätigungsdialog (Löschen)
├── pages/               # Seiten-Komponenten
│   └── Dashboard/       # Hauptseite
├── services/            # API & Daten
│   ├── api.ts          # Axios API-Funktionen (für Backend)
│   └── mockData.ts     # Mock-Daten für Entwicklung
├── types/               # TypeScript Typdefinitionen
│   └── index.ts        # Alle Domain-Typen
└── utils/               # Hilfsfunktionen
    └── noteUtils.ts    # Berechnungen für Noten
```

## 🎨 Farbkonzept

Die Anwendung verwendet ein **beruhigendes Blau** als Primärfarbe (#2563eb), das:
- Vertrauen und Professionalität vermittelt
- Für Bildungsanwendungen geeignet ist
- Gute Kontraste für Lesbarkeit bietet

Noten werden farblich nach Qualität kodiert:
- 🟢 1.0-1.5: Grün (Sehr gut)
- 🟡 1.6-2.5: Hellgrün (Gut)
- 🟠 2.6-3.5: Gelb (Befriedigend)
- 🔴 3.6+: Orange/Rot (Verbesserungsbedarf)

## ♿ Barrierefreiheit

- Alle interaktiven Elemente haben `aria-label`
- Dialoge nutzen `role="dialog"` und `aria-modal`
- Fokus-Management mit sichtbaren Fokus-Indikatoren
- Keyboard-Navigation (Tab, Enter, Escape)
- Farbkontraste nach WCAG 2.1 AA

## 🔌 Backend-Anbindung

Die Anwendung ist vorbereitet für die Anbindung an das .NET Backend:

1. API-Services sind in `src/services/api.ts` definiert
2. Der Vite-Proxy leitet `/api/*` Anfragen weiter
3. Aktuell werden Mock-Daten verwendet (`src/services/mockData.ts`)

Um auf das echte Backend umzuschalten, ändere in den Komponenten:
```typescript
// Statt:
import { mockApi } from '../../services/mockData';

// Verwende:
import { klassenApi, schuelerApi, notenApi } from '../../services/api';
```

## 📱 Responsive Breakpoints

- **Mobile**: < 768px (Kachel-Layout, Zurück-Button)
- **Tablet**: 768px - 1024px (Kompaktere Sidebar)
- **Desktop**: > 1024px (Volle Zwei-Spalten-Ansicht)

## 🧑‍💻 Entwickler

- Alexander (Frontend)
- Alexandra (Backend & UI-Design)
- Jonas (Backend & Datenbank)
