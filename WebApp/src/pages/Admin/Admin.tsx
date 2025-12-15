import { useState } from 'react';
import { BookOpen, Users, GraduationCap, UserPlus } from 'lucide-react';
import { FaecherTab } from './tabs/FaecherTab';
import { KlassenTab } from './tabs/KlassenTab';
import { SchuelerTab } from './tabs/SchuelerTab';
import { LehrerTab } from './tabs/LehrerTab';
import styles from './Admin.module.css';

type TabId = 'faecher' | 'klassen' | 'schueler' | 'lehrer';

interface Tab {
  id: TabId;
  label: string;
  icon: typeof BookOpen;
}

const tabs: Tab[] = [
  { id: 'faecher', label: 'Fächer', icon: BookOpen },
  { id: 'klassen', label: 'Klassen', icon: Users },
  { id: 'schueler', label: 'Schüler', icon: GraduationCap },
  { id: 'lehrer', label: 'Lehrer', icon: UserPlus },
];

export const Admin = () => {
  const [activeTab, setActiveTab] = useState<TabId>('faecher');

  const renderTabContent = () => {
    switch (activeTab) {
      case 'faecher':
        return <FaecherTab />;
      case 'klassen':
        return <KlassenTab />;
      case 'schueler':
        return <SchuelerTab />;
      case 'lehrer':
        return <LehrerTab />;
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Verwaltung</h1>
      
      <div className={styles.tabsContainer}>
        <nav className={styles.tabs} role="tablist" aria-label="Verwaltungsbereiche">
          {tabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <button
                key={tab.id}
                className={`${styles.tab} ${activeTab === tab.id ? styles.tabActive : ''}`}
                onClick={() => setActiveTab(tab.id)}
                role="tab"
                aria-selected={activeTab === tab.id}
                aria-controls={`panel-${tab.id}`}
              >
                <Icon size={18} aria-hidden="true" />
                <span>{tab.label}</span>
              </button>
            );
          })}
        </nav>
        
        <div 
          className={styles.tabPanel}
          role="tabpanel"
          id={`panel-${activeTab}`}
          aria-labelledby={activeTab}
        >
          {renderTabContent()}
        </div>
      </div>
    </div>
  );
};
