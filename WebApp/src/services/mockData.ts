import type { Klasse, Schueler, Fach, Note } from '../types';

// Mock Fächer
export const mockFaecher: Fach[] = [
  { id: '1', bezeichnung: 'Deutsch', kurzbezeichnung: 'DE' },
  { id: '2', bezeichnung: 'Mathematik', kurzbezeichnung: 'MA' },
  { id: '3', bezeichnung: 'Englisch', kurzbezeichnung: 'EN' },
  { id: '4', bezeichnung: 'Informatik', kurzbezeichnung: 'IF' },
  { id: '5', bezeichnung: 'Sport', kurzbezeichnung: 'SP' },
];

// Beispiel-Notizen für mündliche Beteiligung
const notizBeispiele = [
  'Sehr aktive Mitarbeit heute',
  'Gute Antwort bei der Tafelübung',
  'Hat anderen Schülern geholfen',
  'Präsentation war gut strukturiert',
  'Konstruktive Fragen gestellt',
  'Gruppenarbeit sehr gut geleitet',
  undefined, // Manchmal keine Notiz
  undefined,
];

// Mock Noten
const createMockNoten = (schuelerId: string, klassenFaecher: Fach[]): Note[] => {
  const noten: Note[] = [];
  const notenWerte = [1, 1.5, 2, 2.5, 3, 3.5, 4];
  
  // Zufällige Anzahl von Noten (0-5)
  const anzahl = Math.floor(Math.random() * 6);
  
  for (let i = 0; i < anzahl; i++) {
    const fach = klassenFaecher[Math.floor(Math.random() * klassenFaecher.length)];
    const wert = notenWerte[Math.floor(Math.random() * notenWerte.length)];
    const daysAgo = Math.floor(Math.random() * 30);
    const datum = new Date();
    datum.setDate(datum.getDate() - daysAgo);
    
    const notiz = notizBeispiele[Math.floor(Math.random() * notizBeispiele.length)];
    
    noten.push({
      id: `note-${schuelerId}-${i}`,
      schuelerId,
      fachId: fach.id,
      wert,
      erstelltAm: datum.toISOString(),
      angepasstAm: datum.toISOString(),
      fach,
      notiz,
    });
  }
  
  return noten;
};

// Mock Schüler mit Placeholder-Bildern
export const createMockSchueler = (klasseId: string, klassenFaecher: Fach[]): Schueler[] => {
  const vornamen = ['Emma', 'Lukas', 'Sophie', 'Noah', 'Mia', 'Leon', 'Hannah', 'Finn', 'Lina', 'Paul'];
  const nachnamen = ['Schmidt', 'Müller', 'Weber', 'Meyer', 'Wagner', 'Becker', 'Hoffmann', 'Schulz'];
  
  return vornamen.slice(0, 4 + Math.floor(Math.random() * 6)).map((vorname, index) => {
    const id = `schueler-${klasseId}-${index}`;
    return {
      id,
      klasseId,
      vorname,
      nachname: nachnamen[index % nachnamen.length],
      bildUrl: `https://api.dicebear.com/7.x/avataaars/svg?seed=${vorname}${index}`,
      noten: createMockNoten(id, klassenFaecher),
    };
  });
};

// Fächer pro Klasse zuweisen (realistischer - nicht jede Klasse hat alle Fächer)
const klasse10AFaecher = [mockFaecher[0], mockFaecher[1], mockFaecher[2]]; // DE, MA, EN
const klasse10BFaecher = [mockFaecher[1], mockFaecher[3], mockFaecher[4]]; // MA, IF, SP
const klasse11AFaecher = [mockFaecher[0], mockFaecher[2], mockFaecher[3]]; // DE, EN, IF

// Mock Klassen
export const mockKlassen: Klasse[] = [
  {
    id: 'klasse-1',
    bezeichnung: 'Klasse 10A',
    kurzbezeichnung: '10A',
    schueler: createMockSchueler('klasse-1', klasse10AFaecher),
  },
  {
    id: 'klasse-2',
    bezeichnung: 'Klasse 10B',
    kurzbezeichnung: '10B',
    schueler: createMockSchueler('klasse-2', klasse10BFaecher),
  },
  {
    id: 'klasse-3',
    bezeichnung: 'Klasse 11A',
    kurzbezeichnung: '11A',
    schueler: createMockSchueler('klasse-3', klasse11AFaecher),
  },
];

// Mock API Functions (für Entwicklung ohne Backend)
export const mockApi = {
  getKlassen: (): Promise<Klasse[]> => {
    return new Promise((resolve) => {
      setTimeout(() => resolve(mockKlassen), 300);
    });
  },
  
  getKlasse: (id: string): Promise<Klasse | undefined> => {
    return new Promise((resolve) => {
      setTimeout(() => resolve(mockKlassen.find(k => k.id === id)), 200);
    });
  },
  
  getSchueler: (klasseId: string): Promise<Schueler[]> => {
    return new Promise((resolve) => {
      const klasse = mockKlassen.find(k => k.id === klasseId);
      setTimeout(() => resolve(klasse?.schueler || []), 200);
    });
  },
  
  getFaecher: (): Promise<Fach[]> => {
    return new Promise((resolve) => {
      setTimeout(() => resolve(mockFaecher), 150);
    });
  },
  
  addNote: (schuelerId: string, fachId: string, wert: number, notiz?: string): Promise<Note> => {
    return new Promise((resolve) => {
      const fach = mockFaecher.find(f => f.id === fachId);
      const newNote: Note = {
        id: `note-${Date.now()}`,
        schuelerId,
        fachId,
        wert,
        erstelltAm: new Date().toISOString(),
        angepasstAm: new Date().toISOString(),
        fach,
        notiz,
      };
      
      // Finde Schüler und füge Note hinzu
      for (const klasse of mockKlassen) {
        const schuelerList = klasse.schueler || [];
        const schueler = schuelerList.find(s => s.id === schuelerId);
        if (schueler) {
          const noten = schueler.noten || [];
          noten.push(newNote);
          schueler.noten = noten;
          break;
        }
      }
      
      setTimeout(() => resolve(newNote), 200);
    });
  },
  
  updateNote: (noteId: string, wert: number, notiz?: string): Promise<Note | undefined> => {
    return new Promise((resolve) => {
      let updatedNote: Note | undefined;
      
      for (const klasse of mockKlassen) {
        const schuelerList = klasse.schueler || [];
        for (const schueler of schuelerList) {
          const notenList = schueler.noten || [];
          const noteIndex = notenList.findIndex(n => n.id === noteId);
          if (noteIndex >= 0) {
            notenList[noteIndex] = {
              ...notenList[noteIndex],
              wert,
              notiz,
              angepasstAm: new Date().toISOString(),
            };
            updatedNote = notenList[noteIndex];
            break;
          }
        }
      }
      
      setTimeout(() => resolve(updatedNote), 200);
    });
  },
  
  deleteNote: (noteId: string): Promise<boolean> => {
    return new Promise((resolve) => {
      for (const klasse of mockKlassen) {
        const schuelerList = klasse.schueler || [];
        for (const schueler of schuelerList) {
          const notenList = schueler.noten || [];
          const noteIndex = notenList.findIndex(n => n.id === noteId);
          if (noteIndex >= 0) {
            notenList.splice(noteIndex, 1);
            setTimeout(() => resolve(true), 200);
            return;
          }
        }
      }
      setTimeout(() => resolve(false), 200);
    });
  },
};
