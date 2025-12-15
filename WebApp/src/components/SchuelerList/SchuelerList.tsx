import { useState, useMemo } from 'react';
import { Search } from 'lucide-react';
import type { Schueler } from '../../types';
import { SchuelerCard } from '../SchuelerCard';
import styles from './SchuelerList.module.css';

interface SchuelerListProps {
  schueler: Schueler[];
  selectedSchueler: Schueler | null;
  onSelect: (schueler: Schueler) => void;
  isLoading?: boolean;
}

export const SchuelerList = ({
  schueler,
  selectedSchueler,
  onSelect,
  isLoading = false,
}: SchuelerListProps) => {
  const [suchbegriff, setSuchbegriff] = useState('');

  const gefilterteSchueler = useMemo(() => {
    if (!suchbegriff.trim()) return schueler;
    const lower = suchbegriff.toLowerCase();
    return schueler.filter(
      (s) =>
        s.vorname.toLowerCase().includes(lower) ||
        s.nachname.toLowerCase().includes(lower)
    );
  }, [schueler, suchbegriff]);

  if (isLoading) {
    return (
      <div className={styles.container}>
        <h2 className={styles.title}>Schüler</h2>
        <div className={styles.loading} role="status" aria-label="Lädt Schüler">
          <div className={styles.spinner} />
          <span>Lädt...</span>
        </div>
      </div>
    );
  }

  if (schueler.length === 0) {
    return (
      <div className={styles.container}>
        <h2 className={styles.title}>Schüler</h2>
        <p className={styles.empty}>Keine Schüler in dieser Klasse.</p>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Schüler</h2>
      <div className={styles.searchContainer}>
        <Search size={16} className={styles.searchIcon} aria-hidden="true" />
        <input
          type="text"
          className={styles.searchInput}
          placeholder="Schüler suchen..."
          value={suchbegriff}
          onChange={(e) => setSuchbegriff(e.target.value)}
          aria-label="Schüler suchen"
        />
      </div>
      {gefilterteSchueler.length === 0 ? (
        <p className={styles.empty}>Keine Schüler gefunden.</p>
      ) : (
        <ul className={styles.list} role="listbox" aria-label="Schülerliste">
          {gefilterteSchueler.map((s) => (
            <li key={s.id} role="option" aria-selected={selectedSchueler?.id === s.id}>
              <SchuelerCard
                schueler={s}
                isSelected={selectedSchueler?.id === s.id}
                onClick={() => onSelect(s)}
              />
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
