import { useState, useEffect, useRef } from 'react';
import { X } from 'lucide-react';
import type { Fach, Note, NoteFormData } from '../../types';
import { formatDate } from '../../utils/noteUtils';
import styles from './NoteModal.module.css';

interface NoteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: NoteFormData) => void;
  faecher: Fach[];
  editNote?: Note | null;
  selectedFachId?: string; // Wenn ein Fach vorausgewählt ist
}

export const NoteModal = ({
  isOpen,
  onClose,
  onSubmit,
  faecher,
  editNote,
  selectedFachId,
}: NoteModalProps) => {
  const [fachId, setFachId] = useState('');
  const [wert, setWert] = useState('');
  const [notiz, setNotiz] = useState('');
  const [datum, setDatum] = useState('');
  const modalRef = useRef<HTMLDivElement>(null);
  const firstInputRef = useRef<HTMLSelectElement>(null);
  const noteInputRef = useRef<HTMLInputElement>(null);

  // Initialisierung
  useEffect(() => {
    if (isOpen) {
      if (editNote) {
        setFachId(editNote.fachId || editNote.fach?.id || '');
        setWert(editNote.wert.toString());
        setNotiz(editNote.notiz || '');
        setDatum(new Date(editNote.erstelltAm).toISOString().split('T')[0]);
      } else {
        // Wenn ein Fach vorausgewählt ist, verwende es
        setFachId(selectedFachId || '');
        setWert('');
        setNotiz('');
        setDatum(new Date().toISOString().split('T')[0]);
      }
      
      // Focus erstes Element
      setTimeout(() => {
        // Wenn Fach vorausgewählt, fokussiere Note-Input
        if (selectedFachId) {
          noteInputRef.current?.focus();
        } else {
          firstInputRef.current?.focus();
        }
      }, 100);
    }
  }, [isOpen, editNote, selectedFachId]);

  // ESC-Taste zum Schließen
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };
    
    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, onClose]);

  // Body-Scroll blockieren wenn Modal offen
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }
    return () => {
      document.body.style.overflow = '';
    };
  }, [isOpen]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    const wertNumber = parseFloat(wert.replace(',', '.'));
    
    if (!fachId || isNaN(wertNumber) || wertNumber < 1 || wertNumber > 6) {
      return;
    }
    
    onSubmit({
      fachId,
      wert: wertNumber,
      notiz: notiz.trim() || undefined,
      datum,
    });
  };

  const handleOverlayClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div
      className={styles.overlay}
      onClick={handleOverlayClick}
      role="dialog"
      aria-modal="true"
      aria-labelledby="modal-title"
    >
      <div className={styles.modal} ref={modalRef}>
        <div className={styles.header}>
          <h2 id="modal-title" className={styles.title}>
            {editNote ? 'Note bearbeiten' : 'Neue Note hinzufügen'}
          </h2>
          <button
            className={styles.closeButton}
            onClick={onClose}
            aria-label="Dialog schließen"
          >
            <X size={24} aria-hidden="true" />
          </button>
        </div>

        <p className={styles.subtitle}>
          {editNote ? 'Bearbeite die' : 'Geben Sie die'} Details für{' '}
          {editNote ? 'diese' : 'die neue'} Note ein.
        </p>

        {editNote && (
          <div className={styles.metaInfo}>
            <span>Erstellt: {formatDate(editNote.erstelltAm)}</span>
            {editNote.angepasstAm !== editNote.erstelltAm && (
              <span>Zuletzt bearbeitet: {formatDate(editNote.angepasstAm)}</span>
            )}
          </div>
        )}

        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.field}>
            <label htmlFor="fach" className={styles.label}>
              Fach
            </label>
            <select
              id="fach"
              ref={firstInputRef}
              className={styles.select}
              value={fachId}
              onChange={(e) => setFachId(e.target.value)}
              required
              disabled={!!selectedFachId}
            >
              <option value="" disabled>
                Fach auswählen
              </option>
              {faecher.map((fach) => (
                <option key={fach.id} value={fach.id}>
                  {fach.bezeichnung}
                </option>
              ))}
            </select>
          </div>

          <div className={styles.field}>
            <label htmlFor="wert" className={styles.label}>
              Note (1-6)
            </label>
            <input
              id="wert"
              ref={noteInputRef}
              type="text"
              inputMode="decimal"
              className={styles.input}
              placeholder="z.B. 2.5"
              value={wert}
              onChange={(e) => setWert(e.target.value)}
              required
              pattern="[1-6]([\.,][0-9])?"
              title="Note zwischen 1 und 6 (z.B. 2.5)"
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="datum" className={styles.label}>
              Datum
            </label>
            <input
              id="datum"
              type="date"
              className={styles.input}
              value={datum}
              onChange={(e) => setDatum(e.target.value)}
              required
            />
          </div>

          <div className={styles.field}>
            <label htmlFor="notiz" className={styles.label}>
              Notiz <span className={styles.optional}>(optional)</span>
            </label>
            <textarea
              id="notiz"
              className={styles.textarea}
              placeholder="z.B. Referat über Goethe, aktive Mitarbeit..."
              value={notiz}
              onChange={(e) => setNotiz(e.target.value)}
              rows={3}
            />
          </div>

          <div className={styles.actions}>
            <button
              type="submit"
              className={styles.submitButton}
              disabled={!fachId || !wert}
            >
              {editNote ? 'Speichern' : 'Hinzufügen'}
            </button>
            <button
              type="button"
              className={styles.cancelButton}
              onClick={onClose}
            >
              Abbrechen
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
