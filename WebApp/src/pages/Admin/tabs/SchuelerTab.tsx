import { useState, useEffect, useRef, type ChangeEvent } from 'react';
import { Plus, Pencil, Trash2, X, Check, User, Upload } from 'lucide-react';
import { klassenApi, schuelerApi } from '../../../services/api';
import type { Klasse, SchuelerDto } from '../../../types';
import styles from '../Admin.module.css';

export const SchuelerTab = () => {
  const [klassen, setKlassen] = useState<Klasse[]>([]);
  const [schuelerList, setSchuelerList] = useState<SchuelerDto[]>([]);
  const [selectedKlasseId, setSelectedKlasseId] = useState<string>('alle');
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingSchueler, setEditingSchueler] = useState<SchuelerDto | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);

  useEffect(() => {
    loadKlassen();
  }, []);

  useEffect(() => {
    loadSchueler(selectedKlasseId);
  }, [selectedKlasseId]);

  const loadKlassen = async () => {
    setIsLoading(true);
    try {
      const klassenData = await klassenApi.getAll();
      setKlassen(klassenData);
    } catch (error) {
      console.error('Fehler beim Laden der Klassen:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const loadSchueler = async (klasseId: string) => {
    try {
      if (klasseId === 'alle') {
        const schuelerData = await schuelerApi.getAll();
        setSchuelerList(schuelerData);
      } else {
        const schuelerData = await klassenApi.getSchueler(klasseId);
        setSchuelerList(schuelerData);
      }
    } catch (error) {
      console.error('Fehler beim Laden der Schüler:', error);
      setSchuelerList([]);
    }
  };

  const selectedKlasse = klassen.find(k => k.id === selectedKlasseId);
  const alleSchueler = schuelerList;

  const filteredSchueler = alleSchueler.filter(s =>
    s.vorname.toLowerCase().includes(search.toLowerCase()) ||
    s.nachname.toLowerCase().includes(search.toLowerCase())
  );

  const handleAdd = () => {
    setEditingSchueler(null);
    setIsModalOpen(true);
  };

  const handleEdit = (schueler: SchuelerDto) => {
    setEditingSchueler(schueler);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await schuelerApi.delete(id);
      // Schüler neu laden
      await loadSchueler(selectedKlasseId);
    } catch (error) {
      console.error('Fehler beim Löschen:', error);
    }
    setDeleteConfirm(null);
  };

  const handleSave = async (vorname: string, nachname: string, klasseId: string, bildUrl?: string) => {
    try {
      if (editingSchueler) {
        // Bearbeiten
        await schuelerApi.update(editingSchueler.id, {
          vorname,
          nachname,
          klasseId,
          bildByteArray: bildUrl,
        });
      } else {
        // Neu erstellen
        await schuelerApi.create({
          vorname,
          nachname,
          klasseId,
          bildByteArray: bildUrl || `https://api.dicebear.com/7.x/avataaars/svg?seed=${vorname}${Date.now()}`,
        });
      }
      
      // Schüler neu laden
      await loadSchueler(selectedKlasseId);
    } catch (error) {
      console.error('Fehler beim Speichern:', error);
    }
    setIsModalOpen(false);
    setEditingSchueler(null);
  };

  if (isLoading) {
    return <div className={styles.emptyState}>Laden...</div>;
  }

  return (
    <>
      <div className={styles.tabHeader}>
        <select
          value={selectedKlasseId}
          onChange={(e) => setSelectedKlasseId(e.target.value)}
          className={styles.searchInput}
          style={{ maxWidth: '200px' }}
        >
          <option value="alle">Alle Klassen</option>
          {klassen.map(k => (
            <option key={k.id} value={k.id}>
              {k.bezeichnung}
            </option>
          ))}
        </select>
        <input
          type="text"
          placeholder="Schüler suchen..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className={styles.searchInput}
        />
        <button className={styles.addButton} onClick={handleAdd} disabled={klassen.length === 0}>
          <Plus size={18} aria-hidden="true" />
          <span>Neuer Schüler</span>
        </button>
      </div>

      {alleSchueler.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Keine Schüler {selectedKlasseId !== 'alle' ? 'in dieser Klasse' : 'vorhanden'}</h3>
          <p>{klassen.length === 0 
            ? 'Erstelle zuerst eine Klasse im Tab "Klassen", bevor du Schüler hinzufügen kannst.'
            : `Erstelle einen neuen Schüler${selectedKlasse ? ` für ${selectedKlasse.bezeichnung}` : ''}.`
          }</p>
        </div>
      ) : filteredSchueler.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Keine Treffer</h3>
          <p>Kein Schüler entspricht deiner Suche "{search}".</p>
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th style={{ width: '50px' }}></th>
              <th>Vorname</th>
              <th>Nachname</th>
              {selectedKlasseId === 'alle' && <th>Klasse</th>}
              <th>Noten</th>
              <th style={{ width: '100px' }}>Aktionen</th>
            </tr>
          </thead>
          <tbody>
            {filteredSchueler.map(schueler => (
              <tr key={schueler.id}>
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
                    {schueler.bildByteArray ? (
                      <img src={`data:image/png;base64,${schueler.bildByteArray}`} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                    ) : (
                      <User size={18} />
                    )}
                  </div>
                </td>
                <td>{schueler.vorname}</td>
                <td>{schueler.nachname}</td>
                {selectedKlasseId === 'alle' && (
                  <td>
                    <span className={styles.badge}>
                      {schueler.klasseBezeichnung || '–'}
                    </span>
                  </td>
                )}
                <td>
                  <span className={`${styles.badge} ${styles.badgeSecondary}`}>
                    {schueler.anzahlNoten ?? 0} Noten
                  </span>
                </td>
                <td>
                  <div className={styles.actions}>
                    <button
                      className={styles.actionButton}
                      onClick={() => handleEdit(schueler)}
                      title="Bearbeiten"
                    >
                      <Pencil size={16} />
                    </button>
                    {deleteConfirm === schueler.id ? (
                      <>
                        <button
                          className={`${styles.actionButton} ${styles.actionButtonDanger}`}
                          onClick={() => handleDelete(schueler.id)}
                          title="Löschen bestätigen"
                        >
                          <Check size={16} />
                        </button>
                        <button
                          className={styles.actionButton}
                          onClick={() => setDeleteConfirm(null)}
                          title="Abbrechen"
                        >
                          <X size={16} />
                        </button>
                      </>
                    ) : (
                      <button
                        className={`${styles.actionButton} ${styles.actionButtonDanger}`}
                        onClick={() => setDeleteConfirm(schueler.id)}
                        title="Löschen"
                      >
                        <Trash2 size={16} />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* Modal */}
      {isModalOpen && (
        <SchuelerModal
          schueler={editingSchueler}
          klassen={klassen}
          defaultKlasseId={selectedKlasseId}
          onSave={handleSave}
          onClose={() => {
            setIsModalOpen(false);
            setEditingSchueler(null);
          }}
        />
      )}
    </>
  );
};

// Modal Komponente
interface SchuelerModalProps {
  schueler: SchuelerDto | null;
  klassen: Klasse[];
  defaultKlasseId: string;
  onSave: (vorname: string, nachname: string, klasseId: string, bildUrl?: string) => void;
  onClose: () => void;
}

const SchuelerModal = ({ schueler, klassen, defaultKlasseId, onSave, onClose }: SchuelerModalProps) => {
  const [vorname, setVorname] = useState(schueler?.vorname || '');
  const [nachname, setNachname] = useState(schueler?.nachname || '');
  // Wenn defaultKlasseId "alle" ist, wähle die erste Klasse aus der Liste
  const initialKlasseId = defaultKlasseId === 'alle' 
    ? (schueler?.klasseId || klassen[0]?.id || '') 
    : defaultKlasseId;
  const [klasseId, setKlasseId] = useState(initialKlasseId);
  const [bildPreview, setBildPreview] = useState<string | null>(
    schueler?.bildByteArray ? `data:image/png;base64,${schueler.bildByteArray}` : null
  );
  const [newBildUrl, setNewBildUrl] = useState<string | undefined>(undefined);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleBildSelect = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onloadend = () => {
      const result = reader.result as string;
      setBildPreview(result);
      setNewBildUrl(result);
    };
    reader.readAsDataURL(file);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!vorname.trim() || !nachname.trim() || !klasseId) return;
    onSave(vorname.trim(), nachname.trim(), klasseId, newBildUrl);
  };

  const isValid = vorname.trim() && nachname.trim() && klasseId;

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>{schueler ? 'Schüler bearbeiten' : 'Neuer Schüler'}</h2>
          <button className={styles.closeButton} onClick={onClose}>
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className={styles.modalBody}>
            <div className={styles.form}>
              {/* Profilbild */}
              <div className={styles.formGroup}>
                <label>Profilbild</label>
                <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-md)' }}>
                  <div style={{
                    width: '64px',
                    height: '64px',
                    borderRadius: '50%',
                    overflow: 'hidden',
                    backgroundColor: 'var(--color-bg)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    border: '1px solid var(--border-color)'
                  }}>
                    {bildPreview ? (
                      <img src={bildPreview} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                    ) : (
                      <User size={32} color="var(--color-text-muted)" />
                    )}
                  </div>
                  <div>
                    <input
                      ref={fileInputRef}
                      type="file"
                      accept="image/*"
                      onChange={handleBildSelect}
                      style={{ display: 'none' }}
                      aria-label="Bild auswählen"
                    />
                    <button
                      type="button"
                      className={styles.cancelButton}
                      onClick={() => fileInputRef.current?.click()}
                      style={{ display: 'flex', alignItems: 'center', gap: 'var(--spacing-xs)' }}
                    >
                      <Upload size={16} />
                      Bild ändern
                    </button>
                  </div>
                </div>
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="vorname">Vorname</label>
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
                <label htmlFor="nachname">Nachname</label>
                <input
                  id="nachname"
                  type="text"
                  value={nachname}
                  onChange={(e) => setNachname(e.target.value)}
                  placeholder="Nachname"
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="klasse">Klasse</label>
                <select
                  id="klasse"
                  value={klasseId}
                  onChange={(e) => setKlasseId(e.target.value)}
                >
                  {klassen.map(k => (
                    <option key={k.id} value={k.id}>
                      {k.bezeichnung}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>
          <div className={styles.modalFooter}>
            <button type="button" className={styles.cancelButton} onClick={onClose}>
              Abbrechen
            </button>
            <button type="submit" className={styles.saveButton} disabled={!isValid}>
              Speichern
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
