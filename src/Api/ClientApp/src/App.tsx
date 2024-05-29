import ErrorBoundary from '@components/ErrorBoundry';
import ScrollToTop from '@components/ScrollToTop';
import { AuthProvider } from '@context/AuthProvider';
import BrandingProvider, {
  defaultBranding,
} from '@context/BrandingThemeContext';
import FormProvider from '@context/FormContext';
import { LayoutProvider } from '@context/LayoutProvider';
import {
  CSSVariablesResolver,
  MantineProvider,
  createTheme,
} from '@mantine/core';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import { Notifications } from '@mantine/notifications';
import '@mantine/notifications/styles.css';
import '@mantine/tiptap/styles.css';
import AppRoutes from '@routes/AppRoutes';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { BRANDING_SCHEME_KEY } from '@utils/constants';
import generateTints, { BrandingThemeType } from '@utils/services/colorService';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { BrowserRouter } from 'react-router-dom';
import './App.css';

const App = ({ queryClient }: { queryClient: QueryClient }) => {
  const getBrandingTheme = () => {
    const storedValue = localStorage.getItem(BRANDING_SCHEME_KEY);
    return storedValue
      ? storedValue == '#0E99AC'
        ? defaultBranding
        : generateTints(storedValue, 10)
      : defaultBranding;
  };

  useEffect(() => {
    // Set the description tag
    const metaDescription = document.querySelector('meta[name="description"]');
    if (metaDescription) {
      metaDescription.setAttribute('content', 'This is the new description');
    } else {
      const meta = document.createElement('meta');
      meta.name = 'description';
      meta.content = 'This is the new description';
      document.head.appendChild(meta);
    }
  }, []);

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

  const theme = createTheme({
    components: {
      Anchor: {
        styles: (theme: any) => ({
          root: {
            color: theme.colors.brand[7],
          },
        }),
      },
      Popover: {
        styles: () => ({
          dropdown: {
            backgroundColor:
              localStorage.getItem('mantine-color-scheme-value') === 'dark'
                ? '#25262B'
                : '#F2F4F4',
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
      MonthPickerInput: {
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
    fontFamily: 'Poppins, sans-serif',
    fontFamilyMonospace: 'Monaco, Courier, monospace',
    headings: { fontFamily: 'Poppins, sans-serif' },

    colors: {
      brand: brandingThemeValue,
    },
    primaryColor: 'brand',
    breakpoints: {
      cmd: '1050px',
      clg: '1280px',
      cxl: '1780px',
    },
  });

  const resolver: CSSVariablesResolver = (theme) => ({
    variables: {
      '--mantine-hero-height': theme.other.heroHeight,
      '--mantine-color-answer-active': brandingTheme,
    },
    light: {
      '--mantine-color-editor-bg': theme.colors.gray[2],
      '--mantine-color-reply-bg': theme.colors.gray[1],
      '--mantine-color-wrapper-bg': theme.colors.gray[4],
      '--mantine-color-answered': '#09ADC3',
      '--mantine-color-title': theme.black,
      '--mantine-color-paper-hover': theme.colors.dark[2],
      '--mantine-color-training-footer': theme.colors.gray[2],
      '--mantine-color-user-icon': theme.colors.gray[4],
      '--mantine-color-footer-border': theme.colors.gray[2],
      '--mantine-color-error-label': theme.colors.gray[2],
      '--mantine-color-pool-border': theme.colors.gray[2],
      '--mantine-color-classes-section': 'white',
      '--mantine-color-progress-section': theme.colors.teal[6],
      '--mantine-color-progress-bar': theme.white,
      '--mantine-color-drag': theme.colors.gray[3],
      '--mantine-color-drop': theme.colors.gray[4],
      '--mantine-color-correct-circle': theme.colors.green[5],
      '--mantine-color-lesson-bg': theme.white,
    },
    dark: {
      '--mantine-color-editor-bg': theme.colors.dark[1],
      '--mantine-color-reply-bg': theme.colors.dark[3],
      '--mantine-color-wrapper-bg': theme.colors.dark[4],
      '--mantine-color-answered': '#128797',
      '--mantine-color-title': theme.white,
      '--mantine-color-paper-hover': theme.colors.gray[7],
      '--mantine-color-training-footer': theme.colors.dark[4],
      '--mantine-color-user-icon': theme.colors.dark[3],
      '--mantine-color-footer-border': theme.colors.dark[5],
      '--mantine-color-error-label': theme.colors.dark[4],
      '--mantine-color-pool-border': theme.colors.dark[4],
      '--mantine-color-classes-section': theme.colors.dark[6],
      '--mantine-color-progress-section': theme.colors.teal[9],
      '--mantine-color-progress-bar': theme.colors.dark[7],
      '--mantine-color-drag': theme.colors.blue[1],
      '--mantine-color-drop': theme.colors.blue[1],
      '--mantine-color-correct-circle': theme.colors.green[8],
      '--mantine-color-lesson-bg': theme.colors.dark[5],
    },
  });

  return (
    <BrowserRouter>
      <ScrollToTop />
      <MantineProvider theme={theme} cssVariablesResolver={resolver}>
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
  );
};

export default App;
