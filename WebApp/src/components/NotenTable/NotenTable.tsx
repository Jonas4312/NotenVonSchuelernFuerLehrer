import { Pencil, Trash2, MessageSquare } from 'lucide-react';
import type { Note, Fach } from '../../types';
import { formatNote, formatDate, getNoteColorClass, sortNotenByDate } from '../../utils/noteUtils';
import styles from './NotenTable.module.css';

interface NotenTableProps {
  noten: Note[];
  faecher: Fach[];
  onEdit?: (note: Note) => void;
  onDelete?: (note: Note) => void;
}

export const NotenTable = ({ noten, faecher, onEdit, onDelete }: NotenTableProps) => {
  const sortedNoten = sortNotenByDate(noten);
  const isReadOnly = !onEdit && !onDelete;

  if (noten.length === 0) {
    return (
      <div className={styles.empty}>
        <p>Noch keine Noten vorhanden.</p>
        {!isReadOnly && (
          <p className={styles.emptyHint}>Klicke auf "Note hinzufügen" um die erste Note einzutragen.</p>
        )}
      </div>
    );
  }

  const getFachName = (note: Note): string => {
    if (note.fach) return note.fach.bezeichnung;
    const fach = faecher.find((f) => f.id === note.fachId);
    return fach?.bezeichnung || 'Unbekannt';
  };

  return (
    <div className={styles.tableContainer}>
      <table className={styles.table} role="table">
        <thead>
          <tr>
            <th scope="col">Fach</th>
            <th scope="col">Note</th>
            <th scope="col">Erstellt</th>
            <th scope="col" className={styles.bearbeitetHeader}>Bearbeitet</th>
            <th scope="col">Notiz</th>
            {!isReadOnly && (
              <th scope="col">
                <span className="visually-hidden">Aktionen</span>
              </th>
            )}
          </tr>
        </thead>
        <tbody>
          {sortedNoten.map((note) => (
            <tr key={note.id}>
              <td>{getFachName(note)}</td>
              <td>
                <span className={`${styles.noteBadge} ${styles[getNoteColorClass(note.wert)]}`}>
                  {formatNote(note.wert)}
                </span>
              </td>
              <td>{formatDate(note.erstelltAm)}</td>
              <td className={styles.bearbeitetCell}>
                {note.angepasstAm !== note.erstelltAm ? formatDate(note.angepasstAm) : '—'}
              </td>
              <td className={styles.notizCell}>
                {note.notiz ? (
                  <span className={styles.notiz} title={note.notiz}>
                    <MessageSquare size={14} aria-hidden="true" />
                    <span className={styles.notizText}>{note.notiz}</span>
                  </span>
                ) : (
                  <span className={styles.noNotiz}>—</span>
                )}
              </td>
              {!isReadOnly && (
                <td className={styles.actions}>
                  {onEdit && (
                    <button
                      className={styles.actionButton}
                      onClick={() => onEdit(note)}
                      aria-label={`Note ${formatNote(note.wert)} in ${getFachName(note)} bearbeiten`}
                      title="Bearbeiten"
                    >
                      <Pencil size={18} aria-hidden="true" />
                    </button>
                  )}
                  {onDelete && (
                    <button
                      className={`${styles.actionButton} ${styles.deleteButton}`}
                      onClick={() => onDelete(note)}
                      aria-label={`Note ${formatNote(note.wert)} in ${getFachName(note)} löschen`}
                      title="Löschen"
                    >
                      <Trash2 size={18} aria-hidden="true" />
                    </button>
                  )}
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
