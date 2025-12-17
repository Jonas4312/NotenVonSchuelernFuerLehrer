import { useState, useEffect, useCallback, useMemo, useRef, type ChangeEvent } from 'react';
import { ArrowLeft, X, Upload, User } from 'lucide-react';
import type { Klasse, Schueler, Fach, Note, NoteFormData } from '../../types';
import { klassenApi, notenApi, schuelerApi, lehrerApi } from '../../services/api';
import {
  KlasseSelector,
  FachSelector,
  SchuelerList,
  SchuelerDetail,
  NoteModal,
  ConfirmDialog,
} from '../../components';
import styles from './Dashboard.module.css';

export const Dashboard = () => {
  // State - Daten werden frisch vom API geladen, nicht aus dem AuthContext gecacht
  const [klassen, setKlassen] = useState<Klasse[]>([]);
  const [faecher, setFaecher] = useState<Fach[]>([]);
  const [schuelerList, setSchuelerList] = useState<Schueler[]>([]);
  const [selectedKlasse, setSelectedKlasse] = useState<Klasse | null>(null);
  const [selectedFach, setSelectedFach] = useState<Fach | null>(null);
  const [selectedSchueler, setSelectedSchueler] = useState<Schueler | null>(null);
  const [schuelerNoten, setSchuelerNoten] = useState<Note[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingSchueler, setIsLoadingSchueler] = useState(false);
  const [, setIsLoadingNoten] = useState(false);
  const [isMobileDetailView, setIsMobileDetailView] = useState(false);

  // Modal State
  const [isNoteModalOpen, setIsNoteModalOpen] = useState(false);
  const [editingNote, setEditingNote] = useState<Note | null>(null);
  const [deletingNote, setDeletingNote] = useState<Note | null>(null);
  const [isSchuelerModalOpen, setIsSchuelerModalOpen] = useState(false);

  // Verfügbare Fächer für die ausgewählte Klasse (gefiltert nach Lehrer-Fächern)
  const availableFaecher = useMemo(() => {
    if (!selectedKlasse) return [];
    // Nutze die frisch vom API geladenen Fächer
    return faecher;
  }, [selectedKlasse, faecher]);

  // Schüler mit Noten kombinieren
  const schuelerMitNoten = useMemo((): Schueler | null => {
    if (!selectedSchueler) return null;
    
    const notenFuerAnzeige = selectedFach 
      ? schuelerNoten.filter(n => n.fach?.id === selectedFach.id)
      : schuelerNoten;
    
    return {
      ...selectedSchueler,
      noten: notenFuerAnzeige,
    };
  }, [selectedSchueler, schuelerNoten, selectedFach]);

  // Klassen und Fächer frisch vom API laden (nicht aus gecachtem AuthContext)
  useEffect(() => {
    const loadLehrerData = async () => {
      try {
        setIsLoading(true);
        const response = await lehrerApi.getMe();
        
        // Klassen mit leerer Schüler-Liste initialisieren
        const klassenMitListen = response.klassen.map(k => ({
          ...k,
          schueler: [],
        }));
        
        setKlassen(klassenMitListen);
        setFaecher(response.faecher);
        
        if (klassenMitListen.length > 0) {
          setSelectedKlasse(klassenMitListen[0]);
        }
      } catch (error) {
        console.error('Fehler beim Laden der Lehrer-Daten:', error);
        setKlassen([]);
        setFaecher([]);
      } finally {
        setIsLoading(false);
      }
    };

    loadLehrerData();
  }, []);

  // Schüler laden wenn Klasse ausgewählt wird
  useEffect(() => {
    const loadSchueler = async () => {
      if (!selectedKlasse) {
        setSchuelerList([]);
        return;
      }
      
      try {
        setIsLoadingSchueler(true);
        const response = await klassenApi.getSchueler(selectedKlasse.id);
        // Konvertiere SchuelerDto zu Schueler - bildByteArray bleibt unverändert
        const schueler: Schueler[] = response.map((s) => ({
          ...s,
          bildByteArray: s.bildByteArray || undefined,
          noten: [],
        }));
        setSchuelerList(schueler);
        
        // Noten für alle Schüler laden
        const schuelerMitNoten = await Promise.all(
          schueler.map(async (s) => {
            try {
              const notenResponse = await notenApi.getBySchueler(s.id);
              const noten: Note[] = notenResponse.map((n) => ({
                id: n.id,
                wert: n.wert,
                notiz: n.notiz,
                erstelltAm: n.erstelltAm,
                angepasstAm: n.angepasstAm,
                fach: n.fach,
                fachId: n.fach.id,
                schuelerId: s.id,
              }));
              return { ...s, noten };
            } catch {
              return s;
            }
          })
        );
        setSchuelerList(schuelerMitNoten);
      } catch (error) {
        console.error('Fehler beim Laden der Schüler:', error);
        setSchuelerList([]);
      } finally {
        setIsLoadingSchueler(false);
      }
    };

    loadSchueler();
  }, [selectedKlasse?.id]);

  // Noten laden wenn Schüler ausgewählt wird
  useEffect(() => {
    const loadNoten = async () => {
      if (!selectedSchueler) {
        setSchuelerNoten([]);
        return;
      }
      
      try {
        setIsLoadingNoten(true);
        const response = await notenApi.getBySchueler(selectedSchueler.id);
        // Konvertiere NoteDto zu Note
        const noten: Note[] = response.map((n) => ({
          id: n.id,
          wert: n.wert,
          notiz: n.notiz,
          erstelltAm: n.erstelltAm,
          angepasstAm: n.angepasstAm,
          fach: n.fach,
          fachId: n.fach.id,
          schuelerId: selectedSchueler.id,
        }));
        setSchuelerNoten(noten);
      } catch (error) {
        console.error('Fehler beim Laden der Noten:', error);
        setSchuelerNoten([]);
      } finally {
        setIsLoadingNoten(false);
      }
    };

    loadNoten();
  }, [selectedSchueler?.id]);

  // Klasse auswählen
  const handleKlasseSelect = useCallback((klasse: Klasse) => {
    setSelectedKlasse(klasse);
    setSelectedFach(null);
    setSelectedSchueler(null);
    setSchuelerNoten([]);
  }, []);

  // Fach auswählen
  const handleFachSelect = useCallback((fach: Fach | null) => {
    setSelectedFach(fach);
  }, []);

  // Schüler auswählen
  const handleSchuelerSelect = useCallback((schueler: Schueler) => {
    setSelectedSchueler(schueler);
    // Auf Mobile zur Detailansicht wechseln
    if (window.innerWidth <= 768) {
      setIsMobileDetailView(true);
    }
  }, []);

  // Zurück zur Liste (Mobile)
  const handleBackToList = useCallback(() => {
    setIsMobileDetailView(false);
  }, []);

  // Note hinzufügen
  const handleAddNote = useCallback(() => {
    setEditingNote(null);
    setIsNoteModalOpen(true);
  }, []);

  // Note bearbeiten
  const handleEditNote = useCallback((note: Note) => {
    setEditingNote(note);
    setIsNoteModalOpen(true);
  }, []);

  // Note löschen vorbereiten
  const handleDeleteNote = useCallback((note: Note) => {
    setDeletingNote(note);
  }, []);

  // Note löschen bestätigen
  const confirmDeleteNote = useCallback(async () => {
    if (!deletingNote || !selectedSchueler) return;

    try {
      await notenApi.delete(deletingNote.id);
      
      // Noten aktualisieren
      setSchuelerNoten(prev => prev.filter(n => n.id !== deletingNote.id));
      
      // Schülerliste aktualisieren
      setSchuelerList(prev => prev.map(s => 
        s.id === selectedSchueler.id 
          ? { ...s, noten: (s.noten || []).filter(n => n.id !== deletingNote.id) }
          : s
      ));
    } catch (error) {
      console.error('Fehler beim Löschen der Note:', error);
    } finally {
      setDeletingNote(null);
    }
  }, [deletingNote, selectedSchueler]);

  // Note speichern
  const handleSubmitNote = useCallback(
    async (data: NoteFormData) => {
      if (!selectedSchueler) return;

      try {
        if (editingNote) {
          // Bearbeiten
          const response = await notenApi.update(editingNote.id, {
            wert: data.wert,
            notiz: data.notiz,
          });
          
          const updatedNote: Note = {
            id: response.id,
            wert: response.wert,
            notiz: response.notiz,
            erstelltAm: response.erstelltAm,
            angepasstAm: response.angepasstAm,
            fach: response.fach,
            fachId: response.fach.id,
            schuelerId: selectedSchueler.id,
          };
          
          setSchuelerNoten(prev => prev.map(n => n.id === editingNote.id ? updatedNote : n));
          
          // Schülerliste aktualisieren
          setSchuelerList(prev => prev.map(s => 
            s.id === selectedSchueler.id 
              ? { ...s, noten: (s.noten || []).map(n => n.id === editingNote.id ? updatedNote : n) }
              : s
          ));
        } else {
          // Neu erstellen
          const response = await notenApi.create({
            schuelerId: selectedSchueler.id,
            fachId: data.fachId,
            wert: data.wert,
            notiz: data.notiz,
          });
          
          const newNote: Note = {
            id: response.id,
            wert: response.wert,
            notiz: response.notiz,
            erstelltAm: response.erstelltAm,
            angepasstAm: response.angepasstAm,
            fach: response.fach,
            fachId: response.fach.id,
            schuelerId: selectedSchueler.id,
          };
          
          setSchuelerNoten(prev => [...prev, newNote]);
          
          // Schülerliste aktualisieren
          setSchuelerList(prev => prev.map(s => 
            s.id === selectedSchueler.id 
              ? { ...s, noten: [...(s.noten || []), newNote] }
              : s
          ));
        }
      } catch (error) {
        console.error('Fehler beim Speichern der Note:', error);
      } finally {
        setIsNoteModalOpen(false);
        setEditingNote(null);
      }
    },
    [selectedSchueler, editingNote]
  );

  // Prüfe ob Lehrer Fächer zugewiesen hat (basierend auf frisch geladenen Daten)
  const hatKeineFaecher = !isLoading && faecher.length === 0;
  
  // Prüfe ob Lehrer in keiner Klasse unterrichten kann
  const hatKeineKlassen = !isLoading && klassen.length === 0;
  
  // Prüfe ob Lehrer zwar Fächer hat, aber keine davon in verfügbaren Klassen unterrichtet werden
  const hatKeinPassendesFach = !isLoading && 
    selectedKlasse && 
    availableFaecher.length === 0 && 
    !hatKeineFaecher;

  // Read-Only Modus: Lehrer kann Noten sehen aber nicht bearbeiten
  const isReadOnly = hatKeineFaecher || hatKeinPassendesFach;

  // Schüler bearbeiten
  const handleEditSchueler = useCallback(() => {
    setIsSchuelerModalOpen(true);
  }, []);

  // Schüler speichern
  const handleSaveSchueler = useCallback(async (vorname: string, nachname: string, bildUrl?: string) => {
    if (!selectedSchueler) return;

    try {
      // Base64 aus Data URL extrahieren
      let bildByteArray = '';
      if (bildUrl && bildUrl.startsWith('data:')) {
        bildByteArray = bildUrl.split(',')[1] || '';
      } else if (selectedSchueler.bildByteArray) {
        bildByteArray = selectedSchueler.bildByteArray;
      }

      const response = await schuelerApi.update(selectedSchueler.id, {
        vorname,
        nachname,
        bildByteArray,
      });

      // Nur bildByteArray speichern, nicht bildUrl
      const updatedSchueler: Schueler = {
        ...selectedSchueler,
        vorname: response.vorname,
        nachname: response.nachname,
        bildByteArray: response.bildByteArray || undefined,
      };

      setSelectedSchueler(updatedSchueler);
      setSchuelerList(prev => prev.map(s => s.id === updatedSchueler.id ? updatedSchueler : s));
      setIsSchuelerModalOpen(false);
    } catch (error) {
      console.error('Fehler beim Speichern des Schülers:', error);
    }
  }, [selectedSchueler]);

  return (
    <div className={styles.dashboard}>
      <main className={styles.main}>
        {/* Warnung: Keine Fächer zugewiesen */}
        {hatKeineFaecher && (
          <div className={styles.warningBanner}>
            <div className={styles.warningContent}>
              <h2>👁️ Lesezugriff</h2>
              <p>
                Ihnen wurden noch keine Unterrichtsfächer zugewiesen. 
                Sie können Noten einsehen, aber nicht bearbeiten.
              </p>
            </div>
          </div>
        )}

        {/* Warnung: Keine Klassen verfügbar */}
        {!hatKeineFaecher && hatKeineKlassen && (
          <div className={styles.warningBanner}>
            <div className={styles.warningContent}>
              <h2>📚 Keine Klassen verfügbar</h2>
              <p>
                Es sind aktuell keine Klassen im System vorhanden. 
                Bitte wenden Sie sich an die Schulverwaltung.
              </p>
            </div>
          </div>
        )}

        <div className={styles.container}>
          {/* Linke Sidebar - auf Mobile versteckt wenn Detail aktiv */}
          <aside 
            className={`${styles.sidebar} ${isMobileDetailView ? styles.hiddenMobile : ''}`} 
            aria-label="Klassen- und Schülerauswahl"
          >
            <KlasseSelector
              klassen={klassen}
              selectedKlasse={selectedKlasse}
              onSelect={handleKlasseSelect}
              isLoading={isLoading}
            />
            
            {/* Hinweis: Keine passenden Fächer in dieser Klasse */}
            {hatKeinPassendesFach && (
              <div className={styles.infoBox}>
                <p>
                  <strong>👁️ Lesezugriff:</strong> Sie unterrichten in dieser Klasse keine Fächer. 
                  Sie können Noten einsehen, aber nicht bearbeiten.
                </p>
              </div>
            )}
            
            {/* Fachauswahl - nur wenn Klasse ausgewählt und Fächer verfügbar */}
            {selectedKlasse && availableFaecher.length > 0 && (
              <FachSelector
                faecher={availableFaecher}
                selectedFach={selectedFach}
                onSelect={handleFachSelect}
                showAllOption
              />
            )}
            
            {/* Schülerliste - auch im Read-Only Modus anzeigen */}
            {selectedKlasse && (
              <SchuelerList
                schueler={schuelerList}
                selectedSchueler={selectedSchueler}
                onSelect={handleSchuelerSelect}
                isLoading={isLoadingSchueler}
              />
            )}
          </aside>

          {/* Hauptbereich - auf Mobile nur sichtbar wenn Detail aktiv */}
          <section 
            className={`${styles.content} ${!isMobileDetailView ? styles.hiddenMobile : ''}`} 
            aria-label="Schülerdetails"
          >
            {schuelerMitNoten ? (
              <>
                {/* Mobile Zurück-Button */}
                <button 
                  className={styles.backButton}
                  onClick={handleBackToList}
                  aria-label="Zurück zur Schülerliste"
                >
                  <ArrowLeft size={20} aria-hidden="true" />
                  <span>Zurück</span>
                </button>
                <SchuelerDetail
                  schueler={schuelerMitNoten}
                  faecher={selectedFach ? [selectedFach] : availableFaecher}
                  onAddNote={isReadOnly ? undefined : handleAddNote}
                  onEditNote={isReadOnly ? undefined : handleEditNote}
                  onDeleteNote={isReadOnly ? undefined : handleDeleteNote}
                  onEditSchueler={handleEditSchueler}
                  selectedFachName={selectedFach?.bezeichnung}
                  isReadOnly={isReadOnly ?? false}
                />
              </>
            ) : (
              <div className={styles.placeholder}>
                <div className={styles.placeholderContent}>
                  <h2>Schüler auswählen</h2>
                  <p>
                    {selectedFach 
                      ? `Wähle einen Schüler aus der Liste, um dessen ${selectedFach.bezeichnung}-Noten anzuzeigen.`
                      : 'Wähle einen Schüler aus der Liste aus, um dessen Noten zu sehen und zu bearbeiten.'
                    }
                  </p>
                </div>
              </div>
            )}
          </section>
        </div>
      </main>

      {/* Note Modal */}
      <NoteModal
        isOpen={isNoteModalOpen}
        onClose={() => {
          setIsNoteModalOpen(false);
          setEditingNote(null);
        }}
        onSubmit={handleSubmitNote}
        faecher={selectedFach ? [selectedFach] : availableFaecher}
        editNote={editingNote}
        selectedFachId={selectedFach?.id}
      />

      {/* Lösch-Bestätigung */}
      <ConfirmDialog
        isOpen={!!deletingNote}
        title="Note löschen?"
        message={`Möchtest du diese Note wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.`}
        confirmLabel="Löschen"
        cancelLabel="Abbrechen"
        variant="danger"
        onConfirm={confirmDeleteNote}
        onCancel={() => setDeletingNote(null)}
      />

      {/* Schüler bearbeiten Modal */}
      {isSchuelerModalOpen && selectedSchueler && (
        <SchuelerEditModal
          schueler={selectedSchueler}
          onSave={handleSaveSchueler}
          onClose={() => setIsSchuelerModalOpen(false)}
        />
      )}
    </div>
  );
};

// Schüler-Bearbeiten Modal
interface SchuelerEditModalProps {
  schueler: Schueler;
  onSave: (vorname: string, nachname: string, bildUrl?: string) => void;
  onClose: () => void;
}

const SchuelerEditModal = ({ schueler, onSave, onClose }: SchuelerEditModalProps) => {
  const [vorname, setVorname] = useState(schueler.vorname);
  const [nachname, setNachname] = useState(schueler.nachname);
  // bildByteArray als Data-URL für die Vorschau anzeigen
  const [bildPreview, setBildPreview] = useState<string | null>(
    schueler.bildByteArray ? `data:image/jpeg;base64,${schueler.bildByteArray}` : null
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
    if (!vorname.trim() || !nachname.trim()) return;
    onSave(vorname.trim(), nachname.trim(), newBildUrl);
  };

  const isValid = vorname.trim() && nachname.trim();

  return (
    <div className={styles.modalOverlay} onClick={onClose}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.modalHeader}>
          <h2>Schüler bearbeiten</h2>
          <button className={styles.closeButton} onClick={onClose}>
            <X size={20} />
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className={styles.modalBody}>
            {/* Profilbild */}
            <div className={styles.formGroup}>
              <label>Profilbild</label>
              <div className={styles.bildSection}>
                <div className={styles.bildPreview}>
                  {bildPreview ? (
                    <img src={bildPreview} alt="" />
                  ) : (
                    <User size={32} />
                  )}
                </div>
                <div>
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    onChange={handleBildSelect}
                    style={{ display: 'none' }}
                  />
                  <button
                    type="button"
                    className={styles.secondaryButton}
                    onClick={() => fileInputRef.current?.click()}
                  >
                    <Upload size={16} />
                    Bild ändern
                  </button>
                </div>
              </div>
            </div>

            <div className={styles.formGroup}>
              <label htmlFor="schueler-vorname">Vorname</label>
              <input
                id="schueler-vorname"
                type="text"
                value={vorname}
                onChange={(e) => setVorname(e.target.value)}
                placeholder="Vorname"
              />
            </div>
            <div className={styles.formGroup}>
              <label htmlFor="schueler-nachname">Nachname</label>
              <input
                id="schueler-nachname"
                type="text"
                value={nachname}
                onChange={(e) => setNachname(e.target.value)}
                placeholder="Nachname"
              />
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
