import { useState, type FormEvent } from 'react';
import { GraduationCap, User, Lock, AlertCircle } from 'lucide-react';
import { useAuth } from '../../context';
import styles from './Login.module.css';

export const Login = () => {
  const { login, isLoading } = useAuth();
  const [benutzername, setBenutzername] = useState('');
  const [passwort, setPasswort] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');

    if (!benutzername.trim() || !passwort.trim()) {
      setError('Bitte füllen Sie alle Felder aus.');
      return;
    }

    const success = await login({ benutzername, passwort });
    if (!success) {
      setError('Anmeldung fehlgeschlagen. Bitte überprüfen Sie Ihre Eingaben.');
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <div className={styles.header}>
          <div className={styles.logo}>
            <GraduationCap size={48} aria-hidden="true" />
          </div>
          <h1 className={styles.title}>Notenverwaltung</h1>
          <p className={styles.subtitle}>
            Melden Sie sich an, um die mündliche Mitarbeit Ihrer Schüler zu dokumentieren.
          </p>
        </div>

        <form onSubmit={handleSubmit} className={styles.form}>
          {error && (
            <div className={styles.error} role="alert">
              <AlertCircle size={18} aria-hidden="true" />
              <span>{error}</span>
            </div>
          )}

          <div className={styles.field}>
            <label htmlFor="benutzername" className={styles.label}>
              Benutzername
            </label>
            <div className={styles.inputWrapper}>
              <User className={styles.inputIcon} size={20} aria-hidden="true" />
              <input
                id="benutzername"
                type="text"
                className={styles.input}
                placeholder="z.B. mmustermann"
                value={benutzername}
                onChange={(e) => setBenutzername(e.target.value)}
                autoComplete="username"
                autoFocus
              />
            </div>
          </div>

          <div className={styles.field}>
            <label htmlFor="passwort" className={styles.label}>
              Passwort
            </label>
            <div className={styles.inputWrapper}>
              <Lock className={styles.inputIcon} size={20} aria-hidden="true" />
              <input
                id="passwort"
                type="password"
                className={styles.input}
                placeholder="••••••••"
                value={passwort}
                onChange={(e) => setPasswort(e.target.value)}
                autoComplete="current-password"
              />
            </div>
          </div>

          <button
            type="submit"
            className={styles.submitButton}
            disabled={isLoading}
          >
            {isLoading ? 'Anmeldung läuft...' : 'Anmelden'}
          </button>
        </form>

        <p className={styles.hint}>
          Demo-Zugangsdaten: <strong>mschmidt</strong> / <strong>Passwort123</strong>
        </p>
      </div>
    </div>
  );
};
