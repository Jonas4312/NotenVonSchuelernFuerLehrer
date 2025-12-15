import { useState, useEffect } from 'react';
import { Plus, Pencil, Trash2, X, Check, UserPlus, UserMinus } from 'lucide-react';
import { klassenApi } from '../../../services/api';
import { useAuth } from '../../../context';
import type { Klasse } from '../../../types';
import styles from '../Admin.module.css';

export const KlassenTab = () => {
  const { lehrer: currentLehrer } = useAuth();
  const [klassen, setKlassen] = useState<Klasse[]>([]);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingKlasse, setEditingKlasse] = useState<Klasse | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);
  const [assignedKlassen, setAssignedKlassen] = useState<Set<string>>(new Set());

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      // Lade alle Klassen für Admin-Ansicht und separat die dem Lehrer zugeordneten
      const [alleKlassen, meineKlassen] = await Promise.all([
        klassenApi.getAllAdmin(),
        klassenApi.getAll(),
      ]);
      setKlassen(alleKlassen);
      // Merke welche Klassen dem aktuellen Lehrer zugeordnet sind
      setAssignedKlassen(new Set(meineKlassen.map(k => k.id)));
    } catch (error) {
      console.error('Fehler beim Laden:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredKlassen = klassen.filter(k =>
    k.bezeichnung.toLowerCase().includes(search.toLowerCase()) ||
    k.kurzbezeichnung.toLowerCase().includes(search.toLowerCase())
  );

  const handleAdd = () => {
    setEditingKlasse(null);
    setIsModalOpen(true);
  };

  const handleEdit = (klasse: Klasse) => {
    setEditingKlasse(klasse);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await klassenApi.delete(id);
      setKlassen(prev => prev.filter(k => k.id !== id));
    } catch (error) {
      console.error('Fehler beim Löschen:', error);
    }
    setDeleteConfirm(null);
  };

  const handleToggleAssignment = async (klasseId: string) => {
    if (!currentLehrer) return;
    
    try {
      if (assignedKlassen.has(klasseId)) {
        await klassenApi.removeFromLehrer(klasseId, currentLehrer.id);
        setAssignedKlassen(prev => {
          const newSet = new Set(prev);
          newSet.delete(klasseId);
          return newSet;
        });
      } else {
        await klassenApi.assignToLehrer(klasseId, currentLehrer.id);
        setAssignedKlassen(prev => new Set(prev).add(klasseId));
      }
    } catch (error) {
      console.error('Fehler bei Zuordnung:', error);
    }
  };

  const handleSave = async (bezeichnung: string, kurzbezeichnung: string) => {
    try {
      if (editingKlasse) {
        // Bearbeiten
        const updated = await klassenApi.update(editingKlasse.id, { bezeichnung, kurzbezeichnung });
        setKlassen(prev => prev.map(k => k.id === updated.id ? { ...k, ...updated } : k));
      } else {
        // Neu erstellen und automatisch zuordnen
        const newKlasse = await klassenApi.create({ bezeichnung, kurzbezeichnung });
        if (currentLehrer) {
          await klassenApi.assignToLehrer(newKlasse.id, currentLehrer.id);
          setAssignedKlassen(prev => new Set(prev).add(newKlasse.id));
        }
        setKlassen(prev => [...prev, newKlasse]);
      }
    } catch (error) {
      console.error('Fehler beim Speichern:', error);
    }
    setIsModalOpen(false);
    setEditingKlasse(null);
  };

  if (isLoading) {
    return <div className={styles.emptyState}>Laden...</div>;
  }

  return (
    <>
      <div className={styles.tabHeader}>
        <input
          type="text"
          placeholder="Klasse suchen..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className={styles.searchInput}
        />
        <button className={styles.addButton} onClick={handleAdd}>
          <Plus size={18} aria-hidden="true" />
          <span>Neue Klasse</span>
        </button>
      </div>

      {klassen.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Noch keine Klassen vorhanden</h3>
          <p>Erstelle eine neue Klasse, um loszulegen.</p>
        </div>
      ) : filteredKlassen.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Keine Treffer</h3>
          <p>Keine Klasse entspricht deiner Suche "{search}".</p>
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Bezeichnung</th>
              <th>Kürzel</th>
              <th>Schüler</th>
              <th>Meine Zuordnung</th>
              <th style={{ width: '100px' }}>Aktionen</th>
            </tr>
          </thead>
          <tbody>
            {filteredKlassen.map(klasse => (
              <tr key={klasse.id}>
                <td>{klasse.bezeichnung}</td>
                <td>
                  <span className={styles.badge}>{klasse.kurzbezeichnung}</span>
                </td>
                <td>{klasse.anzahlSchueler ?? 0}</td>
                <td>
                  {assignedKlassen.has(klasse.id) ? (
                    <button
                      className={styles.zuordnungButtonActive}
                      onClick={() => handleToggleAssignment(klasse.id)}
                      title="Zuordnung entfernen"
                    >
                      <UserMinus size={16} />
                      <span>Zugeordnet</span>
                    </button>
                  ) : (
                    <button
                      className={styles.zuordnungButton}
                      onClick={() => handleToggleAssignment(klasse.id)}
                      title="Mir zuordnen"
                    >
                      <UserPlus size={16} />
                      <span>Zuordnen</span>
                    </button>
                  )}
                </td>
                <td>
                  <div className={styles.actions}>
                    <button
                      className={styles.actionButton}
                      onClick={() => handleEdit(klasse)}
                      title="Bearbeiten"
                    >
                      <Pencil size={16} />
                    </button>
                    {deleteConfirm === klasse.id ? (
                      <>
                        <button
                          className={`${styles.actionButton} ${styles.actionButtonDanger}`}
                          onClick={() => handleDelete(klasse.id)}
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
                        onClick={() => setDeleteConfirm(klasse.id)}
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
        <KlasseModal
          klasse={editingKlasse}
          onSave={handleSave}
          onClose={() => {
            setIsModalOpen(false);
            setEditingKlasse(null);
          }}
        />
      )}
    </>
  );
};

// Modal Komponente - vereinfacht ohne Fächer-Zuordnung
interface KlasseModalProps {
  klasse: Klasse | null;
  onSave: (bezeichnung: string, kurzbezeichnung: string) => void;
  onClose: () => void;
}

const KlasseModal = ({ klasse, onSave, onClose }: KlasseModalProps) => {
  const [bezeichnung, setBezeichnung] = useState(klasse?.bezeichnung || '');
  const [kurzbezeichnung, setKurzbezeichnung] = useState(klasse?.kurzbezeichnung || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!bezeichnung.trim() || !kurzbezeichnung.trim()) return;
    onSave(bezeichnung.trim(), kurzbezeichnung.trim());
  };

  const isValid = bezeichnung.trim() && kurzbezeichnung.trim();

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>{klasse ? 'Klasse bearbeiten' : 'Neue Klasse'}</h2>
          <button className={styles.closeButton} onClick={onClose}>
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className={styles.modalBody}>
            <div className={styles.form}>
              <div className={styles.formGroup}>
                <label htmlFor="bezeichnung">Bezeichnung</label>
                <input
                  id="bezeichnung"
                  type="text"
                  value={bezeichnung}
                  onChange={(e) => setBezeichnung(e.target.value)}
                  placeholder="z.B. Klasse 10A"
                  autoFocus
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="kurzbezeichnung">Kurzbezeichnung</label>
                <input
                  id="kurzbezeichnung"
                  type="text"
                  value={kurzbezeichnung}
                  onChange={(e) => setKurzbezeichnung(e.target.value)}
                  placeholder="z.B. 10A"
                  maxLength={10}
                />
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
