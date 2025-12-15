import { useState, useEffect } from 'react';
import { Plus, Pencil, Trash2, X, Check, UserPlus, UserMinus } from 'lucide-react';
import { useAuth } from '../../../context';
import { faecherApi, lehrerApi } from '../../../services/api';
import type { Fach } from '../../../types';
import styles from '../Admin.module.css';

export const FaecherTab = () => {
  const { lehrer, updateLehrer } = useAuth();
  const [faecher, setFaecher] = useState<Fach[]>([]);
  const [search, setSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingFach, setEditingFach] = useState<Fach | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);

  // Meine zugeordneten Fächer
  const meineFachIds = new Set((lehrer?.faecher || []).map(f => f.id) || []);

  useEffect(() => {
    loadFaecher();
  }, []);

  const loadFaecher = async () => {
    setIsLoading(true);
    try {
      const data = await faecherApi.getAll();
      setFaecher(data);
    } catch (error) {
      console.error('Fehler beim Laden der Fächer:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredFaecher = faecher.filter(f =>
    f.bezeichnung.toLowerCase().includes(search.toLowerCase()) ||
    f.kurzbezeichnung.toLowerCase().includes(search.toLowerCase())
  );

  const handleAdd = () => {
    setEditingFach(null);
    setIsModalOpen(true);
  };

  const handleEdit = (fach: Fach) => {
    setEditingFach(fach);
    setIsModalOpen(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await faecherApi.delete(id);
      setFaecher(prev => prev.filter(f => f.id !== id));
    } catch (error) {
      console.error('Fehler beim Löschen:', error);
    }
    setDeleteConfirm(null);
  };

  const handleToggleZuordnung = async (fachId: string) => {
    if (!lehrer) return;
    
    try {
      const isZugeordnet = meineFachIds.has(fachId);
      if (isZugeordnet) {
        await faecherApi.removeFromLehrer(fachId, lehrer.id);
      } else {
        await faecherApi.assignToLehrer(fachId, lehrer.id);
      }
      // Lehrer-Daten neu laden
      const updatedLehrerDto = await lehrerApi.getById(lehrer.id);
      if (updatedLehrerDto && updateLehrer) {
        updateLehrer({
          ...lehrer,
          faecher: (updatedLehrerDto.faecher || []).map(f => ({
            id: f.id,
            bezeichnung: f.bezeichnung,
            kurzbezeichnung: f.kurzbezeichnung,
          })),
        });
      }
    } catch (error) {
      console.error('Fehler bei Zuordnung:', error);
    }
  };

  const handleSave = async (bezeichnung: string, kurzbezeichnung: string) => {
    try {
      if (editingFach) {
        // Bearbeiten
        const updated = await faecherApi.update(editingFach.id, { bezeichnung, kurzbezeichnung });
        setFaecher(prev => prev.map(f => f.id === editingFach.id ? updated : f));
      } else {
        // Neu erstellen
        const newFach = await faecherApi.create({ bezeichnung, kurzbezeichnung });
        setFaecher(prev => [...prev, newFach]);
      }
    } catch (error) {
      console.error('Fehler beim Speichern:', error);
    }
    setIsModalOpen(false);
    setEditingFach(null);
  };

  if (isLoading) {
    return <div className={styles.emptyState}>Laden...</div>;
  }

  return (
    <>
      <div className={styles.tabHeader}>
        <input
          type="text"
          placeholder="Fach suchen..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className={styles.searchInput}
        />
        <button className={styles.addButton} onClick={handleAdd}>
          <Plus size={18} aria-hidden="true" />
          <span>Neues Fach</span>
        </button>
      </div>

      {faecher.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Noch keine Fächer vorhanden</h3>
          <p>Erstelle ein neues Fach, um loszulegen.</p>
        </div>
      ) : filteredFaecher.length === 0 ? (
        <div className={styles.emptyState}>
          <h3>Keine Treffer</h3>
          <p>Kein Fach entspricht deiner Suche "{search}".</p>
        </div>
      ) : (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Bezeichnung</th>
              <th>Kürzel</th>
              <th>Meine Zuordnung</th>
              <th style={{ width: '100px' }}>Aktionen</th>
            </tr>
          </thead>
          <tbody>
            {filteredFaecher.map(fach => {
              const isZugeordnet = meineFachIds.has(fach.id);
              return (
                <tr key={fach.id}>
                  <td>{fach.bezeichnung}</td>
                  <td>
                    <span className={styles.badge}>{fach.kurzbezeichnung}</span>
                  </td>
                  <td>
                    {isZugeordnet ? (
                      <button
                        className={styles.zuordnungButtonActive}
                        onClick={() => handleToggleZuordnung(fach.id)}
                        title="Zuordnung entfernen"
                      >
                        <UserMinus size={16} />
                        <span>Zugeordnet</span>
                      </button>
                    ) : (
                      <button
                        className={styles.zuordnungButton}
                        onClick={() => handleToggleZuordnung(fach.id)}
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
                        onClick={() => handleEdit(fach)}
                        title="Bearbeiten"
                      >
                        <Pencil size={16} />
                      </button>
                      {deleteConfirm === fach.id ? (
                        <>
                          <button
                            className={`${styles.actionButton} ${styles.actionButtonDanger}`}
                            onClick={() => handleDelete(fach.id)}
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
                          onClick={() => setDeleteConfirm(fach.id)}
                          title="Löschen"
                        >
                          <Trash2 size={16} />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}

      {/* Modal */}
      {isModalOpen && (
        <FachModal
          fach={editingFach}
          onSave={handleSave}
          onClose={() => {
            setIsModalOpen(false);
            setEditingFach(null);
          }}
        />
      )}
    </>
  );
};

// Modal Komponente
interface FachModalProps {
  fach: Fach | null;
  onSave: (bezeichnung: string, kurzbezeichnung: string) => void;
  onClose: () => void;
}

const FachModal = ({ fach, onSave, onClose }: FachModalProps) => {
  const [bezeichnung, setBezeichnung] = useState(fach?.bezeichnung || '');
  const [kurzbezeichnung, setKurzbezeichnung] = useState(fach?.kurzbezeichnung || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!bezeichnung.trim() || !kurzbezeichnung.trim()) return;
    onSave(bezeichnung.trim(), kurzbezeichnung.trim().toUpperCase());
  };

  const isValid = bezeichnung.trim() && kurzbezeichnung.trim();

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>{fach ? 'Fach bearbeiten' : 'Neues Fach'}</h2>
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
                  placeholder="z.B. Mathematik"
                  autoFocus
                />
              </div>
              <div className={styles.formGroup}>
                <label htmlFor="kurzbezeichnung">Kurzbezeichnung (max. 5 Zeichen)</label>
                <input
                  id="kurzbezeichnung"
                  type="text"
                  value={kurzbezeichnung}
                  onChange={(e) => setKurzbezeichnung(e.target.value)}
                  placeholder="z.B. MA"
                  maxLength={5}
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
