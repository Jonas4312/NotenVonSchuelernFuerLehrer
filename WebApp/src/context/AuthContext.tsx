import { createContext, useContext, useState, useCallback, useEffect, type ReactNode } from 'react';
import type { Lehrer, Klasse, LoginCredentials, AuthState } from '../types';
import { authApi } from '../services/api';

interface AuthContextType extends AuthState {
  login: (credentials: LoginCredentials) => Promise<boolean>;
  logout: () => void;
  updateLehrer: (lehrer: Lehrer) => void;
  klassen: Klasse[];
}

const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [state, setState] = useState<AuthState>({
    isAuthenticated: false,
    lehrer: null,
    token: null,
    isLoading: true,
  });
  const [klassen, setKlassen] = useState<Klasse[]>([]);

  // Beim Start: Check ob bereits eingeloggt (localStorage)
  useEffect(() => {
    const savedToken = localStorage.getItem('token');
    const savedLehrer = localStorage.getItem('lehrer');
    const savedKlassen = localStorage.getItem('klassen');
    
    if (savedToken && savedLehrer) {
      try {
        const lehrer = JSON.parse(savedLehrer) as Lehrer;
        const parsedKlassen = savedKlassen ? JSON.parse(savedKlassen) as Klasse[] : [];
        
        setState({
          isAuthenticated: true,
          lehrer,
          token: savedToken,
          isLoading: false,
        });
        setKlassen(parsedKlassen);
      } catch {
        localStorage.removeItem('token');
        localStorage.removeItem('lehrer');
        localStorage.removeItem('klassen');
        setState(prev => ({ ...prev, isLoading: false }));
      }
    } else {
      setState(prev => ({ ...prev, isLoading: false }));
    }
  }, []);

  const login = useCallback(async (credentials: LoginCredentials): Promise<boolean> => {
    setState(prev => ({ ...prev, isLoading: true }));

    try {
      const response = await authApi.login(credentials.benutzername, credentials.passwort);
      
      // Lehrer mit Fächern zusammenbauen
      const lehrer: Lehrer = {
        id: response.lehrer.id,
        vorname: response.lehrer.vorname,
        nachname: response.lehrer.nachname,
        benutzername: response.lehrer.benutzername,
        bildByteArray: response.lehrer.bildByteArray,
        bildUrl: response.lehrer.bildByteArray 
          ? `data:image/jpeg;base64,${response.lehrer.bildByteArray}` 
          : undefined,
        faecher: response.faecher,
        klassen: response.klassen,
      };

      // In localStorage speichern
      localStorage.setItem('token', response.token);
      localStorage.setItem('lehrer', JSON.stringify(lehrer));
      localStorage.setItem('klassen', JSON.stringify(response.klassen));
      
      setState({
        isAuthenticated: true,
        lehrer,
        token: response.token,
        isLoading: false,
      });
      setKlassen(response.klassen);
      
      return true;
    } catch (error) {
      console.error('Login failed:', error);
      setState(prev => ({ ...prev, isLoading: false }));
      return false;
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('lehrer');
    localStorage.removeItem('klassen');
    setState({
      isAuthenticated: false,
      lehrer: null,
      token: null,
      isLoading: false,
    });
    setKlassen([]);
  }, []);

  const updateLehrer = useCallback((updatedLehrer: Lehrer) => {
    localStorage.setItem('lehrer', JSON.stringify(updatedLehrer));
    setState(prev => ({
      ...prev,
      lehrer: updatedLehrer,
    }));
  }, []);

  return (
    <AuthContext.Provider value={{ ...state, login, logout, updateLehrer, klassen }}>
      {children}
    </AuthContext.Provider>
  );
};
