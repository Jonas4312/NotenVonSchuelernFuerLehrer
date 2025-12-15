import axios from 'axios';
import type { 
  KlasseDto, 
  SchuelerDto, 
  FachDto, 
  NoteDto, 
  LehrerDto,
  LoginResponse 
} from '../types';

// API Base URL - im Docker-Setup nutzt der YARP-Proxy /api als Prefix
// Im Entwicklungsmodus kann VITE_API_URL gesetzt werden (z.B. http://localhost:5000/api)
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Auth-Interceptor: Token automatisch anhängen
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response-Interceptor: Daten extrahieren und bei 401 ausloggen
apiClient.interceptors.response.use(
  (response) => {
    // API gibt { success, data: ... } zurück - extrahiere data automatisch
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      response.data = response.data.data;
    }
    return response;
  },
  (error) => {
    // Bei 401 (Unauthorized) ausloggen - aber nicht bei Login-Requests
    if (error.response?.status === 401 && !error.config?.url?.includes('/auth/login')) {
      localStorage.removeItem('token');
      localStorage.removeItem('lehrer');
      // Nur weiterleiten wenn nicht bereits auf der Login-Seite
      if (!window.location.pathname.includes('/login')) {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  login: async (username: string, password: string): Promise<LoginResponse> => {
    const response = await apiClient.post<{ 
      token: string; 
      lehrer: LoginResponse['lehrer']; 
      faecher: LoginResponse['faecher']; 
      klassen: LoginResponse['klassen'];
    }>('/auth/login', {
      username,
      password,
    });
    // Der Interceptor extrahiert bereits data aus { success, data: ... }
    const { token, lehrer, faecher, klassen } = response.data;
    return {
      token,
      lehrer: {
        id: lehrer.id,
        vorname: lehrer.vorname,
        nachname: lehrer.nachname,
        benutzername: lehrer.benutzername,
        bildByteArray: lehrer.bildByteArray,
      },
      faecher: faecher || [],
      klassen: klassen || [],
    };
  },
};

// Klassen API
export const klassenApi = {
  getAll: async (): Promise<KlasseDto[]> => {
    const response = await apiClient.get<{ klassen: KlasseDto[] }>('/klasse');
    return response.data.klassen;
  },

  getAllAdmin: async (): Promise<KlasseDto[]> => {
    const response = await apiClient.get<{ klassen: KlasseDto[] }>('/klasse/alle');
    return response.data.klassen;
  },
  
  getSchueler: async (klasseId: string): Promise<SchuelerDto[]> => {
    const response = await apiClient.get<{ schueler: SchuelerDto[] }>(`/klasse/${klasseId}`);
    return response.data.schueler;
  },

  create: async (data: { bezeichnung: string; kurzbezeichnung: string }): Promise<KlasseDto> => {
    const response = await apiClient.post<{ klasse: KlasseDto }>('/klasse', data);
    return response.data.klasse;
  },

  update: async (klasseId: string, data: { bezeichnung: string; kurzbezeichnung: string }): Promise<KlasseDto> => {
    const response = await apiClient.put<{ klasse: KlasseDto }>('/klasse', {
      klasseId,
      ...data,
    });
    return response.data.klasse;
  },

  delete: async (klasseId: string): Promise<void> => {
    await apiClient.delete('/klasse', { data: { klasseId } });
  },

  // Klasse-Lehrer Zuweisungen
  assignToLehrer: async (klasseId: string, lehrerId: string): Promise<void> => {
    await apiClient.post(`/klasse/${klasseId}/lehrer/${lehrerId}`);
  },

  removeFromLehrer: async (klasseId: string, lehrerId: string): Promise<void> => {
    await apiClient.delete(`/klasse/${klasseId}/lehrer/${lehrerId}`);
  },
};

// Schüler API
export const schuelerApi = {
  getAll: async (): Promise<SchuelerDto[]> => {
    const response = await apiClient.get<{ schueler: SchuelerDto[] }>('/schueler');
    return response.data.schueler;
  },

  getById: async (schuelerId: string): Promise<SchuelerDto> => {
    const response = await apiClient.get<{ schueler: SchuelerDto }>(`/schueler/${schuelerId}`);
    return response.data.schueler;
  },

  create: async (data: { klasseId: string; vorname: string; nachname: string; bildByteArray?: string }): Promise<SchuelerDto> => {
    const response = await apiClient.post<{ schueler: SchuelerDto }>('/schueler', data);
    return response.data.schueler;
  },

  update: async (schuelerId: string, data: { vorname?: string; nachname?: string; klasseId?: string; bildByteArray?: string }): Promise<SchuelerDto> => {
    const response = await apiClient.put<{ schueler: SchuelerDto }>('/schueler', {
      schuelerId,
      ...data,
    });
    return response.data.schueler;
  },

  delete: async (schuelerId: string): Promise<void> => {
    await apiClient.delete('/schueler', { data: { schuelerId } });
  },
};

// Fächer API
export const faecherApi = {
  getAll: async (): Promise<FachDto[]> => {
    const response = await apiClient.get<{ faecher: FachDto[] }>('/fach');
    return response.data.faecher;
  },

  getById: async (fachId: string): Promise<FachDto> => {
    const response = await apiClient.get<{ fach: FachDto }>(`/fach/${fachId}`);
    return response.data.fach;
  },

  create: async (data: { bezeichnung: string; kurzbezeichnung: string }): Promise<FachDto> => {
    const response = await apiClient.post<{ fach: FachDto }>('/fach', data);
    return response.data.fach;
  },

  update: async (fachId: string, data: { bezeichnung: string; kurzbezeichnung: string }): Promise<FachDto> => {
    const response = await apiClient.put<{ fach: FachDto }>('/fach', {
      fachId,
      ...data,
    });
    return response.data.fach;
  },

  delete: async (fachId: string): Promise<void> => {
    await apiClient.delete('/fach', { data: { fachId } });
  },

  // Fach-Lehrer Zuweisungen
  assignToLehrer: async (fachId: string, lehrerId: string): Promise<void> => {
    await apiClient.post(`/fach/${fachId}/lehrer/${lehrerId}`);
  },

  removeFromLehrer: async (fachId: string, lehrerId: string): Promise<void> => {
    await apiClient.delete(`/fach/${fachId}/lehrer/${lehrerId}`);
  },
};

// Noten API
export const notenApi = {
  getBySchueler: async (schuelerId: string): Promise<NoteDto[]> => {
    const response = await apiClient.get<{ noten: NoteDto[] }>(`/note?schuelerId=${schuelerId}`);
    return response.data.noten;
  },

  create: async (data: { schuelerId: string; fachId: string; wert: number; notiz?: string }): Promise<NoteDto> => {
    const response = await apiClient.post<{ note: NoteDto }>('/note', data);
    return response.data.note;
  },
  
  update: async (noteId: string, data: { wert: number; notiz?: string }): Promise<NoteDto> => {
    const response = await apiClient.put<{ note: NoteDto }>('/note', {
      noteId,
      neueNote: data.wert,
      notiz: data.notiz,
    });
    return response.data.note;
  },
  
  delete: async (noteId: string): Promise<void> => {
    await apiClient.delete('/note', { data: { noteId } });
  },
};

// Lehrer API
export const lehrerApi = {
  getAll: async (): Promise<LehrerDto[]> => {
    const response = await apiClient.get<{ lehrer: LehrerDto[] }>('/lehrer');
    return response.data.lehrer;
  },

  getById: async (lehrerId: string): Promise<LehrerDto> => {
    const response = await apiClient.get<{ lehrer: LehrerDto; faecher?: FachDto[]; klassen?: KlasseDto[] }>(`/lehrer/${lehrerId}`);
    // faecher und klassen kommen auf data-Ebene, nicht innerhalb von lehrer
    return {
      ...response.data.lehrer,
      faecher: response.data.faecher || [],
    };
  },

  create: async (data: { 
    vorname: string; 
    nachname: string; 
    benutzername: string; 
    passwort: string; 
    bildByteArray?: string 
  }): Promise<LehrerDto> => {
    const response = await apiClient.post<{ lehrer: LehrerDto }>('/lehrer', data);
    return response.data.lehrer;
  },

  update: async (lehrerId: string, data: { 
    vorname?: string; 
    nachname?: string; 
    benutzername?: string; 
    passwort?: string; 
    bildByteArray?: string 
  }): Promise<LehrerDto> => {
    const response = await apiClient.put<{ lehrer: LehrerDto }>('/lehrer', {
      lehrerId,
      ...data,
    });
    return response.data.lehrer;
  },

  delete: async (lehrerId: string): Promise<void> => {
    await apiClient.delete('/lehrer', { data: { lehrerId } });
  },
};

export default apiClient;
