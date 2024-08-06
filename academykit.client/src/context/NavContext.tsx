import React, { FC, createContext, useState } from "react";

type IBreadCrumbItem = {
  title: string;
  href: string;
};

type NavContextProps = {
  breadCrumb?: IBreadCrumbItem[];
  setBreadCrumb?: React.Dispatch<
    React.SetStateAction<IBreadCrumbItem[] | undefined>
  >;
};

export const NavContext = createContext<NavContextProps>({});

const NavProvider: FC<React.PropsWithChildren> = ({ children }) => {
  const [breadCrumb, setBreadCrumb] = useState<IBreadCrumbItem[]>();

  return (
    <NavContext.Provider value={{ breadCrumb, setBreadCrumb }}>
      {children}
    </NavContext.Provider>
  );
};

export default NavProvider;
