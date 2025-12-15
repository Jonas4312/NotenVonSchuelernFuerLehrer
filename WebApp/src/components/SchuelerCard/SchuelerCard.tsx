import { User } from 'lucide-react';
import type { Schueler } from '../../types';
import { calculateAverage, formatNote, getNoteColorClass } from '../../utils/noteUtils';
import styles from './SchuelerCard.module.css';

interface SchuelerCardProps {
  schueler: Schueler;
  isSelected: boolean;
  onClick: () => void;
}

export const SchuelerCard = ({ schueler, isSelected, onClick }: SchuelerCardProps) => {
  const noten = schueler.noten || [];
  const durchschnitt = calculateAverage(noten);
  const notenAnzahl = noten.length;
  const vollstaendigerName = `${schueler.vorname} ${schueler.nachname}`;

  return (
    <button
      className={`${styles.card} ${isSelected ? styles.selected : ''}`}
      onClick={onClick}
      aria-pressed={isSelected}
      aria-label={`${vollstaendigerName}, ${notenAnzahl} Noten, Durchschnitt ${durchschnitt > 0 ? formatNote(durchschnitt) : 'keine Noten'}`}
    >
      <div className={styles.avatar}>
        {schueler.bildUrl ? (
          <img
            src={schueler.bildUrl}
            alt=""
            className={styles.avatarImage}
            loading="lazy"
          />
        ) : (
          <User size={40} aria-hidden="true" />
        )}
      </div>
      <div className={styles.info}>
        <span className={styles.vorname}>{schueler.vorname}</span>
        <span className={styles.nachname}>{schueler.nachname}</span>
        {durchschnitt > 0 && (
          <span className={`${styles.durchschnitt} ${styles[getNoteColorClass(durchschnitt)]}`}>
            Durchschnitt: {formatNote(durchschnitt)}
          </span>
        )}
      </div>
      <span className={styles.notenCount}>
        {notenAnzahl} {notenAnzahl === 1 ? 'Note' : 'Noten'}
      </span>
    </button>
  );
};
