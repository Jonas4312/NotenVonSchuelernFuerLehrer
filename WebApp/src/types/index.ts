// Domain Types basierend auf Backend-DTOs

export interface KlasseDto {
  id: string;
  bezeichnung: string;
  kurzbezeichnung: string;
  anzahlSchueler?: number;
}

export interface SchuelerDto {
  id: string;
  vorname: string;
  nachname: string;
  bildByteArray: string; // Base64
  anzahlNoten?: number;
  klasseId?: string;
  klasseBezeichnung?: string;
}

export interface FachDto {
  id: string;
  bezeichnung: string;
  kurzbezeichnung: string;
}

export interface NoteDto {
  id: string;
  wert: number;
  notiz?: string;
  erstelltAm: string;
  angepasstAm: string;
  fach: FachDto;
}

export interface LehrerDto {
  id: string;
  vorname: string;
  nachname: string;
  benutzername: string;
  bildByteArray: string; // Base64
  faecher?: FachDto[]; // Kann bei getById vorhanden sein
  klassen?: KlasseDto[]; // Dem Lehrer zugeordnete Klassen
}

// API Response Types
export interface LoginResponse {
  token: string;
  lehrer: LehrerDto;
  faecher: FachDto[];
  klassen: KlasseDto[];
}

// Legacy Types für UI-Kompatibilität (werden schrittweise ersetzt)
export interface Klasse extends KlasseDto {
  schueler?: Schueler[];
}

export interface Schueler extends Omit<SchuelerDto, 'bildByteArray'> {
  klasseId?: string;
  bildUrl?: string;
  bildByteArray?: string;
  noten?: Note[];
}

export interface Fach extends FachDto {}

export interface Note {
  id: string;
  schuelerId?: string;
  fachId?: string;
  wert: number;
  notiz?: string;
  erstelltAm: string;
  angepasstAm: string;
  fach?: Fach;
}

export interface Lehrer {
  id: string;
  vorname: string;
  nachname: string;
  benutzername: string;
  bildUrl?: string;
  bildByteArray?: string;
  faecher: Fach[];
  klassen?: Klasse[];
}

// Auth Types
export interface LoginCredentials {
  benutzername: string;
  passwort: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  lehrer: Lehrer | null;
  token: string | null;
  isLoading: boolean;
}

// UI Helper Types
export interface NoteFormData {
  fachId: string;
  wert: number;
  notiz?: string;
  datum?: string;
}

export interface SchuelerMitDurchschnitt extends Schueler {
  durchschnitt: number;
  notenAnzahl: number;
}

export interface FachDurchschnitt {
  fach: Fach;
  durchschnitt: number;
  anzahl: number;
}
