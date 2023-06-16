import { Children, FC, createContext, useState } from "react";

interface IFormContext {
  isReady: boolean;
  setReady: () => void;
}

export const FormContext = createContext<IFormContext | null>(null);

const FormProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [isReady, setIsReady] = useState<boolean>(true);
  const setReady = () => {
    setIsReady((isReady) => !isReady);
  };
  return (
    <FormContext.Provider value={{ isReady, setReady }}>
      {children}
    </FormContext.Provider>
  );
};

export default FormProvider;
