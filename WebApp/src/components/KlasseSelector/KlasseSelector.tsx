import { BookOpen, ChevronDown } from 'lucide-react';
import type { Klasse } from '../../types';
import styles from './KlasseSelector.module.css';

interface KlasseSelectorProps {
  klassen: Klasse[];
  selectedKlasse: Klasse | null;
  onSelect: (klasse: Klasse) => void;
  isLoading?: boolean;
}

export const KlasseSelector = ({
  klassen,
  selectedKlasse,
  onSelect,
  isLoading = false,
}: KlasseSelectorProps) => {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const klasse = klassen.find((k) => k.id === e.target.value);
    if (klasse) {
      onSelect(klasse);
    }
  };

  return (
    <div className={styles.container}>
      <h2 className={styles.label}>Klasse</h2>
      <div className={styles.selectWrapper}>
        <BookOpen className={styles.icon} size={20} aria-hidden="true" />
        <select
          className={styles.select}
          value={selectedKlasse?.id || ''}
          onChange={handleChange}
          disabled={isLoading}
          aria-label="Klasse auswählen"
        >
          {!selectedKlasse && (
            <option value="" disabled>
              Klasse auswählen...
            </option>
          )}
          {klassen.map((klasse) => (
            <option key={klasse.id} value={klasse.id}>
              {klasse.bezeichnung} ({klasse.anzahlSchueler ?? 0} Schüler)
            </option>
          ))}
        </select>
        <ChevronDown className={styles.chevron} size={20} aria-hidden="true" />
      </div>
    </div>
  );
};
