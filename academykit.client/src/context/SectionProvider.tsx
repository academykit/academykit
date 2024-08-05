import { useMediaQuery } from '@mantine/hooks';
import { createContext, useContext, useState } from 'react';

interface ISectionContext {
  setIsAddSection: any;
  isAddSection: boolean;
  matches: boolean;
  addLessonClick: boolean;
  setAddLessonClick: any;
  activeSection: string | undefined;
  setActiveSection: (sectionId: string) => void;
}

const SectionContext = createContext<ISectionContext | null>(null);

export const EditSectionProvider = ({ children }: any) => {
  const matches = useMediaQuery(`(min-width: 48em)`) as boolean;
  const [isAddSection, setIsAddSection] = useState<boolean>(false);
  const [addLessonClick, setAddLessonClick] = useState<boolean>(false);
  const [activeSection, setActiveSection] = useState<string | undefined>();

  const changeActiveSection = (sectionId: string) => {
    if (activeSection === sectionId) {
      setActiveSection(undefined);
    } else {
      setActiveSection(sectionId);
    }
  };
  return (
    <SectionContext.Provider
      value={{
        setIsAddSection,
        isAddSection,
        matches,
        addLessonClick,
        setAddLessonClick,
        setActiveSection: changeActiveSection,
        activeSection,
      }}
    >
      {children}
    </SectionContext.Provider>
  );
};

export const useSection = () => useContext(SectionContext);
