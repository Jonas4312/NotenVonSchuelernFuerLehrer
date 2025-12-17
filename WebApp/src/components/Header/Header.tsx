import { useState, useRef, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { GraduationCap, LogOut, User, Settings, BookOpen, Home, ChevronDown } from 'lucide-react';
import type { Lehrer } from '../../types';
import styles from './Header.module.css';

export type PageType = 'dashboard' | 'settings' | 'admin';

interface HeaderProps {
  title?: string;
  lehrer?: Lehrer | null;
  onLogout?: () => void;
  currentPage?: PageType;
  onNavigate?: (page: PageType) => void;
}

export const Header = ({ 
  title = 'Notenverwaltung', 
  lehrer, 
  onLogout,
}: HeaderProps) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isProfileMenuOpen, setIsProfileMenuOpen] = useState(false);
  const profileMenuRef = useRef<HTMLDivElement>(null);

  // Aktuelle Seite aus URL ableiten
  const currentPage: PageType = 
    location.pathname === '/admin' ? 'admin' :
    location.pathname === '/user' ? 'settings' : 
    'dashboard';

  // Schließe Menü bei Klick außerhalb
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (profileMenuRef.current && !profileMenuRef.current.contains(event.target as Node)) {
        setIsProfileMenuOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleProfileMenuClick = () => {
    setIsProfileMenuOpen(!isProfileMenuOpen);
  };

  const handleNavigate = (page: PageType) => {
    switch (page) {
      case 'dashboard':
        navigate('/');
        break;
      case 'admin':
        navigate('/admin');
        break;
      case 'settings':
        navigate('/user');
        break;
    }
  };

  const handleSettingsClick = () => {
    handleNavigate('settings');
    setIsProfileMenuOpen(false);
  };

  const handleLogoutClick = () => {
    setIsProfileMenuOpen(false);
    onLogout?.();
    navigate('/login');
  };

  return (
    <header className={styles.header} role="banner">
      <div className={styles.container}>
        <div className={styles.logo}>
          <GraduationCap size={32} aria-hidden="true" />
          <h1 className={styles.title}>{title}</h1>
        </div>
        
        {/* Navigation */}
        <nav className={styles.nav} aria-label="Hauptnavigation">
          <button
            className={`${styles.navButton} ${currentPage === 'dashboard' ? styles.navButtonActive : ''}`}
            onClick={() => handleNavigate('dashboard')}
            aria-current={currentPage === 'dashboard' ? 'page' : undefined}
          >
            <Home size={18} aria-hidden="true" />
            <span>Noten</span>
          </button>
          <button
            className={`${styles.navButton} ${currentPage === 'admin' ? styles.navButtonActive : ''}`}
            onClick={() => handleNavigate('admin')}
            aria-current={currentPage === 'admin' ? 'page' : undefined}
          >
            <BookOpen size={18} aria-hidden="true" />
            <span>Verwaltung</span>
          </button>
        </nav>
        
        {lehrer && (
          <div className={styles.userSection} ref={profileMenuRef}>
            <button 
              className={`${styles.profileButton} ${currentPage === 'settings' ? styles.profileButtonActive : ''}`}
              onClick={handleProfileMenuClick}
              aria-expanded={isProfileMenuOpen}
              aria-haspopup="true"
            >
              <div className={styles.avatar}>
                {lehrer.bildByteArray ? (
                  <img src={`data:image/jpeg;base64,${lehrer.bildByteArray}`} alt="" className={styles.avatarImage} />
                ) : (
                  <User size={20} aria-hidden="true" />
                )}
              </div>
              <span className={styles.userName}>
                {lehrer.vorname} {lehrer.nachname}
              </span>
              <ChevronDown 
                size={16} 
                className={`${styles.chevron} ${isProfileMenuOpen ? styles.chevronOpen : ''}`} 
                aria-hidden="true" 
              />
            </button>
            
            {/* Dropdown Menü */}
            {isProfileMenuOpen && (
              <div className={styles.profileMenu} role="menu">
                <button 
                  className={styles.menuItem} 
                  onClick={handleSettingsClick}
                  role="menuitem"
                >
                  <Settings size={18} aria-hidden="true" />
                  <span>Einstellungen</span>
                </button>
                <div className={styles.menuDivider} />
                <button 
                  className={`${styles.menuItem} ${styles.menuItemDanger}`} 
                  onClick={handleLogoutClick}
                  role="menuitem"
                >
                  <LogOut size={18} aria-hidden="true" />
                  <span>Abmelden</span>
                </button>
              </div>
            )}
          </div>
        )}
      </div>
    </header>
  );
};
