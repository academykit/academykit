import { createStyles } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import { useContext, useState } from "react";
import { createContext } from "react";

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

const useStyle = createStyles((theme) => ({
  section: {
    background: theme.colorScheme === "dark" ? theme.black[2] : theme.white[2],
  },
}));

export const EditSectionProvider = ({ children }: any) => {
  const { classes, theme } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.sm}px)`);
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
