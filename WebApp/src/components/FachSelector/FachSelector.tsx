import { BookMarked, ChevronDown } from 'lucide-react';
import type { Fach } from '../../types';
import styles from './FachSelector.module.css';

interface FachSelectorProps {
  faecher: Fach[];
  selectedFach: Fach | null;
  onSelect: (fach: Fach | null) => void;
  showAllOption?: boolean;
  disabled?: boolean;
}

export const FachSelector = ({
  faecher,
  selectedFach,
  onSelect,
  showAllOption = false,
  disabled = false,
}: FachSelectorProps) => {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    if (e.target.value === '') {
      onSelect(null);
    } else {
      const fach = faecher.find((f) => f.id === e.target.value);
      if (fach) {
        onSelect(fach);
      }
    }
  };

  return (
    <div className={styles.container}>
      <h2 className={styles.label}>Fach</h2>
      <div className={styles.selectWrapper}>
        <BookMarked className={styles.icon} size={20} aria-hidden="true" />
        <select
          className={styles.select}
          value={selectedFach?.id || ''}
          onChange={handleChange}
          disabled={disabled || faecher.length === 0}
          aria-label="Fach auswählen"
        >
          {showAllOption ? (
            <option value="">Alle Fächer (Übersicht)</option>
          ) : (
            <option value="" disabled>
              Fach auswählen...
            </option>
          )}
          {faecher.map((fach) => (
            <option key={fach.id} value={fach.id}>
              {fach.bezeichnung}
            </option>
          ))}
        </select>
        <ChevronDown className={styles.chevron} size={20} aria-hidden="true" />
      </div>
      {faecher.length === 0 && (
        <p className={styles.hint}>
          Wählen Sie zuerst eine Klasse aus.
        </p>
      )}
    </div>
  );
};
