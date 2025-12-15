import { useState, useRef, type FormEvent, type ChangeEvent } from 'react';
import { User, Lock, Trash2, Upload, Save, AlertTriangle } from 'lucide-react';
import { useAuth } from '../../context';
import { lehrerApi } from '../../services/api';
import styles from './Settings.module.css';

export const Settings = () => {
  const { lehrer, logout, updateLehrer } = useAuth();
  
  // Benutzername
  const [benutzername, setBenutzername] = useState(lehrer?.benutzername || '');
  const [benutzernameError, setBenutzernameError] = useState('');
  const [benutzernameSuccess, setBenutzernameSuccess] = useState(false);
  
  // Passwort
  const [aktuellesPasswort, setAktuellesPasswort] = useState('');
  const [neuesPasswort, setNeuesPasswort] = useState('');
  const [passwortWiederholen, setPasswortWiederholen] = useState('');
  const [passwortError, setPasswortError] = useState('');
  const [passwortSuccess, setPasswortSuccess] = useState(false);
  
  // Profilbild
  const [bildPreview, setBildPreview] = useState<string | null>(lehrer?.bildByteArray || null);
  const [bildFile, setBildFile] = useState<File | null>(null);
  const [bildSuccess, setBildSuccess] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  // Account löschen
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [deleteConfirmation, setDeleteConfirmation] = useState('');

  // Benutzername speichern
  const handleBenutzernameSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setBenutzernameError('');
    setBenutzernameSuccess(false);

    if (!benutzername.trim()) {
      setBenutzernameError('Benutzername darf nicht leer sein');
      return;
    }

    if (benutzername.length < 3) {
      setBenutzernameError('Benutzername muss mindestens 3 Zeichen haben');
      return;
    }

    if (!lehrer) return;

    try {
      const updated = await lehrerApi.update(lehrer.id, { benutzername });
      updateLehrer({ ...lehrer, ...updated });
      setBenutzernameSuccess(true);
    } catch (error) {
      console.error('Fehler beim Speichern:', error);
      setBenutzernameError('Fehler beim Speichern');
    }
  };

  // Passwort speichern
  const handlePasswortSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setPasswortError('');
    setPasswortSuccess(false);

    if (!aktuellesPasswort) {
      setPasswortError('Bitte aktuelles Passwort eingeben');
      return;
    }

    if (!neuesPasswort) {
      setPasswortError('Bitte neues Passwort eingeben');
      return;
    }

    if (neuesPasswort.length < 6) {
      setPasswortError('Neues Passwort muss mindestens 6 Zeichen haben');
      return;
    }

    if (neuesPasswort !== passwortWiederholen) {
      setPasswortError('Passwörter stimmen nicht überein');
      return;
    }

    if (!lehrer) return;

    try {
      // TODO: Das Backend braucht noch einen speziellen Endpoint für Passwort-Änderung
      // Vorerst nutzen wir update mit neuem Passwort
      await lehrerApi.update(lehrer.id, { passwort: neuesPasswort });
      setPasswortSuccess(true);
      setAktuellesPasswort('');
      setNeuesPasswort('');
      setPasswortWiederholen('');
    } catch (error) {
      console.error('Fehler beim Ändern:', error);
      setPasswortError('Fehler beim Ändern des Passworts');
    }
  };

  // Bild auswählen
  const handleBildSelect = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Vorschau erstellen
    const reader = new FileReader();
    reader.onloadend = () => {
      setBildPreview(reader.result as string);
    };
    reader.readAsDataURL(file);
    setBildFile(file);
    setBildSuccess(false);
  };

  // Bild hochladen
  const handleBildUpload = async () => {
    if (!bildFile || !lehrer) return;

    try {
      // Bild als Base64 senden
      const reader = new FileReader();
      reader.onloadend = async () => {
        const base64 = reader.result as string;
        const updated = await lehrerApi.update(lehrer.id, { bildByteArray: base64 });
        updateLehrer({ ...lehrer, ...updated });
        setBildSuccess(true);
        setBildFile(null);
      };
      reader.readAsDataURL(bildFile);
    } catch (error) {
      console.error('Fehler beim Hochladen:', error);
    }
  };

  // Account löschen
  const handleDeleteAccount = async () => {
    if (deleteConfirmation !== 'LÖSCHEN' || !lehrer) return;

    try {
      await lehrerApi.delete(lehrer.id);
      logout();
    } catch (error) {
      console.error('Fehler beim Löschen:', error);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Einstellungen</h1>

      <div className={styles.sections}>
        {/* Profilbild */}
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <User size={20} aria-hidden="true" />
            Profilbild
          </h2>
          <div className={styles.bildSection}>
            <div className={styles.bildPreview}>
              {bildPreview ? (
                <img src={bildPreview} alt="Profilbild" />
              ) : (
                <User size={48} aria-hidden="true" />
              )}
            </div>
            <div className={styles.bildActions}>
              <input
                ref={fileInputRef}
                type="file"
                accept="image/*"
                onChange={handleBildSelect}
                className={styles.fileInput}
                aria-label="Bild auswählen"
              />
              <button
                type="button"
                className={styles.secondaryButton}
                onClick={() => fileInputRef.current?.click()}
              >
                <Upload size={18} aria-hidden="true" />
                Bild auswählen
              </button>
              {bildFile && (
                <button
                  type="button"
                  className={styles.primaryButton}
                  onClick={handleBildUpload}
                >
                  <Save size={18} aria-hidden="true" />
                  Hochladen
                </button>
              )}
              {bildSuccess && (
                <span className={styles.successMessage}>✓ Bild gespeichert</span>
              )}
            </div>
          </div>
        </section>

        {/* Benutzername */}
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <User size={20} aria-hidden="true" />
            Benutzername
          </h2>
          <form onSubmit={handleBenutzernameSubmit} className={styles.form}>
            <div className={styles.formGroup}>
              <label htmlFor="benutzername">Benutzername</label>
              <input
                id="benutzername"
                type="text"
                value={benutzername}
                onChange={(e) => setBenutzername(e.target.value)}
                placeholder="Benutzername eingeben"
              />
            </div>
            {benutzernameError && (
              <p className={styles.errorMessage}>{benutzernameError}</p>
            )}
            {benutzernameSuccess && (
              <p className={styles.successMessage}>✓ Benutzername gespeichert</p>
            )}
            <button type="submit" className={styles.primaryButton}>
              <Save size={18} aria-hidden="true" />
              Speichern
            </button>
          </form>
        </section>

        {/* Passwort */}
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>
            <Lock size={20} aria-hidden="true" />
            Passwort ändern
          </h2>
          <form onSubmit={handlePasswortSubmit} className={styles.form}>
            <div className={styles.formGroup}>
              <label htmlFor="aktuellesPasswort">Aktuelles Passwort</label>
              <input
                id="aktuellesPasswort"
                type="password"
                value={aktuellesPasswort}
                onChange={(e) => setAktuellesPasswort(e.target.value)}
                placeholder="Aktuelles Passwort"
              />
            </div>
            <div className={styles.formGroup}>
              <label htmlFor="neuesPasswort">Neues Passwort</label>
              <input
                id="neuesPasswort"
                type="password"
                value={neuesPasswort}
                onChange={(e) => setNeuesPasswort(e.target.value)}
                placeholder="Neues Passwort (min. 6 Zeichen)"
              />
            </div>
            <div className={styles.formGroup}>
              <label htmlFor="passwortWiederholen">Passwort wiederholen</label>
              <input
                id="passwortWiederholen"
                type="password"
                value={passwortWiederholen}
                onChange={(e) => setPasswortWiederholen(e.target.value)}
                placeholder="Neues Passwort wiederholen"
              />
            </div>
            {passwortError && (
              <p className={styles.errorMessage}>{passwortError}</p>
            )}
            {passwortSuccess && (
              <p className={styles.successMessage}>✓ Passwort geändert</p>
            )}
            <button type="submit" className={styles.primaryButton}>
              <Lock size={18} aria-hidden="true" />
              Passwort ändern
            </button>
          </form>
        </section>

        {/* Account löschen */}
        <section className={`${styles.section} ${styles.dangerSection}`}>
          <h2 className={styles.sectionTitle}>
            <AlertTriangle size={20} aria-hidden="true" />
            Gefahrenzone
          </h2>
          <p className={styles.warningText}>
            Das Löschen deines Accounts kann nicht rückgängig gemacht werden. 
            Alle deine Daten werden entfernt.
          </p>
          <button
            type="button"
            className={styles.dangerButton}
            onClick={() => setShowDeleteDialog(true)}
          >
            <Trash2 size={18} aria-hidden="true" />
            Account löschen
          </button>
        </section>
      </div>

      {/* Lösch-Dialog */}
      {showDeleteDialog && (
        <div className={styles.modalOverlay} onClick={() => setShowDeleteDialog(false)}>
          <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
            <h3>Account wirklich löschen?</h3>
            <p>
              Diese Aktion kann nicht rückgängig gemacht werden. 
              Bitte gib <strong>LÖSCHEN</strong> ein, um zu bestätigen.
            </p>
            <input
              type="text"
              value={deleteConfirmation}
              onChange={(e) => setDeleteConfirmation(e.target.value)}
              placeholder="LÖSCHEN eingeben"
              className={styles.confirmInput}
            />
            <div className={styles.modalActions}>
              <button
                type="button"
                className={styles.secondaryButton}
                onClick={() => {
                  setShowDeleteDialog(false);
                  setDeleteConfirmation('');
                }}
              >
                Abbrechen
              </button>
              <button
                type="button"
                className={styles.dangerButton}
                onClick={handleDeleteAccount}
                disabled={deleteConfirmation !== 'LÖSCHEN'}
              >
                Account löschen
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
