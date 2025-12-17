import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { Dashboard, Login, Settings, Admin } from './pages';
import { AuthProvider, useAuth } from './context';
import { Header, type PageType } from './components';
import './App.css';

// Geschützte Route - leitet auf /login um wenn nicht eingeloggt
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="app-loading">
        <div className="spinner" />
        <p>Laden...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    // Speichere die aktuelle URL um nach Login dorthin zurückzukehren
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
};

// Layout für eingeloggte Benutzer mit Header
const AuthenticatedLayout = ({ page }: { page: PageType }) => {
  const { lehrer, logout } = useAuth();
  const location = useLocation();

  // Aktuelle Seite aus URL ableiten
  const getCurrentPage = (): PageType => {
    if (location.pathname === '/admin') return 'admin';
    if (location.pathname === '/user') return 'settings';
    return 'dashboard';
  };

  return (
    <div className="app">
      <Header 
        lehrer={lehrer}
        onLogout={logout}
        currentPage={page || getCurrentPage()}
        onNavigate={() => {}} // Navigation erfolgt jetzt über Router
      />
      <main className="app-main">
        {page === 'dashboard' && <Dashboard />}
        {page === 'admin' && <Admin />}
        {page === 'settings' && <Settings />}
      </main>
    </div>
  );
};

// Login-Route - leitet auf / um wenn bereits eingeloggt
const LoginRoute = () => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  // Nur beim initialen Laden den Spinner zeigen (Token-Check)
  // Während eines Login-Versuchs zeigen wir weiterhin das Login-Formular
  const isInitialLoading = isLoading && !isAuthenticated;
  
  // Prüfen ob wir bereits auf der Login-Seite sind (um Remount zu vermeiden)
  const isOnLoginPage = location.pathname === '/login';

  if (isInitialLoading && !isOnLoginPage) {
    return (
      <div className="app-loading">
        <div className="spinner" />
        <p>Laden...</p>
      </div>
    );
  }

  if (isAuthenticated) {
    // Zurück zur ursprünglichen Seite oder zur Startseite
    const from = (location.state as { from?: Location })?.from?.pathname || '/';
    return <Navigate to={from} replace />;
  }

  return <Login />;
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginRoute />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <AuthenticatedLayout page="dashboard" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <ProtectedRoute>
                <AuthenticatedLayout page="admin" />
              </ProtectedRoute>
            }
          />
          <Route
            path="/user"
            element={
              <ProtectedRoute>
                <AuthenticatedLayout page="settings" />
              </ProtectedRoute>
            }
          />
          {/* Fallback für unbekannte Routen */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
