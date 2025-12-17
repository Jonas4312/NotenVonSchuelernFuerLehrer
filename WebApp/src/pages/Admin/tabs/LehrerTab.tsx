import { useState, useEffect } from 'react';
import { Plus, X, User } from 'lucide-react';
import { useAuth } from '../../../context';
import { lehrerApi, faecherApi, klassenApi } from '../../../services/api';
import type { Fach, LehrerDto, KlasseDto } from '../../../types';
import styles from '../Admin.module.css';

export const LehrerTab = () => {
  const { lehrer: currentLehrer } = useAuth();
  const [search, setSearch] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [alleLehrer, setAlleLehrer] = useState<LehrerDto[]>([]);
  const [alleFaecher, setAlleFaecher] = useState<Fach[]>([]);
  const [alleKlassen, setAlleKlassen] = useState<KlasseDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const [lehrerData, faecherData, klassenData] = await Promise.all([
        lehrerApi.getAll(),
        faecherApi.getAll(),
        klassenApi.getAllAdmin(),
      ]);
      setAlleLehrer(lehrerData);
      setAlleFaecher(faecherData);
      setAlleKlassen(klassenData);
    } catch (error) {
      console.error('Fehler beim Laden:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredLehrer = alleLehrer.filter((l: LehrerDto) =>
    l.vorname.toLowerCase().includes(search.toLowerCase()) ||
    l.nachname.toLowerCase().includes(search.toLowerCase()) ||
    l.benutzername.toLowerCase().includes(search.toLowerCase())
  );

  const handleAdd = () => {
    setIsModalOpen(true);
  };

  const handleSave = async (vorname: string, nachname: string, benutzername: string, passwort: string, faecherIds: string[], klassenIds: string[]) => {
    try {
      // Neuen Lehrer erstellen
      const newLehrer = await lehrerApi.create({
        vorname,
        nachname,
        benutzername,
        passwort,
        bildByteArray: '',
      });
      
      // Fächer zuweisen
      for (const fachId of faecherIds) {
        await faecherApi.assignToLehrer(fachId, newLehrer.id);
      }
      
      // Klassen zuweisen
      for (const klasseId of klassenIds) {
        await klassenApi.assignToLehrer(klasseId, newLehrer.id);
      }
      
      // Daten neu laden
      await loadData();
    } catch (error) {
      console.error('Fehler beim Erstellen:', error);
    }
    setIsModalOpen(false);
  };

  if (isLoading) {
    return <div className={styles.emptyState}>Laden...</div>;
  }

  return (
    <>
      <div className={styles.tabHeader}>
        <input
          type="text"
          placeholder="Lehrer suchen..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className={styles.searchInput}
        />
        <button className={styles.addButton} onClick={handleAdd}>
          <Plus size={18} aria-hidden="true" />
          <span>Neuer Lehrer</span>
        </button>
      </div>

      <p style={{ marginBottom: 'var(--spacing-md)', color: 'var(--color-text-secondary)', fontSize: '0.9rem' }}>
        Hinweis: Du kannst nur neue Lehrer erstellen. Jeder Lehrer kann nur seinen eigenen Account in den Einstellungen bearbeiten oder löschen.
      </p>

      {alleLehrer.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Noch keine Lehrer vorhanden</h3>
          <p>Erstelle einen neuen Lehrer-Account, um loszulegen.</p>
        </div>
      ) : filteredLehrer.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Keine Treffer</h3>
          <p>Kein Lehrer entspricht deiner Suche "{search}".</p>
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th style={{ width: '50px' }}></th>
              <th>Name</th>
              <th>Benutzername</th>
              <th>Fächer</th>
              <th>Klassen</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {filteredLehrer.map(lehrer => (
              <tr key={lehrer.id}>
                <td>
                  <div className={styles.badge} style={{ 
                    width: '32px', 
                    height: '32px', 
                    borderRadius: '50%', 
                    padding: 0,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    overflow: 'hidden'
                  }}>
                    {lehrer.bildByteArray ? (
                      <img src={`data:image/jpeg;base64,${lehrer.bildByteArray}`} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                    ) : (
                      <User size={18} />
                    )}
                  </div>
                </td>
                <td>
                  <strong>{lehrer.vorname} {lehrer.nachname}</strong>
                </td>
                <td>
                  <code style={{ fontSize: '0.85rem', color: 'var(--color-text-secondary)' }}>
                    {lehrer.benutzername}
                  </code>
                </td>
                <td>
                  {(lehrer.faecher || []).length > 0 ? (
                    (lehrer.faecher || []).map((f: Fach) => (
                      <span key={f.id} className={`${styles.badge} ${styles.badgeSecondary}`} style={{ marginRight: '4px' }}>
                        {f.kurzbezeichnung}
                      </span>
                    ))
                  ) : (
                    <span style={{ color: 'var(--color-text-muted)', fontSize: '0.9rem' }}>
                      Keine Fächer
                    </span>
                  )}
                </td>
                <td>
                  {(lehrer.klassen || []).length > 0 ? (
                    (lehrer.klassen || []).map((k: KlasseDto) => (
                      <span key={k.id} className={`${styles.badge} ${styles.badgeSecondary}`} style={{ marginRight: '4px' }}>
                        {k.kurzbezeichnung}
                      </span>
                    ))
                  ) : (
                    <span style={{ color: 'var(--color-text-muted)', fontSize: '0.9rem' }}>
                      Keine Klassen
                    </span>
                  )}
                </td>
                <td>
                  {lehrer.id === currentLehrer?.id ? (
                    <span className={styles.badge} style={{ backgroundColor: '#dcfce7', color: '#16a34a' }}>
                      Du
                    </span>
                  ) : (
                    <span className={`${styles.badge} ${styles.badgeSecondary}`}>
                      Aktiv
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Modal */}
      {isModalOpen && (
        <LehrerModal
          alleFaecher={alleFaecher}
          alleKlassen={alleKlassen}
          onSave={handleSave}
          onClose={() => setIsModalOpen(false)}
        />
      )}
    </>
  );
};

// Modal Komponente
interface LehrerModalProps {
  alleFaecher: Fach[];
  alleKlassen: KlasseDto[];
  onSave: (vorname: string, nachname: string, benutzername: string, passwort: string, faecherIds: string[], klassenIds: string[]) => void;
  onClose: () => void;
}

const LehrerModal = ({ alleFaecher, alleKlassen, onSave, onClose }: LehrerModalProps) => {
  const [vorname, setVorname] = useState('');
  const [nachname, setNachname] = useState('');
  const [benutzername, setBenutzername] = useState('');
  const [passwort, setPasswort] = useState('');
  const [passwortWiederholen, setPasswortWiederholen] = useState('');
  const [selectedFaecher, setSelectedFaecher] = useState<Set<string>>(new Set());
  const [selectedKlassen, setSelectedKlassen] = useState<Set<string>>(new Set());
  const [error, setError] = useState('');

  const handleToggleFach = (fachId: string) => {
    setSelectedFaecher(prev => {
      const newSet = new Set(prev);
      if (newSet.has(fachId)) {
        newSet.delete(fachId);
      } else {
        newSet.add(fachId);
      }
      return newSet;
    });
  };

  const handleToggleKlasse = (klasseId: string) => {
    setSelectedKlassen(prev => {
      const newSet = new Set(prev);
      if (newSet.has(klasseId)) {
        newSet.delete(klasseId);
      } else {
        newSet.add(klasseId);
      }
      return newSet;
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!vorname.trim() || !nachname.trim() || !benutzername.trim() || !passwort) {
      setError('Bitte alle Pflichtfelder ausfüllen');
      return;
    }

    if (benutzername.length < 3) {
      setError('Benutzername muss mindestens 3 Zeichen haben');
      return;
    }

    if (passwort.length < 6) {
      setError('Passwort muss mindestens 6 Zeichen haben');
      return;
    }

    if (passwort !== passwortWiederholen) {
      setError('Passwörter stimmen nicht überein');
      return;
    }

    onSave(vorname.trim(), nachname.trim(), benutzername.trim(), passwort, Array.from(selectedFaecher), Array.from(selectedKlassen));
  };

  const isValid = vorname.trim() && nachname.trim() && benutzername.trim() && passwort && passwort === passwortWiederholen;

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>Neuer Lehrer</h2>
          <button className={styles.closeButton} onClick={onClose}>
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className={styles.modalBody}>
            <div className={styles.form}>
              <div className={styles.formGroup}>
                <label htmlFor="vorname">Vorname *</label>
                <input
                  id="vorname"
                  type="text"
                  value={vorname}
                  onChange={(e) => setVorname(e.target.value)}
                  placeholder="Vorname"
                  autoFocus
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="nachname">Nachname *</label>
                <input
                  id="nachname"
                  type="text"
                  value={nachname}
                  onChange={(e) => setNachname(e.target.value)}
                  placeholder="Nachname"
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="benutzername">Benutzername *</label>
                <input
                  id="benutzername"
                  type="text"
                  value={benutzername}
                  onChange={(e) => setBenutzername(e.target.value)}
                  placeholder="Benutzername (min. 3 Zeichen)"
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="passwort">Passwort *</label>
                <input
                  id="passwort"
                  type="password"
                  value={passwort}
                  onChange={(e) => setPasswort(e.target.value)}
                  placeholder="Passwort (min. 6 Zeichen)"
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="passwortWiederholen">Passwort wiederholen *</label>
                <input
                  id="passwortWiederholen"
                  type="password"
                  value={passwortWiederholen}
                  onChange={(e) => setPasswortWiederholen(e.target.value)}
                  placeholder="Passwort wiederholen"
                />
              </div>

              {error && (
                <p style={{ color: '#dc2626', fontSize: '0.9rem', margin: 0 }}>
                  {error}
                </p>
              )}
              
              <div className={styles.assignmentSection}>
                <h3>Fächer zuordnen (optional)</h3>
                <div className={styles.checkboxGroup}>
                  {alleFaecher.map(fach => (
                    <label key={fach.id} className={styles.checkboxLabel}>
                      <input
                        type="checkbox"
                        checked={selectedFaecher.has(fach.id)}
                        onChange={() => handleToggleFach(fach.id)}
                      />
                      {fach.bezeichnung}
                    </label>
                  ))}
                </div>
              </div>

              <div className={styles.assignmentSection}>
                <h3>Klassen zuordnen (optional)</h3>
                <div className={styles.checkboxGroup}>
                  {alleKlassen.map(klasse => (
                    <label key={klasse.id} className={styles.checkboxLabel}>
                      <input
                        type="checkbox"
                        checked={selectedKlassen.has(klasse.id)}
                        onChange={() => handleToggleKlasse(klasse.id)}
                      />
                      {klasse.bezeichnung} ({klasse.kurzbezeichnung})
                    </label>
                  ))}
                </div>
              </div>
            </div>
          </div>
          <div className={styles.modalFooter}>
            <button type="button" className={styles.cancelButton} onClick={onClose}>
              Abbrechen
            </button>
            <button type="submit" className={styles.saveButton} disabled={!isValid}>
              Lehrer erstellen
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
