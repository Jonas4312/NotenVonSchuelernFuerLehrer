import { Plus, User, Eye, Settings } from 'lucide-react';
import type { Schueler, Note, Fach } from '../../types';
import { calculateAverage, formatNote, getNoteColorClass, groupNotenByFach } from '../../utils/noteUtils';
import { NotenTable } from '../NotenTable';
import styles from './SchuelerDetail.module.css';

interface SchuelerDetailProps {
  schueler: Schueler;
  faecher: Fach[];
  onAddNote?: () => void;
  onEditNote?: (note: Note) => void;
  onDeleteNote?: (note: Note) => void;
  onEditSchueler?: (schueler: Schueler) => void;
  selectedFachName?: string;
  isReadOnly?: boolean;
}

export const SchuelerDetail = ({
  schueler,
  faecher,
  onAddNote,
  onEditNote,
  onDeleteNote,
  onEditSchueler,
  selectedFachName,
  isReadOnly = false,
}: SchuelerDetailProps) => {
  const noten = schueler.noten || [];
  const durchschnitt = calculateAverage(noten);
  const notenByFach = groupNotenByFach(noten);
  const vollstaendigerName = `${schueler.vorname} ${schueler.nachname}`;

  // Titel basierend auf Fachauswahl
  const notenTitel = selectedFachName 
    ? `Noten in ${selectedFachName}`
    : 'Alle Noten';

  return (
    <div className={styles.container}>
      {/* Read-Only Badge */}
      {isReadOnly && (
        <div className={styles.readOnlyBadge}>
          <Eye size={16} aria-hidden="true" />
          <span>Nur Lesezugriff</span>
        </div>
      )}

      {/* Header mit Schülerinfo */}
      <div className={styles.header}>
        <div className={styles.profileSection}>
          <div className={styles.avatar}>
            {schueler.bildUrl ? (
              <img
                src={schueler.bildUrl}
                alt=""
                className={styles.avatarImage}
              />
            ) : (
              <User size={48} aria-hidden="true" />
            )}
          </div>
          <div className={styles.info}>
            <h2 className={styles.name}>
              {vollstaendigerName}
              {onEditSchueler && (
                <button
                  className={styles.editSchuelerButton}
                  onClick={() => onEditSchueler(schueler)}
                  title="Schüler bearbeiten"
                  aria-label="Schüler bearbeiten"
                >
                  <Settings size={18} aria-hidden="true" />
                </button>
              )}
            </h2>
            {selectedFachName && (
              <p className={styles.fachHinweis}>
                Anzeige: <strong>{selectedFachName}</strong>
              </p>
            )}
            <p className={styles.durchschnittLabel}>
              {selectedFachName ? 'Fachdurchschnitt' : 'Gesamtdurchschnitt'}:{' '}
              {durchschnitt > 0 ? (
                <span className={`${styles.durchschnittValue} ${styles[getNoteColorClass(durchschnitt)]}`}>
                  {formatNote(durchschnitt)}
                </span>
              ) : (
                <span className={styles.noDurchschnitt}>Keine Noten</span>
              )}
            </p>
          </div>
        </div>
        {!isReadOnly && onAddNote && (
          <button
            className={styles.addButton}
            onClick={onAddNote}
            aria-label={selectedFachName ? `Neue ${selectedFachName}-Note hinzufügen` : 'Neue Note hinzufügen'}
          >
            <Plus size={20} aria-hidden="true" />
            <span>Note hinzufügen</span>
          </button>
        )}
      </div>

      {/* Fachdurchschnitte - nur in Übersicht anzeigen */}
      {!selectedFachName && notenByFach.size > 0 && (
        <section className={styles.section} aria-labelledby="fachdurchschnitte-title">
          <h3 id="fachdurchschnitte-title" className={styles.sectionTitle}>
            Fachdurchschnitte
          </h3>
          <div className={styles.fachBadges}>
            {Array.from(notenByFach.entries()).map(([fachId, data]) => {
              const fach = faecher.find((f) => f.id === fachId) || 
                          noten.find((n) => n.fachId === fachId)?.fach;
              return (
                <div key={fachId} className={styles.fachBadge}>
                  <span className={styles.fachName}>{fach?.bezeichnung || 'Unbekannt'}:</span>
                  <span className={`${styles.fachDurchschnitt} ${styles[getNoteColorClass(data.durchschnitt)]}`}>
                    {formatNote(data.durchschnitt)}
                  </span>
                  <span className={styles.fachAnzahl}>({data.noten.length})</span>
                </div>
              );
            })}
          </div>
        </section>
      )}

      {/* Notentabelle */}
      <section className={styles.section} aria-labelledby="alle-noten-title">
        <h3 id="alle-noten-title" className={styles.sectionTitle}>
          {notenTitel}
        </h3>
        <NotenTable
          noten={noten}
          faecher={faecher}
          onEdit={onEditNote}
          onDelete={onDeleteNote}
        />
      </section>
    </div>
  );
};
