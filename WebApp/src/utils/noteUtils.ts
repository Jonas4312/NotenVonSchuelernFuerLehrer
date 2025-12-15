import type { Note } from '../types';

/**
 * Berechnet den Durchschnitt von Noten
 */
export const calculateAverage = (noten: Note[]): number => {
  if (noten.length === 0) return 0;
  
  const summe = noten.reduce((acc, note) => acc + note.wert, 0);
  return summe / noten.length;
};

/**
 * Formatiert eine Note für die Anzeige
 */
export const formatNote = (wert: number): string => {
  return wert % 1 === 0 ? wert.toString() : wert.toFixed(1);
};

/**
 * Gibt die CSS-Klasse für eine Note basierend auf dem Wert zurück
 */
export const getNoteColorClass = (wert: number): string => {
  if (wert <= 1.5) return 'note-excellent';
  if (wert <= 2.5) return 'note-good';
  if (wert <= 3.5) return 'note-satisfactory';
  if (wert <= 4.0) return 'note-adequate';
  if (wert <= 5.0) return 'note-poor';
  return 'note-fail';
};

/**
 * Formatiert ein Datum für die Anzeige
 */
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('de-DE', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });
};

/**
 * Gruppiert Noten nach Fach und berechnet Durchschnitte
 */
export const groupNotenByFach = (noten: Note[]): Map<string, { noten: Note[]; durchschnitt: number }> => {
  const grouped = new Map<string, Note[]>();
  
  for (const note of noten) {
    const fachId = note.fachId || note.fach?.id;
    if (!fachId) continue;
    
    if (!grouped.has(fachId)) {
      grouped.set(fachId, []);
    }
    grouped.get(fachId)!.push(note);
  }
  
  const result = new Map<string, { noten: Note[]; durchschnitt: number }>();
  
  for (const [fachId, fachNoten] of grouped) {
    result.set(fachId, {
      noten: fachNoten,
      durchschnitt: calculateAverage(fachNoten),
    });
  }
  
  return result;
};

/**
 * Sortiert Noten nach Datum (neueste zuerst)
 */
export const sortNotenByDate = (noten: Note[]): Note[] => {
  return [...noten].sort((a, b) => 
    new Date(b.erstelltAm).getTime() - new Date(a.erstelltAm).getTime()
  );
};
