import { createContext, useContext, useState, useCallback, useEffect, type ReactNode } from 'react';
import type { Lehrer, LoginCredentials, AuthState } from '../types';
import { authApi } from '../services/api';

// AuthContext enthält nur Auth-relevante Daten
// Business-Daten (Klassen, Fächer) sollten separat vom API geladen werden
interface AuthContextType extends AuthState {
  login: (credentials: LoginCredentials) => Promise<boolean>;
  logout: () => void;
  updateLehrer: (lehrer: Lehrer) => void;
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

  // Beim Start: Check ob bereits eingeloggt (localStorage)
  // Nur Auth-relevante Daten werden gecacht - Business-Daten werden frisch vom API geladen
  useEffect(() => {
    const savedToken = localStorage.getItem('token');
    const savedLehrer = localStorage.getItem('lehrer');
    
    if (savedToken && savedLehrer) {
      try {
        const lehrer = JSON.parse(savedLehrer) as Lehrer;
        
        setState({
          isAuthenticated: true,
          lehrer,
          token: savedToken,
          isLoading: false,
        });
      } catch {
        localStorage.removeItem('token');
        localStorage.removeItem('lehrer');
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
      
      // Nur Auth-relevante Lehrer-Daten speichern (ohne Klassen/Fächer)
      // Business-Daten werden bei Bedarf frisch vom API geladen
      const lehrer: Lehrer = {
        id: response.lehrer.id,
        vorname: response.lehrer.vorname,
        nachname: response.lehrer.nachname,
        benutzername: response.lehrer.benutzername,
        bildByteArray: response.lehrer.bildByteArray,
        faecher: [], // Wird bei Bedarf frisch geladen
      };

      // Nur Token und Basis-Lehrer-Daten im localStorage
      localStorage.setItem('token', response.token);
      localStorage.setItem('lehrer', JSON.stringify(lehrer));
      
      setState({
        isAuthenticated: true,
        lehrer,
        token: response.token,
        isLoading: false,
      });
      
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
    setState({
      isAuthenticated: false,
      lehrer: null,
      token: null,
      isLoading: false,
    });
  }, []);

  const updateLehrer = useCallback((updatedLehrer: Lehrer) => {
    localStorage.setItem('lehrer', JSON.stringify(updatedLehrer));
    setState(prev => ({
      ...prev,
      lehrer: updatedLehrer,
    }));
  }, []);

  return (
    <AuthContext.Provider value={{ ...state, login, logout, updateLehrer }}>
      {children}
    </AuthContext.Provider>
  );
};
