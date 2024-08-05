import { BRANDING_SCHEME_KEY } from "@utils/constants";
import generateTints, { BrandingThemeType } from "@utils/services/colorService";
import { Dispatch, FC, SetStateAction, createContext } from "react";

export interface IBrandingContext {
  brandingTheme: string;
  brandingThemeValue: BrandingThemeType;
  toggleBrandingTheme: (value: string) => void;
}

export const BrandingContext = createContext<IBrandingContext | null>(null);

type Props = {
  children: React.ReactNode;
  brandingTheme: string;
  brandingThemeValue: BrandingThemeType;
  setBrandingTheme: Dispatch<SetStateAction<string>>;
  setBrandingThemeValue: Dispatch<SetStateAction<BrandingThemeType>>;
};

export const defaultBranding: BrandingThemeType = [
  "#7AD1DD",
  "#5FCCDB",
  "#44CADC",
  "#2AC9DE",
  "#1AC2D9",
  "#11B7CD",
  "#09ADC3",
  "#0E99AC",
  "#128797",
  "#147885",
];

const BrandingProvider: FC<Props> = ({
  children,
  brandingTheme,
  brandingThemeValue,
  setBrandingTheme,
  setBrandingThemeValue,
}) => {
  const toggleBrandingTheme = (value: string) => {
    // set new color if chosen by user
    const brandingColors: BrandingThemeType =
      value == "#0E99AC" ? defaultBranding : generateTints(value, 10);

    setBrandingThemeValue(brandingColors);
    setBrandingTheme(value);
    localStorage.setItem(BRANDING_SCHEME_KEY, value);
  };

  return (
    <BrandingContext.Provider
      value={{ brandingTheme, brandingThemeValue, toggleBrandingTheme }}
    >
      {children}
    </BrandingContext.Provider>
  );
};

export default BrandingProvider;
