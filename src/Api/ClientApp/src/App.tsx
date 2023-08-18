import {
  ColorScheme,
  ColorSchemeProvider,
  MantineProvider,
} from '@mantine/core';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import AppRoutes from '@routes/AppRoutes';
import { Notifications } from '@mantine/notifications';
import { AuthProvider } from '@context/AuthProvider';
import { BRANDING_SCHEME_KEY, COLOR_SCHEME_KEY } from '@utils/constants';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { LayoutProvider } from '@context/LayoutProvider';
import ErrorBoundary from '@components/ErrorBoundry';
import { BrowserRouter } from 'react-router-dom';
import ScrollToTop from '@components/ScrollToTop';
import './App.css';
import { useTranslation } from 'react-i18next';
import FormProvider from '@context/FormContext';
import BrandingProvider from '@context/BrandingThemeContext';
import generateTints from '@utils/services/colorService';

const App = ({ queryClient }: { queryClient: QueryClient }) => {
  const [colorScheme, setColorScheme] = useState<ColorScheme>(
    (localStorage.getItem(COLOR_SCHEME_KEY) as 'light' | 'dark') ?? 'light'
  );
  const toggleColorScheme = (value?: ColorScheme) => {
    const nextColorScheme =
      value || (colorScheme === 'dark' ? 'light' : 'dark');
    setColorScheme(nextColorScheme);
    localStorage.setItem(COLOR_SCHEME_KEY, nextColorScheme);
  };
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

  const getBrandingTheme = () => {
    const storedValue = localStorage.getItem(BRANDING_SCHEME_KEY);
    return storedValue
      ? storedValue == '#0E99AC'
        ? defaultBranding
        : generateTints(storedValue, 10)
      : defaultBranding;
  };

  const [brandingTheme, setBrandingTheme] = useState(
    localStorage.getItem(BRANDING_SCHEME_KEY) ?? '#0E99AC'
  );
  const [brandingThemeValue, setBrandingThemeValue] =
    useState<BrandingThemeType>(getBrandingTheme());

  const { i18n } = useTranslation();

  const lang = localStorage.getItem('lang');

  useEffect(() => {
    i18n.changeLanguage(lang ?? 'en');
  }, [lang]);

  return (
    <ColorSchemeProvider
      colorScheme={colorScheme}
      toggleColorScheme={toggleColorScheme}
    >
      <BrowserRouter>
        <ScrollToTop />
        <MantineProvider
          withGlobalStyles
          withNormalizeCSS
          theme={{
            globalStyles: () => ({
              '.global-astrick': {
                color: '#e03131',
                fontWeight: 'bold',
                fontSize: '20px',
                verticalAlign: 'middle',
              },
            }),
            components: {
              Anchor: {
                styles: (theme) => ({
                  root: {
                    color: theme.colors.brand[7],
                  },
                }),
              },
              Popover: {
                styles: (theme) => ({
                  dropdown: {
                    backgroundColor:
                      theme.colorScheme === 'dark' ? '#25262B' : '#F2F4F4',
                  },
                }),
              },
              TextInput: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                    verticalAlign: 'middle',
                  },
                }),
              },
              MultiSelect: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
              Select: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
              Textarea: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
              TimeInput: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
              DatePickerInput: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
              NumberInput: {
                styles: () => ({
                  required: {
                    fontWeight: 'bold',
                    fontSize: '20px',
                  },
                }),
              },
            },
            colorScheme,
            fontFamily: 'Poppins, sans-serif',
            fontFamilyMonospace: 'Monaco, Courier, monospace',
            headings: { fontFamily: 'Poppins, sans-serif' },
            loader: 'dots',

            colors: {
              brand: brandingThemeValue,
            },
            primaryColor: 'brand',
          }}
        >
          <FormProvider>
            <QueryClientProvider client={queryClient}>
              <ErrorBoundary>
                <AuthProvider>
                  <LayoutProvider>
                    <BrandingProvider
                      brandingTheme={brandingTheme}
                      brandingThemeValue={brandingThemeValue}
                      setBrandingTheme={setBrandingTheme}
                      setBrandingThemeValue={setBrandingThemeValue}
                    >
                      <AppRoutes />
                    </BrandingProvider>
                  </LayoutProvider>
                </AuthProvider>
                <ReactQueryDevtools initialIsOpen={false} />
              </ErrorBoundary>
            </QueryClientProvider>
            <Notifications />
          </FormProvider>
        </MantineProvider>
      </BrowserRouter>
    </ColorSchemeProvider>
  );
};

export default App;
