import { useState } from 'react';
import { Dashboard, Login, Settings, Admin } from './pages';
import { AuthProvider, useAuth } from './context';
import { Header, type PageType } from './components';
import './App.css';

// Innere Komponente die den Auth-Context nutzt
const AppContent = () => {
  const { isAuthenticated, isLoading, lehrer, logout } = useAuth();
  const [currentPage, setCurrentPage] = useState<PageType>('dashboard');

  if (isLoading) {
    return (
      <div className="app-loading">
        <div className="spinner" />
        <p>Laden...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Login />;
  }

  const renderPage = () => {
    switch (currentPage) {
      case 'settings':
        return <Settings />;
      case 'admin':
        return <Admin />;
      case 'dashboard':
      default:
        return <Dashboard />;
    }
  };

  return (
    <div className="app">
      <Header 
        lehrer={lehrer}
        onLogout={logout}
        currentPage={currentPage}
        onNavigate={setCurrentPage}
      />
      <main className="app-main">
        {renderPage()}
      </main>
    </div>
  );
};

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;
