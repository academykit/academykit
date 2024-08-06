import React, { createContext, FC, useContext, useState } from "react";

interface ILayoutContext {
  examPage: boolean;
  setExamPage?: React.Dispatch<React.SetStateAction<boolean>>;
  setExamPageAction?: React.Dispatch<React.SetStateAction<JSX.Element>>;
  setExamPageTitle?: React.Dispatch<React.SetStateAction<JSX.Element>>;
  meetPage: boolean;
  setMeetPage?: React.Dispatch<React.SetStateAction<boolean>>;

  examPageTitle?: JSX.Element;
  examPageAction?: JSX.Element;
}

const initialState = {
  examPage: false,
  examPageTitle: <></>,
  examPageAction: <></>,
  meetPage: false,
};
const LayoutContext = createContext<ILayoutContext>(initialState);

export const LayoutProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [examPage, setExamPage] = useState<boolean>(false);
  const [meetPage, setMeetPage] = useState<boolean>(false);

  const [examPageTitle, setExamPageTitle] = useState<JSX.Element>(
    initialState.examPageTitle
  );
  const [examPageAction, setExamPageAction] = useState<JSX.Element>(
    initialState.examPageAction
  );

  return (
    <LayoutContext.Provider
      value={{
        examPage,
        setExamPage,
        examPageAction,
        examPageTitle,
        setExamPageAction,
        setExamPageTitle,
        meetPage,
        setMeetPage,
      }}
    >
      {children}
    </LayoutContext.Provider>
  );
};
const useCustomLayout = () => useContext(LayoutContext);
export default useCustomLayout;
