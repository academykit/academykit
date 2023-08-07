import { BRANDING_SCHEME_KEY } from '@utils/constants';
import { Dispatch, FC, SetStateAction, createContext } from 'react';

type BrandingThemeType = [
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
  string,
];

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

const BrandingProvider: FC<Props> = ({
  children,
  brandingTheme,
  brandingThemeValue,
  setBrandingTheme,
  setBrandingThemeValue,
}) => {
  const defaultBranding: BrandingThemeType = [
    '#7AD1DD',
    '#5FCCDB',
    '#44CADC',
    '#2AC9DE',
    '#1AC2D9',
    '#11B7CD',
    '#09ADC3',
    '#0E99AC',
    '#128797',
    '#147885',
  ];

  // const [brandingTheme, setBrandingTheme] = useState('#0E99AC');
  // const [brandingThemeValue, setBrandingThemeValue] =
  //   useState<BrandingThemeType>(defaultBranding);

  const toggleBrandingTheme = (value: string) => {
    // set new color if chosen by user
    const brandingColors: BrandingThemeType =
      value == '#0E99AC'
        ? defaultBranding
        : (new Array(10).fill(value) as BrandingThemeType);

    setBrandingThemeValue(brandingColors);
    setBrandingTheme(value);
    console.log('this ran', brandingTheme);
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
